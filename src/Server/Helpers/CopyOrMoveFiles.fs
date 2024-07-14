namespace Helpers.Server

open System
open System.IO
open CEBuilders
open FsToolkit.ErrorHandling

// Free monad for educational purposes
// See also module CopyOrMoveFiles below
module CopyOrMoveFilesFM = 
           
    type private CommandLineInstruction<'a> =
        | SourceFilepath of (Result<string, string> -> 'a)
        | DestinFilepath of (Result<string, string> -> 'a)
        | CopyOrMove of (Result<string, string> * Result<string, string>)

    type private CommandLineProgram<'a> =
        | Pure of 'a 
        | Free of CommandLineInstruction<CommandLineProgram<'a>>

    let private mapI f = 
        function
        | SourceFilepath next -> SourceFilepath (next >> f)
        | DestinFilepath next -> DestinFilepath (next >> f)
        | CopyOrMove s        -> CopyOrMove s 

    let rec private bind f = 
        function
        | Free x -> x |> mapI (bind f) |> Free
        | Pure x -> f x

    type private CommandLineProgramBuilder = CommandLineProgramBuilder with
        member this.Bind(p, f) = //x |> mapI (bind f) |> Free
            match p with
            | Pure x     -> f x
            | Free instr -> Free (mapI (fun p' -> this.Bind(p', f)) instr)
        member _.Return x = Pure x
        member _.ReturnFrom p = p

    let private cmdBuilder = CommandLineProgramBuilder

    [<Struct>]
    type internal Config =
        {
            source: string
            destination: string
            fileName: string
        }

    [<Struct>]
    type internal IO = 
        | Copy
        | Move     

    let rec private interpret config io clp =
               
        let f (source : Result<string, string>) (destination : Result<string, string>) : Result<unit, string> =
            match source, destination with
            | Ok s, Ok d ->
                          try
                              match io with
                              | Copy -> Ok (File.Copy(s, Path.Combine(d, config.fileName), true))
                              | Move -> Ok (File.Move(s, Path.Combine(d, config.fileName), true))
                          with
                          | ex -> Error ex.Message
            | Error e, _ ->
                          Error e
            | _, Error e ->
                          Error e

        match clp with 
        | Pure x                     ->
                                      x

        | Free (SourceFilepath next) ->
                                      let sourceFilepath source =                                        
                                          pyramidOfDoom
                                              {
                                                  let! value = Path.GetFullPath(source) |> Option.ofNullEmpty, Error <| sprintf "Chyba při čtení cesty k %s" source   
                                                  let! value = 
                                                      (
                                                          let fInfodat: FileInfo = new FileInfo(value)   
                                                          Option.fromBool value fInfodat.Exists
                                                      ), Error <| sprintf "Zdrojový soubor %s neexistuje" value
                                                  return Ok value
                                              }

                                      interpret config io (next (sourceFilepath config.source))

        | Free (DestinFilepath next) ->
                                      let destinFilepath destination =                                        
                                          pyramidOfDoom
                                              {
                                                  let! value = Path.GetFullPath(destination) |> Option.ofNullEmpty, Error <| sprintf "Chyba při čtení cesty k %s" destination
                                                  (*
                                                      let! value = 
                                                          (
                                                              let dInfodat: DirectoryInfo = new DirectoryInfo(value)   
                                                              Option.fromBool value dInfodat.Exists
                                                          ), Error <| sprintf "Chyba při čtení cesty k %s" value
                                                  *) 
                                                  return Ok value
                                              }

                                      interpret config io (next (destinFilepath config.destination))

        | Free (CopyOrMove (s, d))  ->
                                     try
                                         f s d 
                                     with
                                     | ex ->
                                           match s, d with
                                           | Ok s, Ok d ->
                                                         Error <| sprintf "Chyba při kopírování nebo přemísťování souboru %s do %s. %s." s d (string ex.Message)
                                           | Error e, _ ->
                                                         Error e
                                           | _, Error e ->
                                                         Error e      

    let internal copyOrMoveFiles config io =

        cmdBuilder
            {
                let! sourceFilepath = Free (SourceFilepath Pure)                
                let! destinFilepath = Free (DestinFilepath Pure)

                return! Free (CopyOrMove (sourceFilepath, destinFilepath))
            }

        |> interpret config io


module CopyOrMoveFiles =
    
    let private processFile source destination action =
                                                
        pyramidOfDoom 
            {
                let! sourceFilepath = Path.GetFullPath(source) |> Option.ofNullEmpty, Error <| sprintf "Chyba při čtení cesty k %s" source
                let! destinFilepath = Path.GetFullPath(destination) |> Option.ofNullEmpty, Error <| sprintf "Chyba při čtení cesty k %s" destination
                let! _ = (new FileInfo(sourceFilepath)).Exists |> Option.ofBool, Error <| sprintf "Zdrojový soubor %s neexistuje" sourceFilepath

                return Ok <| action sourceFilepath destinFilepath
            }           

    let internal copyFiles source destination overwrite =

        try
            let action sourceFilepath destinFilepath = File.Copy(sourceFilepath, destinFilepath, overwrite) 
                in processFile source destination action
        with
        | ex -> Error <| sprintf "Chyba při kopírování souboru %s do %s. %s." source destination (string ex.Message)
              
    let internal moveFiles source destination =
        try
            let action sourceFilepath destinFilepath = File.Move(sourceFilepath, destinFilepath, true) 
                in processFile source destination action
        with
        | _ -> Error <| sprintf "Chyba při přemísťování souboru %s do %s" source destination
    