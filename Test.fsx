#r "nuget: FSharp.Json"
#r "nuget: Plotly.NET"
// #r "nuget: Plotly.NET.ImageExport"

#load "Helpers.fs"
#load "Bestiary.fs"
#load "Compare.fs"
#load "DamageDistribution.fs"
#load "Library.fs"
#load "Transform.fs"
// #load "Request.fs"

open PathfinderAnalysis.Helpers
open PathfinderAnalysis.Library
open PathfinderAnalysis.Bestiary
open PathfinderAnalysis.Compare
open PathfinderAnalysis.Transform
open PathfinderAnalysis.DamageDistribution
// open Plotly.NET
//open Plotly.NET.ImageExport

// let titleFn level = 
//   sprintf "Middle Save - Level %i - Spout Cantrip vs. Martial Arbalest Against Level %i Creatures" level (level+2)

// [|
//   { 
//     AveragesByRollsByLevel = transformedResultsByRollByLevel spout CreatureSave middleSave (casterDc true) bestiaryByLevel 2
//       |> Seq.map resultRollsToAverages
//       |> Seq.map normalizeSavingThrowsForLevel
//       |> Seq.toArray;
//     Title = "Spout"
//   };
//   {
//     AveragesByRollsByLevel = transformedResultsByRollByLevel martialArbalest PlayerAttack creatureAc (highMartialAttack true) bestiaryByLevel 2
//       |> Seq.map resultRollsToAverages
//       |> Seq.toArray;
//     Title = "Arbalest"
//   }
// |]
// |> flatten
// |> generateCharts titleFn
// |> Seq.iteri (fun i chart -> Chart.savePNG (path = (sprintf "spout-arbalest-%02i" i), Width = 800, Height = 800) chart)

// transformedResultsByRollByLevel martialShortbow PlayerAttack creatureAc ((+) -5 << highMartialAttack true) bestiaryByLevel 2
// |> Seq.map resultRollsToAverages
// |> Seq.toArray
// |> mergeRollAveragesByLevel <| (transformedResultsByRollByLevel martialShortbow PlayerAttack creatureAc (highMartialAttack true) bestiaryByLevel 2
//   |> Seq.map resultRollsToAverages
//   |> Seq.toArray)

// rollDistribution D6 4
// |> chunk 20
// |> chunksToAverages

rollDistributions 0 [D12, 4; D6, 3;]
|> Seq.toList
|> chunkRolls 20
|> chunksToAverages
|> Seq.toArray

// |> mergeRolls (rollDistribution D6 3)
// |> List.map (fun (left) -> { Roll = left.Roll + right.Roll; Count = left.Count + right.Count })
// |> chunk 20
// |> chunksToAverages

// [CritSuccess]
// |> List.map (Seq.toList << diceFighterShortbow 19)
// |> chunkDamage 20

// diceFighterShortbow 19 Success
// |> Seq.toList
// |> chunkDamage 20
// |> damageChunksToAverages
// |> Seq.toList


// the below looks good for a single roll to hit, need to handle 2 rolls
// idea: apply transform to a sequence of hit rolls

let test1 = 
  transformedResultsByRollForLevel diceFighterShortbow PlayerAttack creatureAc (highFighterAttack true) bestiaryByLevel 2 20
  |> Seq.collect (fun result -> result.Results)
  |> Seq.collect (fun resultData -> Seq.map (fun damageCount -> { Damage = damageCount.Damage; Count = damageCount.Count * bigint resultData.Count } ) resultData.Result)
  |> Seq.groupBy damageCountDamage
  |> Seq.map (fun (key, damageCounts) -> { Damage = key; Count = Seq.sumBy damageCountCount damageCounts })
  |> Seq.sortBy damageCountDamage
  |> Seq.toList
  |> chunkDamage 20
  |> damageChunksToAverages
  |> Seq.toList

let test2 = 
  transformedResultsByRollForLevel diceDamageTempestSurge CreatureSave middleSave (casterDc true) bestiaryByLevel 2 20
  |> Seq.collect (fun result -> result.Results)
  |> Seq.collect (fun resultData -> Seq.map (fun damageCount -> { Damage = damageCount.Damage; Count = damageCount.Count * bigint resultData.Count } ) resultData.Result)
  |> Seq.groupBy damageCountDamage
  |> Seq.map (fun (key, damageCounts) -> { Damage = key; Count = Seq.sumBy damageCountCount damageCounts })
  |> Seq.sortBy damageCountDamage
  |> Seq.toList
  |> chunkDamage 20
  |> damageChunksToAverages
  |> Seq.toList

type HitDamagePair = {
  HitRolls: int * DiceSize;
  DamagePool: DicePool;
}

// let allCombinations (input: HitDamagePair) =
//   Seq.map (fun x -> x) input.HitRolls,
//   rollDistributions 0 input.DamagePool

// ([1..3],[1..3]) 
// ||> Seq.allPairs 
// |> Seq.map (fun x -> seq {first x; second x}) 
// |> Seq.allPairs [1..3] 
// |> Seq.map (fun x -> Seq.append (seq { (first x) }) (second x)) 
// |> Seq.toArray;;

// type Untuple<'a> =
//   | Pairing of 'a * 'a
//   | PairList of 'a * ('a list)

// let innerHandle x =
//   match x with
//   | Pairing (a, b) -> [a; b]
//   | PairList (a, b) -> a::b

// ([1..3],[1..3])
// ||> Seq.allPairs
// |> Seq.map Pairing
// |> Seq.map innerHandle
// |> Seq.allPairs [1..3] 
// |> Seq.map PairList
// |> Seq.map innerHandle
// |> Seq.toArray;;

let permuteDiceRollsWithModifier dieSize rolls rollModifier =
  let allRolls = seq { rollModifier (minimumRoll dieSize).. rollModifier (maximumRoll dieSize) }

  if rolls = 1 then seq { allRolls } else

  let initial =
    (allRolls, allRolls)
    ||> Seq.allPairs
    |> Seq.map (fun (a, b) -> seq {a; b})

  if rolls = 2 then initial else

  seq { 1 .. rolls - 2}
  |> Seq.fold (fun state _ -> 
    Seq.allPairs allRolls state
    |> Seq.map (fun (a, b) -> Seq.append b (seq { a }))) initial

let allRolls dieSize rollModifier =
  List.toSeq [rollModifier (minimumRoll dieSize)..rollModifier (maximumRoll dieSize)]

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

let totalPermutations1 (dicePool: DicePool) =
  dicePool
  |> Seq.fold (fun state (count, size) -> state * pown (bigint (maximumRoll size)) count ) (bigint 1)

let totalPermutations (dicePools: DicePool seq) =
  dicePools
  |> Seq.fold (fun state pool -> state * totalPermutations1 pool) (bigint 1)

