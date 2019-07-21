// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause


namespace SLPotassco.Clingo

module ParseClingo =
    
    open System

    open FParsec

    open SLPotassco.AspCore.Parser

    type ClingoParser<'ans> = AspParser<'ans>

    /// let pWhiteToNewline = (many (anyOf " \t")) >>. newline

    let pVersionNumber : ClingoParser<string> = manyCharsTill anyChar spaces

    let clingoVersion : ClingoParser<string> = token "clingo" >>. token "version" >>. pVersionNumber
        

    let pSolving : ClingoParser<unit> = token "Solving..." |>> ignore

    let pResult (field : string) (answerParser : ClingoParser<'a>) : ClingoParser<'a> = 
        token field >>. chartoken ':' >>. answerParser .>> spaces

    let pAnswer : ClingoParser<int> = pResult "Answer" pint32

    let pCalls : ClingoParser<int> = pResult "Calls" pint32

    let pStatus () = 
        pstring "SATISFIABLE" <|> pstring "UNSATISFIABLE" <|> pstring "UNKNOWN"

    // The plus symbol for Models is explained on page 15 of the Potassco manual