module PathfinderAnalysis.Bestiary

open FSharp.Json
open System.IO
open Helpers

// "hp": "(\d*).*",
// "hp": \1,

type Creature = {
    name: string
    level: int
    rarity: string
    ac: int
    hp: int
    fortitude: int
    reflex: int
    will: int
    weakestSaves: string
    hasCritImmunity: int
    hasMagicBonus: int
    hasMentalImmunity: int
    isCheating: int
}

let lowestSave creature =
  creature.fortitude |> min creature.reflex |> min creature.will
  |> (+) creature.hasMagicBonus

let highestSave creature =
  creature.fortitude |> max creature.reflex |> max creature.will
  |> (+) creature.hasMagicBonus

let middleSave creature = 
  [creature.fortitude; creature.reflex; creature.will]
  |> Seq.sort
  |> Seq.skip 1
  |> Seq.head
  |> (+) creature.hasMagicBonus

let reflexSave creature =
  creature.reflex + creature.hasMagicBonus

let willSave creature =
  creature.will + creature.hasMagicBonus

let fortitudeSave creature =
  creature.fortitude + creature.hasMagicBonus

let creatureDc save =
  save + 10

let creatureAc creature =
  creature.ac

let creatureHp creature =
  creature.hp

let averageHp creatures =
  creatures
  |> Array.sumBy creatureHp
  |> float
  |> divideByFirst (float creatures.Length)

let shadowSignet creature =
  creature.ac |> min (creatureDc creature.fortitude) |> min (creatureDc creature.reflex)

let load file =
  let config = JsonConfig.create(allowUntyped = true)
  File.ReadAllText file
  |> Json.deserializeEx<Creature[]> config

let loadedBestiary = 
  load "Data\\bestiary.json"

let bestiaryByLevel = 
  loadedBestiary
  |> Seq.groupBy (fun b -> b.level)
  |> Seq.map (fun (level, creatures) -> level, Seq.toArray creatures)
  |> Map

let save (file : string) =
  let config = JsonConfig.create(allowUntyped = true)
  let json = Json.serializeEx config loadedBestiary
  File.WriteAllText(file, json)

let average (creatureDefenseFn: Creature -> int) level =
  bestiaryByLevel[level]
  |> Array.sumBy (fun creature -> creatureDefenseFn creature)
  |> float
  |> divideByFirst (bestiaryByLevel[level].Length |> float)
