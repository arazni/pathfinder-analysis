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
|> chunk 20
|> chunksToAverages
|> Seq.toArray

// |> mergeRolls (rollDistribution D6 3)
// |> List.map (fun (left) -> { Roll = left.Roll + right.Roll; Count = left.Count + right.Count })
// |> chunk 20
// |> chunksToAverages

[CritSuccess]
|> List.map (Seq.toList << diceFighterShortbow 19)
