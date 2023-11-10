namespace Auxiliaries.Server

open System
open System.IO
open Newtonsoft.Json
open System.Runtime.Serialization

open FsToolkit.ErrorHandling

open CEBuilders

open DtoGet.Server.DtoGet
open DtoXml.Server.DtoXml

//tryWith to be implemented for all serialization at the place of its using 
module Serialisation =
   
    //vyzkouseno pro kontakt data
    //System.Runtime.Serialization vyzaduje stejny typ pro serializaci a deserializaci, proto separatni DTO
    //System.Xml.Serialization vyzaduje annotation TODO zrobit, az bude nekdy cas
    let internal serializeToXml (record: 'a) (xmlFile: string) =
        
        pyramidOfDoom
            {
                let filepath = Path.GetFullPath(xmlFile) |> Option.ofNull
                let! filepath = filepath, Error "Zadané hodnoty nebyly uloženy, chyba při čtení cesty k xml souboru"

                let xmlSerializer = new DataContractSerializer(typeof<'a>) |> Option.ofNull
                let! xmlSerializer = xmlSerializer, Error (sprintf "%s%s" "Zadané hodnoty nebyly uloženy, chyba při serializaci do " xmlFile)

                let stream = File.Create(filepath) |> Option.ofNull
                let! stream = stream, Error (sprintf "%s%s" "Zadané hodnoty nebyly uloženy, chyba při serializaci do " xmlFile)

                xmlSerializer.WriteObject(stream, record)                
                stream.Close()
                stream.Dispose()                    

                return Ok ()
            }    

    //vyzkouseno pro links   
    let internal serializeToJson (record: 'a) (jsonFile: string) =

        pyramidOfDoom 
            {
                let filepath = Path.GetFullPath(jsonFile) |> Option.ofNull
                let! filepath = filepath, Error (sprintf "%s%s" "Zadané hodnoty nebyly uloženy, chyba při čtení cesty k souboru " jsonFile)

                let json = JsonConvert.SerializeObject(record) |> Option.ofNull
                let! json = json, Error (sprintf "%s%s" "Zadané hodnoty nebyly uloženy, chyba při serializaci do " jsonFile)

                File.WriteAllText(filepath, json) 

                return Ok ()
           }

//tryWith to be implemented for all deserialization at the place of its using 
module Deserialisation =

    //vyzkouseno pro kontakt data
    //System.Runtime.Serialization
    //System.Xml.Serialization vyzaduje annotation TODO zrobit, az bude nekdy cas
    let internal deserializeFromXml<'a> (xmlFile : string) =

        pyramidOfDoom
            {
                let filepath = Path.GetFullPath(xmlFile) |> Option.ofNull
                let! filepath = filepath, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot kontaktů byly dosazeny defaultní hodnoty, chyba při čtení cesty k souboru " xmlFile)

                let fInfodat: FileInfo = new FileInfo(filepath)
                let! _ =  fInfodat.Exists |> Option.ofBool, Error (sprintf "Soubor %s nenalezen" xmlFile) 

                let xmlSerializer = new DataContractSerializer(typeof<'a>) |> Option.ofNull
                let! xmlSerializer = xmlSerializer, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot kontaktů byly dosazeny defaultní hodnoty, chyba při deserializaci ze souboru " xmlFile)  

                let stream = File.OpenRead(filepath) |> Option.ofNull
                let! stream = stream, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot kontaktů byly dosazeny defaultní hodnoty, chyba při deserializaci ze souboru " xmlFile)

                let read = xmlSerializer.ReadObject(stream) |> Option.ofNull
                let! read = read, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot kontaktů byly dosazeny defaultní hodnoty, chyba při čtení dat ze souboru " xmlFile)

                stream.Close()
                stream.Dispose() 

                let result = read |> Casting.castAs<KontaktValuesDtoXml>
                let! result = result, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot kontaktů byly dosazeny defaultní hodnoty, chyba při čtení dat ze souboru (downcasting) " xmlFile)

                return Ok result
            }    
        
    //vyzkouseno pro links 
    let internal deserializeFromJson<'a> (jsonFile : string) =

        pyramidOfDoom
            {
                let filepath = Path.GetFullPath(jsonFile) |> Option.ofNull
                let! filepath = filepath, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot odkazů byly dosazeny defaultní hodnoty, chyba při čtení cesty k souboru " jsonFile)

                let fInfodat: FileInfo = new FileInfo(filepath)
                let! _ =  fInfodat.Exists |> Option.ofBool, Error (sprintf "Pro zobrazování navrhovaných a předchozích hodnot odkazů byly dosazeny defaultní hodnoty, soubor %s nenalezen" jsonFile) 

                let json = File.ReadAllText(filepath) |> Option.ofNull
                let! json = json, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot odkazů byly dosazeny defaultní hodnoty, chyba při deserializaci ze souboru " jsonFile) 

                let result = JsonConvert.DeserializeObject<'a>(json) |> Casting.castAs<LinkAndLinkNameValuesDtoGet> 
                let! result = result, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot odkazů byly dosazeny defaultní hodnoty, chyba při čtení dat ze souboru (downcasting) " jsonFile)

                return Ok result
            }    






