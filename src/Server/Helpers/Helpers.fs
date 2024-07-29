namespace Helpers.Server

open System
open System.IO

open CEBuilders
open FsToolkit.ErrorHandling

[<RequireQualifiedAccess>]
module Result =

    let internal toOption = 
        function   
        | Ok value -> Some value 
        | Error _  -> None  

[<RequireQualifiedAccess>]
module Option =

    let internal ofBool = 
        function   
        | true  -> Some ()  
        | false -> None

    let internal fromBool value =  
        function   
        | true  -> Some value  
        | false -> None

    let internal toResult err = 
        function   
        | Some value -> Ok value 
        | None       -> Error err

    let internal ofNullEmpty (value: 'nullableValue) = //NullOrEmpty

        pyramidOfHell
            {
                let!_ = not <| System.Object.ReferenceEquals(value, null), None 
                let value = string value 
                let! _ = not <| String.IsNullOrEmpty(value), None 

                return Some value
            }

    let internal ofNullEmptySpace (value: 'nullableValue) = //NullOrEmpty, NullOrWhiteSpace
    
        pyramidOfHell
            {
                let!_ = not <| System.Object.ReferenceEquals(value, null), None 
                let value = string value 
                let! _ = not <| (String.IsNullOrEmpty(value) || String.IsNullOrWhiteSpace(value)), None
    
                return Some value
            }

module Resources =

    let internal pathToResources path = 
        try
            //sprintf "%s%s%s" AppDomain.CurrentDomain.BaseDirectory "Resources" path //nefunguje, haze to do debug
            Path.Combine("Resources", path) //CopyAlways
        with
        | ex -> failwith (sprintf "Kontaktuj programátora, závažná chyba na serveru !!! %s" ex.Message)

module Casting =

    //For xml deserialization
    let internal castAs<'a> (o: obj) : 'a option =    //SRTPs are not applicable for this specific type casting.
        match Option.ofNull o with
        | Some (:? 'a as result) -> Some result
        | _                      -> None

module Miscellaneous = 

    open System

    let internal strContainsOnlySpace str =
        str |> Seq.forall (fun item -> item = (char)32)  //A string is a sequence of characters => use Seq.forall to test directly //(char)32 = space*

(*       
System.IO.File provides static members related to working with files, whereas System.IO.FileInfo represents a specific file and contains non-static members for working with that file.          
Because all File methods are static, it might be more efficient to use a File method rather than a corresponding FileInfo instance method if you want to perform only one action. All File methods 
require the path to the file that you are manipulating.    
The static methods of the File class perform security checks on all methods. If you are going to reuse an object several times, consider using the corresponding 
instance method of FileInfo instead, because the security check will not always be necessary.    
*) 
        
    
        
       
          



