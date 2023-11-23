#r "nuget: FSharp.Json"
#r "nuget: Plotly.NET"
#r "nuget: Plotly.NET.ImageExport"

#load "Helpers.fs"
#load "Bestiary.fs"
#load "Compare.fs"
#load "Library.fs"
#load "Transform.fs"

open PathfinderAnalysis.Library
open PathfinderAnalysis.Bestiary
open PathfinderAnalysis.Compare
open PathfinderAnalysis.Transform
open Plotly.NET
open Plotly.NET.ImageExport

let titleFn level = 
  sprintf "Middle Save - Level %i - Spout Cantrip vs. Martial Arbalest Against Level %i Creatures" level (level+2)

[|
  { 
    AveragesByRollsByLevel = transformedResultsByRollByLevel spout CreatureSave middleSave (casterDc true) bestiaryByLevel 2
      |> Seq.map resultRollsToAverages
      |> Seq.map normalizeSavingThrowsForLevel
      |> Seq.toArray;
    Title = "Spout"
  };
  {
    AveragesByRollsByLevel = transformedResultsByRollByLevel martialArbalest PlayerAttack creatureAc (highMartialAttack true) bestiaryByLevel 2
      |> Seq.map resultRollsToAverages
      |> Seq.toArray;
    Title = "Arbalest"
  }
|]
|> flatten
|> generateCharts titleFn
|> Seq.iteri (fun i chart -> Chart.savePNG (path = (sprintf "spout-arbalest-%02i" i)) chart)

// #r "nuget: Plotly.NET.Interactive"
// open Plotly.NET

// resultsByRoll PlayerAttack (fun c -> c.ac) (highMartialAttack false 2) bestiaryByLevel[2]
// |> Seq.toArray
// |> printf "%A"

// transformedResultsByRoll defaultCastMultiplier CreatureSave middleSave (casterDc false 2) bestiaryByLevel[2]
// |> Seq.toArray
// |> printf "%A"

// transformedResultsByRoll (martialShortbow 10) PlayerAttack (fun c -> c.ac) (highMartialAttack false 10) bestiaryByLevel[10]
// |> Seq.toArray
// |> printf "%A"

// transformedResultsByRoll (telekineticProjectile 10) PlayerAttack (fun c -> c.ac) (casterAttack false false 10) bestiaryByLevel[10]
// |> Seq.toArray
// |> printf "%A"

// transformedResultsByRoll (spout 10) CreatureSave middleSave (casterDc true 10) bestiaryByLevel[10]
// |> Seq.map (fun (die: int, resultAndAverageDamageSeq) -> die, resultsToAverage resultAndAverageDamageSeq)
// |> rollAveragesToAverage
// |> printf "%A"

// transformedResultsByRollByLevel spout CreatureSave middleSave (casterDc true) bestiaryByLevel 0
// |> Seq.toArray
// |> printf "%A"

// transformedResultsByRollByLevel spout CreatureSave middleSave (casterDc true) bestiaryByLevel 0
// |> resultsByRollByLevelToXyz
// |> Seq.map savingThrowNormalizeXyz
// |> Seq.toArray
// |> printf "%A"

