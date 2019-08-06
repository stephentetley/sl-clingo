// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause


namespace SLClingo.Clingo

module Invoke =
    
    open FParsec

    open SLFormat.CommandOptions
    open SLFormat.CommandOptions.SimpleInvoke

    open SLClingo.Clingo.Base
    open SLClingo.Clingo.ParseClingoOutput
    
    /// To be added to SLFormat...
    let intLiteral (i : int) : CmdOpt = 
        i.ToString() |> literal 

    let private gringoArgs (options : CmdOpt list) (files : string list): CmdOpt list = 
        let fileArgs = List.map literal files
        options @ fileArgs

    let gringo (workingDirectory : string) (options : CmdOpt list) (files : string list) = 
        SimpleInvoke.runProcess (Some workingDirectory) "gringo" (gringoArgs options files)


    let private clingoArgs (maxAnswerSets : int option) (options : CmdOpt list) (files : string list)  : CmdOpt list = 
        let fileArgs = List.map literal files
        let numberArgs = match maxAnswerSets with | None -> [] | Some i -> [ intLiteral i ]
        numberArgs @ options @ fileArgs

    type RunClingoFailure = 
        | SysFail of exn
        | ClingoFail of ClingoFailure
        | OutputParseFail of string
        

    let clingoFailureDescription (clingoFailure : RunClingoFailure) : string = 
        match clingoFailure with
        | SysFail exc -> sprintf "*** SYSTEM: %s" exc.Message
        | ClingoFail x -> sprintf "%s\n%s" x.Error x.Info
        | OutputParseFail msg -> sprintf "*** Parsing failure: %s" msg    


    let clingo (workingDirectory : string) 
                (maxAnswerSets : int option) 
                (options : CmdOpt list) 
                (files : string list)  : Result<ProcessAnswer, exn> = 
        match runProcess (Some workingDirectory) "clingo" (clingoArgs maxAnswerSets options files) with
        | ProcessResult.SysExn excptn -> Result.Error excptn
        | ProcessResult.Answer ans -> Result.Ok ans

    let runClingo (workingDirectory : string) 
                    (maxAnswerSets : int option) 
                    (options : CmdOpt list) 
                    (files : string list)  : Result<ClingoOutput, RunClingoFailure> = 
            match clingo workingDirectory maxAnswerSets options files with
            | Result.Error excptn -> Result.Error (SysFail excptn)
            | Result.Ok ans ->
                // clingo does not necessarily return 0 when it finds answers
                if ans.StdErr = "" then
                    match runParserOnString pClingoOutput () "stdout" ans.StdOut with
                    | ParserResult.Success(ans, _, _) -> Result.Ok ans
                    | ParserResult.Failure(msg, _, _) -> Result.Error (OutputParseFail msg)
                else
                    match runParserOnString pClingoFailure () "stderr" ans.StdErr with
                    | ParserResult.Success(ans, _, _) -> Result.Error (ClingoFail ans)
                    | ParserResult.Failure(msg, _, _) -> Result.Error (OutputParseFail msg)

    let clasp (workingDirectory : string) (options : CmdOpt list) (files : string list) (number : int option) = 
        SimpleInvoke.runProcess (Some workingDirectory) "clasp" (clingoArgs number options files)

