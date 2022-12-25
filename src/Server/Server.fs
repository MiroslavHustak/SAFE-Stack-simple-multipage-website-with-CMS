namespace Server

open System
open System.IO
open System.Data.SqlClient

open Saturn
open Dapper.FSharp
open Fable.Remoting.Server
open Fable.Remoting.Giraffe

open Shared
open SharedTypes

open DbAccess.Sql       //uncomment to test plain SQL (and, at the same time, comment out "open DbAccess.Dapper")
//open DbAccess.Dapper  //uncomment to test Dapper.FSharp (and, at the same time, comment out "open DbAccess.Sql")

open Auxiliaries.Connection
open Auxiliaries.ROP_Functions
open Auxiliaries.Helpers.CopyingFiles
open Auxiliaries.Helpers.Serialisation
open Auxiliaries.Helpers.Deserialisation

module Server = 

    let private (>>=) condition nextFunc = 
        match condition with
        | false -> SharedApi.UsernameOrPasswordIncorrect  
        | true  -> nextFunc() 

    [<Struct>]
    type private MyPatternBuilder = MyPatternBuilder with            
        member _.Bind(condition, nextFunc) = (>>=) <| condition <| nextFunc
        member _.Using x = x
        member _.Return x = x

    let private strContainsOnlySpace str = str |> Seq.forall (fun item -> item = (char)32)  //A string is a sequence of characters => use Seq.forall to test directly //(char)32 = space
    let private isValidLogin inputUsrString inputPswString = not (strContainsOnlySpace inputUsrString || strContainsOnlySpace inputPswString)
    let private isValidCenik param = ()   //TODO validation upon request from the user 
    let private isValidKontakt param = () //TODO validation upon request from the user 
    let private isValidLink param = ()    //TODO validation upon request from the user 

    let private verifyLogin (login: LoginInfo) =   // LoginInfo -> Async<LoginResult>>

        (*
        MyPatternBuilder    
            {  
                let! _ = SharedLoginValues.isValid login.Username login.Password
                let! _ = login.Username = "q" && login.Password = "q"                
                return SharedApi.LoggedIn { Username = login.Username; AccessToken = SharedApi.AccessToken "Dummy" }
            } 
        *)

        //containing redundant code for learning purposes
        MyPatternBuilder    
            {  
                let! _ = isValidLogin login.Username login.Password
                let securityTokenFile = Path.GetFullPath("securityToken.txt")
                                        |> Option.ofObj
                                        |> function
                                            | Some value -> value
                                            | None       -> //TODO some action only in case you decide to use saved accessToken
                                                            String.Empty           
           
                let! _ = login.Username = "q" && login.Password = "q" 

                //TODO trywith only in case you decide to use the saved accessToken
                let result =                
                    let accessToken = string <| System.Guid.NewGuid() //encodeJwt securityToken //TODO only in case you decide to use saved accessToken
                    //********************************************************************************
                    let mySeq = seq { login.Username; accessToken }
                    use sw = new StreamWriter(Path.GetFullPath(securityTokenFile)) 
                             |> Option.ofObj
                             |> function
                                 | Some value -> value
                                 | None       -> //TODO some action only in case you decide to use accessToken
                                                 new StreamWriter(Path.GetFullPath(securityTokenFile)) 
                    mySeq |> Seq.iter (fun item -> do sw.WriteLine(item)) 
                    //code with saved accessToken (originally a workaround) left for potential exploitation in the future

                    //*********************************************************************************************
                    SharedApi.LoggedIn { Username = login.Username; AccessToken = SharedApi.AccessToken accessToken }
                return result
            }
    
      //TODO validation upon request from the user 
    let private verifyCenikValues (cenikValues: GetCenikValues) =
        match isValidCenik () with
        | () -> Ok ()        
        // | _  -> //Error "" 

     //TODO validation upon request from the user 
    let private verifyKontaktValues (kontaktValues: GetKontaktValues) =
       match isValidKontakt () with
       | () -> Ok ()        
       // | _  -> Error ""

    //TODO validation upon request from the user 
    let private verifyLinkAndLinkNameValues (linkValues: GetLinkAndLinkNameValues) =
       match isValidLink () with
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
                          | false -> //TODO error + some action
                                     return Seq.empty  
                          | true  -> //StreamReader refused to work here, thar is why File.ReadAllLines was used                              
                                     match File.ReadAllLines("securityToken.txt") |> Option.ofObj with
                                     | Some value -> return (value |> Seq.ofArray) 
                                     | None       -> return Seq.empty  //TODO error + some action                        
                      }        
      
          deleteSecurityTokenFile =
              fun deleteSecurityTokenFile ->  //TODO try with
                  async
                      {
                          File.Delete(Path.GetFullPath("securityToken.txt"))
                          return ()
                      }   
          *)
            getCenikValues =  //choose between Json/XML and DB
                fun getCenikValues ->
                    async
                        {                   
                            let getNewCenikValues: GetCenikValues =                        
                                match verifyCenikValues getCenikValues with                
                                | Ok () ->
                                          let dbNewCenikValues = { getCenikValues with Id = 2; ValueState = "new" }

                                          //************* plain SQL or Dapper.FSharp ********************                                                                         
                                          let exnSql = insertOrUpdate dbNewCenikValues

                                          //************* Json/XML ********************
                                          let serializeNow x =
                                              //failwith "Simulated exception2" 
                                              serialize dbNewCenikValues "jsonCenikValues.xml"  //leave it here despite using db in order to update xml                                
                                          let exnJson = (serializeNow, (fun x -> ()), "Error2") |||> tryWith |> deconstructor1

                                          //************* For both DB and Json/XML ********************
                                          { dbNewCenikValues with Msgs = { Messages.Default with Msg1 = exnSql; Msg2 = exnJson } }
                                   
                                | _    -> GetCenikValues.Default
                          return getNewCenikValues
                      }

            sendOldCenikValues = //choose between db and Json/XML
                fun _ -> 
                    async
                        {
                          //************* Json/XML ********************
                          //leave it here despite using db in order to update xml
                            let copyFilesNow x =
                               //failwith "Simulated exception3" 
                               copyFiles 
                               <| "jsonCenikValues.xml"
                               <| "jsonCenikValuesBackUp.xml"
                            let exnJson1 = (copyFilesNow, (fun x -> ()), "Error3") |||> tryWith |> deconstructor1

                            (*
                            let sendOldCenikValuesNow x =
                               //failwith "Simulated exception4" 
                                deserialize "jsonCenikValuesBackUp.xml"
                            let (sendOldCenikValues, exnJson2) = (sendOldCenikValuesNow, (fun x -> ()), "Error4") |||> tryWith |> deconstructor2 GetCenikValues.Default
                            return { sendOldCenikValues with Msgs = { Messages.Default with Msg1 = exnJson1; Msg2 = exnJson2 } }
                            *)

                            //************* plain SQL or Dapper.FSharp ********************                     
                            let IdNew = 2
                            let IdOld = 3
                                                                    
                            let (dbGetNewCenikValues, exnSql2) = selectValues IdNew                                         
                            let exnSql = insertOrUpdate { dbGetNewCenikValues with Id = IdOld; ValueState = "old" }//eqv of the aforementioned copying 
                            let (dbSendOldCenikValues, exnSql3) = selectValues IdOld

                            return { dbSendOldCenikValues with Msgs = { Messages.Default with Msg1 = exnJson1; Msg2 = exnSql2; Msg3 = exnSql3 } }                    
                        }

            sendDeserialisedCenikValues = //choose between db and Json/XML
               fun _ ->
                   async
                       {
                           //************* Json/XML ********************                   
                           (*
                           let sendCenikValuesNow x =
                               //failwith "Simulated exception8" 
                               deserialize "jsonCenikValues.xml" 
                           let (sendCenikValues, exnJson1) = (sendCenikValuesNow, (fun x -> ()), "Error8") |||> tryWith |> deconstructor2
                           return { sendCenikValues with Msgs = { Messages.Default with Msg1 = exnJson1 } }
                           *)

                           //************* plain SQL or Dapper.FSharp ********************
                           let IdNew = 2
                    
                           let (dbSendCenikValues, exnSql1) = selectValues IdNew

                           return { dbSendCenikValues with Msgs = { Messages.Default with Msg1 = exnSql1 } }
                       }

            getKontaktValues =
                fun getKontaktValues ->
                    async
                        {
                            let getNewKontaktValues: GetKontaktValues = 
                                match verifyKontaktValues getKontaktValues with
                                | Ok () ->                                  
                                           let serializeNow x =
                                              //failwith "Simulated exception10" 
                                              serialize getKontaktValues "jsonKontaktValues.xml"                            
                                           let exnJson = (serializeNow, (fun x -> ()), "Error10") |||> tryWith |> deconstructor1
                                           { getKontaktValues with Msgs = { Messages.Default with Msg1 = exnJson } }                                   
                                | _     -> GetKontaktValues.Default    
                            return getNewKontaktValues
                        }

            sendOldKontaktValues =
                fun _ ->
                    async
                        {
                            let copyFilesNow x =
                                //failwith "Simulated exception11" 
                                copyFiles 
                                <| "jsonKontaktValues.xml"
                                <| "jsonKontaktValuesBackUp.xml"
                            let exnJson1 = (copyFilesNow, (fun x -> ()), "Error11") |||> tryWith |> deconstructor1
                    
                            let sendOldKontaktValuesNow x =
                                //failwith "Simulated exception12" 
                                deserialize "jsonKontaktValuesBackUp.xml"
                            let (sendOldKontaktValues, exnJson2) = (sendOldKontaktValuesNow, (fun x -> ()), "Error12") |||> tryWith |> deconstructor2 GetKontaktValues.Default
                            return { sendOldKontaktValues with Msgs = { Messages.Default with Msg1 = exnJson1; Msg2 = exnJson2 } }                 
                        } 

            sendDeserialisedKontaktValues =
                fun _ ->
                    async
                        {                  
                            let sendKontaktValuesNow x =
                                //failwith "Simulated exception13" 
                                deserialize "jsonKontaktValues.xml" 
                            let (sendKontaktValues, exnJson1) = (sendKontaktValuesNow, (fun x -> ()), "Error13") |||> tryWith |> deconstructor2 GetKontaktValues.Default
                            return { sendKontaktValues with Msgs = { Messages.Default with Msg1 = exnJson1 } }                   
                        }

            getLinkAndLinkNameValues =
               fun getLinkAndLinkNameValues ->
                   async
                      {
                         let getNewLinkAndLinkNameValues: GetLinkAndLinkNameValues = 
                             match verifyLinkAndLinkNameValues getLinkAndLinkNameValues with
                             | Ok () ->
                                        let serializeNow x =
                                            //failwith "Simulated exception14" 
                                            serialize getLinkAndLinkNameValues "jsonLinkAndLinkNameValues.xml"                             
                                        let exnJson = (serializeNow, (fun x -> ()), "Error14") |||> tryWith |> deconstructor1
                                        { getLinkAndLinkNameValues with Msgs = { Messages.Default with Msg1 = exnJson } }     
                             | _     -> GetLinkAndLinkNameValues.Default                        
                        return getNewLinkAndLinkNameValues
                      }
           
            sendOldLinkAndLinkNameValues =
                fun _ ->
                   async
                       {
                           let copyFilesNow x =
                                  //failwith "Simulated exception15" 
                               copyFiles 
                               <| "jsonLinkAndLinkNameValues.xml"
                               <| "jsonLinkAndLinkNameValuesBackUp.xml"
                           let exnJson1 = (copyFilesNow, (fun x -> ()), "Error15") |||> tryWith |> deconstructor1
                      
                           let sendOldLinkAndLinkNameValuesNow x =
                               //failwith "Simulated exception16" 
                               deserialize "jsonLinkAndLinkNameValuesBackUp.xml" 
                           let (sendOldLinkAndLinkNameValues, exnJson2) = (sendOldLinkAndLinkNameValuesNow, (fun x -> ()), "Error16") |||> tryWith |> deconstructor2 GetLinkAndLinkNameValues.Default
                           return { sendOldLinkAndLinkNameValues with Msgs = { Messages.Default with Msg1 = exnJson1; Msg2 = exnJson2 } }  
                       } 

            sendDeserialisedLinkAndLinkNameValues =
               fun _ ->
                   async
                       {
                           let sendLinkAndLinkNameValuesNow x =
                               //failwith "Simulated exception17" 
                               deserialize "jsonLinkandLinkNameValues.xml"  
                           let (sendLinkAndLinkNameValues, exnJson1) = (sendLinkAndLinkNameValuesNow, (fun x -> ()), "Error17") |||> tryWith |> deconstructor2 GetLinkAndLinkNameValues.Default
                           return { sendLinkAndLinkNameValues with Msgs = { Messages.Default with Msg1 = exnJson1 } }  
                       }
        }

    let webApp =
        Remoting.createApi ()
        |> Remoting.withRouteBuilder Route.builder
        |> Remoting.fromValue IGetApi
        |> Remoting.buildHttpHandler

    let app =
        //insertOrUpdate GetCenikValues.Default |> ignore //Not necessary, GetCenikValues.Default everywhere takes care in cases when data in db are not available
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