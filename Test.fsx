#r "nuget: FSharp.Json"

#load "Helpers.fs"
#load "Bestiary.fs"
#load "Compare.fs"
#load "Library.fs"

open PathfinderAnalysis.Library
open PathfinderAnalysis.Bestiary
open PathfinderAnalysis.Compare

// #r "nuget: Plotly.NET.Interactive"
// open Plotly.NET

resultsByRoll PlayerAttack (fun c -> c.ac) (highMartialAttack false 2) bestiaryByLevel[2]
|> Seq.toArray
|> printf "%A"

transformedResultsByRoll defaultCastMultiplier CreatureSave middleSave (casterDc false 2) bestiaryByLevel[2]
|> Seq.toArray
|> printf "%A"

transformedResultsByRoll (damageMartialShortbow 2) PlayerAttack (fun c -> c.ac) (highMartialAttack false 2) bestiaryByLevel[2]
|> Seq.toArray
|> printf "%A"
