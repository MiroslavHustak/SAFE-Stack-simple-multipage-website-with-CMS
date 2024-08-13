namespace Server

open System
open System.IO
open System.Data.SqlClient

open Saturn
open Giraffe

open Fable.Remoting.Server
open Fable.Remoting.Giraffe

open Shared

open Settings
open ServerVerify

open Database.Select
open Database.Errors.Errors
open Database.InsertOrUpdate

open Logging.Logging
open Connections.Connection

open Helpers.Server.CEBuilders
open Helpers.Server.CopyOrMoveFiles
open Helpers.Server.CopyOrMoveFilesFM

open Serialization.Server.Serialisation
open Serialization.Server.Deserialisation

open DtoFromStorage.Server.DtoFromStorage

open TransLayerXml.Server.TransLayerXml
open TransLayerSend.Server.TransLayerSend
open TransLayerFromStorage.Server.TransLayerFromStorage

module ServerApi =

    let internal IGetApi connection errMsg =
        {            
            login = fun login -> async { return (verifyLogin () login) }

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
                                            let exnSql = errorMsgBoxIU (insertOrUpdate connection cenikValuesSend) cond
                                            
                                            { dbNewCenikValues with Msgs = { SharedMessageDefaultValues.messageDefault with Msg1 = exnSql } }
                                                                           
                                 | Error _ ->
                                            logInfoMsg <| sprintf "Error001 %s" String.Empty
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
                                 match selectValues connection (insertDefaultValues insertOrUpdate connection) IdNew with   
                                 | Ok value  ->
                                              value, String.Empty                                            
                                 | Error err ->
                                              logInfoMsg <| sprintf "Error002 %s" String.Empty 
                                              errorMsgBoxS err

                             //********************************************************
                             let dbCenikValues = { dbGetNewCenikValues with Id = IdOld; ValueState = "old" }
                             let cond = dbCenikValues.Msgs.Msg1 = "First run"
                             let cenikValuesSend = cenikValuesTransformLayerToStorage dbCenikValues
                             let exnSql = errorMsgBoxIU (insertOrUpdate connection cenikValuesSend) cond
                           
                             //********************************************************
                             let (dbSendOldCenikValues, exnSql3) =  
                                 match selectValues connection (insertDefaultValues insertOrUpdate connection) IdOld with   
                                 | Ok value  ->
                                              value, String.Empty                                            
                                 | Error err ->
                                              logInfoMsg <| sprintf "Error003 %s" String.Empty
                                              errorMsgBoxS err 

                             return { dbSendOldCenikValues with Msgs = { SharedMessageDefaultValues.messageDefault with Msg1 = exnSql; Msg2 = exnSql2; Msg3 = exnSql3 } }
                          }

            getDeserialisedCenikValues = 
               fun _
                   ->
                    async
                        {           
                            let IdNew = 2
                    
                            let (dbSendCenikValues, exnSql1) =                                                          
                                match selectValues connection (insertDefaultValues insertOrUpdate connection) IdNew with   
                                | Ok value  ->
                                             value, String.Empty                                            
                                | Error err ->
                                             logInfoMsg <| sprintf "Error004 %s" String.Empty 
                                             errorMsgBoxS err
                                                                    
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
                                                | Error err ->
                                                             logInfoMsg <| sprintf "Error005 %s" err
                                                             Error (sprintf"%s %s" "Zadané hodnoty nebyly uloženy, neb došlo k této chybě: " err)                                                                                                       
                                            with
                                            | ex -> Error (string ex.Message)

                                            |> function
                                                | Ok _      ->
                                                             sendKontaktValues  
                                                | Error err ->
                                                             logInfoMsg <| sprintf "Error005A %s" err  
                                                             { sendKontaktValues with Msgs = { SharedMessageDefaultValues.messageDefault with Msg1 = err } }                                                                             
                                 | Error _ ->
                                            logInfoMsg <| sprintf "Error005B %s" String.Empty   
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
                                     Ok
                                     <|
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
                                 | ex -> Error (string ex.Message)

                                 |> function
                                     | Ok value  ->
                                                  value  
                                     | Error err ->
                                                  logInfoMsg <| sprintf "Error006 %s" err 
                                                  let errMsg = "Pro zobrazování navrhovaných a předchozích hodnot kontaktů byly dosazeny defaultní hodnoty, neb došlo k této chybě 1: "  
                                                  SharedKontaktValues.kontaktValuesDomainDefault, sprintf"%s %s" errMsg err

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
                                     Ok
                                     <|
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
                                 | ex -> Error (string ex.Message)

                                 |> function
                                     | Ok value  ->
                                                  value  
                                     | Error err ->
                                                  logInfoMsg <| sprintf "Error007 %s" err 
                                                  let errMsg = "Pro zobrazování navrhovaných a předchozích hodnot kontaktů byly dosazeny defaultní hodnoty, neb došlo k této chybě 2: "
                                                  SharedKontaktValues.kontaktValuesDomainDefault, sprintf"%s %s" errMsg err

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
                                               | Ok _      ->
                                                            serializeToJsonThoth2 sendLinkAndLinkNameValuesDtoSend pathToJson
                                               | Error err ->
                                                            logInfoMsg <| sprintf "Error008 %s" String.Empty 
                                                            Error (sprintf"%s %s" "Zadané hodnoty nebyly uloženy, neb došlo k této chybě: " err)                                                   
                                           with
                                           | ex -> Error (string ex.Message)

                                           |> function
                                               | Ok _      ->                                                            
                                                            sendLinkAndLinkNameValues  
                                               | Error err ->
                                                            logInfoMsg <| sprintf "Error008A %s" err 
                                                            { sendLinkAndLinkNameValues with Msgs = { SharedMessageDefaultValues.messageDefault with Msg1 = err } }    
                                | Error _ ->
                                           logInfoMsg <| sprintf "Error008B %s" String.Empty 
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
                                     Ok
                                     <|
                                     pyramidOfInferno
                                         {
                                             let copy = copyFiles pathToJson pathToJsonBackup true
                                             let! _ = copy, SharedLinkValues.linkValuesDomainDefault

                                             let deserialize = deserializeFromJsonThoth2<LinkValuesDtoFromStorage> pathToJsonBackup
                                             let! deserialize = deserialize, SharedLinkValues.linkValuesDomainDefault

                                             return linkValuesTransformLayerFromStorage deserialize, String.Empty
                                         }                                      
                                 
                                 with
                                 | ex -> Error (string ex.Message)

                                 |> function
                                     | Ok value  ->
                                                  value 
                                     | Error err ->
                                                  logInfoMsg <| sprintf "Error009 %s" err 
                                                  let errMsg = "Pro zobrazování navrhovaných a předchozích hodnot odkazů byly dosazeny defaultní hodnoty, neb došlo k této chybě: "
                                                  SharedLinkValues.linkValuesDomainDefault, sprintf"%s %s" errMsg err

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
                                    Ok
                                    <|
                                    pyramidOfInferno
                                        {
                                            let copy = copyFiles pathToJson pathToJsonBackup true
                                            let! _ = copy, SharedLinkValues.linkValuesDomainDefault

                                            let deserialize = deserializeFromJsonThoth2<LinkValuesDtoFromStorage> pathToJsonBackup
                                            let! deserialize = deserialize, SharedLinkValues.linkValuesDomainDefault

                                            return linkValuesTransformLayerFromStorage deserialize, String.Empty
                                        }                                      
                                
                                with
                                | ex -> Error (string ex.Message)

                                |> function
                                    | Ok value  ->
                                                 value 
                                    | Error err ->
                                                 logInfoMsg <| sprintf "Error010 %s" err 
                                                 let errMsg = "Pro zobrazování navrhovaných a předchozích hodnot odkazů byly dosazeny defaultní hodnoty, neb došlo k této chybě: "
                                                 SharedLinkValues.linkValuesDomainDefault, sprintf"%s %s" errMsg err

                            return { getLinkAndLinkNameValues with Msgs = { SharedMessageDefaultValues.messageDefault with Msg1 = err } }  
                        }

            (*
            getSecurityTokenFile = //TODO try with
                fun getSecurityTokenFile -> async { return File.Exists(Path.GetFullPath("securityToken.txt")) }             
                
            getSecurityToken =
                fun getSecurityToken
                    ->  //TODO try with
                     async
                         {       
                             //some code
                         }        
                 
            deleteSecurityTokenFile =
                fun deleteSecurityTokenFile
                    ->  //TODO try with
                     async
                         {
                             File.Delete(Path.GetFullPath("securityToken.txt"))
                             return ()
                         }   
            *)
        }

    let handler (createConnection: SqlConnection) exnSql : HttpHandler =

        Remoting.createApi ()
        |> Remoting.withRouteBuilder Route.builder
        |> Remoting.fromValue (IGetApi createConnection exnSql)
        |> Remoting.buildHttpHandler

    let app exnSql (createConnection: SqlConnection) =
    
        application
            {
                use_router (handler createConnection exnSql)
                memory_cache
                use_static "public"
                use_gzip
            }

    [<EntryPoint>]
    let main _ =

        (*
        <!-- Add this xml code into Directory.Build.props in the root (or create the file if missing)  -->
        <Project>
            <PropertyGroup>
                <!-- <WarningsAsErrors>0025</WarningsAsErrors> -->
                <TreatWarningsAsErrors>true</TreatWarningsAsErrors>        
            </PropertyGroup>
        </Project>
        *)

        //Dapper.FSharp.OptionTypes.register()

        //pswHash() //to be used only once && before bundling  

        try
            let createConnection : SqlConnection = Connections.Connection.getConnection()

            try
                //failwith "DB connection exception test"
                let dbCenikValues = { SharedCenikValues.cenikValuesDomainDefault with Msgs = { SharedMessageDefaultValues.messageDefault with Msg1 = "First run" } }
                let cenikValuesSend = cenikValuesTransformLayerToStorage dbCenikValues
                let exnSql = errorMsgBoxIU (insertOrUpdate createConnection cenikValuesSend) true //true == first run

                (app exnSql) >> run <| createConnection

                Ok ()

            finally                
                closeConnection createConnection
        
        with
        | ex -> Error (string ex.Message)

        |> function
            | Ok value  ->
                         value 
            | Error err ->
                         logInfoMsg <| sprintf "Error011 %s" err  
                         let exnSql = sprintf "Došlo k následující chybě na serveru: %s" err
                         (app exnSql) >> run <| (new SqlConnection(String.Empty)) //dummy connection
        0