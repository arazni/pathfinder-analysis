module PathfinderAnalysis.Library

open Helpers
open Compare
open DamageDistribution

let npcLevels = [-1 .. 25]

let highModifier apex level =
  4 + atLeasts [10; 20] level + (atLeast 17 >> and1 apex) level

let offHighModifier apex level =
  3 + atLeasts [5; 15] level + (atLeast 17 >> and1 apex) level

let martialProficiency level =
  level + 2 * atLeasts [1; 5; 13] level

let fighterProficiency level =
  level + 2 * atLeasts [1; 1; 5; 13] level

let martialWeaponSpecialization level =
  match level with
  | x when x < 7 -> 0
  | x when x < 15 -> atLeasts [1; 5; 13] level
  | _ -> 2 * atLeasts [1; 5; 13] level

let fighterWeaponSpecialization level =
  match level with
  | x when x < 7 -> 0
  | x when x < 15 -> atLeasts [1; 1; 5; 13] level
  | _ -> 2 * atLeasts [1; 1; 5; 13] level

let spellProficiency level= 
  level + 2 * atLeasts [1; 7; 15; 19] level

let gateAttenuatorBonus level =
  atLeasts [3; 11] level

let potencyBonus level =
  atLeasts [2; 10; 16] level

let weaponDice level =
  atLeasts [1; 4; 12; 19] level

let propertyDice level =
  atLeasts [8; 10; 16] level

let deadlyDice level =
  atLeasts [1; 12; 19] level

let highMartialAttack hasApex level =
  [highModifier hasApex; martialProficiency; potencyBonus]
  |> Seq.sumBy (fun fx -> fx level)

let highFighterAttack hasApex level =
  [highModifier hasApex; fighterProficiency; potencyBonus]
  |> Seq.sumBy (fun fx -> fx level)

let casterAttack hasApex hasGate level =
  [highModifier hasApex; spellProficiency; (gateAttenuatorBonus >> and1 hasGate) ]
  |> Seq.sumBy (fun fx -> fx level)

let casterDc hasApex level =
  [highModifier hasApex; spellProficiency;]
  |> Seq.sumBy (fun fx -> fx level)
  |> (+) 10

let spellRank (level: int) =
  (level + 1) / 2

let defaultCastMultiplier result =
  match result with
  | CritFail -> 2.0
  | Fail -> 1.0
  | Success -> 0.5
  | CritSuccess -> 0.0
  | CritWithImmunity -> 0.0 // should be impossible

let defaultHitMultiplier result =
  match result with
  | CritSuccess -> 2.0
  | CritWithImmunity | Success -> 1.0
  | Fail -> 0.0
  | CritFail -> 0.0

let averageDamageNeedleDarts result level =
  2 + spellRank level
  |> float
  |> (*) (averageRoll D4)
  |> (*) (defaultHitMultiplier result)

let toDamageCount damageFromResultFunction result rollCounts =
  rollCounts
  |> Seq.map (fun rollCount -> { Damage = float rollCount.Roll * damageFromResultFunction result; Count = rollCount.Count })

let diceDamageNeedleDarts result level =
  2 + spellRank level
  |> rollDistribution D4
  |> toDamageCount defaultHitMultiplier result

let averageDamageTelekineticProjectile result level =
  1 + spellRank level
  |> float
  |> (*) (averageRoll D6)
  |> (*) (defaultHitMultiplier result)

let diceDamageTelekineticProjectile result level =
  1 + spellRank level
  |> rollDistribution D6
  |> toDamageCount defaultHitMultiplier result

let averageDamageSpout result level =
  1 + spellRank level
  |> float
  |> (*) (averageRoll D4)
  |> (*) (defaultCastMultiplier result)

let diceDamageSpout result level =
  1 + spellRank level
  |> rollDistribution D4
  |> toDamageCount defaultCastMultiplier result

let averageDamageTempestSurge result level =
  spellRank level
  |> float
  |> (*) (averageRoll D12)
  |> (*) (defaultCastMultiplier result)

let diceDamageTempestSurge level result  =
  spellRank level
  |> rollDistribution D12
  |> toDamageCount defaultCastMultiplier result

let averageDamageThunderstrike result level =
  spellRank level
  |> float
  |> (*) (averageRoll D12 + averageRoll D4)
  |> (*) (defaultCastMultiplier result)

let diceDamageThunderstrike result level = 
  seq {D12, spellRank level; D4, spellRank level}
  |> rollDistributions 0
  |> toDamageCount defaultCastMultiplier result

let averageDamageFireRay result level =
  2 * spellRank level
  |> float
  |> (*) (averageRoll D6)
  |> (*) (defaultHitMultiplier result)

let diceDamageFireRay result level =
  2 * spellRank level
  |> rollDistribution D6
  |> toDamageCount defaultHitMultiplier result

let averageDamageForceBarrage actions result level =
  actions * ((spellRank level + 1) / 2)
  |> float
  |> (*) (averageRoll D4 + 1.0)

let diceDamageForceBarrage actions result level =
  actions * ((spellRank level + 1) / 2)
  |> rollDistribution D4
  |> applyModifierToDistribution 1
  |> toDamageCount (fun _ -> 1.0) result

let averageDamagePropertyRune result level =
  float (propertyDice level) * (averageRoll D6)
  |> (*) (defaultHitMultiplier result)

let averageDamageWeapon dieSize result level =
  float (weaponDice level) * averageRoll dieSize
  |> (*) (defaultHitMultiplier result)

let averageDamageFatal dieSize fatalDieSize result level =
  match result with
  | CritSuccess -> 
    float (weaponDice level)
    |> (*) (averageRoll fatalDieSize)
    |> (*) 2.0
    |> (+) (averageRoll fatalDieSize)
  | CritWithImmunity ->
    float (weaponDice level + 1)
    |> (*) (averageRoll fatalDieSize)
  | Success | Fail | CritFail -> 
    averageDamageWeapon dieSize result level

let damageAttribute attributeSelector result (level: int) =
  attributeSelector level
  |> float
  |> (*) (defaultHitMultiplier result)

let averageDamageDeadly dieSize result level =
  match result with
  | CritSuccess | CritWithImmunity -> deadlyDice level |> float |> (*) (averageRoll dieSize)
  | Success | Fail | CritFail -> 0.0

let damageMartialWeaponSpecialization result level =
  defaultHitMultiplier result * float (martialWeaponSpecialization level)

let damageFighterWeaponSpecialization result level =
  defaultHitMultiplier result * float (fighterWeaponSpecialization level)

let martialShortbow level result =
  [averageDamageDeadly D10; averageDamageWeapon D6; damageMartialWeaponSpecialization] //; averageDamagePropertyRune result]
  |> Seq.sumBy (fun fn -> fn result level)

let fighterShortbow level result =
  [averageDamageDeadly D10; averageDamageWeapon D6; damageFighterWeaponSpecialization; averageDamagePropertyRune]
  |> Seq.sumBy (fun fn -> fn result level)

let diceAbilityDamage poolResultFunction dicePools modifierFunctions level result =
  dicePools
  |> rollDistributions 0
  |> rollCountsToDamageDice
  |> applyDamageFunctionToDamageDice (*) (poolResultFunction result)
  |> tuple (modifierFunctions |> Seq.sumBy (fun fn -> fn result level))
  ||> applyDamageFunctionToDamageDice (+)

let diceFighterShortbow level result =
  diceAbilityDamage defaultHitMultiplier [D6, propertyDice level; D6, weaponDice level] [damageFighterWeaponSpecialization; averageDamageDeadly D10] level result

let fighterArbalest level result =
  [averageDamageWeapon D10; damageFighterWeaponSpecialization; averageDamagePropertyRune]
  |> Seq.sumBy (fun fn -> fn result level)

let martialRepeatingHandCrossbow level result =
  [averageDamageWeapon D6 result; damageMartialWeaponSpecialization result; averageDamagePropertyRune result]
  |> Seq.sumBy (fun fn -> fn level)

let martialArbalest level result =
  [averageDamageWeapon D10 result; damageMartialWeaponSpecialization result] //; averageDamagePropertyRune result]
  |> Seq.sumBy (fun fn -> fn level)

let fighterLongsword level result =
  [averageDamageWeapon D8; damageFighterWeaponSpecialization; averageDamagePropertyRune; damageAttribute (highModifier true)]
  |> Seq.sumBy (fun fn -> fn result level)

let fighterShortsword level result =
  [averageDamageWeapon D6; damageFighterWeaponSpecialization; averageDamagePropertyRune; damageAttribute (highModifier true)]
  |> Seq.sumBy (fun fn -> fn result level)

let telekineticProjectile level result =
  [averageDamageTelekineticProjectile result]
  |> Seq.sumBy (fun fn -> fn level)

let spout level result =
  [averageDamageSpout result]
  |> Seq.sumBy (fun fn -> fn level)

let tempestSurge level result =
  [averageDamageTempestSurge result]
  |> Seq.sumBy (fun fn -> fn level)

let fireRay level result = 
  [averageDamageFireRay result]
  |> Seq.sumBy (fun fn -> fn level)

let thunderstrike level result =
  averageDamageThunderstrike result level

let forceBarrage actions level result = 
  averageDamageForceBarrage actions result level

  