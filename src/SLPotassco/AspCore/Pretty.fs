// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause


namespace SLPotassco.AspCore

module Pretty =

    
    open SLFormat.Pretty            // Lib: sl-format

    open SLPotassco.AspCore.Syntax

    let ppIdentifier (name : Identifier) : Doc = text name

    let ppArithOp (op : ArithOp) : Doc = 
        match op with
        | OpPlus -> character '+'
        | OpMinus -> character '-'
        | OpTimes -> character '*'
        | OpDiv -> character '/'

    let ppPredicateName (predicateName : PredicateName) : Doc = 
        match predicateName with
        | Id name -> ppIdentifier name
        | QuotedName name -> dquotes (text name)

    let ppGroundTerm (term : GroundTerm) : Doc = 
        match term with
        | SymbolicConstant str -> text str
        | String str -> dquotes (text str)      // escaping to do
        | Number int -> intDoc int

    let ppExpression (expr : Expression) : Doc = 
        let rec work (src : Expression) (sk :Doc -> Doc) : Doc =
            match src with
            | ArithmeticExpr(expr1, op, expr2) -> 
                let rator = ppArithOp op
                work expr1 (fun lhs ->
                work expr2 (fun rhs -> 
                sk (lhs ^+^ rator ^+^ rhs)))
            | SimpleExpr expr1 -> 
                work expr1 (fun body -> 
                sk (parens body))
            | GroundExpr term -> 
                sk (ppGroundTerm term)
            | VariableExpr (negation, name) ->
                let result = 
                    if negation then 
                        character '-' ^^ ppIdentifier name
                    else ppIdentifier name
                sk result
        work expr (fun x -> x)


