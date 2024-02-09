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
        member _.Bind((optionExpr, errDuCase), nextFunc) =
            match optionExpr with
            | Some value -> nextFunc value 
            | _          -> errDuCase  
        member _.Return x : 'a = x

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

    //************************************************************************************* 
    [<Struct>]
    type internal MyTypeBuilder (param: string option) =        
         member _.Bind(condition, nextFunc) = 
             match condition with
             | false -> nextFunc() 
             | true  -> param
         member _.Return x = x  
     
    let internal strictStringCheck = MyTypeBuilder 

module Result =

    let internal toOption = 
        function   
        | Ok value -> Some value 
        | Error _  -> None  

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

    let internal ofStringObj (value: 'nullableValue) = //NullOrEmpty

        CEBuilders.strictStringCheck None
            {
                let!_ = System.Object.ReferenceEquals(value, null) 
                let value = string value 
                let! _ = String.IsNullOrEmpty(value) 

                return Some value
            }

    let internal ofStringObjXXL (value: 'nullableValue) = //NullOrEmpty, NullOrWhiteSpace
    
        CEBuilders.strictStringCheck None
            {
                let!_ = System.Object.ReferenceEquals(value, null) 
                let value = string value 
                let! _ = String.IsNullOrEmpty(value) || String.IsNullOrWhiteSpace(value)
    
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

    //for educational purposes only 
    let inline internal downCast (x: obj) = //With this function, null values are not explicitly handled, potential runtime exception if 'x' is null. 
        match x with
        | :? ^a as value -> Some value 
        | _              -> None

    //Objects handled with extra care due to potential type-related concerns (you can call it "paranoia" :-)).
    let internal castAs<'a> (o: obj) : 'a option =    //SRTPs are not applicable for this specific type casting.
        match Option.ofNull o with
        | Some (:? 'a as result) -> Some result
        | _                      -> None

module private TryParserInt =

    let tryParseWith (tryParseFunc: string -> bool * _) = tryParseFunc >> function
        | true, value -> Some value
        | false, _    -> None

    let parseInt = tryParseWith <| System.Int32.TryParse

    let (|Int|_|) = parseInt        

module Parsing =

    let private f x =
        let isANumber = x                                          
        isANumber   
                   
    let internal parseMe = 
        function            
        | TryParserInt.Int i -> f i
        | _                  -> 0  

    let internal parseMeOption = 
        function            
        | TryParserInt.Int i -> f Some i
        | _                  -> None     

module Miscellaneous = 

    open System

    let internal (|StringNonN|) s = 
        s 
        |> Option.ofNull 
        |> function 
            | Some value -> string value
            | None       -> String.Empty

    let internal strContainsOnlySpace str =
        str |> Seq.forall (fun item -> item = (char)32)  //A string is a sequence of characters => use Seq.forall to test directly //(char)32 = space*

(*       
System.IO.File provides static members related to working with files, whereas System.IO.FileInfo represents a specific file and contains non-static members for working with that file.          
Because all File methods are static, it might be more efficient to use a File method rather than a corresponding FileInfo instance method if you want to perform only one action. All File methods 
require the path to the file that you are manipulating.    
The static methods of the File class perform security checks on all methods. If you are going to reuse an object several times, consider using the corresponding 
instance method of FileInfo instead, because the security check will not always be necessary.    
*) 
        
    
        
       
          



