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

// rollDistributions 0 [D12, 4; D6, 3;]
// |> Seq.toList
// |> chunkRolls 20
// |> chunksToAverages
// |> Seq.toArray

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

// let test1 = 
//   transformedResultsByRollForLevel diceFighterShortbow PlayerAttack creatureAc (highFighterAttack true) bestiaryByLevel 2 20
//   |> Seq.collect (fun result -> result.Results)
//   |> Seq.collect (fun resultData -> Seq.map (fun damageCount -> { Damage = damageCount.Damage; Count = damageCount.Count * bigint resultData.Count } ) resultData.Result)
//   |> Seq.groupBy damageCountDamage
//   |> Seq.map (fun (key, damageCounts) -> { Damage = key; Count = Seq.sumBy damageCountCount damageCounts })
//   |> Seq.sortBy damageCountDamage
//   |> Seq.toList
//   |> chunkDamage 20
//   |> damageChunksToAverages
//   |> Seq.toList

// let test2 = 
//   transformedResultsByRollForLevel diceDamageTempestSurge CreatureSave middleSave (casterDc true) bestiaryByLevel 2 20
//   |> Seq.collect (fun result -> result.Results)
//   |> Seq.collect (fun resultData -> Seq.map (fun damageCount -> { Damage = damageCount.Damage; Count = damageCount.Count * bigint resultData.Count } ) resultData.Result)
//   |> Seq.groupBy damageCountDamage
//   |> Seq.map (fun (key, damageCounts) -> { Damage = key; Count = Seq.sumBy damageCountCount damageCounts })
//   |> Seq.sortBy damageCountDamage
//   |> Seq.toList
//   |> chunkDamage 20
//   |> damageChunksToAverages
//   |> Seq.toList

type DamageContext = {
  HitRollCount: int;
  HitModifier: int;
  Contest: Contested;
  CreatureDefenseFunction: Creature -> int;
  DamageFunction: HitResult -> DamageCount seq;
}

let pcLevel = 1
let creatureLevel = 1

let damageContexts = seq {
  { HitRollCount = 1; HitModifier = highFighterAttack true pcLevel; Contest = PlayerAttack; CreatureDefenseFunction = creatureAc; DamageFunction = diceFighterLongbow pcLevel};
  { HitRollCount = 1; HitModifier = highFighterAttack true pcLevel - 5; Contest = PlayerAttack; CreatureDefenseFunction = creatureAc; DamageFunction = diceFighterLongbow pcLevel};
  { HitRollCount = 2; HitModifier = casterDc true pcLevel; Contest = PlayerAttack; CreatureDefenseFunction = middleSave; DamageFunction = diceDamageSpout pcLevel}
}

let allPossibleTurnHitRolls = 
  damageContexts
  |> Seq.sumBy (fun context -> context.HitRollCount)
  |> allD20Rolls
  |> permuteDiceRolls
  // |> Seq.toList

allPossibleTurnHitRolls
|> Seq.map (fun turnHitRolls ->
  let flatContexts = 
    damageContexts
    |> Seq.collect (fun context -> Seq.replicate context.HitRollCount context)

  (turnHitRolls, flatContexts)
  ||> Seq.map2 (fun hitRoll flatContext -> resultsForRoll PlayerAttack creatureAc flatContext.HitModifier bestiaryByLevel[creatureLevel] hitRoll)
)

// let attackModifiers = [
//   highFighterAttack true pcLevel;
//   highFighterAttack true pcLevel - 5
// ]

// allD20Rolls (List.length attackModifiers)
// |> permuteDiceRolls
// |> Seq.map (fun rolls ->
//   rolls
//   |> Seq.mapi (fun attackIndex roll -> resultsForRoll PlayerAttack creatureAc attackModifiers[attackIndex] bestiaryByLevel[creatureLevel] roll)
//   |> Seq.toList
// )
// |> Seq.toList

// // |> Seq.map (fun (modifier, rolls) -> Seq.map (fun roll -> roll, resultsForRoll PlayerAttack creatureAc modifier bestiaryByLevel[creatureLevel]) rolls)
// // |> Seq.map (fun rolls -> Seq.map (fun roll -> roll, resultsForRoll PlayerAttack creatureAc 0 bestiaryByLevel[1] roll) rolls)

// diceFighterLongbow pcLevel