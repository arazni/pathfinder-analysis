module PathfinderAnalysis.Helpers

let atLeast comparison level =
  if comparison <= level then 1 else 0

let atLeasts comparisons level =
  Seq.filter (fun x -> x <= level) comparisons
  |> Seq.length

let and1 left right1 =
  if left && right1 <> 0 then right1 else 0

let selfFn x =
  x

let asBool number =
  number = 1