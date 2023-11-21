module PathfinderAnalysis.Library

open Helpers

let pcLevels = [1 .. 20]
let npcLevels = [-1 .. 25]

let highModifier apex level =
  4 + atLeasts [10; 20] level + (atLeast 17 >> and1 apex) level

let offHighModifier apex level =
  3 + atLeasts [5; 15] level + (atLeast 17 >> and1 apex) level

let martialProficiency level =
  level + 2 * atLeasts [1; 5; 13] level

let spellProficiency level= 
  level + 2 * atLeasts [1; 7; 15; 19] level

let gateAttenuatorBonus level =
  atLeasts [3; 11] level

let potencyBonus level =
  atLeasts [2; 10; 16] level

let highMartialAttack hasApex level =
  [highModifier hasApex; martialProficiency; potencyBonus]
  |> Seq.sumBy (fun fx -> fx level)

let casterAttack hasApex hasGate level =
  [highModifier hasApex; spellProficiency; (gateAttenuatorBonus >> and1 hasGate) ]
  |> Seq.sumBy (fun fx -> fx level)

let casterDc hasApex level =
  [highModifier hasApex; spellProficiency;]
  |> Seq.sumBy (fun fx -> fx level)
  |> (+) 10

