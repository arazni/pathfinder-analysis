module PathfinderAnalysis.ChartTools

open Helpers
open Bestiary
open Compare
open Library
open Transform
open DamageDistribution
open Plotly.NET

let chartWidth = 1400
let chartHeight = 800

type SaveSelector =
  | Lowest
  | Middle
  | Highest
  | Reflex
  | Will
  | Fortitude

let saveSelectorFunction saveSelector =
  match saveSelector with
  | Lowest -> lowestSave
  | Middle -> middleSave
  | Highest -> highestSave
  | Reflex -> reflexSave
  | Will -> willSave
  | Fortitude -> fortitudeSave

let saveSelectorText saveSelector =
  match saveSelector with
  | Lowest -> "Lowest"
  | Middle -> "Middle"
  | Highest -> "Highest"
  | Reflex -> "Reflex"
  | Will -> "Will"
  | Fortitude -> "Fortitude"

let generateLevelScaleChart (title: string) flatChartData =
  flatChartData
  |> Seq.groupBy (fun flatData -> flatData.Title)
  |> Seq.map (fun (title, dataForTitle) ->
    title,
    dataForTitle
    |> Seq.groupBy (fun data -> data.Level)
    |> Seq.map (fun (level, dataForLevel) ->
      level,
      dataForLevel
      |> Seq.sumBy (fun data -> data.AveragesByRoll |> Seq.sumBy (fun averageForRoll -> averageForRoll.Average))
      |> divideByFirst 20.0 // 20 dice sides
    )
  )
  |> Seq.map (fun (title, xys) -> 
    Chart.Line(
      xy = xys,
      ShowMarkers = true,
      Name = sprintf "%s %.1f" title (xys |> Seq.sumBy second |> divideByFirst 20.0) // 20 levels
    )
  )
  |> Chart.combine
  |> Chart.withLineStyle(Shape = StyleParam.Shape.Hvh)
  |> Chart.withXAxisStyle "Level"
  |> Chart.withYAxisStyle "Average Damage of Average Roll Luck"
  |> Chart.withTitle title
  |> Chart.withSize(chartWidth, chartHeight)

let generateCharts (titleFn: int -> string) flatChartData =
  flatChartData
  |> Seq.groupBy (fun flatData -> flatData.Level)
  |> Seq.map (fun (level, flatDataList) ->
    flatDataList
    |> Seq.map (fun flatData -> 
      Chart.Line(
        xy = (flatData.AveragesByRoll |> Seq.map(fun abr -> abr.Roll, abr.Average)),
        ShowMarkers = true,
        Name = sprintf "%s %.1f" flatData.Title (rollAveragesToAverage flatData.AveragesByRoll))
      )
    |> Chart.combine
    |> Chart.withLineStyle(Shape = StyleParam.Shape.Hvh)
    |> Chart.withXAxisStyle("Player Roll Luck")
    |> Chart.withYAxisStyle("Average Damage")
    |> Chart.withTitle (titleFn level)
    |> Chart.withSize(chartWidth, chartHeight)
  )

let generateChartsTTK (titleFn: int -> string) creatureLevelBump flatChartData =
  flatChartData
  |> Seq.groupBy (fun flatData -> flatData.Level)
  |> Seq.map (fun (level, flatDataList) ->
    let hp = averageHp bestiaryByLevel[level + creatureLevelBump]
    flatDataList
    |> Seq.map (fun flatData -> 
      Chart.Line(
        xy = (flatData.AveragesByRoll |> Seq.map(fun abr -> abr.Roll, hp / abr.Average )),
        ShowMarkers = true,
        Name = sprintf "%s %.1f" flatData.Title (hp / rollAveragesToAverage flatData.AveragesByRoll))
      )
    |> Chart.combine
    // |> Chart.withLineStyle(Shape = StyleParam.Shape.Hvh)
    |> Chart.withXAxisStyle
      (
        "Player Roll Luck"
      )
    |> Chart.withYAxisStyle
      (
        "Times to KO"
        , AxisType = StyleParam.AxisType.Log
        , MinMax = (1.30103, 0.0)
      )
    |> Chart.withTitle (titleFn level)
    |> Chart.withSize(chartWidth, chartHeight)
  )

let generateChartsPercent (titleFn: int -> string) creatureLevelBump flatChartData =
  flatChartData
  |> Seq.groupBy (fun flatData -> flatData.Level)
  |> Seq.map (fun (level, flatDataList) ->
    let hp = averageHp bestiaryByLevel[level + creatureLevelBump]
    flatDataList
    |> Seq.map (fun flatData -> 
      Chart.Line(
        xy = (flatData.AveragesByRoll |> Seq.map(fun abr -> abr.Roll, 100.0 * abr.Average / hp )),
        ShowMarkers = true,
        Name = sprintf "%s %.1f%%" flatData.Title (100.0 * rollAveragesToAverage flatData.AveragesByRoll / hp))
      )
    |> Chart.combine
    |> Chart.withLineStyle(Shape = StyleParam.Shape.Hvh)
    |> Chart.withXAxisStyle("Player Roll Luck")
    |> Chart.withYAxisStyle
      ("% of Average Max HP"
      //  AxisType = StyleParam.AxisType.Log
      , MinMax = (0, 100)
      )
    |> Chart.withTitle (titleFn level)
    |> Chart.withSize(chartWidth, chartHeight)
  )

let generateTTKHistogram (titleFn: int -> string) creatureLevelBump flatChartData =
  flatChartData
  |> Seq.groupBy (fun flatData -> flatData.Level)
  |> Seq.map (fun (level, flatDataList) ->
    let hp = averageHp bestiaryByLevel[level + creatureLevelBump]
    flatDataList
    |> Seq.map (fun flatData -> 
      Chart.Histogram(
        Seq.map (fun abr -> System.Math.Clamp(hp / abr.Average |> ceil |> int |> (fun x -> if x < 0 then 100000 else x), 1, 13)) flatData.AveragesByRoll,
        Name = sprintf "%s %.1f" flatData.Title (hp / rollAveragesToAverage flatData.AveragesByRoll),
        NBinsX = 13
      )
    )
    |> Chart.combine
    |> Chart.withXAxisStyle("Times to KO", MinMax = (1, 13))
    |> Chart.withYAxisStyle("Count of Turn Roll Lucks")
    |> Chart.withTitle (titleFn level)
    |> Chart.withSize(chartWidth, chartHeight)
  )

let generateShape (titleFn: int -> string) (shapeables: (string * int * DamageCount seq) list seq) =
  shapeables
  |> Seq.map (fun chartables ->
    let level = Seq.head chartables |> fun (_,x,_) -> x
    // let mutable minThreshold = 1000.0
    // let mutable maxThreshold = 0.0

    chartables
    |> Seq.map (fun (title, _, result) ->
      let totalCount = Seq.sumBy damageCountCount result |> float
      let bucketed = 
        result
        |> Seq.groupBy (round << damageCountDamage)
        |> Seq.map (fun (roundedDamage, damageCounts) ->
          roundedDamage,
          Seq.sumBy damageCountCount damageCounts 
          |> float
          |> divideByFirst totalCount
          |> (*) 100.0
        )
        |> Seq.cache

      // minThreshold <- min minThreshold (
      //   bucketed
      //   |> Seq.filter (fun (_, chance) -> chance >= 1.0)
      //   |> Seq.minBy first
      //   |> first
      // )

      // maxThreshold <- max maxThreshold (
      //   bucketed
      //   |> Seq.maxBy first
      //   |> first
      // )

      Chart.Area(
        bucketed,
        Name = title
      )
    )
    |> Chart.combine
    |> Chart.withXAxisStyle
      (
        "Damage for Turn"
        // , MinMax = (minThreshold, maxThreshold)
      )
    |> Chart.withYAxisStyle
      (
        "% Chance of Outcome for Turn"
        // , MinMax = (0, 10)
      )
    |> Chart.withTitle (titleFn level)
    |> Chart.withSize(chartWidth, chartHeight)
  )

let toTtk hp (damageCount: DamageCount) =
  hp
  |> divideByFirst damageCount.Damage
  |> ceil
  |> int
  |> (fun x -> if x < 0 then 100000 else x)
  |> max 1
  |> min 21
  |> flipTuple damageCount.Count

let damageCountsToTtks hp damageCounts =
  let total = 
    damageCounts
    |> Seq.sumBy damageCountCount 
    |> float

  damageCounts
  |> Seq.map (toTtk hp)
  |> Seq.groupBy first
  |> Seq.map (fun (ttk, counts) -> (ttk, Seq.sumBy second counts |> float |> divideByFirst total))

let generateTTKArea (titleFn: int -> string) creatureLevelBump (chartables: list<list<string * int * seq<DamageCount>>>) =
  chartables
  |> Seq.mapi (fun i chartables ->
    let level = Seq.head chartables |> fun (_,x,_) -> x
    let hp = averageHp bestiaryByLevel[level + creatureLevelBump]

    chartables
    |> Seq.map (fun (title, level, result) ->
      Chart.Area(
        damageCountsToTtks hp result,
        Name = title
        // ,Opacity = 1.0 / (float i)
      )
    )
    |> Chart.combine
    |> Chart.withXAxisStyle("Times to KO", MinMax = (1, 21))
    |> Chart.withYAxisStyle("% Chance of Outcome for Turn")
    |> Chart.withTitle (titleFn level)
    |> Chart.withSize (800.0, 800.0)
  )

let barbarianGreatsword2StrikesChart creatureLevelBump =
  {
    AveragesByRollsByLevel = mapMerge (highMartialAttack true) creatureLevelBump 5 averageBarbarianGreatsword
    Title = "🐉 Greatsword (2 Strikes) ❖❖"
  }

let fighterArbatlest1StrikeChart creatureLevelBump =
  {
    AveragesByRollsByLevel = transformedResultsByRollByLevel fighterArbalest PlayerAttack creatureAc (highFighterAttack true) bestiaryByLevel creatureLevelBump
      |> Seq.map resultRollsToAverages
      |> Seq.toArray;
    Title = "⚔️ Arbalest (1 Strike) ❖L"
  };

let fighterDoubleSliceChart creatureLevelBump =
  {
    AveragesByRollsByLevel =
      (
        transformedResultsByRollByLevel fighterLongsword PlayerAttack creatureAc (highFighterAttack true) bestiaryByLevel creatureLevelBump
        |> Seq.map resultRollsToAverages,
        transformedResultsByRollByLevel fighterShortsword PlayerAttack creatureAc (highFighterAttack true) bestiaryByLevel creatureLevelBump
        |> Seq.map resultRollsToAverages
      )
      ||> mergeRollAveragesByLevel
      |> Seq.toArray;
    Title = "⚔️ Double Slice (Swords) ❖❖"
  };

let fighterGreatswordChart creatureLevelBump =
  {
    AveragesByRollsByLevel = transformedResultsByRollByLevel averageFighterGreatsword PlayerAttack creatureAc (highFighterAttack true) bestiaryByLevel creatureLevelBump
    |> Seq.map resultRollsToAverages
    |> Seq.toArray;
    Title = "⚔️ Greatsword (1 Strike) ❖"
  }

let fighterGreatsword2StrikesChart creatureLevelBump =
  {
    AveragesByRollsByLevel = mapMerge (highFighterAttack true) creatureLevelBump 5 averageFighterGreatsword
    Title = "⚔️ Greatsword (2 Strikes) ❖❖"
  }

let fighterLongsword2StrikesChart creatureLevelBump =
  {
    AveragesByRollsByLevel = mapMerge (highFighterAttack true) creatureLevelBump 5 fighterLongsword
    Title = "⚔️ Longsword (2 Strikes) ❖❖"
  }

let fighterLongbow2StrikesChart creatureLevelBump  =
  {
    AveragesByRollsByLevel = mapMerge (highFighterAttack true) creatureLevelBump 5 fighterLongbow
    Title = "⚔️ Longbow (2 Strikes) ❖❖"
  }

let fighterLongbowDoubleShotChart creatureLevelBump =
  {
    AveragesByRollsByLevel = mapMerge ((+) -2 << highFighterAttack true) creatureLevelBump 0 fighterLongbow
    Title = "⚔️ Longbow (Double Shot) ❖❖"
  };

let fighterLongbowIncredibleAimChart creatureLevelBump =
  {
    AveragesByRollsByLevel = transformedResultsByRollByLevel fighterLongbow PlayerAttack creatureAc ((+) 2 << highFighterAttack true) bestiaryByLevel creatureLevelBump
      |> Seq.map resultRollsToAverages
      |> Seq.toArray;
    Title = "⚔️ Longbow (Incredible Aim) ❖❖"
  }

let fighterLongbow3StrikesChart creatureLevelBump =
  {
    AveragesByRollsByLevel = ([1..20], [
      transformedResultsByRollForLevel fighterLongbow PlayerAttack creatureAc (highFighterAttack true) bestiaryByLevel creatureLevelBump;
      transformedResultsByRollForLevel fighterLongbow PlayerAttack creatureAc ((+) -5 << highFighterAttack true) bestiaryByLevel creatureLevelBump;
      transformedResultsByRollForLevel fighterLongbow PlayerAttack creatureAc ((+) -10 << highFighterAttack true) bestiaryByLevel creatureLevelBump
    ])
    ||> prepMergeNRollAverages
    |> Seq.map mergeNRollAverages
    Title = "⚔️ Longbow (3 Strikes) ❖❖❖"
  };

let fighterLongbowTripleShotChart creatureLevelBump =
  {
    AveragesByRollsByLevel = ([1..20], Seq.replicate 3 (transformedResultsByRollForLevel fighterLongbow PlayerAttack creatureAc ((+) -4 << highFighterAttack true) bestiaryByLevel creatureLevelBump))
    ||> prepMergeNRollAverages
    |> Seq.map mergeNRollAverages
    Title = "⚔️ Longbow (Triple Shot) ❖❖❖"
  }

let fighterLongbowDoubleShotThenStrikeChart creatureLevelBump = 
  {
    AveragesByRollsByLevel = ([1..20], [
      transformedResultsByRollForLevel fighterLongbow PlayerAttack creatureAc ((+) -2 << highFighterAttack true) bestiaryByLevel creatureLevelBump;
      transformedResultsByRollForLevel fighterLongbow PlayerAttack creatureAc ((+) -2 << highFighterAttack true) bestiaryByLevel creatureLevelBump;
      transformedResultsByRollForLevel fighterLongbow PlayerAttack creatureAc ((+) -10 << highFighterAttack true) bestiaryByLevel creatureLevelBump
    ])
    ||> prepMergeNRollAverages
    |> Seq.map mergeNRollAverages
    Title = "⚔️ Longbow (DS + Strike) ❖❖❖"
  };

let fighterLongbowStrikeThenDoubleShotChart creatureLevelBump =
  {
    AveragesByRollsByLevel = ([1..20], [
      transformedResultsByRollForLevel fighterLongbow PlayerAttack creatureAc (highFighterAttack true) bestiaryByLevel creatureLevelBump;
      transformedResultsByRollForLevel fighterLongbow PlayerAttack creatureAc ((+) -7 << highFighterAttack true) bestiaryByLevel creatureLevelBump;
      transformedResultsByRollForLevel fighterLongbow PlayerAttack creatureAc ((+) -7 << highFighterAttack true) bestiaryByLevel creatureLevelBump
    ])
    ||> prepMergeNRollAverages
    |> Seq.map mergeNRollAverages
    Title = "⚔️ Longbow (Strike + DS) ❖❖❖"
  }

let fighterLongbowIncredibleAimThenStrikeChart creatureLevelBump =
  {
    AveragesByRollsByLevel = mapMerge ((+) 2 << highFighterAttack true) creatureLevelBump 7 fighterLongbow
    Title = "⚔️ Longbow (IA + Strike) ❖❖❖"
  };

let fighterLongbowMultiShotStanceTripleShotChart creatureLevelBump =
  {
    AveragesByRollsByLevel = ([1..20], Seq.replicate 3 (transformedResultsByRollForLevel fighterLongbow PlayerAttack creatureAc ((+) -2 << highFighterAttack true) bestiaryByLevel creatureLevelBump))
    ||> prepMergeNRollAverages
    |> Seq.map mergeNRollAverages
    Title = "⚔️ Multi-Shot Stance TS ❖❖❖"
  };

let highestRankForceBarrage2ActionChart creatureLevelBump =
  {
    AveragesByRollsByLevel = transformedResultsByRollByLevel (forceBarrage 2) PlayerAttack creatureAc (highFighterAttack true) bestiaryByLevel creatureLevelBump
    |> Seq.map resultRollsToAverages
    |> Seq.toArray;
    Title = "🧙 Force Barrage ❖❖"
  };

let highestRankForceBarrage3ActionChart creatureLevelBump =
  {
    AveragesByRollsByLevel = transformedResultsByRollByLevel (forceBarrage 3) PlayerAttack creatureAc (highFighterAttack true) bestiaryByLevel creatureLevelBump
    |> Seq.map resultRollsToAverages
    |> Seq.toArray;
    Title = "🧙 Force Barrage ❖❖❖"
  };

let clericFireRayMovesChart creatureLevelBump =
  { 
    AveragesByRollsByLevel = transformedResultsByRollByLevel fireRayMove PlayerAttack creatureAc (casterAttack true false) bestiaryByLevel creatureLevelBump
      |> Seq.map resultRollsToAverages
      |> Seq.toArray;
    Title = "📿 Fire Ray (Moves) ❖❖";
  };

let clericFireRayStaysChart creatureLevelBump =
  { 
    AveragesByRollsByLevel = transformedResultsByRollByLevel fireRayStay PlayerAttack creatureAc (casterAttack true false) bestiaryByLevel creatureLevelBump
      |> Seq.map resultRollsToAverages
      |> Seq.toArray;
    Title = "📿 Fire Ray (Stays) ❖❖";
  };

let standardSpellChart spellTitle spellFn saveSelector creatureLevelBump =
  { 
    AveragesByRollsByLevel = transformedResultsByRollByLevel spellFn CreatureSave (saveSelectorFunction saveSelector) (casterDc true) bestiaryByLevel creatureLevelBump
      |> Seq.map resultRollsToAverages
      |> Seq.map normalizeSavingThrowsForLevel
      |> Seq.toArray;
    Title = spellTitle
  };

let druidTempestSurgeChart saveSelector creatureLevelBump =
  standardSpellChart (sprintf "🌿 Tempest Surge (%s) ❖❖" (saveSelectorText saveSelector)) tempestSurge saveSelector creatureLevelBump

let druidThunderstrikeChart saveSelector creatureLevelBump =
  standardSpellChart (sprintf "🌿 Thunderstrike (%s) ❖❖" (saveSelectorText saveSelector)) thunderstrike saveSelector creatureLevelBump

let sorcererDragonBreath1MiddleTargetChart creatureLevelBump =
  {
    AveragesByRollsByLevel = transformedResultsByRollByLevel averageDamageDragonBreath CreatureSave middleSave (casterDc true) bestiaryByLevel creatureLevelBump
      |> Seq.map resultRollsToAverages
      |> Seq.map normalizeSavingThrowsForLevel
      |> Seq.toArray;
    Title = "🩸 Dragon Breath (1 Middle) ❖❖"
  }
