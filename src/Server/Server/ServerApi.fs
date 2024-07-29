namespace Server

open System
open System.IO

open Saturn

open Fable.Remoting.Server
open Fable.Remoting.Giraffe

open Shared
open Shared

open Errors
open Settings
open ServerVerify

open Database.Select
open Database.Errors.Errors
open Database.InsertOrUpdate

open ErrorTypes.Server

open Connections.Connection

open Helpers.Server.CEBuilders
open Helpers.Server.CopyOrMoveFiles
open Helpers.Server.CopyOrMoveFilesFM

open Serialization.Server.Serialisation
open Serialization.Server.Deserialisation

open DtoXml.Server.DtoXml
open DtoFromStorage.Server.DtoFromStorage

open TransLayerXml.Server.TransLayerXml
open TransLayerSend.Server.TransLayerSend
open TransLayerFromStorage.Server.TransLayerFromStorage

module ServerApi =

    let internal IGetApi errMsg =
        {            
            login = fun login -> async { return (verifyLogin login) }

            //************* plain SQL or Dapper.FSharp ********************  
            sendCenikValues =  
                fun sendCenikValues
                    ->
                     async
                         {                   
                             let sendNewCenikValues: CenikValuesShared =                        
                                 match verifyCenikValues sendCenikValues with                
                                 | Ok ()   ->
                                            let dbNewCenikValues = { sendCenikValues with Id = 2; ValueState = "new" }
                                            let cond = dbNewCenikValues.Msgs.Msg1 = "First run"
                                            let cenikValuesSend = cenikValuesTransformLayerToStorage dbNewCenikValues
                                            let exnSql = errorMsgBoxIU (insertOrUpdate getConnection closeConnection cenikValuesSend) cond
                                            
                                            { dbNewCenikValues with Msgs = { SharedMessageDefaultValues.messageDefault with Msg1 = exnSql } }
                                                                           
                                 | Error _ ->
                                            SharedCenikValues.cenikValuesDomainDefault

                             return sendNewCenikValues
                         }

            getOldCenikValues = 
                fun _
                    -> 
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
                             let cenikValuesSend = cenikValuesTransformLayerToStorage dbCenikValues
                             let exnSql = errorMsgBoxIU (insertOrUpdate getConnection closeConnection cenikValuesSend) cond
                           
                             //********************************************************
                             let (dbSendOldCenikValues, exnSql3) =  
                                 match selectValues getConnection closeConnection (insertDefaultValues insertOrUpdate) IdOld with   
                                 | Ok value  -> value, String.Empty                                            
                                 | Error err -> errorMsgBoxS err 

                             return { dbSendOldCenikValues with Msgs = { SharedMessageDefaultValues.messageDefault with Msg1 = exnSql; Msg2 = exnSql2; Msg3 = exnSql3 } }
                          }

            getDeserialisedCenikValues = 
               fun _
                   ->
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

                fun sendKontaktValues
                    ->
                     async
                         {
                             let sendNewKontaktValues: KontaktValuesShared = 
                                 match verifyKontaktValues sendKontaktValues with
                                 | Ok ()   ->                                            
                                            try
                                                let config = 
                                                    {
                                                        source = pathToXml3
                                                        destination = pathToXmlBackup3
                                                        fileName = String.Empty
                                                    }

                                                //failwith "Simulated exception10"
                                                let sendKontaktValuesDtoXml = kontaktValuesTransformLayerDomainToXml sendKontaktValues
                                                
                                                match copyOrMoveFiles config Copy with
                                                | Ok _      -> parseToXml3 sendKontaktValuesDtoXml pathToXml3
                                                | Error err -> Error (sprintf"%s %s" "Zadané hodnoty nebyly uloženy, neb došlo k této chybě: " err)                                                                                                       
                                            with
                                            | ex -> Error (string ex.Message)

                                            |> function
                                                | Ok _      -> sendKontaktValues  
                                                | Error err -> { sendKontaktValues with Msgs = { SharedMessageDefaultValues.messageDefault with Msg1 = err } }                                                                             
                                 | Error _ ->
                                            SharedKontaktValues.kontaktValuesDomainDefault

                             return sendNewKontaktValues
                         }

            getOldKontaktValues =
                fun _
                    ->
                     async
                         {
                             let (getOldKontaktValues, err) =
                                 try
                                     pyramidOfInferno
                                         {
                                             let config = 
                                                 {
                                                     source = pathToXml3
                                                     destination = pathToXmlBackup3
                                                     fileName = String.Empty
                                                 }

                                             let copy = copyOrMoveFiles config Copy //copyFiles pathToXml3 pathToXmlBackup3 true
                                             let! _ = copy, SharedKontaktValues.kontaktValuesDomainDefault

                                             let deserialize = parseFromXml3 pathToXmlBackup3
                                             let! deserialize = deserialize, SharedKontaktValues.kontaktValuesDomainDefault

                                             return kontaktValuesTransformLayerXmlToDomain deserialize, String.Empty
                                         }                  
                                 with
                                 | ex -> SharedKontaktValues.kontaktValuesDomainDefault, sprintf"%s %s" "Pro zobrazování navrhovaných a předchozích hodnot kontaktů byly dosazeny defaultní hodnoty, neb došlo k této chybě 1: " (string ex.Message)
                             
                             return { getOldKontaktValues with Msgs = { SharedMessageDefaultValues.messageDefault with Msg1 = err } }                 
                         } 

            getDeserialisedKontaktValues =
                fun _
                    ->
                     async
                         {                                              
                             let (getKontaktValues, err) =
                                 try
                                     //failwith "Simulated exception12"
                                     pyramidOfInferno
                                         {
                                             let config = 
                                                 {
                                                     source = pathToXml3
                                                     destination = pathToXmlBackup3
                                                     fileName = String.Empty
                                                 }

                                             let copy = copyOrMoveFiles config Copy //copyFiles pathToXml3 pathToXmlBackup3 true
                                             let! _ = copy, SharedKontaktValues.kontaktValuesDomainDefault

                                             //let deserialize = deserializeFromXml2<KontaktValuesDtoXml2> pathToXmlBackup2
                                             let deserialize = parseFromXml3 pathToXmlBackup3
                                             let! deserialize = deserialize, SharedKontaktValues.kontaktValuesDomainDefault

                                             return kontaktValuesTransformLayerXmlToDomain deserialize, String.Empty
                                         }                                  
                                 with
                                 | ex -> SharedKontaktValues.kontaktValuesDomainDefault, sprintf"%s %s" "Pro zobrazování navrhovaných a předchozích hodnot kontaktů byly dosazeny defaultní hodnoty, neb došlo k této chybě 2: " (string ex.Message) 

                             return { getKontaktValues with Msgs = { SharedMessageDefaultValues.messageDefault with Msg1 = err } }                   
                         }

            //************* testing JSON ********************                               
            sendLinkAndLinkNameValues =
               fun sendLinkAndLinkNameValues
                   ->
                    async
                        {                       
                            let sendNewLinkAndLinkNameValues: LinkValuesShared = 
                                match verifyLinkAndLinkNameValues sendLinkAndLinkNameValues with
                                | Ok ()   ->                                       
                                           try 
                                               //failwith "Simulated exception14"   
                                               let sendLinkAndLinkNameValuesDtoSend = linkValuesTransformLayerToStorage sendLinkAndLinkNameValues                                               
                                               match copyFiles pathToJson pathToJsonBackup true with
                                               | Ok _      -> serializeToJsonThoth2 sendLinkAndLinkNameValuesDtoSend pathToJson
                                               | Error err -> Error (sprintf"%s %s" "Zadané hodnoty nebyly uloženy, neb došlo k této chybě: " err)                                                   
                                           with
                                           | ex -> Error (string ex.Message)

                                           |> function
                                               | Ok _      -> sendLinkAndLinkNameValues  
                                               | Error err -> { sendLinkAndLinkNameValues with Msgs = { SharedMessageDefaultValues.messageDefault with Msg1 = err } }    
                                | Error _ ->
                                           SharedLinkValues.linkValuesDomainDefault

                            return sendNewLinkAndLinkNameValues
                        }
           
            getOldLinkValues =
                fun _
                    ->
                     async
                         {                         
                             let (getOldLinkAndLinkNameValues, err) =
                                 try
                                     //failwith "Simulated exception15"
                                     pyramidOfInferno
                                         {
                                             let copy = copyFiles pathToJson pathToJsonBackup true
                                             let! _ = copy, SharedLinkValues.linkValuesDomainDefault

                                             let deserialize = deserializeFromJsonThoth2<LinkValuesDtoFromStorage> pathToJsonBackup
                                             let! deserialize = deserialize, SharedLinkValues.linkValuesDomainDefault

                                             return linkValuesTransformLayerFromStorage deserialize, String.Empty
                                         }                                      
                                 with
                                 | ex -> SharedLinkValues.linkValuesDomainDefault, sprintf"%s %s" "Pro zobrazování navrhovaných a předchozích hodnot odkazů byly dosazeny defaultní hodnoty, neb došlo k této chybě: " (string ex.Message) 

                             return { getOldLinkAndLinkNameValues with Msgs = { SharedMessageDefaultValues.messageDefault with Msg1 = err } }  
                         } 

            getDeserialisedLinkAndLinkNameValues =
               fun _
                   ->
                    async
                        {
                            let (getLinkAndLinkNameValues, err) =
                                try
                                    //failwith "Simulated exception15"
                                    pyramidOfInferno
                                        {
                                            let copy = copyFiles pathToJson pathToJsonBackup true
                                            let! _ = copy, SharedLinkValues.linkValuesDomainDefault

                                            let deserialize = deserializeFromJsonThoth2<LinkValuesDtoFromStorage> pathToJsonBackup
                                            let! deserialize = deserialize, SharedLinkValues.linkValuesDomainDefault

                                            return linkValuesTransformLayerFromStorage deserialize, String.Empty
                                        }                                      
                                with
                                | ex -> SharedLinkValues.linkValuesDomainDefault, sprintf"%s %s" "Pro zobrazování navrhovaných a předchozích hodnot odkazů byly dosazeny defaultní hodnoty, neb došlo k této chybě: " (string ex.Message)  

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
        let cenikValuesSend = cenikValuesTransformLayerToStorage dbCenikValues
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