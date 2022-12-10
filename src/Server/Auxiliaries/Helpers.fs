module Helpers

open System
open System.IO;
open Newtonsoft.Json
open System.Runtime.Serialization
open ROP_Functions

    module private TryParserInt =

         let tryParseWith (tryParseFunc: string -> bool * _) = tryParseFunc >> function
             | true, value -> Some value
             | false, _    -> None
         let parseInt    = tryParseWith <| System.Int32.TryParse  
         let (|Int|_|)   = parseInt        

    module Parsing =

         let f x = let isANumber = x                                          
                   isANumber   
                   
         let rec parseMe = 
             function            
             | TryParserInt.Int i -> f i
             | _                  -> 0  

         let rec parseMeOption = 
             function            
             | TryParserInt.Int i -> f Some i
             | _                  -> None     

    //TODO All serialisation => try with (+ Option.ofObj if necessary) 
    module Serialisation = 

         let serialize record xmlFile = 
            
             let filepath = Path.GetFullPath(xmlFile) 
                            |> Option.ofObj 
                            |> optionToGenerics2 "Chyba při čtení cesty k souboru json.xml" String.Empty

             let xmlSerializer = new DataContractSerializer(typedefof<string>)          
                                 |> Option.ofObj 
                                 |> optionToGenerics2 "Chyba při serializaci" (new DataContractSerializer(typedefof<string>))
             use stream = File.Create(filepath)   
             xmlSerializer.WriteObject(stream, JsonConvert.SerializeObject(record))            

    //TODO All deserialisation => try with (+ Option.ofObj if necessary) 
    module Deserialisation =       
              
       let deserialize xmlFile = 
           
           let filepath = Path.GetFullPath(xmlFile) 
                          |> Option.ofObj 
                          |> optionToGenerics2 (sprintf "%s%s" "Chyba při čtení cesty k souboru " xmlFile) String.Empty //TODO
          
           let jsonString() = 

               let xmlSerializer = new DataContractSerializer(typedefof<string>) 
                                   |> Option.ofObj 
                                   |> optionToGenerics2 "Chyba při serializaci" (new DataContractSerializer(typedefof<string>))
               let fileStream = File.ReadAllBytes(filepath)
                                |> Option.ofObj 
                                |> optionToGenerics2 (sprintf "%s%s" "Chyba při čtení dat ze souboru " xmlFile) [||] //TODO
               use memoryStream = new MemoryStream(fileStream) 
               let resultObj = xmlSerializer.ReadObject(memoryStream)  
                               |> Option.ofObj 
                               |> optionToGenerics2 (sprintf "%s%s" "Chyba při čtení dat ze souboru " xmlFile) ()  //TODO  
               let resultString = unbox resultObj  
                                  |> Option.ofObj 
                                  |> optionToGenerics2 (sprintf "%s%s" "Chyba při čtení dat ze souboru (unboxing) " xmlFile) String.Empty //TODO     
               let jsonString = JsonConvert.DeserializeObject<'a>(resultString) 
               jsonString
           
           let fInfodat: FileInfo = new FileInfo(filepath)  
           match fInfodat.Exists with 
           | true  -> jsonString()              
           | false -> failwith (sprintf "Soubor %s nenalezen" xmlFile) 

    module CopyingFiles =     
        
       let copyFiles source destination =
                                                                    
          let perform x =                                    
              let sourceFilepath = Path.GetFullPath(source) 
                                   |> Option.ofObj 
                                   |> optionToGenerics2 (sprintf "%s%s" "Chyba při čtení cesty k souboru " source) String.Empty //TODO
              let destinFilepath = Path.GetFullPath(destination) 
                                   |> Option.ofObj 
                                   |> optionToGenerics2 (sprintf "%s%s" "Chyba při čtení cesty k souboru " source) String.Empty //TODO
                    
              let fInfodat: FileInfo = new FileInfo(sourceFilepath)  
              match fInfodat.Exists with 
              | true  -> File.Copy(sourceFilepath, destinFilepath, true)             
              | false -> failwith (sprintf "Soubor %s nenalezen" source)
          tryWith perform (fun x -> ()) (fun ex -> failwith) |> deconstructor4 () //TODO                
    (*       
    System.IO.File provides static members related to working with files, whereas System.IO.FileInfo represents a specific file and contains non-static members for working with that file.          
    Because all File methods are static, it might be more efficient to use a File method rather than a corresponding FileInfo instance method if you want to perform only one action. All File methods 
    require the path to the file that you are manipulating.    
    The static methods of the File class perform security checks on all methods. If you are going to reuse an object several times, consider using the corresponding 
    instance method of FileInfo instead, because the security check will not always be necessary.    
    *) 
        
    
        
       
          



