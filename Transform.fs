module PathfinderAnalysis.Transform

open Compare
open System.Collections.Generic
open Bestiary

let pcLevels = [1 .. 20]

let resultsToAverage (resultAndAverageDamageSeq: seq<float * int>) =
  (Seq.sumBy (fun (_, count) -> count) resultAndAverageDamageSeq |> float)
  |> (/) (Seq.sumBy (fun (damage, count) -> damage * (float count)) resultAndAverageDamageSeq)

let rollAveragesToAverage (rollAndAverageDamage: seq<int * float>) =
  Seq.sumBy (fun (_, averageDamage) -> averageDamage) rollAndAverageDamage / float (Seq.length rollAndAverageDamage)

let transformedResultsByRollForLevel resultTransform contested defenseSelector offenseModifier (creaturesByLevel: IDictionary<int, seq<Creature>>) creatureLevelDiff playerLevel =
  transformedResultsByRoll (resultTransform playerLevel) contested defenseSelector (offenseModifier playerLevel) creaturesByLevel[playerLevel + creatureLevelDiff]

let transformedResultsByRollByLevel resultTransform contested defenseSelector offenseModifier (creaturesByLevel: IDictionary<int, seq<Creature>>) creatureLevelDiff =
  pcLevels
  |> Seq.map (fun level -> level, transformedResultsByRollForLevel resultTransform contested defenseSelector offenseModifier creaturesByLevel creatureLevelDiff level)

let resultsByRollByLevelToXyz (rbrbl: seq<int * seq<int * seq<float * int>>>) =
  rbrbl
  |> Seq.collect (fun (level, rbr) -> Seq.map (fun (roll, results) -> level, roll, resultsToAverage results) rbr)

let savingThrowNormalize (level, roll, average) =
  level, 21 - roll, average
