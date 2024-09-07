module PathfinderAnalysis.DamageDistribution

// https://www.analyticscheck.net/posts/sums-dice-rolls

open PathfinderAnalysis.Helpers

type DamageCount = { Damage: float; Count: bigint }

type RollCount = { Roll: int; Count: bigint }

type DicePool = (int * DiceSize) seq

let rollCountCount rollCount =
  rollCount.Count

let damageCountCount (damageCount : DamageCount) =
  damageCount.Count

let rollCountRoll rollCount =
  rollCount.Roll

let damageCountDamage damageCount =
  damageCount.Damage

let rollDistribution dieSize rollCount =
  [(rollCount*(minimumRoll dieSize))..(rollCount*(maximumRoll dieSize))]
  |> Seq.map (fun i -> { Roll = i; Count = coefficientOfExponentiatedGeometricSeries rollCount (maximumRoll dieSize) i })

let applyModifierToDistribution modifier rollCounts =
  if modifier = 0 then rollCounts else
  rollCounts
  |> Seq.map (fun rollCount -> { Roll = modifier + rollCount.Roll; Count = rollCount.Count })

let rollCountsToDamageDice rollCounts =
  rollCounts
  |> Seq.map (fun rollCount -> { Damage = float rollCount.Roll; Count = rollCount.Count })

let applyDamageModifierToRollCount modifier rollCounts =
  rollCounts
  |> Seq.map (fun rollCount -> { Damage = modifier + float rollCount.Roll; Count = rollCount.Count })

let applyDamageFunctionToDamageDice modifierFunction modifier damageCounts =
  damageCounts
  |> Seq.map (fun damageCount -> { Damage = modifierFunction modifier damageCount.Damage; Count = damageCount.Count })

let rollDistributions modifier dicePool =
  let workingPool = 
    dicePool
    |> Seq.filter (fun (_, pool) -> pool > 0)
    |> Seq.groupBy first
    |> Seq.map (fun (group, dice) -> group, Seq.sumBy second dice)

  if Seq.length workingPool = 0
  then Seq.empty
  else

  if Seq.length workingPool = 1 
  then rollDistribution (Seq.head workingPool |> first) (Seq.head workingPool |> second)
    |> applyModifierToDistribution modifier
  else

  let rolls = 
    Seq.map (fun (size, number) -> rollDistribution size number) workingPool
  
  (Seq.head rolls |> applyModifierToDistribution modifier, Seq.tail rolls)
  ||> Seq.fold (fun state rollCountList -> 
    Seq.allPairs state rollCountList
    |> Seq.map (fun (left, right) -> { Roll = left.Roll + right.Roll; Count = left.Count * right.Count })
    )
  |> Seq.groupBy (fun rollCount -> rollCount.Roll)
  |> Seq.map (fun (roll, rollCounts) -> { Roll = roll; Count = Seq.sumBy (fun counts -> counts.Count) rollCounts }) // bigint (Seq.length workingPool) })
  |> Seq.sortBy (fun rollCount -> rollCount.Roll )

let getBucketSizes (nBuckets: int) (total: bigint) =
  let bucketSize = total / bigint nBuckets
  let remainder = total % bigint nBuckets
  let halfRemainder = remainder / bigint 2
  let isOdd = remainder % bigint 2

  [1..nBuckets]
  |> List.map bigint
  |> List.map (fun i -> bucketSize + if i <= halfRemainder + isOdd || i > bigint nBuckets - halfRemainder then bigint 1 else bigint 0)

let chunkAddRollBucket sum count =
  { Roll = sum.Roll; Count = count; }

let chunkAddDamageBucket sum count = 
  { Damage = sum.Damage; Count = count; }

let chunk chunkBucketAdder countGetter nBuckets distribution =
  let totalPermutations =
    distribution
    |> List.sumBy countGetter
  
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
      let next = countGetter sum - takenFromDistribution

      if next + inBucket <= bucketCap then
        inBucket <- inBucket + next
        slice <- Seq.append [chunkBucketAdder sum next] slice
        iDistribution <- iDistribution + 1
        takenFromDistribution <- bigint 0
        // printfn "in: %A sumCount: %A sumRoll: %i iDistribution: %i slice: %A" inBucket sum.Count sum.Roll iDistribution slice
      else
        let takenNow = bucketCap - inBucket
        takenFromDistribution <- takenFromDistribution + takenNow
        inBucket <- bucketCap
        slice <- Seq.append [chunkBucketAdder sum takenNow] slice
        // printfn "from: %A sumCount: %A sumRoll: %i iDistribution: %i slice: %A" takenFromDistribution sum.Count sum.Roll iDistribution slice

    buckets <- Seq.append [slice] buckets
    // printfn "Final Bucket Count: %A" (List.sumBy (fun i -> i.Count) slice)
    // printfn "Final Bucket: %A" [slice]
  
  buckets

let chunkRolls =
  chunk chunkAddRollBucket rollCountCount

let chunkDamage =
  chunk chunkAddDamageBucket damageCountCount

let chunksToAverages (chunks : RollCount seq seq) =
  chunks
  |> Seq.map (fun rollCounts -> 
    Seq.fold (fun (numerator, denominator) next -> numerator + (next.Count * bigint next.Roll), denominator + next.Count) (bigint 0, bigint 0) rollCounts
    |> fun (x, y) -> float x / float y
  )
  |> Seq.rev
  |> Seq.mapi (fun i x -> i + 1, x)

let damageChunksToAverages (chunks: DamageCount seq seq) =
  chunks
  |> Seq.map (fun damageCounts -> 
    Seq.fold (fun (numerator, denominator) (next: DamageCount) -> numerator + float next.Count * next.Damage, denominator + float next.Count) (0.0, 0.0) damageCounts
    |> fun (x, y) -> x / y
  )
  |> Seq.rev
  |> Seq.mapi (fun i x -> i + 1, x)

// let permuteDiceRollsWithModifier dieSize rolls rollModifier =
//   let allRolls = seq { rollModifier (minimumRoll dieSize).. rollModifier (maximumRoll dieSize) }

//   if rolls = 1 then seq { allRolls } else

//   let initial =
//     (allRolls, allRolls)
//     ||> Seq.allPairs
//     |> Seq.map (fun (a, b) -> seq {a; b})

//   if rolls = 2 then initial else

//   seq { 1 .. rolls - 2}
//   |> Seq.fold (fun state _ -> 
//     Seq.allPairs allRolls state
//     |> Seq.map (fun (a, b) -> Seq.append b (seq { a }))) initial

let allRolls dieSize rollModifier =
  List.toSeq [rollModifier (minimumRoll dieSize)..rollModifier (maximumRoll dieSize)]

let allD20Rolls numberOfRolls =
  [1..numberOfRolls]
  |> Seq.map (fun _ -> allRolls D20 selfFn)

let permuteDiceRolls (allDiceRolls: 'a seq seq) =
  if Seq.length allDiceRolls = 1 then allDiceRolls else

  let initial =
    (Seq.head allDiceRolls, Seq.skip 1 allDiceRolls |> Seq.head)
    ||> Seq.allPairs
    |> Seq.map (fun (a, b) -> seq {a; b})

  if Seq.length allDiceRolls = 2 then initial else

  Seq.skip 2 allDiceRolls
  |> Seq.fold (fun state allRolls -> 
    Seq.allPairs state allRolls
    |> Seq.map (fun (a, b) -> Seq.append a (seq { b }))) initial

let totalPermutationsForDicePool (dicePool: DicePool) =
  dicePool
  |> Seq.fold (fun state (count, size) -> state * pown (bigint (maximumRoll size)) count ) (bigint 1)

let totalPermutationForDicePools (dicePools: DicePool seq) =
  dicePools
  |> Seq.fold (fun state pool -> state * totalPermutationsForDicePool pool) (bigint 1)

