// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause


namespace SLPotassco.Potassco

module ParseClingoOutput =
    
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

    let pReadingFrom : ClingoParser<string> = token "Reading" >>. token "from" >>. restOfLine true

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

    let pModels : ClingoParser<ModelCount> = 
        let body = 
            pipe2 (pint32 |>> int)
                    (opt (pchar '+'))
                    (fun n optPlus -> match optPlus with | None -> Exactly n; | Some _ -> AtLeast n)
        pResult "Models" body

    let pCalls : ClingoParser<int> = pResult "Calls" pint32

    let pTimeStats : ClingoParser<string> = pResult "Time" (restOfLine false)

    let pCpuTime : ClingoParser<string> = pResult "CPU Time" (restOfLine false)

    let pClingoOutput : ClingoParser<ClingoOutput> = 
        parse { 
            let! vnum = pClingoVersion
            let! input = pReadingFrom
            do! pSolving
            let! answers = many1 pAnswer
            let! models = pModels
            let! calls = pCalls
            let! timeStats = pTimeStats
            let! cpuTime = pCpuTime
            return { VersionNumber = vnum 
                     InputSource = input
                     Answers = answers
                     Models = models
                     Calls = calls
                     TimeStats = timeStats
                     CpuTime = cpuTime
                   }
        }

    // The plus symbol for Models is explained on page 15 of the Potassco manual

    let pERROR : ClingoParser<string> = 
        pResult "*** ERROR" (restOfLine false)

    let pInfo : ClingoParser<string> = 
        pResult "*** Info" (restOfLine false)

    let pClingoFailure : ClingoParser<ClingoFailure> = 
        pipe2 pERROR pInfo
                (fun msg info -> { Error = msg; Info = info })