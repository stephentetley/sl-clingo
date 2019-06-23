// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause


namespace SLPotassco.AspCore

module Syntax =

    type Identifier = string

    type ArithOp = 
        | OpPlus
        | OpMinus
        | OpTimes
        | OpDiv


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
    

        
