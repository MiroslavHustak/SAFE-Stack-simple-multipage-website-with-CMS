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

open System
open System.IO
open System.Xml.Linq
open Newtonsoft.Json
open System.Xml.Serialization
open System.Runtime.Serialization

open FsToolkit.ErrorHandling

open Helpers.Server
open Helpers.Server.CEBuilders

open DtoXml.Server.DtoXml
open DtoSend.Server.DtoSend

open Serialization.Coders.Server.ThothCoders


// Implement 'try with' block for serialization at each location in the code where it is used.
module Serialisation =
   
    //System.Runtime.Serialization (requires equal types for serialization and deserialization; hence separated DTOs)
    let internal serializeToXml (record: 'a) (xmlFile: string) =
        
        pyramidOfDoom 
            {
                let filepath = Path.GetFullPath(xmlFile) |> Option.ofNullEmpty   
                let! filepath = filepath, Error (sprintf "%s%s" "Zadané hodnoty nebyly uloženy, chyba při čtení cesty k souboru " xmlFile)

                let xmlSerializer = new DataContractSerializer(typeof<'a>) //cannot be null, exn caught with tryWith elsewhere

                use stream = File.Create(filepath) //exn caught with tryWith
                let! _ = stream |> Option.ofNull, Error (sprintf "%s%s" "Zadané hodnoty nebyly uloženy, chyba při serializaci do souboru " xmlFile)

                xmlSerializer.WriteObject(stream, record) //non-nullable, exn caught with tryWith 
                              
                return Ok ()
            }

    //System.Xml.Serialization          
    let internal serializeToXml2 (record: 'a) (xmlFile: string) =

        pyramidOfDoom 
            {
                let filepath = Path.GetFullPath(xmlFile) |> Option.ofNullEmpty  
                let! filepath = filepath, Error (sprintf "%s%s" "Zadané hodnoty nebyly uloženy, chyba při čtení cesty k souboru " xmlFile)

                let xmlSerializer = new XmlSerializer(typeof<'a>) //cannot be null, exn caught with tryWith elsewhere              
                                
                use stream = new FileStream(filepath, FileMode.Create)
                let! _ = stream |> Option.ofNull, Error (sprintf "%s%s" "Zadané hodnoty nebyly uloženy, chyba při serializaci do souboru " xmlFile)
                
                xmlSerializer.Serialize(stream, record)
              
                return Ok ()
            }

    //LINQ to XML System.Xml.Linq
    let internal parseToXml3 (record: KontaktValuesDtoXml3) (xmlFile: string) = //no reflection

        let msgsElements = //non-nullable if the strings are not nulls (ensured)
            [
                new XElement(XName.Get("Msg1"), record.Msgs.Msg1)
                new XElement(XName.Get("Msg2"), record.Msgs.Msg2)
                new XElement(XName.Get("Msg3"), record.Msgs.Msg3)
                new XElement(XName.Get("Msg4"), record.Msgs.Msg4)
                new XElement(XName.Get("Msg5"), record.Msgs.Msg5)
                new XElement(XName.Get("Msg6"), record.Msgs.Msg6)
            ]
    
        let msgsElement = new XElement(XName.Get("Msgs"), msgsElements)
    
        let rootElement =
            new XElement(XName.Get("KontaktValuesDtoXml3"),
                [
                    new XElement(XName.Get("V001"), record.V001)
                    new XElement(XName.Get("V002"), record.V002)
                    new XElement(XName.Get("V003"), record.V003)
                    new XElement(XName.Get("V004"), record.V004)
                    new XElement(XName.Get("V005"), record.V005)
                    new XElement(XName.Get("V006"), record.V006)
                    new XElement(XName.Get("V007"), record.V007)
                    msgsElement
                ])

        (*
        // See MS Docs: this example does not work in F# due to an ambiguity in the XDeclaration constructor parameters. 
        // By default, XDocument includes the XML declaration when saving an XML document.
        let xmlDoc = // non-nullable
            new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"), // This line causes ambiguity in F#, preventing compilation.
                new XElement(XName.Get("KontaktValuesDtoXml3"),
                    [|
                        new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance")
                        new XAttribute(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema")
                    |],
                    rootElement.Elements()
                )
            )
        *)         
        
        let xmlDoc = new XDocument() //non-nullable

        let declaration = new XDeclaration("1.0", "utf-16", "yes") //non-nullable

        xmlDoc.Declaration <- declaration

        xmlDoc.Add(
            new XElement(XName.Get("KontaktValuesDtoXml3"),
                [|
                    new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance")
                    new XAttribute(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema")
                |] 
            )
        )
      
        xmlDoc.Root.Add(rootElement.Elements())     
        xmlDoc.Save(Path.GetFullPath(xmlFile)) //tryWith in Server.Api

        Ok ()  
        
    //**************************************************************************************

    //Newtonsoft.Json 
    let internal serializeToJson (record: 'a) (jsonFile: string) =

        pyramidOfDoom 
            {
                let filepath = Path.GetFullPath(jsonFile) |> Option.ofNullEmpty 
                let! filepath = filepath, Error (sprintf "%s%s" "Zadané hodnoty nebyly uloženy, chyba při čtení cesty k souboru " jsonFile)

                let json = JsonConvert.SerializeObject(record) |> Option.ofNullEmpty 
                let! json = json, Error (sprintf "%s%s" "Zadané hodnoty nebyly uloženy, chyba při serializaci do " jsonFile)

                File.WriteAllText(filepath, json) //non-nullable, ex caught with tryWith elsewhere

                return Ok ()
           }

    //Thoth.Json.Net, Thoth.Json + Newtonsoft.Json
    let internal serializeToJsonThoth (record: LinkAndLinkNameValuesDtoSend) (jsonFile: string) =
       
        pyramidOfDoom 
            {
                let filepath = Path.GetFullPath(jsonFile) |> Option.ofNullEmpty  
                let! filepath = filepath, Error (sprintf "%s%s" "Zadané hodnoty nebyly uloženy, chyba při čtení cesty k souboru " jsonFile)

                let json = JsonConvert.SerializeObject(encoder record) |> Option.ofNullEmpty 
                let! json = json, Error (sprintf "%s%s" "Zadané hodnoty nebyly uloženy, chyba při serializaci do " jsonFile)

                File.WriteAllText(filepath, json) //non-nullable, ex caught with tryWith 

                return Ok ()
           }

    //Thoth.Json.Net, Thoth.Json + StreamWriter (System.IO (File.WriteAllText) did not work)    
    let internal serializeToJsonThoth2 (record: LinkAndLinkNameValuesDtoSend) (jsonFile: string) =

        pyramidOfDoom
            {
                let filepath = Path.GetFullPath(jsonFile) |> Option.ofNullEmpty 
                let! filepath = filepath, Error (sprintf "%s%s" "Zadané hodnoty nebyly uloženy, chyba při čtení cesty k souboru " jsonFile)
    
                let json = Encode.toString 2 (encoder record) |> Option.ofNullEmpty // Serialize the record to JSON with indentation
                let! json = json, Error (sprintf "%s%s" "Zadané hodnoty nebyly uloženy, chyba při serializaci do " jsonFile)
    
                use writer = new StreamWriter(filepath, false)                
                let! _ = writer |> Option.ofNull, Error (sprintf "%s%s" "Zadané hodnoty nebyly uloženy, chyba při serializaci do " jsonFile)

                writer.Write(json)

                return Ok ()
            }
    
//Implement 'try with' block for deserialization at each location in the code where it is used.
module Deserialisation =

    //System.Runtime.Serialization
    let internal deserializeFromXml<'a> (xmlFile : string) =

        pyramidOfDoom
            {
                let filepath = Path.GetFullPath(xmlFile) |> Option.ofNullEmpty 
                let! filepath = filepath, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot kontaktů byly dosazeny defaultní hodnoty, chyba při čtení cesty k souboru " xmlFile)

                let fInfodat: FileInfo = new FileInfo(filepath)
                let! _ =  fInfodat.Exists |> Option.ofBool, Error (sprintf "Soubor %s nenalezen" xmlFile)                      

                let xmlSerializer = new DataContractSerializer(typeof<'a>) //cannot be null, exn caught with tryWith elsewhere            
                                
                use stream = File.OpenRead(filepath) //exn caught with tryWith
                let! _ = stream |> Option.ofNull, Error (sprintf "%s%s" "Zadané hodnoty nebyly uloženy, chyba při deserializaci ze souboru " xmlFile)

                let read = xmlSerializer.ReadObject(stream) |> Option.ofNull 
                let! read = read, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot kontaktů byly dosazeny defaultní hodnoty, při chyba při deserializaci ze souboru " xmlFile)
                                
                let result = read |> Casting.castAs<KontaktValuesDtoXml> //casting is necessary here, even ChatGPT has not figured out anything better
                let! result = result, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot kontaktů byly dosazeny defaultní hodnoty, chyba při čtení dat ze souboru (downcasting) " xmlFile)

                return Ok result
            }

    //System.Xml.Serialization          
    let internal deserializeFromXml2<'a> (xmlFile : string) =

        pyramidOfDoom
            {
                let filepath = Path.GetFullPath(xmlFile) |> Option.ofNullEmpty  
                let! filepath = filepath, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot kontaktů byly dosazeny defaultní hodnoty, chyba při čtení cesty k souboru " xmlFile)

                let fInfodat: FileInfo = new FileInfo(filepath)
                let! _ =  fInfodat.Exists |> Option.ofBool, Error (sprintf "Soubor %s nenalezen" xmlFile)                      

                let xmlSerializer = new XmlSerializer(typeof<'a>) //cannot be null, exn caught with tryWith elsewhere   
                                
                use stream = new FileStream(filepath, FileMode.Open) //exn caught with tryWith
                let! _ = stream |> Option.ofNull, Error (sprintf "%s%s" "Zadané hodnoty nebyly uloženy, chyba při deserializaci ze souboru " xmlFile)

                let read = xmlSerializer.Deserialize(stream) |> Option.ofNull //my paranoia about the object type 
                let! read = read, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot kontaktů byly dosazeny defaultní hodnoty, při chyba při deserializaci ze souboru " xmlFile)
                                
                let result = read |> Casting.castAs<KontaktValuesDtoXml2> //casting is necessary here, even ChatGPT has not figured out anything better               
                let! result = result, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot kontaktů byly dosazeny defaultní hodnoty, chyba při čtení dat ze souboru (downcasting) " xmlFile)

                return Ok result
            }

    //LINQ to XML System.Xml.Linq
    let internal parseFromXml3 xmlFile : Result<KontaktValuesDtoXml3, string> = //no reflection

        pyramidOfDoom
            {
                let filepath = Path.GetFullPath(xmlFile) |> Option.ofNullEmpty  
                let! filepath = filepath, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot kontaktů byly dosazeny defaultní hodnoty, chyba při čtení cesty k souboru " xmlFile)

                let fInfodat: FileInfo = new FileInfo(filepath)
                let! _ =  fInfodat.Exists |> Option.ofBool, Error (sprintf "Soubor %s nenalezen" xmlFile)                      

                let xmlString = File.ReadAllText(Path.GetFullPath(filepath)) |> Option.ofNullEmpty
                let! xmlString = xmlString, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot odkazů byly dosazeny defaultní hodnoty, chyba při deserializaci ze souboru " xmlFile)
                                
                let doc = XDocument.Parse(xmlString) //non-nullable if the parameter is not null (ensured above) -> another Option.ofObj is not necessary

                let parseMsgs (msgsElement: XElement) : MessagesDtoXml3 =
                    {
                        Msg1 = msgsElement.Element(XName.Get("Msg1")).Value
                        Msg2 = msgsElement.Element(XName.Get("Msg2")).Value
                        Msg3 = msgsElement.Element(XName.Get("Msg3")).Value
                        Msg4 = msgsElement.Element(XName.Get("Msg4")).Value
                        Msg5 = msgsElement.Element(XName.Get("Msg5")).Value
                        Msg6 = msgsElement.Element(XName.Get("Msg6")).Value
                    }
    
                // Extracting data from XML
                let root = doc.Root //non-nullable if doc is not null (ensured above) -> another Option.ofObj is not necessary                    

                return Ok
                    {
                        V001 = root.Element(XName.Get("V001")).Value
                        V002 = root.Element(XName.Get("V002")).Value
                        V003 = root.Element(XName.Get("V003")).Value
                        V004 = root.Element(XName.Get("V004")).Value
                        V005 = root.Element(XName.Get("V005")).Value
                        V006 = root.Element(XName.Get("V006")).Value
                        V007 = root.Element(XName.Get("V007")).Value
                        Msgs = parseMsgs (root.Element(XName.Get("Msgs")))
                    }
            }    
                   
    //Newtonsoft.Json 
    let internal deserializeFromJson<'a> (jsonFile : string) =

        pyramidOfDoom
            {
                let filepath = Path.GetFullPath(jsonFile) |> Option.ofNullEmpty  
                let! filepath = filepath, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot odkazů byly dosazeny defaultní hodnoty, chyba při čtení cesty k souboru " jsonFile)

                let fInfodat: FileInfo = new FileInfo(filepath)
                let! _ =  fInfodat.Exists |> Option.ofBool, Error (sprintf "Pro zobrazování navrhovaných a předchozích hodnot odkazů byly dosazeny defaultní hodnoty, soubor %s nenalezen" jsonFile) 
                 
                let json = File.ReadAllText(filepath) |> Option.ofNullEmpty 
                let! json = json, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot odkazů byly dosazeny defaultní hodnoty, chyba při deserializaci ze souboru " jsonFile) 

                let result = JsonConvert.DeserializeObject<'a>(json) |> Option.ofNull //The type is established when calling deserializeFromJson<'a>, casting is not necessary here //|> Casting.castAs<LinkAndLinkNameValuesDtoGet>  
                let! result = result, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot odkazů byly dosazeny defaultní hodnoty, chyba při čtení dat ze souboru (downcasting) " jsonFile)

                return Ok result
            }

    //Thoth.Json.Net, Thoth.Json + System.IO (File.ReadAllText)
    let internal deserializeFromJsonThoth<'a> (jsonFile : string) =

        pyramidOfDoom
            {
                let filepath = Path.GetFullPath(jsonFile) |> Option.ofNullEmpty 
                let! filepath = filepath, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot odkazů byly dosazeny defaultní hodnoty, chyba při čtení cesty k souboru " jsonFile)

                let fInfodat: FileInfo = new FileInfo(filepath)
                let! _ =  fInfodat.Exists |> Option.ofBool, Error (sprintf "Pro zobrazování navrhovaných a předchozích hodnot odkazů byly dosazeny defaultní hodnoty, soubor %s nenalezen" jsonFile) 
                 
                let json = File.ReadAllText(filepath) |> Option.ofNullEmpty
                let! json = json, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot odkazů byly dosazeny defaultní hodnoty, chyba při deserializaci ze souboru " jsonFile) 

                let result = Decode.fromString decoder json  //Thoth does not use reflection  

                return result //Thoth output is of Result type 
            }

    //Thoth.Json.Net, Thoth.Json + StreamReader
    let internal deserializeFromJsonThoth2<'a> (jsonFile : string) =

        pyramidOfDoom
            {
                let filepath = Path.GetFullPath(jsonFile) |> Option.ofNullEmpty 
                let! filepath = filepath, Error (sprintf "%s%s" "Pro zobrazování navrhovaných a předchozích hodnot odkazů byly dosazeny defaultní hodnoty, chyba při čtení cesty k souboru " jsonFile)

                let fInfodat: FileInfo = new FileInfo(filepath)
                let! _ =  fInfodat.Exists |> Option.ofBool, Error (sprintf "Pro zobrazování navrhovaných a předchozích hodnot odkazů byly dosazeny defaultní hodnoty, soubor %s nenalezen" jsonFile) 
                 
                use fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.None) 
                let! _ = fs |> Option.ofNull, Error (sprintf "%s%s" "Chyba při čtení dat ze souboru " filepath)                        
                    
                use reader = new StreamReader(fs) //For large files, StreamReader may offer better performance and memory efficiency
                let! _ = reader |> Option.ofNull, Error (sprintf "%s%s" "Chyba při čtení dat ze souboru " filepath) 
                
                let json = reader.ReadToEnd()
                let! json = json |> Option.ofNullEmpty, Error (sprintf "%s%s" "Chyba při čtení dat ze souboru " filepath)  
                    
                let result = Decode.fromString decoder json  //Thoth does not use reflection  
                                  
                return result //Thoth output is of Result type 
            }
