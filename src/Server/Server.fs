module Server

open System
open System.IO
open System.Data.SqlClient

open Saturn
open Dapper.FSharp
open Fable.Remoting.Server
open Fable.Remoting.Giraffe

open Shared
open SharedTypes

//open Sql   //plain SQL    //uncomment to test plain SQL
open Dapper  //Dapper.FSharp  //uncomment to test Dapper.FSharp 

open Security

open Connection
open ROP_Functions
open Helpers.CopyingFiles
open Helpers.Serialisation
open Helpers.Deserialisation

let private (>>=) condition nextFunc = 
    match condition with
    | false -> SharedApi.UsernameOrPasswordIncorrect  
    | true  -> nextFunc() 

[<Struct>]
type private MyPatternBuilder = MyPatternBuilder with            
    member _.Bind(condition, nextFunc) = (>>=) <| condition <| nextFunc
    member _.Using x = x
    member _.Return x = x

let private verifyLogin (login: LoginInfo) =   // LoginInfo -> Async<LoginResult>>   

    MyPatternBuilder    
        {  
            let! _ = SharedLoginValues.isValid login.Username login.Password
            let securityTokenFile = Path.GetFullPath("securityToken.txt")
                                    |> Option.ofObj
                                    |> function
                                        | Some value -> value
                                        | None       -> //TODO 
                                                        String.Empty 
            let! _ = login.Username = "q" && login.Password = "q" 
            let result =                
                let accessToken = string <| System.Guid.NewGuid() //encodeJwt securityToken //TODO
                //********************************************************************************
                let mySeq = seq { login.Username; accessToken }
                use sw = new StreamWriter(Path.GetFullPath(securityTokenFile)) //TODO vse do trywith
                         |> Option.ofObj
                         |> function
                             | Some value -> value
                             | None       -> //TODO
                                             new StreamWriter(Path.GetFullPath(securityTokenFile)) 
                mySeq |> Seq.iter (fun item -> do sw.WriteLine(item)) //TODO vse do trywith
                //vyse uvedeny kod pro ukladani na server (puvodne workaround) ponechan pro pripadne pristi pouziti
                //**********************************************************************************
                SharedApi.LoggedIn { Username = login.Username; AccessToken = SharedApi.AccessToken accessToken }
            return result
        }
    
 //TODO pripadne pouziti validace dle potreby klienta //TODO konzultovat s klientem
let private verifyCenikValues (cenikValues: GetCenikValues) =
    match SharedCenikValues.isValid () with
    | () -> Ok ()        
    // | _  -> Error "" 

//TODO pripadne pouziti validTokenreby klienta /Tokenltovat s klientem
let private verifyKontaktValues (kontaktValues: GetKontaktValues) =
   match SharedCenikValues.isValid () with
   | () -> Ok ()        
   // | _  -> Error ""

//TODO pripadne pouziti validace dle potreby klienta //TODO konzultovat s klientem
let private verifyLinkAndLinkNameValues (linkValues: GetLinkAndLinkNameValues) =
   match SharedLinkAndLinkNameValues.isValid () with
   | () -> Ok ()        
   // | _  -> Error ""


let IGetApi =
    {
      login =
          fun login -> async { return (verifyLogin login) }          
     (*
      getSecurityTokenFile = //TODO try with
          fun getSecurityTokenFile -> async { return File.Exists(Path.GetFullPath("securityToken.txt")) }             
     
      getSecurityToken =
          fun getSecurityToken ->  //TODO try with
              async
                  {       
                      match File.Exists(Path.GetFullPath("securityToken.txt")) with
                      | false -> return Seq.empty  //TODO error+reseni
                      | true  -> //StreamReader taky nejak nechtel fungovat                              
                                 match File.ReadAllLines("securityToken.txt") |> Option.ofObj with
                                 | Some value -> return (value |> Seq.ofArray) 
                                 | None       -> return Seq.empty  //TODO error+reseni                           
                  }        
      
      deleteSecurityTokenFile =
          fun deleteSecurityTokenFile ->  //TODO try with
              async
                  {
                      File.Delete(Path.GetFullPath("securityToken.txt"))
                      return ()
                  }   
      *)
      getCenikValues =  //moznost vyberu mezi Json/XML ci db
          fun getCenikValues ->
              async
                  {                   
                    let getNewCenikValues: GetCenikValues =                        
                        match verifyCenikValues getCenikValues with                
                        | Ok () ->
                                   //************* plain SQL or Dapper.FSharp ******************** 
                                   //insertOrUpdateNew { getCenikValues with Id = 2; ValueState = "new" }
                                   insertOrUpdateUniversal { getCenikValues with Id = 2; ValueState = "new" }  

                                   //************* Json/XML ******************** 
                                   serialize getCenikValues "jsonCenikValues.xml"  //TODO try with   //aji pri db ponechat serializaci kvuli aktualizaci xml                           
                                   {
                                       Id = 2; ValueState = "new";
                                       V001 = getCenikValues.V001; V002 = getCenikValues.V002; V003 = getCenikValues.V003;
                                       V004 = getCenikValues.V004; V005 = getCenikValues.V005; V006 = getCenikValues.V006;
                                       V007 = getCenikValues.V007; V008 = getCenikValues.V008; V009 = getCenikValues.V009                                       
                                   }                                  
                        | _    ->  GetCenikValues.Default
                    return getNewCenikValues
                  }

      sendOldCenikValues = //moznost vyberu mezi Json/XML ci db
          fun _ ->
              async
                  {
                     //************* Json/XML ********************
                     //aji pri testovani db ponechat kopirovani kvuli aktualizaci xml 
                     copyFiles 
                     <| "jsonCenikValues.xml"
                     <| "jsonCenikValuesBackUp.xml"

                      //let sendOldCenikValues = deserialize "jsonCenikValuesBackUp.xml" //TODO try with
                      //return sendOldCenikValues

                     //************* plain SQL or Dapper.FSharp ********************
                     let IdNew = 2
                     let IdOld = 3
                     //let newGetCenikValuesDb = selectNewValues IdNew
                     //insertOrUpdateOld { newGetCenikValuesDb with Id = IdOld; ValueState = "old" }//eqv vyse uvedeneho kopirovani                    
                     //let dbSendOldCenikValues = selectOldValues IdOld         
                     let newGetCenikValuesDb = selectValuesUniversal IdNew
                     insertOrUpdateUniversal { newGetCenikValuesDb with Id = IdOld; ValueState = "old" }//eqv vyse uvedeneho kopirovani                    
                     let dbSendOldCenikValues = selectValuesUniversal IdOld                  
                     
                     return dbSendOldCenikValues
                  } 

      sendDeserialisedCenikValues = //moznost vyberu mezi Json/XML ci db
         fun _ ->
             async
                 {
                    //************* Json/XML ********************
                    //let sendCenikValues = deserialize "jsonCenikValues.xml" //TODO try with
                     //return sendCenikValues

                    //************* plain SQL or Dapper.FSharp ********************
                    let IdNew = 2
                    let dbSendCenikValues = selectValuesUniversal IdNew
                    // let dbSendCenikValues = selectDeserValues IdNew

                    return dbSendCenikValues
                 }

      getKontaktValues =
          fun getKontaktValues ->
              async
                  {
                    let getNewKontaktValues: GetKontaktValues = 
                        match verifyKontaktValues getKontaktValues with                
                        | Ok () -> serialize getKontaktValues "jsonKontaktValues.xml"  //TODO try with
                                   {
                                       V001 = getKontaktValues.V001; V002 = getKontaktValues.V002;
                                       V003 = getKontaktValues.V003; V004 = getKontaktValues.V004;
                                       V005 = getKontaktValues.V005; V006 = getKontaktValues.V006;
                                       V007 = getKontaktValues.V007 
                                   }
                        | _     ->
                                   {
                                       V001 = String.Empty; V002 = String.Empty;
                                       V003 = String.Empty; V004 = String.Empty;
                                       V005 = String.Empty; V006 = String.Empty;
                                       V007 = String.Empty
                                   }

                    return getNewKontaktValues
                  }

      sendOldKontaktValues =
         fun _ ->
             async
                 {
                    copyFiles 
                    <| "jsonKontaktValues.xml"
                    <| "jsonKontaktValuesBackUp.xml"

                    let sendOldKontaktValues = deserialize "jsonKontaktValuesBackUp.xml" //TODO try with
                    return sendOldKontaktValues
                 } 

      sendDeserialisedKontaktValues =
          fun _ ->
             async
                {
                    let sendKontaktValues = deserialize "jsonKontaktValues.xml" //TODO try with
                    return sendKontaktValues
                }

      getLinkAndLinkNameValues =
          fun getLinkAndLinkNameValues ->
              async
                  {
                    let getNewLinkAndLinkNameValues: GetLinkAndLinkNameValues = 
                        match verifyLinkAndLinkNameValues getLinkAndLinkNameValues with                
                        | Ok () -> serialize getLinkAndLinkNameValues "jsonLinkAndLinkNameValues.xml"  //TODO try with
                                   {
                                       V001 = getLinkAndLinkNameValues.V001; V002 = getLinkAndLinkNameValues.V002;
                                       V003 = getLinkAndLinkNameValues.V003; V004 = getLinkAndLinkNameValues.V004;
                                       V005 = getLinkAndLinkNameValues.V005; V006 = getLinkAndLinkNameValues.V006
                                       V001n = getLinkAndLinkNameValues.V001n; V002n = getLinkAndLinkNameValues.V002n;
                                       V003n = getLinkAndLinkNameValues.V003n; V004n = getLinkAndLinkNameValues.V004n;
                                       V005n = getLinkAndLinkNameValues.V005n; V006n = "Facebook"
                                   }
                        | _     ->
                                   {
                                       V001 = String.Empty; V002 = String.Empty;
                                       V003 = String.Empty; V004 = String.Empty;
                                       V005 = String.Empty; V006 = String.Empty
                                       V001n = String.Empty; V002n = String.Empty;
                                       V003n = String.Empty; V004n = String.Empty;
                                       V005n = String.Empty; V006n = "Facebook"
                                   }

                    return getNewLinkAndLinkNameValues
                  }
           
      sendOldLinkAndLinkNameValues =
          fun _ ->
              async
                  {
                    copyFiles 
                    <| "jsonLinkAndLinkNameValues.xml"
                    <| "jsonLinkAndLinkNameValuesBackUp.xml"

                    let sendOldLinkAndLinkNameValues = deserialize "jsonLinkAndLinkNameValuesBackUp.xml" //TODO try with
                    return sendOldLinkAndLinkNameValues
                  } 

      sendDeserialisedLinkAndLinkNameValues =
          fun _ ->
              async
                  {
                    let sendLinkAndLinkNameValues = deserialize "jsonLinkandLinkNameValues.xml" //TODO try with
                    return sendLinkAndLinkNameValues
                  }
    }

let webApp =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue IGetApi
    |> Remoting.buildHttpHandler

let app =
    insertOrUpdateUniversal GetCenikValues.Default
    application
        {
            use_router webApp
            memory_cache
            use_static "public"
            use_gzip
        }

[<EntryPoint>]
let main _ =   
    Dapper.FSharp.OptionTypes.register()
    run app    
    0