namespace Auxiliaries.Server

open System
open System.IO
open Newtonsoft.Json
open System.Runtime.Serialization

open FsToolkit.ErrorHandling

open DtoGet.Server.DtoGet
open DtoXml.Server.DtoXml

module PatternBuilders = 

    let private (>>=) condition nextFunc =
        match fst condition with
        | false -> snd condition
        | true  -> nextFunc()   

    [<Struct>]
    type internal Builder1 = Builder1 with            
        member _.Bind(condition, nextFunc) = (>>=) <| condition <| nextFunc
        member _.Using x = x
        member _.Return x = x

    let internal pyramidOfHell = Builder1

 //************************************************************************************* 

    let private (>>==) (optionExpr, errDuCase) nextFunc =
        match optionExpr with
        | Some value -> nextFunc value 
        | _          -> errDuCase  
                
    [<Struct>]
    type internal Builder2 = Builder2 with    
        member _.Bind(condition, nextFunc) = (>>==) condition nextFunc
        member _.Return x : 'a = x

    let internal pyramidOfDoom = Builder2

module Result =

    let internal toOption f : 'a option = 
        f                      
        |> function   
            | Ok value -> Some value 
            | Error _  -> None  

module Option =

    let internal ofBool cond = 
           cond                      
           |> function   
               | true  -> Some ()  
               | false -> None

    let internal fromBool value cond : 'a option = 
        cond                      
        |> function   
            | true  -> Some value  
            | false -> None

module Resources =

    let internal pathToResources path = 
        try
            sprintf "%s%s%s" AppDomain.CurrentDomain.BaseDirectory "Resources" path //CopyAlways      
        with
        | ex -> failwith (sprintf "Závažná chyba na serveru !!! %s" ex.Message)

module Casting =

    //for educational purposes only
    let inline internal downCast (x: obj) = //The downCast function does not handle null values explicitly and may raise a runtime exception if x is null regardless of using srtp or generics. 
        match x with
        | :? ^a as value -> Some value 
        | _              -> None

    let internal castAs<'a> (o: obj) : 'a option =    //srtp nefunguje pro tento zpusob type casting 
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

//tryWith to be implemented for all serialization at the place of its using 
module Serialisation =

    let private optionToFailwith str = //TODO
         function
         | Some value -> value
         | None       -> failwith str  

    //vyzkouseno pro kontakt data
    //System.Runtime.Serialization vyzaduje stejny typ pro serializaci a deserializaci, proto separatni DTO
    //System.Xml.Serialization tady nefungoval
    let internal serializeToXml (record: 'a) (xmlFile: string) =

        let filepath =
            Path.GetFullPath(xmlFile) 
            |> Option.ofNull
            |> optionToFailwith "Chyba při čtení cesty k xml souboru"            
            
        let xmlSerializer =
            new DataContractSerializer(typeof<'a>)        
            |> Option.ofNull
            |> optionToFailwith (sprintf "%s%s" "Chyba při serializaci do " xmlFile)

        let stream =
            File.Create(filepath)
            |> Option.ofNull
            |> optionToFailwith (sprintf "%s%s" "Chyba při serializaci do " xmlFile)

        xmlSerializer.WriteObject(stream, record)

        stream.Close()
        stream.Dispose()        

    //vyzkouseno pro links   
    let internal serializeToJson (record: 'a) (jsonFile: string) =

        let filepath =
            Path.GetFullPath(jsonFile) 
            |> Option.ofNull
            |> optionToFailwith (sprintf "%s%s" "Chyba při čtení cesty k souboru " jsonFile)

        let json =
            JsonConvert.SerializeObject(record) 
            |> Option.ofNull 
            |> optionToFailwith (sprintf "%s%s" "Chyba při serializaci do " jsonFile)

        File.WriteAllText(filepath, json)

    //nepouzivano, ale vyzkouseno, co to udela - json v xml (ChatGPT vyrazil protest :-)), a funguje to :-) 
    let internal serialize record xmlFile =

        let filepath =
            Path.GetFullPath(xmlFile) 
            |> Option.ofNull
            |> optionToFailwith "Chyba při čtení cesty k souboru json.....xml" 

        let xmlSerializer =
            new DataContractSerializer(typedefof<string>)          
            |> Option.ofNull 
            |> optionToFailwith "Chyba při serializaci"

        use stream = File.Create(filepath)   
        xmlSerializer.WriteObject(stream, JsonConvert.SerializeObject(record))            

//tryWith to be implemented for all deserialization at the place of its using 
module Deserialisation =

    let private optionToFailwith str = //TODO
        function
        | Some value -> value
        | None       -> failwith str  

    //vyzkouseno pro kontakt data
    //System.Runtime.Serialization (System.Xml.Serialization tady nefungoval)
    let internal deserializeFromXml<'a> (xmlFile : string) =
                  
        let filepath =
            Path.GetFullPath(xmlFile) 
            |> Option.ofNull 
            |> optionToFailwith (sprintf "%s%s" "Chyba při čtení cesty k souboru " xmlFile)

        let fInfodat: FileInfo = new FileInfo(filepath)  
        match fInfodat.Exists with 
        | true  ->
                let xmlSerializer =
                    new DataContractSerializer(typeof<'a>) 
                    |> Option.ofNull
                    |> optionToFailwith (sprintf "%s%s" "Chyba při serializaci z " xmlFile)

                let stream =
                    File.OpenRead(filepath)
                    |> Option.ofNull
                    |> optionToFailwith (sprintf "%s%s" "Chyba při deserializaci z " xmlFile)

                let result =
                    xmlSerializer.ReadObject(stream)  
                    |> Option.ofNull
                    |> optionToFailwith (sprintf "%s%s" "Chyba při čtení dat ze souboru " xmlFile)
                    |> Casting.castAs<KontaktValuesDtoXml> 
                    |> optionToFailwith (sprintf "%s%s" "Chyba při čtení dat ze souboru (downcasting) " xmlFile)

                stream.Close()
                stream.Dispose()

                result
        | false ->
                failwith (sprintf "Soubor %s nenalezen" xmlFile)    

    //vyzkouseno pro links 
    let internal deserializeFromJson<'a> (jsonFile : string) =
                 
        let filepath =
            Path.GetFullPath(jsonFile) 
            |> Option.ofNull 
            |> optionToFailwith (sprintf "%s%s" "Chyba při čtení cesty k souboru " jsonFile) 

        let fInfodat: FileInfo = new FileInfo(filepath)  
        match fInfodat.Exists with 
        | true  ->
                let json =
                    File.ReadAllText(filepath)
                    |> Option.ofNull 
                    |> optionToFailwith (sprintf "%s%s" "Chyba při deserializaci z " jsonFile)

                let result =
                    JsonConvert.DeserializeObject<'a>(json)
                    |> Casting.castAs<LinkAndLinkNameValuesDtoGet> 
                    |> optionToFailwith (sprintf "%s%s" "Chyba při čtení dat ze souboru (downcasting) " jsonFile)
                result   
        | false ->
                failwith (sprintf "Soubor %s nenalezen" filepath) 

    //nepouzivano, ale vyzkouseno, co to udela - json v xml (ChatGPT vyrazil protest :-)), a funguje to :-)     
    let internal deserialize xmlFile = 
           
        let filepath =
            Path.GetFullPath(xmlFile) 
            |> Option.ofNull
            |> optionToFailwith (sprintf "%s%s" "Chyba při čtení cesty k souboru " xmlFile) 
          
        let jsonString() = 

            let xmlSerializer =
                new DataContractSerializer(typedefof<string>) 
                |> Option.ofNull 
                |> optionToFailwith "Chyba při serializaci"

            let fileStream =
                File.ReadAllBytes(filepath)  
                |> Option.ofNull 
                |> optionToFailwith (sprintf "%s%s" "Chyba při čtení dat ze souboru " xmlFile)

            use memoryStream = new MemoryStream(fileStream)

            let resultObj =
                xmlSerializer.ReadObject(memoryStream)  
                |> Option.ofNull
                |> optionToFailwith (sprintf "%s%s" "Chyba při čtení dat ze souboru " xmlFile)

            let resultString =
                unbox resultObj  
                |> Option.ofNull
                |> optionToFailwith (sprintf "%s%s" "Chyba při čtení dat ze souboru (unboxing) " xmlFile)    

            let jsonString = JsonConvert.DeserializeObject<'a>(resultString) 
            jsonString
           
        let fInfodat: FileInfo = new FileInfo(filepath)  
        match fInfodat.Exists with 
        | true  -> jsonString()              
        | false -> failwith (sprintf "Soubor %s nenalezen" xmlFile) 

module CopyingFiles =  //trywith transferred to Server.fs

    let private optionToFailwith str = //TODO
        function
        | Some value -> value
        | None       -> failwith str  
        
    let internal copyFiles source destination =
                                                                    
        let perform x =                                    
            let sourceFilepath =
                Path.GetFullPath(source) 
                |> Option.ofNull
                |> optionToFailwith (sprintf "%s%s" "Chyba při čtení cesty k souboru " source)

            let destinFilepath =
                Path.GetFullPath(destination) 
                |> Option.ofNull 
                |> optionToFailwith (sprintf "%s%s" "Chyba při čtení cesty k souboru " source) 
                    
            let fInfodat: FileInfo = new FileInfo(sourceFilepath)  
            match fInfodat.Exists with 
            | true  -> File.Copy(sourceFilepath, destinFilepath, true)             
            | false -> failwith (sprintf "Soubor %s nenalezen" source)

        perform ()

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
        
    
        
       
          



