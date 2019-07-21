// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause

#r "netstandard"

#I @"C:\Users\stephen\.nuget\packages\FParsec\1.0.4-rc3\lib\portable-net45+win8+wp8+wpa81"
#r "FParsec"
#r "FParsecCS"
open FParsec

#I @"C:\Users\stephen\.nuget\packages\slformat\1.0.2-alpha-20190616\lib\netstandard2.0"
#r @"SLFormat.dll"
open SLFormat.CommandOptions
open SLFormat.CommandOptions.SimpleInvoke

#load @"..\src\SLPotassco\AspCore\Syntax.fs"
#load @"..\src\SLPotassco\AspCore\Parser.fs"
#load @"..\src\SLPotassco\AspCore\Pretty.fs"
#load @"..\src\SLPotassco\Potassco\Base.fs"
#load @"..\src\SLPotassco\Potassco\ParseClingo.fs"
#load @"..\src\SLPotassco\Potassco\Invoke.fs"
open SLPotassco.AspCore.Syntax
open SLPotassco.AspCore.Parser
open SLPotassco.Potassco.Base
open SLPotassco.Potassco.ParseClingo
open SLPotassco.Potassco.Invoke

let demoDirectory () = 
    System.IO.Path.Combine(__SOURCE_DIRECTORY__, @"..\demo\")



let demo01 () = 
    let demoDir = demoDirectory () 
    clingo demoDir [ literal "--version"] [] None


let demo02 () = 
    let demoDir = demoDirectory ()
    match clingo demoDir [] ["toh_ins.lp"; "toh_enc.lp"] None with
    | Result.Ok result -> printfn "%s" result.StdOut
    | Result.Error msg -> printfn "Fail: %s" msg


let demo03 () = 
    let demoDir = demoDirectory ()
    match clingo demoDir [] ["toh_ins.lp"; "toh_enc.lp"] None with
    | Result.Ok result -> run pClingoOutput result.StdOut
    | Result.Error msg -> run (failFatally msg) ""




