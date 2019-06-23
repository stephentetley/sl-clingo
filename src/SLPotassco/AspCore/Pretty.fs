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

    let ppBinOp (op : BinOp) : Doc = 
        match op with 
        | OpEqual -> character '='
        | OpUnequal -> text "<>"
        | OpLess -> character '<'
        | OpGreater -> character '>'
        | OpLessOrEq -> text "<="
        | OpGreaterOrEq -> text ">="
    
    let ppAggregateFunction (aggregate : AggregateFunction) : Doc = 
        match aggregate with
        | AggrCount -> text "#count"
        | AggrMax -> text "#max"
        | AggrMin -> text "#min"
        | AggrSum -> text "#sum"



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
        let rec work (src : Expression) (sk : Doc -> Doc) : Doc =
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

    let ppTerm (term : Term) : Doc = 
        let rec work (src : Term) (sk : Doc -> Doc) : Doc =
            match src with
            | GroundTerm term1 -> 
                sk (ppGroundTerm term1)
            | VariableTerm name -> 
                sk (ppIdentifier name)
            | ExpressionTerm expr -> 
                sk (ppExpression expr)
            | FunctionTerm(name, terms) ->
                let ident = ppPredicateName name
                workList terms (fun xs -> 
                sk (ident ^^ tupled xs))
        and workList (items : Term list) (sk : Doc list -> Doc) : Doc = 
            match items with
            | [] -> 
                sk []
            | t1 :: rest -> 
                work t1 (fun a1 -> 
                workList rest (fun ac -> 
                sk (a1::ac)))
        work term (fun x -> x)

    let ppAtom (atom : Atom) : Doc = 
        let rec work (src : Atom) (sk : Doc -> Doc) : Doc = 
            match src with
            | Atom(name, terms) ->
                let ident = ppPredicateName name
                workList terms (fun xs -> 
                sk (ident ^^ tupled xs))
        and workList (items : Term list) (sk : Doc list -> Doc) : Doc = 
            match items with
            | [] -> 
                sk []
            | t1 :: rest -> 
                let d1 = ppTerm t1
                workList rest (fun ac -> 
                sk (d1::ac))
        work atom (fun x -> x)
        