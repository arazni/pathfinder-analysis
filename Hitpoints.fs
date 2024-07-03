module SideAnalysis.HitPoints

type ClassTier = 
  | Wizard
  | Rogue
  | Fighter
  | Barbarian

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
  |> (+) con

let ancestryBonus ancestryTier =
  match ancestryTier with
  | Elf -> 6
  | Human -> 8
  | Orc -> 10

let hitPointsAtLevel ancestryTier con classTier level =
  hitPointsPerLevel con classTier
  |> (*) level
  |> (+) (ancestryBonus ancestryTier)

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

let hitsToKnockOut hitPointsAtLevel monsterDamageAtLevel level =
  ((float << hitPointsAtLevel) level) / (monsterDamageAtLevel level)

