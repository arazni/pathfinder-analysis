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

let swashbucklerAbpShortbow2Shot creatureLevelBump =
  {
    AveragesByRollsByLevel = mapMerge (highMartialAttack true) creatureLevelBump 5 martialAbpShortbow;
    Title = sprintf "🤺 Shortbow (ABP) ❖❖"
  };

let sorcererTelekineticProjectile creatureLevelBump =
  { 
    AveragesByRollsByLevel = transformedResultsByRollByLevel telekineticProjectile PlayerAttack creatureAc (casterAttack true false) bestiaryByLevel creatureLevelBump
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
  |> Seq.append [sorcererTelekineticProjectile]
  |> cantripTierCharts creatureLevelBump
  |> generateLevelScaleChart (sprintf "Caster and Martial Backup Options - Creatures PL+%i" creatureLevelBump)

let cantripBackupCompareCharts creatureLevelBump =
  seq {
    sorcererTelekineticProjectile;
    swashbucklerShortbow2Shot 4;
    swashbucklerAbpShortbow2Shot
  }
  |> cantripTierCharts creatureLevelBump
  |> generateLevelScaleChart (sprintf "Caster and Martial Backup Options - Creatures PL+%i" creatureLevelBump)

let cantripBackupCreatureLevelCharts creatureLevelBump =
  seq {
    sorcererTelekineticProjectile;
    swashbucklerAbpShortbow2Shot;
    standardSpellChart "🩸 Spout (Reflex) ❖❖" spout SaveSelector.Reflex;
  }
  |> cantripTierCharts creatureLevelBump
  |> generateLevelScaleChart (sprintf "Caster and Martial Backup Options - Creatures PL+%i" creatureLevelBump)

let cantripArbalestVsShortbowCharts creatureLevelBump =
  seq {
    swashbucklerAbpShortbow2Shot;
    swashbucklerArbalest1Shot;
    swashbucklerArbalest2Shot;
    swashbucklerShortbow2Shot 0;
    swashbucklerArbalestBehind1Shot 0;
    swashbucklerArbalestBehind2Shot 0;
    swashbucklerShortbow2Shot 4;
  }
  |> cantripTierCharts creatureLevelBump
  |> generateLevelScaleChart (sprintf "Martial Backup Options - Creatures PL%s" (sigh creatureLevelBump))

    // standardSpellChart "🌿 Spout (Middle) ❖❖" spout SaveSelector.Middle creatureLevelBump;
    // standardSpellChart "🌿 Spout (Lowest) ❖❖" spout SaveSelector.Lowest creatureLevelBump;
    // standardSpellChart "🌿 Spout (Highest) ❖❖" spout SaveSelector.Highest creatureLevelBump;
    // 
