module PathfinderAnalysis.DamageDistribution

// https://www.analyticscheck.net/posts/sums-dice-rolls

open PathfinderAnalysis.Library

type RollCount = { Roll: int; Count: bigint }

let choose n kin =
  // if n = 0 then 0 else
  if kin = 0 || kin = n then bigint 1 else

  let k = 
    if kin > n - kin then n - kin
    else kin
  
  let kList = [k..(-1)..1] |> List.map bigint
  let deltaList = [n..(-1)..(n-k+1)] |> List.map bigint
  
  let kFactorial = List.reduce (*) kList
  let deltaFactorial = List.reduce (*) deltaList
  deltaFactorial / kFactorial

let coefficientOfExponentiatedGeometricSeries exponent seriesLength coefficientSlot =
  [0..(coefficientSlot - exponent)/seriesLength]
  |> List.sumBy (fun i -> bigint (pown -1 i ) * (choose exponent i) * (choose (coefficientSlot - i*seriesLength - 1) (exponent - 1)) )

let minimumRoll (dieSize : DiceSize) = 
  1

let maximumRoll dieSize =
  match dieSize with
  | D4 -> 4
  | D6 -> 6
  | D8 -> 8
  | D10 -> 10
  | D12 -> 12
  | D20 -> 20

let rollNotation dieSize =
  match dieSize with
  | D4 -> "d4"
  | D6 -> "d6"
  | D8 -> "d8"
  | D10 -> "d10"
  | D12 -> "d12"
  | D20 -> "d20"

let rollDistribution dieSize rollCount =
  [(rollCount*(minimumRoll dieSize))..(rollCount*(maximumRoll dieSize))]
  |> List.map (fun i -> { Roll = i; Count = coefficientOfExponentiatedGeometricSeries rollCount (maximumRoll dieSize) i })

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
  let mutable buckets = []

  for iBucket in [0..(nBuckets-1)] do
    let mutable slice = []
    let mutable inBucket = bigint 0
    let bucketCap = bucketSizes[iBucket]
    // printfn "****bucket %i****" iBucket

    while inBucket < bucketCap do
      let sum = distribution[iDistribution]
      let next = sum.Count - takenFromDistribution

      if next + inBucket <= bucketCap then
        inBucket <- inBucket + next
        slice <- List.append [{ Roll = sum.Roll; Count = next }] slice
        iDistribution <- iDistribution + 1
        takenFromDistribution <- bigint 0
        // printfn "in: %A sumCount: %A sumRoll: %i iDistribution: %i slice: %A" inBucket sum.Count sum.Roll iDistribution slice
      else
        let takenNow = bucketCap - inBucket
        takenFromDistribution <- takenFromDistribution + takenNow
        inBucket <- bucketCap
        slice <- List.append [{ Roll = sum.Roll; Count = takenNow }] slice
        // printfn "from: %A sumCount: %A sumRoll: %i iDistribution: %i slice: %A" takenFromDistribution sum.Count sum.Roll iDistribution slice

    buckets <- List.append [slice] buckets
    // printfn "Final Bucket Count: %A" (List.sumBy (fun i -> i.Count) slice)
    // printfn "Final Bucket: %A" [slice]
  
  buckets

let chunksToAverages (chunks : RollCount list list) =
  chunks
  |> List.map (fun rollCounts -> 
    List.fold (fun (numerator, denominator) next -> numerator + (next.Count * bigint next.Roll), denominator + next.Count) (bigint 0, bigint 0) rollCounts
    |> fun (x,y) -> float x / float y
  )
  |> List.rev
  |> List.mapi (fun i x -> i + 1, x)