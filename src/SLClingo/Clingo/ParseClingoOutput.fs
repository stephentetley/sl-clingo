// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause


namespace SLClingo.Clingo

module ParseClingoOutput =
    
    open System

    open FParsec

    // open SLPotassco.AspCore.Parser
    open SLClingo.Clingo.Base

    type ClingoParser<'ans> = Parser<'ans, unit>

    let lexeme (parser : ClingoParser<'a>) : ClingoParser<'a> = 
        parser .>> spaces
    
    let token (str : string) : ClingoParser<string> = 
        lexeme (pstring str)

    let chartoken (ch : char) : ClingoParser<char> = 
        lexeme (pchar ch)

    let parens (p1 : ClingoParser<'a>) : ClingoParser<'a> = 
        between (chartoken '(') (chartoken ')') p1

    let private makeIdentifierParser (firstChar : ClingoParser<char>) : ClingoParser<string> = 
        parse { 
            let! start = firstChar
            let! rest = many (letter <|> digit <|> pchar '_')
            return (String.Concat (Array.ofList (start :: rest)))
        }

    let pIdentifier : ClingoParser<string> = 
        lexeme (makeIdentifierParser letter)

    let pSymbolicConstant : ClingoParser<string> = 
        lexeme (makeIdentifierParser lower)

    let pQuotedString : ClingoParser<string> =
        let escape = pchar '\\' >>. pchar '"' >>. preturn "\\\""
        let body1 = escape <|> (noneOf ['\\'; '"'] |>> string)
        let qString = pchar '"' >>. many body1 .>> pchar '"'
        lexeme (qString |>> String.concat "")

    let pNumber : ClingoParser<int64> = 
        lexeme pint64

    let pPredicateName : ClingoParser<string> = 
        pIdentifier <|> pQuotedString 

    let pGroundTerm : ClingoParser<GroundTerm> = 
        ((pSymbolicConstant |>> SymbolicConstant)
            <|> (pQuotedString |>> String)
            <|> (pNumber |>> Number))

    let pGroundTerms : ClingoParser<GroundTerm list> = 
        sepBy1 pGroundTerm (chartoken ',')

    let pAnswerTerm: ClingoParser<AnswerTerm> = 
        pipe2 pPredicateName
                (parens pGroundTerms)
                (fun name terms -> AnswerTerm(name, terms))

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
                    (manyTill pAnswerTerm (lookAhead pStatus))
                    pStatus
                    (fun idx terms status -> { Index = idx; AnswerTerms = terms; Status = status})

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