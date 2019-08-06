// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause


namespace SLClingo.AspCore

module Syntax =

    // Note - ASP Core syntax is probably a bit heavy for representing 
    // output terms reported for #show directives in Clingo.

    type Identifier = string

    type ArithOp = 
        | OpPlus
        | OpMinus
        | OpTimes
        | OpDiv
    
    type BinOp = 
        | OpEqual
        | OpUnequal
        | OpLess
        | OpGreater
        | OpLessOrEq
        | OpGreaterOrEq

    type AggregateFunction = 
        | AggrCount
        | AggrMax
        | AggrMin
        | AggrSum

    type PredicateName = 
        | Id of Identifier
        | QuotedName of string

    type GroundTerm = 
        | SymbolicConstant of string
        | String of string
        | Number of int


    type Expression = 
        | ArithmeticExpr of Expression * ArithOp * Expression
        | SimpleExpr of Expression          // in parens
        | GroundExpr of GroundTerm
        | VariableExpr of negation: bool * Identifier
   
    type Term = 
        | GroundTerm of term : GroundTerm
        | VariableTerm of Identifier
        | ExpressionTerm of Expression
        | FunctionTerm of name : PredicateName * terms : Term list

    type Atom = 
        | Atom of name : PredicateName * terms : Term list

        
