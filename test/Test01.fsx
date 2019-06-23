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

#load @"..\src\SLPotassco\ParseClingo.fs"
#load @"..\src\SLPotassco\Invoke.fs"
open SLPotassco.ParseClingo
open SLPotassco.Invoke

let demoDirectory () = 
    System.IO.Path.Combine(__SOURCE_DIRECTORY__, @"..\demo\")


let demo01 () = 
    run (clingoVersion ()) "clingo version 5.3.0"

let demo02 () = 
    run (pAnswer ()) "Answer: 1"

let demo03 () = 
    let demoDir = demoDirectory () 
    clingo demoDir [ literal "--version"] [] None

