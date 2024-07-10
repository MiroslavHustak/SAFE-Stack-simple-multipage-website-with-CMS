namespace Helpers.Server

open System
open System.IO
open Newtonsoft.Json
open System.Runtime.Serialization

open FsToolkit.ErrorHandling

module CEBuilders = 
      
    [<Struct>]
    type internal Builder1 = Builder1 with            
        member _.Bind(condition, nextFunc) =
            match fst condition with
            | false -> snd condition
            | true  -> nextFunc() 
        member _.Using x = x
        member _.Return x = x

    let internal pyramidOfHell = Builder1

 //************************************************************************************* 
                
    [<Struct>]
    type internal Builder2 = Builder2 with    
        member _.Bind((optionExpr, err), nextFunc) =
            match optionExpr with
            | Some value -> nextFunc value 
            | _          -> err  
        member _.Return x : 'a = x   
        member _.ReturnFrom x : 'a = x 
        member _.TryFinally(body, compensation) =
            try 
                body()
            finally
                compensation()
        member _.Zero () = ()
        member _.Using(resource, binder) =
            use r = resource
            binder r
    
    let internal pyramidOfDoom = Builder2

//************************************************************************************* 

    [<Struct>]
    type internal Builder3 = Builder3 with    
        member _.Bind((resultExpr, defaultRc), nextFunc) =
            match resultExpr with
            | Ok value  -> nextFunc value 
            | Error err -> defaultRc, err  
        member _.Return x : 'a = x

    let internal pyramidOfInferno = Builder3