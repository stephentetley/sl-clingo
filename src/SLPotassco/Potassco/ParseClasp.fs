// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause


namespace SLPotassco.Potassco

module ParseClasp =
    
    open System

    open FParsec

    open SLPotassco.AspCore.Parser
    open SLPotassco.Potassco.Base

    type ClaspParser<'ans> = AspParser<'ans>

    /// let pWhiteToNewline = (many (anyOf " \t")) >>. newline

    let pVersionNumber : ClaspParser<string> = manyCharsTill anyChar spaces

    let clingoVersion : ClaspParser<string> = token "clingo" >>. token "version" >>. pVersionNumber
        

    let pSolving : ClaspParser<unit> = token "Solving..." |>> ignore

    let pResult (field : string) (answerParser : ClaspParser<'a>) : ClaspParser<'a> = 
        token field >>. chartoken ':' >>. answerParser .>> spaces

    
    let pStatus : ClaspParser<AnswerStatus> = 
        let sat     = token "SATISFIABLE"       >>. preturn Satisfiable
        let unsat   = token "UNSATISFIABLE"     >>. preturn Unsatisfiable
        let unkwn   = token "UNKNOWN"           >>. preturn Unknown
        sat <|> unsat <|> unkwn

    let pAnswer : ClaspParser<ClaspAnswer> = 
        let body = 
            pipe3 (lexeme pint32)
                    pTerms
                    pStatus
                    (fun idx terms status -> { Index = idx; Terms = terms; Status = status})

        pResult "Answer" body

    let pCalls : ClaspParser<int> = pResult "Calls" pint32



    // The plus symbol for Models is explained on page 15 of the Potassco manual