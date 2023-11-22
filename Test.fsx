#r "nuget: FSharp.Json"

#load "Helpers.fs"
#load "Bestiary.fs"
#load "Compare.fs"
#load "Library.fs"
#load "Transform.fs"

open PathfinderAnalysis.Library
open PathfinderAnalysis.Bestiary
open PathfinderAnalysis.Compare
open PathfinderAnalysis.Transform

// #r "nuget: Plotly.NET.Interactive"
// open Plotly.NET

resultsByRoll PlayerAttack (fun c -> c.ac) (highMartialAttack false 2) bestiaryByLevel[2]
|> Seq.toArray
|> printf "%A"

transformedResultsByRoll defaultCastMultiplier CreatureSave middleSave (casterDc false 2) bestiaryByLevel[2]
|> Seq.toArray
|> printf "%A"

transformedResultsByRoll (martialShortbow 10) PlayerAttack (fun c -> c.ac) (highMartialAttack false 10) bestiaryByLevel[10]
|> Seq.toArray
|> printf "%A"

transformedResultsByRoll (telekineticProjectile 10) PlayerAttack (fun c -> c.ac) (casterAttack false false 10) bestiaryByLevel[10]
|> Seq.toArray
|> printf "%A"

transformedResultsByRoll (spout 10) CreatureSave middleSave (casterDc true 10) bestiaryByLevel[10]
|> Seq.map (fun (die: int, resultAndAverageDamageSeq) -> die, resultsToAverage resultAndAverageDamageSeq)
|> rollAveragesToAverage
|> printf "%A"

transformedResultsByRollByLevel spout CreatureSave middleSave (casterDc true) bestiaryByLevel 0
|> Seq.toArray
|> printf "%A"

transformedResultsByRollByLevel spout CreatureSave middleSave (casterDc true) bestiaryByLevel 0
|> resultsByRollByLevelToXyz
|> Seq.map savingThrowNormalizeXyz
|> Seq.toArray
|> printf "%A"

