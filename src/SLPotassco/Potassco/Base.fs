// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause


namespace SLPotassco.Potassco

module Base =
    

    open SLPotassco.AspCore.Syntax

    type AnswerStatus = 
        | Satisfiable
        | Unsatisfiable
        | Unknown

    type ClingoAnswer = 
        { Index : int 
          Terms : Term list 
          Status : AnswerStatus 
        }

    type ClingoOutput = 
        { VersionNumber : string
          InputSource : string
          Answers : ClingoAnswer list
        
        }
