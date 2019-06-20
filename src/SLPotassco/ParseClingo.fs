// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause


namespace SLPotassco

module ParseClingo =
    
    open System

    open FParsec

    let token (str : string) : Parser<string, 'u> = 
        pstring str .>> spaces

    let versionNumber () : Parser<string, 'u> = manySatisfy (not << Char.IsWhiteSpace)

    let clingoVersion () = token "clingo" >>. token "version" >>. versionNumber ()
        

    let pAnswer () = pstring "Answer:" >>. spaces >>. pint32

    let pStatus () = 
        pstring "SATISFIABLE" <|> pstring "UNSATISFIABLE" <|> pstring "UNKNOWN"