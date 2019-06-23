// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause


namespace SLPotassco.AspCore

module Parser =

    
    open FParsec

    open SLPotassco.AspCore.Syntax



    let lexeme (parser : Parser<'a, 'u>) : Parser<'a, 'u> = 
        parser .>> spaces


    let pIdentifier () : Parser<string, 'u> = 
        let opts = new IdentifierOptions(isAsciiIdStart = isAsciiLetter)
        lexeme (identifier opts)
        
    let pArithOp () : Parser<ArithOp, 'u> = 
        let plus    = pchar '+' |>> fun _ -> OpPlus
        let minus   = pchar '-' |>> fun _ -> OpMinus
        let times   = pchar '*' |>> fun _ -> OpTimes
        let divide  = pchar '/' |>> fun _ -> OpDiv
        lexeme (plus <|> minus <|> times <|> divide)


