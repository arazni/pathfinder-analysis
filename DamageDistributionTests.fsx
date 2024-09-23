#r "nuget: FSharp.Json"
#r "nuget: Plotly.NET"

#load "Helpers.fs"
#load "Bestiary.fs"
#load "Compare.fs"
#load "DamageDistribution.fs"
#load "Library.fs"
#load "Transform.fs"

open PathfinderAnalysis.Helpers
open PathfinderAnalysis.Bestiary
open PathfinderAnalysis.Compare
open PathfinderAnalysis.Library
open PathfinderAnalysis.DamageDistribution

let pcLevel = 1
let creatureLevel = 1
let creatures = bestiaryByLevel[creatureLevel]
let creatureCount = Array.length creatures

let damageContexts = [|
  { HitRollCount = 1; HitModifier = highFighterAttack true pcLevel; Contest = PlayerAttack; CreatureDefenseFunction = creatureAc; DamageFunction = diceFighterLongbow pcLevel};
  // { HitRollCount = 1; HitModifier = highFighterAttack true pcLevel - 5; Contest = PlayerAttack; CreatureDefenseFunction = creatureAc; DamageFunction = diceFighterLongbow pcLevel};
  // { HitRollCount = 2; HitModifier = casterDc true pcLevel; Contest = CreatureSave; CreatureDefenseFunction = middleSave; DamageFunction = diceDamageSpout pcLevel}
|]

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

let testHitMultiplier result = 
  match result with
  | CritSuccess | CritWithImmunity -> 9.0
  | Success -> 3.0
  | Fail -> 1.0/3.0
  | CritFail -> 1.0/9.0

let testAc = (fun _ -> 20)
let testDamage = (fun hr -> seq { { Damage = 1.0 * testHitMultiplier hr; Count = bigint 1 }; { Damage = 10.0 * testHitMultiplier hr; Count = bigint 10 } })

let testContexts =
  [|
    { HitRollCount = 1; HitModifier = highFighterAttack true pcLevel; Contest = PlayerAttack; CreatureDefenseFunction = testAc; DamageFunction = testDamage };
    // { HitRollCount = 1; HitModifier = highFighterAttack true pcLevel; Contest = PlayerAttack; CreatureDefenseFunction = testAc; DamageFunction = testDamage };
    // { HitRollCount = 1; HitModifier = highFighterAttack true pcLevel - 5; Contest = PlayerAttack; CreatureDefenseFunction = (fun _ -> 20); DamageFunction = diceFighterLongbow pcLevel };
    // { HitRollCount = 2; HitModifier = casterDc true pcLevel; Contest = CreatureSave; CreatureDefenseFunction = middleSave; DamageFunction = diceDamageSpout pcLevel }
  |]

theBigThing creatures testContexts
|> Seq.toList
|> damageCountsToAverage
// |> chunkDamage 20
// |> damageChunksToAverages
// |> Seq.toArray

