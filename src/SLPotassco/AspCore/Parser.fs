// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause


namespace SLPotassco.AspCore

#nowarn "40"
module Parser =

    open System
    
    open FParsec

    open SLPotassco.AspCore.Syntax

    type AspParser<'ans> = Parser<'ans, unit>

    // ************************************************************************
    // Lexer

    let lexeme (parser : AspParser<'a>) : AspParser<'a> = 
        parser .>> spaces

    let token (str : string) : AspParser<string> = 
        lexeme (pstring str)

    let chartoken (ch : char) : AspParser<char> = 
        lexeme (pchar ch)

    let parens (p1 : AspParser<'a>) : AspParser<'a> = 
        between (chartoken '(') (chartoken ')') p1

    let private makeIdentifierParser (firstChar : AspParser<char>) : AspParser<string> = 
        parse { 
            let! start = firstChar
            let! rest = many (letter <|> digit <|> pchar '_')
            return (String.Concat (Array.ofList (start :: rest)))
        }

    let pIdentifier : AspParser<string> = 
        lexeme (makeIdentifierParser letter)

    let pSymbolicConstant : AspParser<string> = 
        lexeme (makeIdentifierParser lower)
    
    let pVariable : AspParser<string> = 
        lexeme (makeIdentifierParser upper)

    let pQuotedString : AspParser<string> =
        let escape = pchar '\\' >>. pchar '"' >>. preturn "\\\""
        let body1 = escape <|> (noneOf ['\\'; '"'] |>> string)
        let qString = pchar '"' >>. many body1 .>> pchar '"'
        lexeme (qString |>> String.concat "")

    let pAnonVariable : AspParser<string> = 
        token "_"

    let pNumber : AspParser<int> = 
        lexeme (pint32 |>> int)


    let pNeg : AspParser<string> = 
        token "-"

    let pNaf : AspParser<string> = 
        token "not"

    // ************************************************************************
    // Parser

    let pPredicateName : AspParser<PredicateName> = 
        (pIdentifier |>> Id)
            <|> (pQuotedString |>> QuotedName)
        
        
    let pAggregateFunction : AspParser<AggregateFunction> = 
        let aggcount    = token "#count"    >>. preturn AggrCount
        let aggmax      = token "#max"      >>. preturn AggrMax
        let aggmin      = token "#min"      >>. preturn AggrMin
        let aggsum      = token "#sum"      >>. preturn AggrSum
        aggcount <|> aggmax <|> aggmin <|> aggsum
        
    let pBinOp : AspParser<BinOp> = 
        let eq          = chartoken '='                 >>. preturn OpEqual
        let ueq         = (token "<>" <|> token "!=")   >>. preturn OpUnequal
        let less        = chartoken '<'                 >>. preturn OpLess
        let greater     = chartoken '>'                 >>. preturn OpGreater
        let lessEq      = token "<="                    >>. preturn OpLessOrEq
        let greaterEq   = token ">="                    >>. preturn OpGreaterOrEq
        eq <|> ueq <|> lessEq <|> greaterEq <|> less <|> greater
        
        
    let pMulOp : AspParser<ArithOp> = 
        let times   = chartoken '*' >>. preturn OpTimes
        let divide  = chartoken '/' >>. preturn OpDiv
        times <|> divide

    let pMulOp2 : AspParser<Expression -> Expression -> Expression> = 
        pMulOp >>= fun op -> preturn (fun e1 e2 ->  ArithmeticExpr(e1,op,e2))
    
    let pAddOp : AspParser<ArithOp> = 
        let plus    = chartoken '+' >>. preturn OpPlus
        let minus   = chartoken '-' >>. preturn OpMinus
        plus <|> minus
    
    let pAddOp2 : AspParser<Expression -> Expression -> Expression> = 
        pAddOp >>= fun op -> preturn (fun e1 e2 -> ArithmeticExpr(e1,op,e2))

    let pVariableTerm : AspParser<Term> = 
        (pVariable <|> pAnonVariable) |>> VariableTerm
    
    let pGroundTerm : AspParser<GroundTerm> = 
        ((pSymbolicConstant |>> SymbolicConstant)
            <|> (pQuotedString |>> String)
            <|> (pNumber |>> Number))

    let pBasicTerm : AspParser<Term> = 
        (pGroundTerm |>> GroundTerm) <|> pVariableTerm

    let pBasicTerms : AspParser<Term list> = 
        sepBy1 pBasicTerm (token ",")


    let consExpressionTerm (expr1 : Expression) : Term = 
        match expr1 with
        | GroundExpr groundTerm -> GroundTerm groundTerm
        | _ -> ExpressionTerm expr1
    
    // Term parser and related parsers


    let rec pTerm : AspParser<Term> = 
         (attempt pFunctionTerm) <|> pExpressionTerm <|>  pBasicTerm
    
    and pTerms : AspParser<Term list> = 
        sepBy1 pTerm (token ",")

    
    and pGroundExpr : AspParser<Expression> = 
        pGroundTerm |>> GroundExpr

    and pFunctionTerm : AspParser<Term> = 
        parse.Delay (fun () -> 
            pipe2 pPredicateName
                (parens pTerms)
                (fun name terms -> FunctionTerm(name, terms)))
    
    and pExpressionTerm : AspParser<Term> =
        pMulExpr |>> consExpressionTerm
    
    and pMulExpr : AspParser<Expression> = 
        parse.Delay (fun () ->chainl1 pAddExpr pMulOp2)

    and pAddExpr : AspParser<Expression> = 
        chainl1 pFactor pAddOp2

    and pFactor : AspParser<Expression> = 
        parens pMulExpr <|> pGroundExpr



