module PathfinderAnalysis.Library

open Helpers
open Compare

let npcLevels = [-1 .. 25]

type DiceSize = D4 | D6 | D8 | D10 | D12 | D20

let averageRoll dieSize =
  match dieSize with
  | D4 -> 2.5
  | D6 -> 3.5
  | D8 -> 4.5
  | D10 -> 5.5
  | D12 -> 6.5
  | D20 -> 10.5

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

let damageNeedleDarts result level =
  2 + spellRank level
  |> float
  |> (*) (averageRoll D4)
  |> (*) (defaultHitMultiplier result)

let damageTelekineticProjectile result level =
  1 + spellRank level
  |> float
  |> (*) (averageRoll D6)
  |> (*) (defaultHitMultiplier result)

let damageSpout result level =
  1 + spellRank level
  |> float
  |> (*) (averageRoll D4)
  |> (*) (defaultCastMultiplier result)

let damageTempestSurge result level =
  spellRank level
  |> float
  |> (*) (averageRoll D12)
  |> (*) (defaultCastMultiplier result)

let damageThunderstrike result level =
  spellRank level
  |> float
  |> (*) (averageRoll D12 + averageRoll D4)
  |> (*) (defaultCastMultiplier result)

let damageFireRay result level =
  2 * spellRank level
  |> float
  |> (*) (averageRoll D6)
  |> (*) (defaultHitMultiplier result)

let damageForceBarrage actions result level =
  actions * ((spellRank level + 1) / 2)
  |> float
  |> (*) (averageRoll D4 + 1.0)

let damagePropertyRune result level =
  float (propertyDice level) * (averageRoll D6)
  |> (*) (defaultHitMultiplier result)

let damageWeapon dieSize result level =
  float (weaponDice level) * averageRoll dieSize
  |> (*) (defaultHitMultiplier result)

let damageFatal dieSize fatalDieSize result level =
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
    damageWeapon dieSize result level

let damageAttribute attributeSelector result (level: int) =
  attributeSelector level
  |> float
  |> (*) (defaultHitMultiplier result)

let damageDeadly dieSize result level =
  match result with
  | CritSuccess | CritWithImmunity -> deadlyDice level |> float |> (*) (averageRoll dieSize)
  | Success | Fail | CritFail -> 0.0

let damageMartialWeaponSpecialization result level =
  defaultHitMultiplier result * float (martialWeaponSpecialization level)

let damageFighterWeaponSpecialization result level =
  defaultHitMultiplier result * float (fighterWeaponSpecialization level)

let martialShortbow level result =
  [damageDeadly D10; damageWeapon D6; damageMartialWeaponSpecialization] //; damagePropertyRune result]
  |> Seq.sumBy (fun fn -> fn result level)

let fighterShortbow level result =
  [damageDeadly D10; damageWeapon D6; damageFighterWeaponSpecialization; damagePropertyRune]
  |> Seq.sumBy (fun fn -> fn result level)

let fighterArbalest level result =
  [damageWeapon D10; damageFighterWeaponSpecialization; damagePropertyRune]
  |> Seq.sumBy (fun fn -> fn result level)

let martialRepeatingHandCrossbow level result =
  [damageWeapon D6 result; damageMartialWeaponSpecialization result; damagePropertyRune result]
  |> Seq.sumBy (fun fn -> fn level)

let martialArbalest level result =
  [damageWeapon D10 result; damageMartialWeaponSpecialization result] //; damagePropertyRune result]
  |> Seq.sumBy (fun fn -> fn level)

let fighterLongsword level result =
  [damageWeapon D8; damageFighterWeaponSpecialization; damagePropertyRune; damageAttribute (highModifier true)]
  |> Seq.sumBy (fun fn -> fn result level)

let fighterShortsword level result =
  [damageWeapon D6; damageFighterWeaponSpecialization; damagePropertyRune; damageAttribute (highModifier true)]
  |> Seq.sumBy (fun fn -> fn result level)

let telekineticProjectile level result =
  [damageTelekineticProjectile result]
  |> Seq.sumBy (fun fn -> fn level)

let spout level result =
  [damageSpout result]
  |> Seq.sumBy (fun fn -> fn level)

let tempestSurge level result =
  [damageTempestSurge result]
  |> Seq.sumBy (fun fn -> fn level)

let fireRay level result = 
  [damageFireRay result]
  |> Seq.sumBy (fun fn -> fn level)

let thunderstrike level result =
  damageThunderstrike result level

let forceBarrage actions level result = 
  damageForceBarrage actions result level