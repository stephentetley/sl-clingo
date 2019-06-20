// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause

#r "netstandard"

#I @"C:\Users\stephen\.nuget\packages\FParsec\1.0.4-rc3\lib\portable-net45+win8+wp8+wpa81"
#r "FParsec"
#r "FParsecCS"
open FParsec

#load @"..\src\SLPotassco\ParseClingo.fs"
open SLPotassco.ParseClingo

let demo01 () = 
    run (clingoVersion ()) "clingo version 5.3.0"

let demo02 () = 
    run (pAnswer ()) "Answer: 1"
