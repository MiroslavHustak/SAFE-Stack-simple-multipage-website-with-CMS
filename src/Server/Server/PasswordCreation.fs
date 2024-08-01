namespace Server

open System
open System.IO

open Fable.Remoting.Server
open FsToolkit.ErrorHandling

open Settings
open Helpers.Server.Security2

module PasswordCreation =
       
    //Password creation
    //To be used one-time only
    
    let internal pswHash() = //to be used only once before bundling             

        try
            let usr = uberHash "" //delete username before bundling
            let psw = uberHash "" //delete password before bundling
            let mySeq = seq { usr; psw }

            use sw = 
                match Path.GetFullPath(pathToUberHashTxt) |> Option.ofNull with
                | Some value ->
                              new StreamWriter(Path.GetFullPath(pathToUberHashTxt)) //non-nullable, ex caught with tryWith
                | None       ->
                              //to be manually verified at this code location
                              //TODO log 
                              new StreamWriter(Path.GetFullPath(String.Empty)) //ex caught with tryWith      
           
            mySeq
            |> Seq.iter (fun item -> do sw.WriteLine(item))
            |> Ok

        with
        | ex -> Error (string ex.Message)

        |> function
            | Ok value  -> value
            | Error err -> () //TODO log 