// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause

#r "netstandard"



#I @"C:\Users\stephen\.nuget\packages\FParsec\1.0.4-rc3\lib\netstandard1.6"
#r "FParsec"
#r "FParsecCS"
open FParsec

#I @"C:\Users\stephen\.nuget\packages\slformat\1.0.2-alpha-20190721\lib\netstandard2.0"
#r @"SLFormat.dll"
open SLFormat.CommandOptions
open SLFormat.CommandOptions.SimpleInvoke

#load @"..\src\SLClingo\AspCore\Syntax.fs"
#load @"..\src\SLClingo\AspCore\Parser.fs"
#load @"..\src\SLClingo\AspCore\Pretty.fs"
#load @"..\src\SLClingo\Clingo\Base.fs"
#load @"..\src\SLClingo\Clingo\ParseClingoOutput.fs"
#load @"..\src\SLClingo\Clingo\Invoke.fs"
open SLClingo.AspCore.Syntax
open SLClingo.AspCore.Parser
open SLClingo.Clingo.Base
open SLClingo.Clingo.ParseClingoOutput
open SLClingo.Clingo.Invoke

let demoDirectory () = 
    System.IO.Path.Combine(__SOURCE_DIRECTORY__, @"..\demo\")



let demo01 () = 
    let demoDir = demoDirectory () 
    clingo demoDir None [ literal "--version"] []


let demo02 () = 
    let demoDir = demoDirectory ()
    clingo demoDir None [] ["toh_ins.lp"; "toh_enc.lp"]

let demo02b () = 
    let demoDir = demoDirectory ()
    runClingo demoDir None [] ["toh_ins.lp"; "toh_enc.lp"]

let demo03 () = 
    let demoDir = demoDirectory ()
    clingo demoDir (Some 0) [literal "--BAD"] ["toh_ins.lp"; "toh_enc.lp"]

let demo03b () = 
    let demoDir = demoDirectory ()
    runClingo demoDir (Some 0) [literal "--BAD"] ["toh_ins.lp"; "toh_enc.lp"]

let demo04 () = 
    let demoDir = demoDirectory ()
    runClingo demoDir None [] ["negative_numbers.lp"]
