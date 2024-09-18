module PathfinderAnalysis.Helpers

type DiceSize = D4 | D6 | D8 | D10 | D12 | D20

let atLeast comparison level =
  if comparison <= level then 1 else 0

let atLeasts comparisons level =
  Seq.filter (fun x -> x <= level) comparisons
  |> Seq.length

let and1 left right1 =
  if left && right1 <> 0 then right1 else 0

let inline divideByFirst a b =
  b / a

let selfFn x =
  x

let selfFn2 _ x =
  x

let first (x, _) =
  x

let second (_, x) =
  x

let tuple a b =
  a, b


let flipTuple a b =
  b, a

let asBool number =
  number = 1

let minimumRoll (dieSize : DiceSize) = 
  1

let averageRoll dieSize =
  match dieSize with
  | D4 -> 2.5
  | D6 -> 3.5
  | D8 -> 4.5
  | D10 -> 5.5
  | D12 -> 6.5
  | D20 -> 10.5

let maximumRoll dieSize =
  match dieSize with
  | D4 -> 4
  | D6 -> 6
  | D8 -> 8
  | D10 -> 10
  | D12 -> 12
  | D20 -> 20

let rollNotation dieSize =
  match dieSize with
  | D4 -> "d4"
  | D6 -> "d6"
  | D8 -> "d8"
  | D10 -> "d10"
  | D12 -> "d12"
  | D20 -> "d20"

let choose n kin =
  // if n = 0 then 0 else
  if kin = 0 || kin = n then bigint 1 else

  let k = 
    if kin > n - kin then n - kin
    else kin
  
  let kList = [k..(-1)..1] |> Seq.map bigint
  let deltaList = [n..(-1)..(n-k+1)] |> Seq.map bigint
  
  let kFactorial = Seq.reduce (*) kList
  let deltaFactorial = Seq.reduce (*) deltaList
  deltaFactorial / kFactorial

let coefficientOfExponentiatedGeometricSeries exponent seriesLength coefficientSlot =
  [0..(coefficientSlot - exponent)/seriesLength]
  |> Seq.sumBy (fun i -> bigint (pown -1 i ) * (choose exponent i) * (choose (coefficientSlot - i*seriesLength - 1) (exponent - 1)) )

