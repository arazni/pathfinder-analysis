module PathfinderAnalysis.Compare

open Helpers
open Bestiary

type RollResult =
    CritFail | Fail | Success | CritSuccess

type HitResult =
    CritFail | Fail | Success | CritSuccess | CritWithImmunity

type Contested =
    PlayerAttack | CreatureSave

type ResultData<'a> = { Result: 'a; Count: int }

type ResultDataForRoll<'a> = { Roll: int; Results: (ResultData<'a> seq) }

let toResultData (x, y) =
    { Result = x; Count = y }

let toResultDataForRoll (roll, results) =
    { Roll = roll; Results = results }

let upgradeResult result =
    match result with
    | RollResult.CritFail -> RollResult.Fail
    | RollResult.Fail -> RollResult.Success
    | RollResult.Success | RollResult.CritSuccess -> RollResult.CritSuccess

let downgradeResult result =
    match result with
    | RollResult.CritSuccess -> RollResult.Success
    | RollResult.Success -> RollResult.Fail
    | RollResult.Fail | RollResult.CritFail -> RollResult.CritFail

let toHitResult hasCritImmunity rollResult =
    match rollResult with
    | RollResult.CritSuccess -> if hasCritImmunity then CritWithImmunity else CritSuccess
    | RollResult.Success -> Success
    | RollResult.Fail -> Fail
    | RollResult.CritFail -> CritFail

let handleNats roll result =
    if roll = 1 then downgradeResult result
    else if roll = 20 then upgradeResult result
    else result

let rollResult dc rollTotal d20Roll =
    [|dc + 10, RollResult.CritSuccess; dc, RollResult.Success; dc - 9, RollResult.Fail; -1000, RollResult.CritFail|]
    |> Seq.find (fun (dc, _)  -> rollTotal >= dc)
    |> fun (_, result) -> handleNats d20Roll result

let defenseRollResult defenseSelector creature dc d20 =
    rollResult dc (d20 + defenseSelector creature) d20
    |> toHitResult false

let attackRollResult defenseSelector creature attackModifier d20 =
    rollResult (defenseSelector creature) (d20 + attackModifier) d20
    |> toHitResult (asBool creature.hasCritImmunity)

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
    |> Seq.map toResultData

let resultsForRoll contested defenseSelector offenseModifier (creatures: Creature seq) d20 =
    transformedResultsForRoll selfFn contested defenseSelector offenseModifier creatures d20

let transformedResultsByRoll resultTransform contested defenseSelector offenseModifier (creatures : Creature seq) =
    [| 1..20 |]
    |> Seq.map (fun d20 -> d20, transformedResultsForRoll resultTransform contested defenseSelector offenseModifier creatures d20)
    |> Seq.map toResultDataForRoll

let resultsByRoll contested defenseSelector offenseModifier (creatures : Creature seq) =
    transformedResultsByRoll selfFn contested defenseSelector offenseModifier creatures
