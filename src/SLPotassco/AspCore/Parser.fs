// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause


namespace SLPotassco.AspCore

module Parser =

    
    open FParsec

    open SLPotassco.AspCore.Syntax

    type AspParser<'ans> = Parser<'ans, unit>

    let lexeme (parser : AspParser<'a>) : AspParser<'a> = 
        parser .>> spaces


    let pIdentifier : AspParser<string> = 
        let opts = new IdentifierOptions()
        lexeme (identifier opts)

    let pSymbolicConstant : AspParser<string> = 
        let opts = new IdentifierOptions(isAsciiIdStart = isAsciiLower)
        lexeme (identifier opts)
    
    let pVariable : AspParser<string> = 
        let opts = new IdentifierOptions(isAsciiIdStart = isAsciiUpper)
        lexeme (identifier opts)

    let pQuotedString : AspParser<string> =
        let escape = pchar '\\' >>. pchar '"' >>. preturn "\\\""
        let body1 = escape <|> (noneOf ['\\'; '"'] |>> string)
        let qString = pchar '"' >>. many body1 .>> pchar '"'
        lexeme (qString |>> String.concat "")

    let pAnonVariable : AspParser<string> = 
        lexeme (pstring "_")

    let pNumber : AspParser<int> = 
        lexeme (puint32 |>> int)


    let pNeg : AspParser<string> = 
        lexeme (pstring "-")

    let pNaf : AspParser<string> = 
        lexeme (pstring "not")


    let pGroundTerm : AspParser<GroundTerm> = 
        (pSymbolicConstant |>> SymbolicConstant)
            <|> (pQuotedString |>> String)
            <|> (pNumber |>> Number)        // TODO - minus?


    let pAggregateFunction : AspParser<AggregateFunction> = 
        let aggcount    = pstring "#count"  >>. preturn AggrCount
        let aggmax      = pstring "#max"    >>. preturn AggrMax
        let aggmin      = pstring "#min"    >>. preturn AggrMin
        let aggsum      = pstring "#sum"    >>. preturn AggrSum
        lexeme (aggcount <|> aggmax <|> aggmin <|> aggsum)

    let pBinOp : AspParser<BinOp> = 
        let eq          = pchar '='                         >>. preturn OpEqual
        let ueq         = (pstring "<>" <|> pstring "!=")   >>. preturn OpUnequal
        let less        = pchar '<'                         >>. preturn OpLess
        let greater     = pchar '>'                         >>. preturn OpGreater
        let lessEq      = pstring "<="                      >>. preturn OpLessOrEq
        let greaterEq   = pstring ">="                      >>. preturn OpGreaterOrEq
        lexeme (eq <|> ueq <|> lessEq <|> greaterEq <|> less <|> greater)


    let pArithOp : AspParser<ArithOp> = 
        let plus    = pchar '+' >>. preturn OpPlus
        let minus   = pchar '-' >>. preturn OpMinus
        let times   = pchar '*' >>. preturn OpTimes
        let divide  = pchar '/' >>. preturn OpDiv
        lexeme (plus <|> minus <|> times <|> divide)


