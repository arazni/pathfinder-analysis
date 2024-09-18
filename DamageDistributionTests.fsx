#r "nuget: FSharp.Json"
#r "nuget: Plotly.NET"

#load "Helpers.fs"
#load "Bestiary.fs"
#load "Compare.fs"
#load "DamageDistribution.fs"
#load "Library.fs"
#load "Transform.fs"
// #load "Request.fs"

open PathfinderAnalysis.Helpers
open PathfinderAnalysis.Bestiary
open PathfinderAnalysis.Compare
open PathfinderAnalysis.Library
// open PathfinderAnalysis.Transform
open PathfinderAnalysis.DamageDistribution

let pcLevel = 1
let creatureLevel = 1
let creatures = bestiaryByLevel[creatureLevel]
let creatureCount = Array.length creatures

let damageContexts = [|
  { HitRollCount = 1; HitModifier = highFighterAttack true pcLevel; Contest = PlayerAttack; CreatureDefenseFunction = creatureAc; DamageFunction = diceFighterLongbow pcLevel};
  { HitRollCount = 1; HitModifier = highFighterAttack true pcLevel - 5; Contest = PlayerAttack; CreatureDefenseFunction = creatureAc; DamageFunction = diceFighterLongbow pcLevel};
  { HitRollCount = 2; HitModifier = casterDc true pcLevel; Contest = CreatureSave; CreatureDefenseFunction = middleSave; DamageFunction = diceDamageSpout pcLevel}
|]

let allPossibleTurnHitRolls = 
  damageContexts
  |> Seq.sumBy (fun context -> context.HitRollCount)
  |> allD20Rolls selfFn
  |> permuteDiceRolls
  // |> Seq.toList

allPossibleTurnHitRolls
|> Seq.map (fun turnHitRolls ->
  let flatContexts = 
    damageContexts
    |> Seq.collect (fun context -> Seq.replicate context.HitRollCount context)

  let hitResultsForTurn = 
    (turnHitRolls, flatContexts)
    ||> Seq.map2 (fun hitRoll flatContext -> 
      hitRoll
      |> resultsForRoll flatContext.Contest flatContext.CreatureDefenseFunction flatContext.HitModifier bestiaryByLevel[creatureLevel]
    )
    |> Seq.toList

  damageContexts
  |> Seq.mapi (fun i damageContext -> Seq.replicate damageContext.HitRollCount i)
  |> Seq.map2 tuple hitResultsForTurn
  |> Seq.groupBy (fun (_, repeatedIndex) -> Seq.head repeatedIndex)
  |> Seq.map (fun (key, group) -> 
    Seq.map first group
    |> sumHitResultsForDamageContext creatureCount damageContexts[key]
  )
  |> combineDamageCountsFromDamageContexts
)
|> Seq.collect selfFn
|> Seq.groupBy (fun dc -> dc.Damage)
|> Seq.map (fun (damage, damageCounts) -> { Damage = damage; Count = Seq.sumBy damageCountCount damageCounts })
|> Seq.sortBy damageCountDamage
// |> Seq.sumBy (fun xs -> xs |> Seq.sumBy (fun x -> x.Count))

let testCreatures = 100;
let testResultSequence = seq {
  seq {
    { Result = Fail; Count = 100 };
    { Result = CritSuccess; Count = 1}
  };
  seq {
    { Result = CritSuccess; Count = 100 };
    { Result = CritFail; Count = 1}
  }
}
let testDamageContext =   { HitRollCount = 2; HitModifier = casterDc true pcLevel; Contest = CreatureSave; CreatureDefenseFunction = middleSave; DamageFunction = diceDamageSpout pcLevel }

sumHitResultsForDamageContext testCreatures testDamageContext testResultSequence
|> Seq.toList

combineDamageCountsFromDamageContexts (seq 
{ seq [
    { Damage = 1.0; Count = bigint 2 };
    { Damage = 10.0; Count = bigint 3 }
  ];
  seq [
    { Damage = 0.1; Count = bigint 5 };
    { Damage = 0.01; Count = bigint 7 };
    { Damage = 0.001; Count = bigint 11 };
  ] 
} )
|> Seq.toList


let lookup = hitDamageLookup creatures damageContexts

damageCountsFromHitDamageLookup lookup 2 0

