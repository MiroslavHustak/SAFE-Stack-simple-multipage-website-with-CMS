module ROP_Functions

open System
open System.IO

open Errors
open DiscriminatedUnions

let tryWith f1 f2 x = //x se ne vzdy pouziva, ale z duvodu jednotnosti ponechano 
    try
        try          
           f1 x |> Success
        finally
           f2 x
    with
    | ex -> Failure ex.Message  

let deconstructor1 =  
    function
    | Success x  -> x, String.Empty                                                   
    | Failure ex -> Array.empty, ex 

let deconstructor2 =  
    function
    | Success x  -> x                                                   
    | Failure ex -> true

let deconstructor4 y =  
    function
    | Success x  -> x                                                   
    | Failure ex -> error4 ex |> ignore //TODO remove ignore and create some action
                    y

let optionToString str x = 
    match x with 
    | Some value -> value
    | None       -> error4 str

let optionToGenerics2 str x = 
    function
    | Some value -> value
    | None       -> error4 str |> ignore //TODO remove ignore and create some action                                 
                    x //whatever of the particular type 
                    
                                      
                    





