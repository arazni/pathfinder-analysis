// #r "nuget: FSharp.Json"
// #r "nuget: Plotly.NET"
// #r "nuget: Plotly.NET.ImageExport"

#load "Helpers.fs"
// #load "Bestiary.fs"
// #load "Compare.fs"
// #load "Library.fs"
// #load "Transform.fs"
// #load "DamageDistribution.fs"
#load "Request.fs"
open PathfinderAnalysis.Helpers
open Request.Guimard

// open PathfinderAnalysis.Library
// open PathfinderAnalysis.Bestiary
// open PathfinderAnalysis.Compare
// open PathfinderAnalysis.Transform
// open PathfinderAnalysis.DamageDistribution
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

diceValues D4
|> List.map (fun x -> [x])
|> pairUp2 (diceValues D4)
// |> pairUp2 (diceValues D4)
|> List.map evaluateRoll
|> List.groupBy selfFn
|> List.map (fun (left, right) -> left, List.length right)

// // |> List.length

requestWork D4 2
|> List.groupBy selfFn

type DataPrep = {
  ValuesChanceAndKeysDiceRolled: (float * int) list;
  NameResult: string;
}

type Data = {
  Values: float list;
  Keys: int list;
  Name: string;
}

let work = 
  [1..6]
  |> List.map (fun x -> x, requestWork D6 x)

[Success; SuccessWithComplication; PartialSuccess; PartialSuccessWithComplication; Failure]
|> List.map (fun result -> 
  List.fold (fun state diceAndResultChances ->
    List.where (fun resultChance -> resultChance.Result = result) (second diceAndResultChances)
    |> List.tryExactlyOne
    |> fun resultChance -> 
      if Option.isNone resultChance then state 
      else List.append state [((Option.get resultChance).Chance, first diceAndResultChances)]
    ) [] work
  |> fun pairs -> { NameResult = string result; ValuesChanceAndKeysDiceRolled = pairs })
  |> List.map (fun preps -> { Name = preps.NameResult; Values = List.map first preps.ValuesChanceAndKeysDiceRolled; Keys = List.map second preps.ValuesChanceAndKeysDiceRolled })

// List.where (fun (dice, resultChances) -> 
    // List.where (fun resultChance -> resultChance.Result = result) resultChances) work
  