module PublishBackend.Hp

open Plotly.NET
open SideAnalysis.HitPoints

let chartWidth = 770
let chartHeight = 770

let levels = [1..20]

let humanWizardHitPointsAtLevel = hitPointsAtLevel AncestryTier.Human 0 ClassTier.Wizard
let humanRogueHitPointsAtLevel = hitPointsAtLevel AncestryTier.Human 0 ClassTier.Rogue
let humanFighterHitPointsAtLevel = hitPointsAtLevel AncestryTier.Human 0 ClassTier.Fighter
let humanBarbarianHitPointsAtLevel = hitPointsAtLevel AncestryTier.Human 0 ClassTier.Barbarian
let human2ConBarbarianHitPointsAtLevel = hitPointsAtLevel AncestryTier.Human 2 ClassTier.Barbarian
let elfWizardHitPointsAtLevel = hitPointsAtLevel AncestryTier.Elf 0 ClassTier.Wizard
let orcFighterHitPointsAtLevel = hitPointsAtLevel AncestryTier.Orc 0 ClassTier.Fighter
let humanKineticistHitPointsAtLevel =
  fun level ->
    let con = kineticistConAtLevel level
    hitPointsAtLevel AncestryTier.Human con ClassTier.Kineticist level

let monsterDamageTierDisplayName damageTier =
  match damageTier with
  | MonsterDamageTier.Low -> "Low Strike Damage"
  | MonsterDamageTier.Moderate -> "Moderate Strike Damage"
  | MonsterDamageTier.High -> "High Strike Damage"
  | MonsterDamageTier.Extreme -> "Extreme Strike Damage"
  | MonsterDamageTier.LimitedAreaOfEffect -> "Failed Save Limited AoE Damage"
  | MonsterDamageTier.UnlimitedAreaOfEffect -> "Failed Save Unlimited AoE Damage"

let monsterLevelModDisplayName monsterLevelMod =
  if monsterLevelMod = 0 then "On-Level Monster"
  elif monsterLevelMod > 0 then sprintf "Monster Level +%i" monsterLevelMod
  else sprintf "Monster Level %i" monsterLevelMod

let baseChartGenerator comparingFunctions =
  comparingFunctions
  |> List.map (fun (name, hpFunction) -> 
  Chart.Line(
    x = levels,
    y = List.map hpFunction levels,
    Name = name,
    ShowMarkers = true
  ))

let maxY comparingFunctions =
  comparingFunctions
  |> Seq.map snd
  |> Seq.map (fun x -> x (Seq.max levels))
  |> Seq.max
  |> ceil
  |> int

let classGrowthChart = 
  let comparingFunctions = [
    "0 Con Wizard", humanWizardHitPointsAtLevel;
    "0 Con Rogue", humanRogueHitPointsAtLevel;
    "0 Con Fighter", humanFighterHitPointsAtLevel;  
    "0 Con Barbarian", humanBarbarianHitPointsAtLevel;
    "Max Con Kineticist", humanKineticistHitPointsAtLevel;
  ]

  comparingFunctions
  |> baseChartGenerator
  |> Chart.combine
  |> Chart.withLineStyle(Shape = StyleParam.Shape.Hvh)
  |> Chart.withXAxisStyle(TitleText = "Character Level", MinMax = (0, 21))
  |> Chart.withYAxisStyle(TitleText = "Hit Points")
  |> Chart.withTitle "Human Hit Points By Class Per Level"
  |> Chart.withSize(chartWidth, chartHeight)

let monsterDamageVsPartyChart monsterDamageTier monsterLevelMod =
  let monsterDamageFunc level = monsterDamageAtLevel monsterDamageTier (monsterLevelMod + level)
  let hitsTKO hpFunc = hitsToKnockOut hpFunc monsterDamageFunc

  let comparingFunctions = [
    "0 Con Wizard", hitsTKO humanWizardHitPointsAtLevel;
    "0 Con Rogue", hitsTKO humanRogueHitPointsAtLevel;
    "0 Con Fighter", hitsTKO humanFighterHitPointsAtLevel;
    "0 Con Barbarian", hitsTKO humanBarbarianHitPointsAtLevel;
    "Max Con Kineticist", hitsTKO humanKineticistHitPointsAtLevel;
  ]

  let max = maxY comparingFunctions

  comparingFunctions
  |> baseChartGenerator
  |> Chart.combine
  |> Chart.withLineStyle(Shape = StyleParam.Shape.Hvh)
  |> Chart.withXAxisStyle(TitleText = "Character Level", MinMax = (0, 21))
  |> Chart.withYAxisStyle(TitleText = "Hits To Knockout", MinMax = (0, max))
  |> Chart.withTitle (sprintf "Average Hits To KO Humans (%s; %s)" (monsterDamageTierDisplayName monsterDamageTier) (monsterLevelModDisplayName monsterLevelMod))
  |> Chart.withSize(chartWidth, chartHeight)

let flatRatioChart =
  let monsterDamageFunc = monsterDamageAtLevel MonsterDamageTier.LimitedAreaOfEffect
  let hitsTKO hpFunc = hitsToKnockOut hpFunc monsterDamageFunc

  let comparingFunctions = [
    "Elf Wizard (6)", hitsTKO elfWizardHitPointsAtLevel;
    "Human Rogue (8)", hitsTKO humanRogueHitPointsAtLevel;
    "Orc Fighter (10)", hitsTKO orcFighterHitPointsAtLevel;
  ]

  comparingFunctions
  |> baseChartGenerator
  |> Chart.combine
  |> Chart.withLineStyle(Shape = StyleParam.Shape.Hvh)
  |> Chart.withXAxisStyle(TitleText = "Character Level", MinMax = (0, 21))
  |> Chart.withYAxisStyle(TitleText = "Hits To Knockout", MinMax = (0, 4))
  |> Chart.withTitle "Failed Saves To KO (Limited AoE Damage; On-Level Monster)"
  |> Chart.withSize(chartWidth, chartHeight)

let fighterVsMonsterLevelsChart monsterDamageTier =
  let hitsTKO levelBump level = hitsToKnockOutBump humanFighterHitPointsAtLevel (monsterDamageAtLevel monsterDamageTier) level (levelBump + level)

  let comparingFunctions = [
    "On-Level", hitsTKO 0;
    "Level +1", hitsTKO 1;
    "Level +2", hitsTKO 2;
    "Level +3", hitsTKO 3;
    "Level +4", hitsTKO 4;
  ]

  let max = maxY comparingFunctions

  comparingFunctions
  |> baseChartGenerator
  |> Chart.combine
  |> Chart.withLineStyle(Shape = StyleParam.Shape.Hvh)
  |> Chart.withXAxisStyle(TitleText = "Character Level", MinMax = (0, 21))
  |> Chart.withYAxisStyle(TitleText = "Hits To Knockout", MinMax = (0, max))
  |> Chart.withTitle (sprintf "Average Hits To KO 0 Con Human Fighter (%s)" (monsterDamageTierDisplayName monsterDamageTier))
  |> Chart.withSize(chartWidth, chartHeight)