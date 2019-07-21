// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause


namespace SLPotassco.Clingo

module Base =
    

    open SLPotassco.AspCore.Syntax

    type AnswerStatus = 
        | Satisfiable
        | Unsatisfiable
        | Unknown

    type ClaspAnswer = ClaspAnswer of terms : Term list * status : AnswerStatus

    type ClingoResult = 
        { Answers : ClaspAnswer list
        
        }
