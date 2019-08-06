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

#load @"..\src\SLClingo\AspCore\Syntax.fs"
#load @"..\src\SLClingo\AspCore\Parser.fs"
#load @"..\src\SLClingo\AspCore\Pretty.fs"
open SLClingo.AspCore.Syntax
open SLClingo.AspCore.Parser

let demoDirectory () = 
    System.IO.Path.Combine(__SOURCE_DIRECTORY__, @"..\demo\")


//let demo01 () = 
//    run (clingoVersion ()) "clingo version 5.3.0"

//let demo02 () = 
//    run (pAnswer ()) "Answer: 1"

//let demo03 () = 
//    let demoDir = demoDirectory () 
//    clingo demoDir [ literal "--version"] [] None

//let demo04 () = 
//    let demoDir = demoDirectory ()
//    clingo demoDir [] ["toh_ins.lp"; "toh_enc.lp"] None

let demo05 () : ParserResult<Identifier, unit> = 
    run pIdentifier "potassco_identifier "

let demo06 () : ParserResult<Identifier, unit> =
    run pQuotedString "\"Hello\" World"

let demo07 () : ParserResult<Term, unit> =
    run pTerm "3 * (1 + 100)"

/// ideally:    GroundTerm (Number 3)
let demo08 () : ParserResult<Term, unit> =
    run pTerm "3"

let demo09 () : ParserResult<Term, unit> =
    run pTerm "hello(3)"

let demo10 () : ParserResult<Term, unit> =
    run pTerm "hello(\"World\", 1)"
    
    
// Maybe these would be useful, maybe not

//let tryGetString (ix:int) (term:AnswerTerm) : string option = 
//    match term with
//    | AnswerTerm(_, vals) -> 
//        match List.tryItem ix vals with
//        | Some (String str)  -> Some str
//        | _ -> None


//let tryGetInt64 (ix:int) (term:AnswerTerm) : int64 option = 
//    match term with
//    | AnswerTerm(_, vals) -> 
//        match List.tryItem ix vals with
//        | Some (Number n)  -> Some n
//        | _ -> None

//let tryGetSymbolicConstant (ix:int) (term:AnswerTerm) : string option = 
//    match term with
//    | AnswerTerm(_, vals) -> 
//        match List.tryItem ix vals with
//        | Some (SymbolicConstant str)  -> Some str
//        | _ -> None
    

