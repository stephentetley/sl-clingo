// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause


namespace SLPotassco.Potassco

module ParseClingo =
    
    open System

    open FParsec

    open SLPotassco.AspCore.Parser
    open SLPotassco.Potassco.Base

    type ClingoParser<'ans> = AspParser<'ans>

    // let pWhiteToNewline = (many (anyOf " \t")) >>. newline

    let pVersionNumber : ClingoParser<string> = many1CharsTill (digit <|> pchar '.') spaces1

    let pClingoVersion : ClingoParser<string> = 
        token "clingo" >>. token "version" >>. pVersionNumber
        
        
    let pClaspVersion : ClingoParser<string> = 
        token "clasp" >>. token "version" >>. pVersionNumber


    let pSolving : ClingoParser<unit> = token "Solving..." |>> ignore

    let pReadingFrom : ClingoParser<string> = token "Reading" >>. token "from" >>. many1CharsTill anyChar newline

    let pResult (field : string) (answerParser : ClingoParser<'a>) : ClingoParser<'a> = 
        token field >>. chartoken ':' >>. answerParser .>> spaces

    
    let pStatus : ClingoParser<AnswerStatus> = 
        let sat     = token "SATISFIABLE"       >>. preturn Satisfiable
        let unsat   = token "UNSATISFIABLE"     >>. preturn Unsatisfiable
        let unkwn   = token "UNKNOWN"           >>. preturn Unknown
        sat <|> unsat <|> unkwn

    let pAnswer : ClingoParser<ClingoAnswer> = 
        let body = 
            pipe3 (lexeme pint32)
                    (manyTill pTerm (lookAhead pStatus))
                    pStatus
                    (fun idx terms status -> { Index = idx; Terms = terms; Status = status})

        pResult "Answer" body

    let pCalls : ClingoParser<int> = pResult "Calls" pint32



    let pClingoOutput : ClingoParser<ClingoOutput> = 
        parse { 
            let! vnum = pClingoVersion
            let! input = pReadingFrom
            do! pSolving
            let! answers = many1 pAnswer
            return { VersionNumber = vnum 
                     InputSource = input
                     Answers = answers
                   }
        }

    // The plus symbol for Models is explained on page 15 of the Potassco manual