module PublishBackend.AverageLevel

open PathfinderAnalysis.Library
open PathfinderAnalysis.Bestiary
open PathfinderAnalysis.Compare
open PathfinderAnalysis.Transform
open PathfinderAnalysis.ChartTools

let swashbucklerArbalest1Shot creatureLevelBump = 
  {
    AveragesByRollsByLevel = transformedResultsByRollByLevel martialArbalest PlayerAttack creatureAc (highMartialAttack true) bestiaryByLevel creatureLevelBump
      |> Seq.map resultRollsToAverages
      |> Seq.toArray;
    Title = "🤺 Arbalest (1 shot) ❖"
  };

let swashbucklerArbalest2Shot creatureLevelBump = 
  {
    AveragesByRollsByLevel = mapMerge (highMartialAttack true) creatureLevelBump 5 martialArbalest;
    Title = "🤺 Arbalest (2 shots) ❖❖❖"
  };

let swashbucklerShortbow2Shot creatureLevelBump =
  {
    AveragesByRollsByLevel = mapMerge (highMartialAttack true) creatureLevelBump 5 martialShortbow;
    Title = "🤺 Shortbow (2 shots) ❖❖"
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

let cantripTierCharts1 creatureLevelBump =
  seq {
    // standardSpellChart "🌿 Spout (Middle) ❖❖" spout SaveSelector.Middle creatureLevelBump;
    // standardSpellChart "🌿 Spout (Lowest) ❖❖" spout SaveSelector.Lowest creatureLevelBump;
    // standardSpellChart "🌿 Spout (Highest) ❖❖" spout SaveSelector.Highest creatureLevelBump;
    swashbucklerShortbow2Shot
    sorcererTelekineticProjectile
    standardSpellChart "🩸 Spout (Reflex) ❖❖" spout SaveSelector.Reflex;
  }
  |> cantripTierCharts creatureLevelBump
  |> generateLevelScaleChart (sprintf "Caster and Martial Backup Options (no Property Runes) - Creatures PL+%i" creatureLevelBump)
