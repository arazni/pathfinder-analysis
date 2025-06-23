module SideAnalysis.HitPoints
open PathfinderAnalysis.Helpers

type ClassTier = 
  | Wizard
  | Rogue
  | Fighter
  | Barbarian
  | Kineticist

type AncestryTier =
  | Elf
  | Human
  | Orc

type MonsterDamageTier =
  | Low
  | Moderate
  | High
  | Extreme
  | UnlimitedAreaOfEffect
  | LimitedAreaOfEffect

let hitPointsPerLevel con classTier =
  match classTier with
  | Wizard -> 6
  | Rogue -> 8
  | Fighter -> 10
  | Barbarian -> 12
  | Kineticist -> 10
  |> (+) con

let kineticistConAtLevel level =
  4 + atLeasts [10; 17; 20] level

let ancestryBonus ancestryTier =
  match ancestryTier with
  | Elf -> 6
  | Human -> 8
  | Orc -> 10

let hitPointsAtLevel ancestryTier con classTier level =
  hitPointsPerLevel con classTier
  |> (*) level
  |> (+) (ancestryBonus ancestryTier)

// use average growth rates, very close to the real chart
let monsterDamagePerLevel monsterDamageTier =
  match monsterDamageTier with
  | Low -> 1.35
  | Moderate -> 1.65
  | High -> 2.0
  | Extreme -> 2.6
  | UnlimitedAreaOfEffect -> 1.5
  | LimitedAreaOfEffect -> 3.5

let initialMonsterDamage monsterDamageTier =
  match monsterDamageTier with
  | Low -> 4
  | Moderate -> 5
  | High -> 6
  | Extreme -> 8
  | UnlimitedAreaOfEffect -> 5
  | LimitedAreaOfEffect -> 7

let monsterDamageAtLevel monsterDamageTier level =
  monsterDamagePerLevel monsterDamageTier
  |> (*) (float (level - 1))
  // |> round
  |> (+) ((float << initialMonsterDamage) monsterDamageTier)

let hitsToKnockOutBump hitPointsAtLevel monsterDamageAtLevel playerLevel monsterLevel =
  ((float << hitPointsAtLevel) playerLevel) / (monsterDamageAtLevel monsterLevel)

let hitsToKnockOut hitPointsAtLevel monsterDamageAtLevel level  =
  hitsToKnockOutBump hitPointsAtLevel monsterDamageAtLevel level level  

let sturdyShieldHardnessAtLevel level =
  5 + 3 * atLeasts [4; 10; 19] level + 2 * atLeasts [7; 13; 16] level

let sturdyShieldBrokenThresholdAtLevel level =
  10 + 22 * atLeast 4 level + 8 * atLeasts [7; 13; 16] level + 12 * atLeasts [10; 19] level

let sturdyShieldBlockMonsterDamageAtLevel monsterDamageTier monsterLevel shieldLevel =
  monsterDamageAtLevel monsterDamageTier monsterLevel - float (sturdyShieldHardnessAtLevel shieldLevel)
  |> max 0
