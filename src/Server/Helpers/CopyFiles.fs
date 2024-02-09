namespace Helpers.Server

open System.IO
open CEBuilders
open FsToolkit.ErrorHandling

module CopyingFiles =
    
    let private processFile source destination action =
                         
        pyramidOfDoom 
            {
                let! sourceFilepath = Path.GetFullPath(source) |> Option.ofStringObj, Error <| sprintf "Chyba při čtení cesty k %s" source
                let! destinFilepath = Path.GetFullPath(destination) |> Option.ofStringObj, Error <| sprintf "Chyba při čtení cesty k %s" destination
                let! _ = (new FileInfo(sourceFilepath)).Exists |> Option.ofBool, Error <| sprintf "Zdrojový soubor %s neexistuje" sourceFilepath

                return Ok <| action sourceFilepath destinFilepath
            }           

    let internal copyFiles source destination overwrite =
        try
            let action sourceFilepath destinFilepath = File.Copy(sourceFilepath, destinFilepath, overwrite) 
                in processFile source destination action
        with
        | ex -> Error <| sprintf "Chyba při kopírování souboru %s do %s. %s." source destination (string ex.Message)

    (*  
    let internal moveFiles source destination =
        try
            let action sourceFilepath destinFilepath = File.Move(sourceFilepath, destinFilepath, true) 
                in processFile source destination action
        with
        | _ -> Error <| sprintf "Chyba při přemísťování souboru %s do %s" source destination
    *)  