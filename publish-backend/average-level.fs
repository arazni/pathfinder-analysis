module PublishBackend.AverageLevel

open PathfinderAnalysis.Library
open PathfinderAnalysis.Bestiary
open PathfinderAnalysis.Compare
open PathfinderAnalysis.Transform
open PathfinderAnalysis.ChartTools

let sigh int =
  if int < 0 then sprintf "%i" int
  else sprintf "+%i" int

let swashbucklerArbalest1Shot creatureLevelBump = 
  {
    AveragesByRollsByLevel = transformedResultsByRollByLevel martialAbpArbalest PlayerAttack creatureAc (highMartialAttack true) bestiaryByLevel creatureLevelBump
      |> Seq.map resultRollsToAverages
      |> Seq.toArray;
    Title = "🤺 Arbalest (ABP) ❖"
  };

let swashbucklerArbalest2Shot creatureLevelBump = 
  {
    AveragesByRollsByLevel = mapMerge (highMartialAttack true) creatureLevelBump 5 martialAbpArbalest;
    Title = "🤺 Arbalest (ABP) ❖❖❖"
  };

let swashbucklerArbalestBehind1Shot behind creatureLevelBump = 
  {
    AveragesByRollsByLevel = transformedResultsByRollByLevel (martialBackupArbalest behind) PlayerAttack creatureAc (backupHighMartialAttack behind true) bestiaryByLevel creatureLevelBump
      |> Seq.map resultRollsToAverages
      |> Seq.toArray;
    Title = sprintf "🤺 Arbalest (PL-%i) ❖" behind
  };

let swashbucklerArbalestBehind2Shot behind creatureLevelBump = 
  {
    AveragesByRollsByLevel = mapMerge (backupHighMartialAttack behind true) creatureLevelBump 5 (martialBackupArbalest behind);
    Title = sprintf "🤺 Arbalest (PL-%i) ❖❖❖" behind
  };

let swashbucklerShortbow2Shot weaponLevelPenalty creatureLevelBump =
  {
    AveragesByRollsByLevel = mapMerge (backupHighMartialAttack weaponLevelPenalty true) creatureLevelBump 5 (martialBackupShortbow weaponLevelPenalty);
    Title = sprintf "🤺 Shortbow (PL-%i) ❖❖" weaponLevelPenalty
  };

let swashbucklerAbpShortbow2Shot buff creatureLevelBump =
  {
    AveragesByRollsByLevel = mapMerge ((+) buff << highMartialAttack true) creatureLevelBump 5 martialAbpShortbow;
    Title = sprintf "🤺 Shortbow (ABP) ❖❖"
  };

let sorcererTelekineticProjectile buff creatureLevelBump =
  { 
    AveragesByRollsByLevel = transformedResultsByRollByLevel telekineticProjectile PlayerAttack creatureAc ((+) buff << casterAttack true false) bestiaryByLevel creatureLevelBump
      |> Seq.map resultRollsToAverages
      |> Seq.toArray;
    Title = "🩸 Telekinetic Projectile ❖❖"
  };

let cantripTierCharts creatureLevelBump chartDataGenerators = 
  chartDataGenerators
  |> Seq.map (fun x -> x creatureLevelBump)
  |> Seq.cache
  |> flatten

let cantripBackupLevelPenaltyCharts weaponLevelPenalties creatureLevelBump =
  weaponLevelPenalties
  |> Seq.map swashbucklerShortbow2Shot
  |> Seq.append [sorcererTelekineticProjectile 0]
  |> cantripTierCharts creatureLevelBump
  |> generateLevelScaleChart (sprintf "Caster and Martial Backup Options - Creatures PL+%i" creatureLevelBump)

let cantripBackupCompareCharts creatureLevelBump =
  seq {
    sorcererTelekineticProjectile 0;
    swashbucklerShortbow2Shot 4;
    swashbucklerAbpShortbow2Shot 0
  }
  |> cantripTierCharts creatureLevelBump
  |> generateLevelScaleChart (sprintf "Caster and Martial Backup Options - Creatures PL+%i" creatureLevelBump)

let cantripBackupCreatureLevelCharts creatureLevelBump =
  seq {
    sorcererTelekineticProjectile 0;
    swashbucklerAbpShortbow2Shot 0;
    standardSpellChart "🩸 Spout (Reflex) ❖❖" spout SaveSelector.Reflex;
  }
  |> cantripTierCharts creatureLevelBump
  |> generateLevelScaleChart (sprintf "Caster and Martial Backup Options - Creatures PL%s" (sigh creatureLevelBump))

let cantripArbalestVsShortbowCharts creatureLevelBump =
  seq {
    swashbucklerAbpShortbow2Shot 0;
    swashbucklerArbalest1Shot;
    swashbucklerArbalest2Shot;
    swashbucklerShortbow2Shot 0;
    swashbucklerArbalestBehind1Shot 0;
    swashbucklerArbalestBehind2Shot 0;
    swashbucklerShortbow2Shot 4;
  }
  |> cantripTierCharts creatureLevelBump
  |> generateLevelScaleChart (sprintf "Martial Backup Options - Creatures PL%s" (sigh creatureLevelBump))

let sprintfSpout save = 
  sprintf "🩸 Spout (%s) ❖❖" (saveSelectorText save)

let cantripSpoutSavesCharts saves creatureLevelBump =
  saves
  |> Seq.map (fun save -> standardSpellChart (sprintfSpout save) spout save)
  |> cantripTierCharts creatureLevelBump
  |> generateLevelScaleChart (sprintf "Hypothetical Spout Comparisons - Creatures PL%s" (sigh creatureLevelBump))

let sprintfSaveBump save bump =
  sprintf "PL%s (%s)" (sigh bump) (saveSelectorText save)

let cantripSpoutSavesLevelsCharts (saveBumps: (SaveSelector * int) seq) =
  saveBumps
  |> Seq.map (fun (save, bump) -> standardSpellChart (sprintfSaveBump save bump) spout save bump)
  |> Seq.cache
  |> flatten
  |> generateLevelScaleChart "Impact of Creature Level Gap and Save Choice on Hypothetical Spouts"

let cantripSpoutWeakestVsShortbowCharts creatureLevelBump =
  seq {
    sorcererTelekineticProjectile 2;
    swashbucklerAbpShortbow2Shot 2;
    standardSpellChart "🩸 Spout (Lowest) ❖❖" spout SaveSelector.Lowest;
  }
  |> cantripTierCharts creatureLevelBump
  |> generateLevelScaleChart (sprintf "Off-Guard vs. Lowest Save - Creature PL%s" (sigh creatureLevelBump))