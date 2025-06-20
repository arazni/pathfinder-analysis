module Request.Guimard

open PathfinderAnalysis.Helpers

type Result = 
  | Success
  | SuccessWithComplication
  | PartialSuccess
  | PartialSuccessWithComplication
  | Failure

type DieResult =
  | Hit
  | Complication
  | Miss

type ResultChance = {
  Result: Result;
  Chance: float;
}

let evaluateDie rolledNumber =
  match rolledNumber with
  | x when x > 0 && x < 3 -> Hit
  | x when x = 3 -> Complication
  | _ -> Miss

let diceValues dieSize =
  [(minimumRoll dieSize)..1..(maximumRoll dieSize)]
  |> List.map evaluateDie

let evaluateRoll diceResults =
  if (List.where (fun x -> x = Hit) >> List.length) diceResults > 1 then Success
  else if List.contains Hit diceResults && List.contains Complication diceResults then SuccessWithComplication
  else if List.contains Hit diceResults then PartialSuccess
  else if List.contains Complication diceResults then PartialSuccessWithComplication
  else Failure

let pairUp (lst1: 'a list) (lst2: 'a list) =
  [ for i in lst1 do 
      for j in lst2 ->
        [i; j]
  ]

let pairUp2 (list2: DieResult list) (list1: DieResult list list) =
  [ for i in list1 do
      for j in list2 do
        List.append [j] i
  ]

let generateDicePool dieSize diceRolled =
  let values = diceValues dieSize
  let valuesInLists = List.map (fun x -> [x]) values
  
  if diceRolled = 1 then valuesInLists else

  [2..diceRolled]
  |> List.fold (fun i _ -> pairUp2 values i) valuesInLists

let requestWork dieSize diceRolled =
  let resultCounts = 
    generateDicePool dieSize diceRolled
    |> List.map evaluateRoll
    |> List.groupBy selfFn
    |> List.map (fun group -> first group, List.length (second group))

  let total =
    List.sumBy (fun group -> second group) resultCounts

  resultCounts
  |> List.map (fun group -> { Result = first group; Chance = float (second group) / float total })
  