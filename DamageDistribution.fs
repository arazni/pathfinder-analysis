module PathfinderAnalysis.DamageDistribution

// https://www.analyticscheck.net/posts/sums-dice-rolls

open PathfinderAnalysis.Helpers

type RollCount = { Roll: int; Count: bigint }

type DicePool = (int * DiceSize) list

let rollDistribution dieSize rollCount =
  [(rollCount*(minimumRoll dieSize))..(rollCount*(maximumRoll dieSize))]
  |> Seq.map (fun i -> { Roll = i; Count = coefficientOfExponentiatedGeometricSeries rollCount (maximumRoll dieSize) i })

let applyModifierToDistribution modifier rollCounts =
  rollCounts
  |> Seq.map (fun rollCount -> { Roll = modifier + rollCount.Roll; Count = rollCount.Count })

let rollDistributions modifier dicePool =
  if Seq.length dicePool = 1 
  then rollDistribution (Seq.head dicePool |> first) (Seq.head dicePool |> second)
    |> applyModifierToDistribution modifier
  else

  let rolls = 
    Seq.map (fun (size, number) -> rollDistribution size number) dicePool
  
  (Seq.head rolls |> applyModifierToDistribution modifier, Seq.tail rolls)
  ||> Seq.fold (fun state rollCountList -> 
    Seq.allPairs state rollCountList
    |> Seq.map (fun (left, right) -> { Roll = left.Roll + right.Roll; Count = left.Count + right.Count })
    )
  |> Seq.sortBy (fun rollCount -> rollCount.Roll)

let getBucketSizes (nBuckets: int) (total: bigint) =
  let bucketSize = total / bigint nBuckets
  let remainder = total % bigint nBuckets
  let halfRemainder = remainder / bigint 2
  let isOdd = remainder % bigint 2

  [1..nBuckets]
  |> List.map bigint
  |> List.map (fun i -> bucketSize + if i <= halfRemainder + isOdd || i > bigint nBuckets - halfRemainder then bigint 1 else bigint 0)

let chunk nBuckets distribution =
  let totalPermutations =
    distribution
    |> List.sumBy (fun i -> i.Count)
  
  let bucketSizes = getBucketSizes nBuckets totalPermutations

  let mutable iDistribution = 0
  let mutable takenFromDistribution = bigint 0
  let mutable buckets = Seq.empty

  for iBucket in [0..(nBuckets-1)] do
    let mutable slice = Seq.empty
    let mutable inBucket = bigint 0
    let bucketCap = bucketSizes[iBucket]
    // printfn "****bucket %i****" iBucket

    while inBucket < bucketCap do
      let sum = distribution[iDistribution]
      let next = sum.Count - takenFromDistribution

      if next + inBucket <= bucketCap then
        inBucket <- inBucket + next
        slice <- Seq.append [{ Roll = sum.Roll; Count = next }] slice
        iDistribution <- iDistribution + 1
        takenFromDistribution <- bigint 0
        // printfn "in: %A sumCount: %A sumRoll: %i iDistribution: %i slice: %A" inBucket sum.Count sum.Roll iDistribution slice
      else
        let takenNow = bucketCap - inBucket
        takenFromDistribution <- takenFromDistribution + takenNow
        inBucket <- bucketCap
        slice <- Seq.append [{ Roll = sum.Roll; Count = takenNow }] slice
        // printfn "from: %A sumCount: %A sumRoll: %i iDistribution: %i slice: %A" takenFromDistribution sum.Count sum.Roll iDistribution slice

    buckets <- Seq.append [slice] buckets
    // printfn "Final Bucket Count: %A" (List.sumBy (fun i -> i.Count) slice)
    // printfn "Final Bucket: %A" [slice]
  
  buckets

let chunksToAverages (chunks : RollCount seq seq) =
  chunks
  |> Seq.map (fun rollCounts -> 
    Seq.fold (fun (numerator, denominator) next -> numerator + (next.Count * bigint next.Roll), denominator + next.Count) (bigint 0, bigint 0) rollCounts
    |> fun (x, y) -> float x / float y
  )
  |> Seq.rev
  |> Seq.mapi (fun i x -> i + 1, x)