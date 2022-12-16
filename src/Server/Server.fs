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

open Sql   //uncomment to test plain SQL (and, at the same time, comment out "open Dapper")
//open Dapper  //uncomment to test Dapper.FSharp (and, at the same time, comment out "open QSql")

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
    
  //TODO validation upon request from the user 
let private verifyCenikValues (cenikValues: GetCenikValues) =
    match SharedCenikValues.isValid () with
    | () -> Ok ()        
    // | _  -> Error "" 

 //TODO validation upon request from the user 
let private verifyKontaktValues (kontaktValues: GetKontaktValues) =
   match SharedCenikValues.isValid () with
   | () -> Ok ()        
   // | _  -> Error ""

//TODO validation upon request from the user 
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
                      | true  -> //StreamReader taky nechtel fungovat, takze cteme data jinak                              
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
      getCenikValues =  //moznost vyberu mezi Json/XML ci DB
          fun getCenikValues ->
              async
                  {                   
                    let getNewCenikValues: GetCenikValues =                        
                        match verifyCenikValues getCenikValues with                
                        | Ok () ->
                                   let dbNewCenikValues = { getCenikValues with Id = 2; ValueState = "new" }

                                   //************* plain SQL or Dapper.FSharp ******************** 
                                   insertOrUpdate dbNewCenikValues //TODO try with

                                   //************* Json/XML ******************** 
                                   serialize dbNewCenikValues "jsonCenikValues.xml"  //TODO try with   //leave it here despite using db in order to update xml
                                   
                                   dbNewCenikValues
                        | _    ->  GetCenikValues.Default
                    return getNewCenikValues
                  }

      sendOldCenikValues = //choose between db and Json/XML
          fun _ -> 
              async
                  {
                     //************* Json/XML ********************
                     //leave it here despite using db in order to update xml   
                     copyFiles 
                     <| "jsonCenikValues.xml"
                     <| "jsonCenikValuesBackUp.xml"

                     //let sendOldCenikValues = deserialize "jsonCenikValuesBackUp.xml" //TODO try with
                     //return sendOldCenikValues

                     //************* plain SQL or Dapper.FSharp ********************                     
                     let IdNew = 2
                     let IdOld = 3
                     let newGetCenikValuesDb = selectValues IdNew //TODO try with
                     insertOrUpdate { newGetCenikValuesDb with Id = IdOld; ValueState = "old" }//eqv of the aforementioned copying  //TODO try with                   
                     let dbSendOldCenikValues = selectValues IdOld  //TODO try with                
                     
                     return dbSendOldCenikValues                     
                  } 

      sendDeserialisedCenikValues = //choose between db and Json/XML
         fun _ ->
             async
                 {
                    //************* Json/XML ********************
                    //let sendCenikValues = deserialize "jsonCenikValues.xml" //TODO try with
                    //return sendCenikValues

                    //************* plain SQL or Dapper.FSharp ********************
                    let IdNew = 2
                    let dbSendCenikValues = selectValues IdNew //TODO try with

                    return dbSendCenikValues
                 }

      getKontaktValues =
          fun getKontaktValues ->
              async
                  {
                    let getNewKontaktValues: GetKontaktValues = 
                        match verifyKontaktValues getKontaktValues with                
                        | Ok () -> serialize getKontaktValues "jsonKontaktValues.xml"  //TODO try with
                        | _     -> ()                                   
                        getKontaktValues
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
                        | _     -> ()
                        getLinkAndLinkNameValues
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
    insertOrUpdate GetCenikValues.Default
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