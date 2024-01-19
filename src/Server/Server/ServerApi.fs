namespace Server

open System
open System.IO

open Saturn

open Fable.Remoting.Server
open Fable.Remoting.Giraffe

open Shared
open SharedTypes

open Errors
open Settings
open ServerVerify

open Database.Select
open Database.Errors.Errors
open Database.InsertOrUpdate

open ErrorTypes.Server

open Connections.Connection

open Helpers.Server.CEBuilders
open Helpers.Server.CopyingFiles
open Helpers.Server.Serialisation
open Helpers.Server.Deserialisation

open DtoXml.Server.DtoXml
open DtoGet.Server.DtoGet

open TransLayerXml.Server.TransLayerXml
open TransLayerGet.Server.TransLayerGet
open TransLayerSend.Server.TransLayerSend

module ServerApi =

    let internal IGetApi errMsg =
        {            
            login = fun login -> async { return (verifyLogin login) }

            //************* plain SQL or Dapper.FSharp ********************  
            sendCenikValues =  
                fun sendCenikValues ->
                    async
                        {                   
                            let sendNewCenikValues: CenikValuesDomain =                        
                                match verifyCenikValues sendCenikValues with                
                                | Ok ()   ->
                                           let dbNewCenikValues = { sendCenikValues with Id = 2; ValueState = "new" }
                                           let cond = dbNewCenikValues.Msgs.Msg1 = "First run"
                                           let cenikValuesSend = cenikValuesTransferLayerSend dbNewCenikValues
                                           let exnSql = errorMsgBoxIU (insertOrUpdate getConnection closeConnection cenikValuesSend) cond
                                            
                                           { dbNewCenikValues with Msgs = { SharedMessageDefaultValues.messageDefault with Msg1 = exnSql } }
                                                                           
                                | Error _ ->
                                           SharedCenikValues.cenikValuesDomainDefault

                          return sendNewCenikValues
                      }

            getOldCenikValues = 
                fun _ -> 
                    async
                        {                     
                            let IdNew = 2
                            let IdOld = 3
                            
                            let (dbGetNewCenikValues, exnSql2) =                              
                                match selectValues getConnection closeConnection (insertDefaultValues insertOrUpdate) IdNew with   
                                | Ok value  -> value, String.Empty                                            
                                | Error err -> errorMsgBoxS err

                            //********************************************************
                            let dbCenikValues = { dbGetNewCenikValues with Id = IdOld; ValueState = "old" }
                            let cond = dbCenikValues.Msgs.Msg1 = "First run"
                            let cenikValuesSend = cenikValuesTransferLayerSend dbCenikValues
                            let exnSql = errorMsgBoxIU (insertOrUpdate getConnection closeConnection cenikValuesSend) cond
                           
                            //********************************************************
                            let (dbSendOldCenikValues, exnSql3) =  
                                match selectValues getConnection closeConnection (insertDefaultValues insertOrUpdate) IdOld with   
                                | Ok value  -> value, String.Empty                                            
                                | Error err -> errorMsgBoxS err 

                            return { dbSendOldCenikValues with Msgs = { SharedMessageDefaultValues.messageDefault with Msg1 = exnSql; Msg2 = exnSql2; Msg3 = exnSql3 } }
                        }

            getDeserialisedCenikValues = 
               fun _ ->
                   async
                       {           
                           let IdNew = 2
                    
                           let (dbSendCenikValues, exnSql1) =                                                          
                               match selectValues getConnection closeConnection (insertDefaultValues insertOrUpdate) IdNew with   
                               | Ok value  -> value, String.Empty                                            
                               | Error err -> errorMsgBoxS err
                                                                    
                           return { dbSendCenikValues with Msgs = { SharedMessageDefaultValues.messageDefault with Msg1 = exnSql1; Msg2 = errMsg } } 
                       }

            //************* testing XML ********************          
            sendKontaktValues =
                fun sendKontaktValues ->
                    async
                        {
                            let sendNewKontaktValues: KontaktValuesDomain = 
                                match verifyKontaktValues sendKontaktValues with
                                | Ok ()    ->                                            
                                            let f3 = sprintf"%s %s" "Zadané hodnoty nebyly uloženy, neb došlo k této chybě: "
                                            let f1 () = 
                                                //failwith "Simulated exception10"
                                                let sendKontaktValuesDtoXml = kontaktValuesTransferLayerDomainToXml sendKontaktValues
                                                match copyFiles pathToXml pathToXmlBackup true with
                                                | Ok _      -> serializeToXml sendKontaktValuesDtoXml pathToXml
                                                | Error err -> Error (f3 err)                                                                                                       
                                            tryWithResult f1 () f3
                                            |> function
                                                | Ok _      -> sendKontaktValues  
                                                | Error err -> { sendKontaktValues with Msgs = { SharedMessageDefaultValues.messageDefault with Msg1 = err } }                                                                             
                                | Error _  ->
                                            SharedKontaktValues.kontaktValuesDomainDefault

                            return sendNewKontaktValues
                        }

            getOldKontaktValues =
                fun _ ->
                    async
                        {
                            let (getOldKontaktValues, err) =
                                let f1 () =
                                    //failwith "Simulated exception12"
                                    pyramidOfInferno
                                        {
                                            let copy = copyFiles pathToXml pathToXmlBackup false
                                            let! _ = copy, SharedKontaktValues.kontaktValuesDomainDefault

                                            let deserialize = deserializeFromXml<KontaktValuesDtoXml> pathToXmlBackup
                                            let! deserialize = deserialize, SharedKontaktValues.kontaktValuesDomainDefault

                                            return kontaktValuesTransferLayerXmlToDomain deserialize, String.Empty
                                        }                                  
                                let f3 ex = SharedKontaktValues.kontaktValuesDomainDefault, sprintf"%s %s" "Pro zobrazování navrhovaných a předchozích hodnot kontaktů byly dosazeny defaultní hodnoty, neb došlo k této chybě: " ex 
                                tryWithResult1 f1 () f3      

                            return { getOldKontaktValues with Msgs = { SharedMessageDefaultValues.messageDefault with Msg1 = err } }                 
                        } 

            getDeserialisedKontaktValues =
                fun _ ->
                    async
                        {                                              
                            let (getKontaktValues, err) =
                                let f1 () =
                                    //failwith "Simulated exception12"
                                    pyramidOfInferno
                                        {
                                            let copy = copyFiles pathToXml pathToXmlBackup true
                                            let! _ = copy, SharedKontaktValues.kontaktValuesDomainDefault

                                            let deserialize = deserializeFromXml<KontaktValuesDtoXml> pathToXmlBackup
                                            let! deserialize = deserialize, SharedKontaktValues.kontaktValuesDomainDefault

                                            return kontaktValuesTransferLayerXmlToDomain deserialize, String.Empty
                                        }                                  
                                let f3 ex = SharedKontaktValues.kontaktValuesDomainDefault, sprintf"%s %s" "Pro zobrazování navrhovaných a předchozích hodnot kontaktů byly dosazeny defaultní hodnoty, neb došlo k této chybě: " ex 
                                tryWithResult1 f1 () f3      

                            return { getKontaktValues with Msgs = { SharedMessageDefaultValues.messageDefault with Msg1 = err } }                   
                        }

            //************* testing JSON ********************                               
            sendLinkAndLinkNameValues =
               fun sendLinkAndLinkNameValues ->
                   async
                      {
                         let sendNewLinkAndLinkNameValues: LinkAndLinkNameValuesDomain = 
                             match verifyLinkAndLinkNameValues sendLinkAndLinkNameValues with
                             | Ok ()   -> 
                                        let f3 = sprintf"%s %s" "Zadané hodnoty nebyly uloženy, neb došlo k této chybě: "
                                        let f1 () = 
                                            //failwith "Simulated exception14"   
                                            let sendLinkAndLinkNameValuesDtoSend = linkAndLinkNameValuesTransferLayerSend sendLinkAndLinkNameValues                                               
                                            match copyFiles pathToJson pathToJsonBackup true with
                                            | Ok _      -> serializeToJson sendLinkAndLinkNameValuesDtoSend pathToJson
                                            | Error err -> Error (f3 err)                                                   
                                        tryWithResult f1 () f3
                                        |> function
                                            | Ok _      -> sendLinkAndLinkNameValues  
                                            | Error err -> { sendLinkAndLinkNameValues with Msgs = { SharedMessageDefaultValues.messageDefault with Msg1 = err } }    
                             | Error _ ->
                                        SharedLinkAndLinkNameValues.linkAndLinkNameValuesDomainDefault

                         return sendNewLinkAndLinkNameValues
                      }
           
            getOldLinkAndLinkNameValues =
                fun _ ->
                   async
                       {                         
                           let (getOldLinkAndLinkNameValues, err) =
                               let f1 () =
                                   //failwith "Simulated exception15"
                                   pyramidOfInferno
                                       {
                                           let copy = copyFiles pathToJson pathToJsonBackup false
                                           let! _ = copy, SharedLinkAndLinkNameValues.linkAndLinkNameValuesDomainDefault

                                           let deserialize = deserializeFromJson<LinkAndLinkNameValuesDtoGet> pathToJsonBackup
                                           let! deserialize = deserialize, SharedLinkAndLinkNameValues.linkAndLinkNameValuesDomainDefault

                                           return linkAndLinkNameValuesTransferLayerGet deserialize, String.Empty
                                       }                                      
                               let f3 ex = SharedLinkAndLinkNameValues.linkAndLinkNameValuesDomainDefault, sprintf"%s %s" "Pro zobrazování navrhovaných a předchozích hodnot odkazů byly dosazeny defaultní hodnoty, neb došlo k této chybě: " ex 
                               tryWithResult1 f1 () f3

                           return { getOldLinkAndLinkNameValues with Msgs = { SharedMessageDefaultValues.messageDefault with Msg1 = err } }  
                       } 

            getDeserialisedLinkAndLinkNameValues =
               fun _ ->
                   async
                       {
                           let (getLinkAndLinkNameValues, err) =
                               let f1 () =
                                   //failwith "Simulated exception15"
                                   pyramidOfInferno
                                       {
                                           let copy = copyFiles pathToJson pathToJsonBackup true
                                           let! _ = copy, SharedLinkAndLinkNameValues.linkAndLinkNameValuesDomainDefault

                                           let deserialize = deserializeFromJson<LinkAndLinkNameValuesDtoGet> pathToJsonBackup
                                           let! deserialize = deserialize, SharedLinkAndLinkNameValues.linkAndLinkNameValuesDomainDefault

                                           return linkAndLinkNameValuesTransferLayerGet deserialize, String.Empty
                                       }                                      
                               let f3 ex = SharedLinkAndLinkNameValues.linkAndLinkNameValuesDomainDefault, sprintf"%s %s" "Pro zobrazování navrhovaných a předchozích hodnot odkazů byly dosazeny defaultní hodnoty, neb došlo k této chybě: " ex 
                               tryWithResult1 f1 () f3

                           return { getLinkAndLinkNameValues with Msgs = { SharedMessageDefaultValues.messageDefault with Msg1 = err } }  
                       }

            (*
            getSecurityTokenFile = //TODO try with
                fun getSecurityTokenFile -> async { return File.Exists(Path.GetFullPath("securityToken.txt")) }             
                
            getSecurityToken =
                fun getSecurityToken ->  //TODO try with
                                     async
                                         {       
                                            //some code
                                         }        
                 
            deleteSecurityTokenFile =
                fun deleteSecurityTokenFile ->  //TODO try with
                                            async
                                                {
                                                    File.Delete(Path.GetFullPath("securityToken.txt"))
                                                    return ()
                                                }   
            *)
        }

    let webApp exnSql =
        Remoting.createApi ()
        |> Remoting.withRouteBuilder Route.builder
        |> Remoting.fromValue (IGetApi exnSql)
        |> Remoting.buildHttpHandler

    let app =
        //let exnSql = insertOrUpdate { GetCenikValues.Default with Msgs = { Messages.Default with Msg1 = "First run" } }
        let dbCenikValues = { SharedCenikValues.cenikValuesDomainDefault with Msgs = { SharedMessageDefaultValues.messageDefault with Msg1 = "First run" } }
        let cenikValuesSend = cenikValuesTransferLayerSend dbCenikValues
        let exnSql = errorMsgBoxIU (insertOrUpdate getConnection closeConnection cenikValuesSend) true //true == first run

        application
            {
                use_router (webApp exnSql)
                memory_cache
                use_static "public"
                use_gzip
            }

    [<EntryPoint>]
    let main _ =   
        Dapper.FSharp.OptionTypes.register()
        //pswHash() //to be used only once && before bundling 
        run app    
        0