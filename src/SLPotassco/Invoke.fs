// Copyright (c) Stephen Tetley 2019
// License: BSD 3 Clause


namespace SLPotassco

module Invoke =
    

    open SLFormat.CommandOptions
    
    /// To be added to SLFormat...
    let intLiteral (i : int) : CmdOpt = 
        i.ToString() |> literal 

    let private gringoArgs (options : CmdOpt list) (files : string list): CmdOpt list = 
        let fileArgs = List.map literal files
        options @ fileArgs

    let gringo (workingDirectory : string) (options : CmdOpt list) (files : string list) = 
        SimpleInvoke.runProcess (Some workingDirectory) "gringo" (gringoArgs options files)


    let private clingoArgs (options : CmdOpt list) (files : string list) (number : int option) : CmdOpt list = 
        let fileArgs = List.map literal files
        let numberArgs = match number with | None -> [] | Some i -> [ intLiteral i ]
        options @ fileArgs @ numberArgs


    let clingo (workingDirectory : string) (options : CmdOpt list) (files : string list) (number : int option) = 
        SimpleInvoke.runProcess (Some workingDirectory) "clingo" (clingoArgs options files number)

    let clasp (workingDirectory : string) (options : CmdOpt list) (files : string list) (number : int option) = 
        SimpleInvoke.runProcess (Some workingDirectory) "clasp" (clingoArgs options files number)
