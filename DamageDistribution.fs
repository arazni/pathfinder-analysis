module PathfinderAnalysis.DamageDistribution

// https://www.analyticscheck.net/posts/sums-dice-rolls

open PathfinderAnalysis.Helpers
open PathfinderAnalysis.Bestiary
open PathfinderAnalysis.Compare

type DamageCount = { Damage: float; Count: bigint }

type RollCount = { Roll: int; Count: bigint }

type DicePool = (int * DiceSize) seq

type DamageContext = {
  HitRollCount: int;
  HitModifier: int;
  Contest: Contested;
  CreatureDefenseFunction: Creature -> int;
  DamageFunction: HitResult -> DamageCount seq;
}

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

let allRolls dieSize rollModifier =
  seq { rollModifier (minimumRoll dieSize)..rollModifier (maximumRoll dieSize) }

let allD20Rolls modifierFunction numberOfRolls =
  seq { 1..numberOfRolls }
  |> Seq.map (fun _ -> allRolls D20 modifierFunction)

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

let sumHitResultsForDamageContext totalCreatures (damageContext: DamageContext) (resultsSequence: ResultData<HitResult> seq seq) =
  resultsSequence
  |> Seq.collect selfFn
  |> Seq.groupBy (fun result -> result.Result)
  |> Seq.map (fun (key, group) -> { Result = key; Count = Seq.sumBy (fun (result: ResultData<HitResult>) -> result.Count) group })
  |> Seq.map (fun resultCount -> 
    damageContext.DamageFunction resultCount.Result
    |> Seq.map (fun damageCount -> { Damage = damageCount.Damage * float resultCount.Count; Count = damageCount.Count })
  )
  |> Seq.reduce (fun state next ->
    Seq.map2 (fun left right -> { Damage = left.Damage + right.Damage; Count = left.Count; }) state next)
  |> Seq.map (fun damageCount -> { Damage = damageCount.Damage / float totalCreatures; Count = damageCount.Count })

let combineDamageCountsFromDamageContexts (damageCountsForTurn: DamageCount seq seq) =
  damageCountsForTurn
  |> Seq.reduce (fun state next ->
    Seq.allPairs state next
    |> Seq.map (fun (left, right) -> { Damage = left.Damage + right.Damage; Count = left.Count * right.Count })
  )

let hitResultLookup creatures (damageContexts : DamageContext[]) = 
  let len = 
    damageContexts
    |> Seq.sumBy (fun dContext -> dContext.HitRollCount)

  let flatContexts =
    damageContexts
    |> Seq.collect (fun context -> Seq.replicate context.HitRollCount context)
    |> Seq.toArray
  
  seq { 0..len-1 }
  |> Seq.map (fun i -> i, seq { 1..20 })
  |> Seq.map (fun (i, rolls) -> 
    rolls
    |> Seq.map (fun roll -> 
      resultsForRoll flatContexts[i].Contest flatContexts[i].CreatureDefenseFunction flatContexts[i].HitModifier creatures roll
      |> Seq.toList
    )
    |> Seq.toArray
  )
  |> Seq.toArray

let damageResultLookup (damageContexts: DamageContext[]) =
  let allResults = [| CritFail; Fail; Success; CritWithImmunity; CritSuccess |]
  damageContexts
  |> Seq.map (fun dc -> dc.DamageFunction)
  |> Seq.map (fun df -> 
    allResults
    |> Seq.map (fun result -> result, df result |> Seq.toList)
    |> Map
  )
  |> Seq.toArray

let hitDamageIndexPairs (damageContexts: DamageContext[]) =
    damageContexts
  |> Seq.mapi (fun damageIndex context -> damageIndex, context.HitRollCount)
  |> Seq.collect (fun (damageIndex, hitRollCount) -> Seq.replicate hitRollCount damageIndex)
  |> Seq.mapi (fun hitIndex damageIndex -> hitIndex, damageIndex)
  |> Seq.toArray

let hitDamageLookup creatures (damageContexts: DamageContext[]) =
  let allHitIndexes = [| 0..19 |]
  let hitLookup = hitResultLookup creatures damageContexts
  let damageLookup = damageResultLookup damageContexts
  let creatureCount = Array.length creatures |> float

  let indexPairs = hitDamageIndexPairs damageContexts

  indexPairs
  |> Seq.map (fun (hitIndex, damageIndex) -> 
    allHitIndexes
    |> Seq.map (fun roll ->
      let damageCountsPerResult = 
        hitLookup[hitIndex][roll]
        |> Seq.map (fun hitResultCount -> { Result = damageLookup[damageIndex][hitResultCount.Result]; Count = hitResultCount.Count })
        |> Seq.map (fun damageResultCount -> 
          damageResultCount.Result
          |> Seq.map (fun damageCount -> { Damage = damageCount.Damage * float damageResultCount.Count; Count = damageCount.Count })
          |> Seq.toArray
        )

      seq { 0..(Seq.head damageCountsPerResult |> Array.length) - 1}
      |> Seq.map (fun i ->
        {
          Damage = 
            damageCountsPerResult 
            |> Seq.sumBy (fun damageCounts -> damageCounts[i].Damage)
            |> divideByFirst creatureCount;
          Count = ((Seq.head damageCountsPerResult)[i]).Count
        })
      |> Seq.toArray
    )
    |> Seq.toArray
  )
  |> Seq.toArray

let damageCountsFromHitDamageLookup (hitDamageLookup: DamageCount[][][]) hitRollIndex rollValueIndex =
  hitDamageLookup[hitRollIndex][rollValueIndex]

let theBigThing creatures (damageContexts: DamageContext[]) =
  let hitRollsPerTurn =
    damageContexts
    |> Seq.sumBy (fun context -> context.HitRollCount)

  let allHitRollsPerTurns =
    hitRollsPerTurn
    |> allD20Rolls ((+) -1)
    |> permuteDiceRolls
    |> Seq.map Seq.toArray
    |> Seq.toArray

  let lookup = hitDamageLookup creatures damageContexts

  let indexPairs = hitDamageIndexPairs damageContexts

  allHitRollsPerTurns
  |> Seq.map (fun rollValueIndexesForTurn ->
    rollValueIndexesForTurn
    |> Seq.mapi (fun rollIndex rollValueIndex -> second indexPairs[rollIndex], damageCountsFromHitDamageLookup lookup rollIndex rollValueIndex)
    |> Seq.groupBy first
    |> Seq.map (fun (_, damageCountPairs) ->
      let damageCountsForSingleDamageRoll = Seq.map second damageCountPairs

      Seq.head damageCountsForSingleDamageRoll
      |> Seq.mapi (fun damageIndex damageRoll -> { 
        Damage = damageCountsForSingleDamageRoll |> Seq.sumBy (fun x -> x[damageIndex].Damage)
        Count = damageRoll.Count
      })
    )
    |> combineDamageCountsFromDamageContexts
  )
  |> Seq.collect selfFn
  |> Seq.groupBy damageCountDamage
  |> Seq.sortBy first
  |> Seq.map (fun (damage, damageCounts) -> { Damage = damage; Count = damageCounts |> Seq.sumBy damageCountCount })
  |> Seq.toList
  |> chunkDamage 20
  |> damageChunksToAverages
