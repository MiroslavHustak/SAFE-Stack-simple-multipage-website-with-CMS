namespace Serialization.Server

//Compiler-independent template suitable for Shared as well

//************************************************

//Compiler directives
#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

//*********************************************** 

open System.IO
open Newtonsoft.Json
open System.Runtime.Serialization

open FsToolkit.ErrorHandling

open Helpers.Server
open Helpers.Server.CEBuilders

open DtoXml.Server.DtoXml
open DtoSend.Server.DtoSend

open Serialization.Coders.Server.ThothCoders

// Implement 'try with' block for serialization at each location in the code where it is used.
module Serialisation =
   
    //Tried and tested for "kontakt" data
    //System.Runtime.Serialization requires equal types for serialization and deserialization; hence separated DTOs
    //System.Xml.Serialization requires annotation //TODO do it sometime
    let internal serializeToXml (record: 'a) (xmlFile: string) =
        
        pyramidOfDoom //z duvodu jednotnosti a pripadneho rozsireni
            {
                let filepath = Path.GetFullPath(xmlFile) |> Option.ofNullEmpty //Strings handled with extra care due to potential type-related concerns (you can call it "paranoia" :-)).  
                let! filepath = filepath, Error "Zadané hodnoty nebyly uloženy, chyba při čtení cesty k xml souboru"

                let xmlSerializer = new DataContractSerializer(typeof<'a>) //non-nullable, ex caught with tryWith 
                
                let stream = File.Create(filepath) //non-nullable, ex caught with tryWith 

                xmlSerializer.WriteObject(stream, record) //non-nullable, ex caught with tryWith 
                
                stream.Close()
                stream.Dispose()                    

                return Ok ()
            }    

    //Tried and tested for "links" data
    //Newtonsoft.Json
    let internal serializeToJson (record: 'a) (jsonFile: string) =

        pyramidOfDoom 
            {
                let filepath = Path.GetFullPath(jsonFile) |> Option.ofNullEmpty //Strings handled with extra care due to potential type-related concerns (you can call it "paranoia" :-)).  
                let! filepath = filepath, Error (sprintf "%s%s" "Zadané hodnoty nebyly uloženy, chyba při čtení cesty k souboru " jsonFile)

                let json = JsonConvert.SerializeObject(record) |> Option.ofNull //Strings handled with extra care due to potential type-related concerns (you can call it "paranoia" :-)).  
                let! json = json, Error (sprintf "%s%s" "Zadané hodnoty nebyly uloženy, chyba při serializaci do " jsonFile)

                File.WriteAllText(filepath, json) //non-nullable, ex caught with tryWith 

                return Ok ()
           }

    //Tried and tested for "links" data
    //Thoth.Json.Net, Thoth.Json
    let internal serializeToJsonThoth (record: LinkAndLinkNameValuesDtoSend) (jsonFile: string) =
       
        pyramidOfDoom 
            {
                let filepath = Path.GetFullPath(jsonFile) |> Option.ofNullEmpty //Strings handled with extra care due to potential type-related concerns (you can call it "paranoia" :-)).  
                let! filepath = filepath, Error (sprintf "%s%s" "Zadané hodnoty nebyly uloženy, chyba při čtení cesty k souboru " jsonFile)

                let json = JsonConvert.SerializeObject(encoder record) |> Option.ofNull //No reflection  
                let! json = json, Error (sprintf "%s%s" "Zadané hodnoty nebyly uloženy, chyba při serializaci do " jsonFile)

                File.WriteAllText(filepath, json) //non-nullable, ex caught with tryWith 

                return Ok ()
           }

//Implement 'try with' block for deserialization at each location in the code where it is used.
module Deserialisation =

    //Tried and tested for "kontakt" data
    //System.Runtime.Serialization
    //System.Xml.Serialization requires annotation //TODO do it sometime
    let internal deserializeFromXml<'a> (xmlFile : string) =

        pyramidOfDoom
            {
                let filepath = Path.GetFullPath(xmlFile) |> Option.ofNullEmpty //Strings handled with extra care due to potential type-related concerns (you can call it "paranoia" :-)). 
                let! filepath = filepath, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot kontaktů byly dosazeny defaultní hodnoty, chyba při čtení cesty k souboru " xmlFile)

                let fInfodat: FileInfo = new FileInfo(filepath)
                let! _ =  fInfodat.Exists |> Option.ofBool, Error (sprintf "Soubor %s nenalezen" xmlFile) 

                let xmlSerializer = new DataContractSerializer(typeof<'a>) //non-nullable, ex caught with tryWith 

                let stream = File.OpenRead(filepath) //non-nullable, ex caught with tryWith

                let read = xmlSerializer.ReadObject(stream) |> Option.ofNull //my paranoia about the object type 
                let! read = read, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot kontaktů byly dosazeny defaultní hodnoty, chyba při čtení dat ze souboru " xmlFile)

                stream.Close()
                stream.Dispose() 

                let result = read |> Casting.castAs<KontaktValuesDtoXml> //casting is necessary here, even ChatGPT has not figured out anything better
                let! result = result, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot kontaktů byly dosazeny defaultní hodnoty, chyba při čtení dat ze souboru (downcasting) " xmlFile)

                return Ok result
            }    
        
    //Tried and tested for "links" data
    //Newtonsoft.Json
    let internal deserializeFromJson<'a> (jsonFile : string) =

        pyramidOfDoom
            {
                let filepath = Path.GetFullPath(jsonFile) |> Option.ofNullEmpty //Strings handled with extra care due to potential type-related concerns (you can call it "paranoia" :-)). 
                let! filepath = filepath, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot odkazů byly dosazeny defaultní hodnoty, chyba při čtení cesty k souboru " jsonFile)

                let fInfodat: FileInfo = new FileInfo(filepath)
                let! _ =  fInfodat.Exists |> Option.ofBool, Error (sprintf "Pro zobrazování navrhovaných a předchozích hodnot odkazů byly dosazeny defaultní hodnoty, soubor %s nenalezen" jsonFile) 
                 
                let json = File.ReadAllText(filepath) |> Option.ofNull //Strings handled with extra care due to potential type-related concerns (you can call it "paranoia" :-)). 
                let! json = json, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot odkazů byly dosazeny defaultní hodnoty, chyba při deserializaci ze souboru " jsonFile) 

                let result = JsonConvert.DeserializeObject<'a>(json) |> Option.ofNull //The type is established when calling deserializeFromJson<'a>, casting is not necessary here //|> Casting.castAs<LinkAndLinkNameValuesDtoGet>  
                let! result = result, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot odkazů byly dosazeny defaultní hodnoty, chyba při čtení dat ze souboru (downcasting) " jsonFile)

                return Ok result
            }

    //Tried and tested for "links" data
    //Thoth.Json.Net, Thoth.Json
    let internal deserializeFromJsonThoth<'a> (jsonFile : string) =

        pyramidOfDoom
            {
                let filepath = Path.GetFullPath(jsonFile) |> Option.ofNullEmpty //Strings handled with extra care due to potential type-related concerns (you can call it "paranoia" :-)). 
                let! filepath = filepath, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot odkazů byly dosazeny defaultní hodnoty, chyba při čtení cesty k souboru " jsonFile)

                let fInfodat: FileInfo = new FileInfo(filepath)
                let! _ =  fInfodat.Exists |> Option.ofBool, Error (sprintf "Pro zobrazování navrhovaných a předchozích hodnot odkazů byly dosazeny defaultní hodnoty, soubor %s nenalezen" jsonFile) 
                 
                let json = File.ReadAllText(filepath) |> Option.ofNull //Strings handled with extra care due to potential type-related concerns (you can call it "paranoia" :-)). 
                let! json = json, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot odkazů byly dosazeny defaultní hodnoty, chyba při deserializaci ze souboru " jsonFile) 

                let result = Decode.fromString decoder json  //Thoth does not not use reflection  

                return result //Thoth output is of Result type 
            }



