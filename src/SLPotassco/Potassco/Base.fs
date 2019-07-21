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

    /// AtLeast is indicated by cling printing a plus suffix on the model count
    type ModelCount = 
        | AtLeast of int        
        | Exactly of int

    type ClingoOutput = 
        { VersionNumber : string
          InputSource : string
          Answers : ClingoAnswer list
          Models : ModelCount
          Calls : int
          TimeStats : string
          CpuTime : string
        }
