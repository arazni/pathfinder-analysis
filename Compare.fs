module PathfinderAnalysis.Compare

open Helpers
open Bestiary

type RollResult =
    CritFail | Fail | Success | CritSuccess

type Contested =
    PlayerAttack | CreatureSave

let upgradeResult result =
    match result with
    | CritFail -> Fail
    | Fail -> Success
    | Success | CritSuccess -> CritSuccess

let downgradeResult result =
    match result with
    | CritSuccess -> Success
    | Success -> Fail
    | Fail | CritFail -> CritFail

let handleNats roll result =
    if roll = 1 then downgradeResult result
    else if roll = 20 then upgradeResult result
    else result

let rollResult dc rollTotal d20Roll =
    [|dc + 10, CritSuccess; dc, Success; dc - 9, Fail; -1000, CritFail|]
    |> Seq.find (fun (dc, _)  -> rollTotal >= dc)
    |> fun (_, result) -> handleNats d20Roll result

let defenseRollResult defenseSelector creature dc d20 =
    rollResult dc (d20 + defenseSelector creature) d20

let attackRollResult defenseSelector creature attackModifier d20 =
    rollResult (defenseSelector creature) (d20 + attackModifier) d20

let handleRollresult contested defenseSelector creature dcOrAttackModifier d20 =
    (match contested with
    | PlayerAttack -> attackRollResult
    | CreatureSave -> defenseRollResult)
        defenseSelector creature dcOrAttackModifier d20

let transformedResultsForRoll resultTransform contested defenseSelector dcOrAttackModifier (creatures: Creature seq) d20 =
    creatures
    |> Seq.map (fun (creature: Creature: Creature) -> 
        handleRollresult contested defenseSelector creature dcOrAttackModifier d20
        |> resultTransform)
    |> Seq.groupBy selfFn
    |> Seq.map (fun (key, results) -> key, Seq.length results)

let resultsForRoll contested defenseSelector offenseModifier (creatures: Creature seq) d20 =
    transformedResultsForRoll selfFn contested defenseSelector offenseModifier creatures d20

let transformedResultsByRoll resultTransform contested defenseSelector offenseModifier (creatures : Creature seq) =
    [| 1..20 |]
    |> Seq.map (fun d20 -> d20, transformedResultsForRoll resultTransform contested defenseSelector offenseModifier creatures d20)

let resultsByRoll contested defenseSelector offenseModifier (creatures : Creature seq) =
    transformedResultsByRoll selfFn contested defenseSelector offenseModifier creatures

let defaultCastMultiplier result =
    match result with
    | CritFail -> 2.0
    | Fail -> 1.0
    | Success -> 0.5
    | CritSuccess -> 0.0

let defaultHitMultiplier result =
    match result with 
    | CritSuccess -> 2.0
    | Success -> 1.0
    | Fail -> 0.0
    | CritFail -> 0.0