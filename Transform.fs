module PathfinderAnalysis.Transform

open Compare
open System.Collections.Generic
open Helpers
open Bestiary
open Plotly.NET

let pcLevels = [1 .. 20]

type ResultDataByRollForLevel<'a> = 
  { Level: int; ResultsByRoll: ResultDataForRoll<'a> seq }

type AverageForRoll =
  { Roll: int; Average: float }

type AverageByRollForLevel =
  { Level: int; AveragesByRoll: AverageForRoll seq }

type ChartSequenceData = 
  { AveragesByRollsByLevel: AverageByRollForLevel seq; Title: string }

type FlatChartData =
  { Title: string; Level: int; AveragesByRoll: AverageForRoll seq }

let toResultDataByRollForLevel (level, resultsForRoll) =
  { Level = level; ResultsByRoll = resultsForRoll }

let resultsToAverage (resultAndAverageDamageSeq: ResultData<float> seq) =
  (Seq.sumBy (fun (x) -> x.Count) resultAndAverageDamageSeq |> float)
  |> (/) (Seq.sumBy (fun x -> x.Result * (float x.Count)) resultAndAverageDamageSeq)

let rollAveragesToAverage (rollAndAverageDamage: AverageForRoll seq) =
  Seq.sumBy (fun x -> x.Average) rollAndAverageDamage / float (Seq.length rollAndAverageDamage)

let transformedResultsByRollForLevel resultTransform contested defenseSelector offenseModifier (creaturesByLevel: Map<int, Creature[]>) creatureLevelDiff playerLevel =
  transformedResultsByRoll (resultTransform playerLevel) contested defenseSelector (offenseModifier playerLevel) creaturesByLevel[playerLevel + creatureLevelDiff]

let transformedResultsByRollByLevel resultTransform contested defenseSelector offenseModifier (creaturesByLevel: Map<int, Creature[]>) creatureLevelDiff =
  pcLevels
  |> Seq.map (fun level -> level, transformedResultsByRollForLevel resultTransform contested defenseSelector offenseModifier creaturesByLevel creatureLevelDiff level)
  |> Seq.map toResultDataByRollForLevel

let normalizeSavingThrowsForLevel (averageByRollForLevel: AverageByRollForLevel) =
  { 
    Level = averageByRollForLevel.Level; 
    AveragesByRoll = Seq.map (fun rdfr -> { Roll = 21 - rdfr.Roll; Average = rdfr.Average }) averageByRollForLevel.AveragesByRoll
  }

let resultRollsToAverages (resultDataByRollForLevel: ResultDataByRollForLevel<float>) =
  {
    Level = resultDataByRollForLevel.Level
    AveragesByRoll = 
      resultDataByRollForLevel.ResultsByRoll
      |> Seq.map (fun resultDataForRoll -> { Roll = resultDataForRoll.Roll; Average = resultsToAverage resultDataForRoll.Results } ) 
  }

let flatten (chartDataSequence: ChartSequenceData seq) =
  chartDataSequence
  |> Seq.collect (fun x -> 
    x.AveragesByRollsByLevel
    |> Seq.map (fun y -> { Title = x.Title; Level = y.Level; AveragesByRoll = y.AveragesByRoll })
  )

let mergeRollAverages (leftAveragesForRolls: AverageForRoll seq) (rightAveragesForRolls: AverageForRoll seq) =
  rightAveragesForRolls
  |> Seq.allPairs leftAveragesForRolls
  |> Seq.map (fun (l, r) -> l.Average + r.Average)
  |> Seq.sort
  |> Seq.mapi (fun i sum -> sum, i / 20 + 1)
  |> Seq.groupBy second
  |> Seq.map (fun (rollLuck, sums) -> {
    Roll = rollLuck;
    Average = Seq.sumBy (fun (sum, _) -> sum) sums / 20.0
  })

let mergeRollAveragesByLevel (left: AverageByRollForLevel seq) (right: AverageByRollForLevel seq) =
  right
  |> Seq.zip left
  |> Seq.map (fun (l, r) -> {
    Level = l.Level;
    AveragesByRoll = mergeRollAverages l.AveragesByRoll r.AveragesByRoll
  })

let generateCharts (titleFn: int -> string) flatChartData =
  flatChartData
  |> Seq.groupBy (fun flatData -> flatData.Level)
  |> Seq.map (fun (level, flatDataList) -> 
    flatDataList
    |> Seq.map (fun flatData -> 
      Chart.Line(
        xy = Seq.map(fun abr -> (abr.Roll, abr.Average)) flatData.AveragesByRoll,
        ShowMarkers = true,
        Name = sprintf "%s %.1f" flatData.Title (rollAveragesToAverage flatData.AveragesByRoll)
      )
    )
    |> Chart.combine
    |> Chart.withLineStyle(Shape = StyleParam.Shape.Hvh)
    |> Chart.withXAxisStyle("Player Roll Luck")
    |> Chart.withYAxisStyle("Average Damage")
    // |> Chart.withTitle (sprintf "Middle Save - Level %i - Spout Cantrip vs. Martial Arbalest Against Level %i Creatures" level (level+2))
    |> Chart.withTitle (titleFn level)
    |> Chart.withSize (800.0, 800.0)
  )

let chunksToFlatChartData title level (buckets: (int*float)[]) =
  {
    Title = title;
    Level = level;
    AveragesByRoll = 
      buckets
      |> Seq.map (fun (rollLuck, damage) -> { Roll = rollLuck; Average = damage })
  }

let chunksByLevelToFlatChartData title (bucketsByLevel: (int * float)[] seq) =
  bucketsByLevel
  |> Seq.mapi (fun i buckets -> chunksToFlatChartData title (i+1) buckets)

// let resultsByRollByLevelToXyz (rbrbl: seq<int * seq<int * seq<float * int>>>) =
//   rbrbl
//   |> Seq.collect (fun (level, rbr) -> Seq.map (fun (roll, results) -> level, roll, resultsToAverage results) rbr)

// let resultsByRollByLevelToRollsAndAveragesByLevel (rbrbl: seq<int * seq<int * seq<float * int>>>) =
//   rbrbl
//   |> Seq.map (fun (level, rbr) -> level, Seq.map (fun (roll, results) -> roll, resultsToAverage results) rbr)

// let savingThrowNormalizeXyz (level, roll, average) =
//   level, 21 - roll, average

// let savingThrowNormalizeRollsAndAveragesByLevel (raabl) =
//   raabl
//   |> Seq.map (fun (level, rab) -> level, Seq.map (fun (roll, average) -> (21 - roll, average)) rab)
  
// type ResultDataForRoll<'a> = { Roll: int; Results: (ResultData<'a> seq) }
// type ResultData<'a> = { Result: 'a; Count: int }
// type HitResult = CritFail | Fail | Success | CritSuccess | CritWithImmunity
// type DamageCount = { Damage: float; Count: bigint }
// need to combine ResultDataForRoll<HitResult> with DamageCount, multiplicatively, bucket them to 20 stacks, then extrapolate to each level and do the graph stuff

