namespace Server

open System
open System.IO

open Saturn

open Fable.Remoting.Server
open Fable.Remoting.Giraffe

open Shared
open SharedTypes

open Errors
open ServerVerify

open Database.Select
open Database.InsertOrUpdate

open ErrorTypes.Server

open Auxiliaries.Errors.Errors
open Auxiliaries.Server.CopyingFiles
open Auxiliaries.Server.Serialisation
open Auxiliaries.Server.Deserialisation
open Auxiliaries.Connections.Connection

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
                                            
                                           { dbNewCenikValues with Msgs = { MessagesDomain.Default with Msg1 = exnSql } }
                                                                           
                                | Error _ ->
                                           CenikValuesDomain.Default

                          return sendNewCenikValues
                      }

            getOldCenikValues = 
                fun _ -> 
                    async
                        {                     
                            let IdNew = 2
                            let IdOld = 3

                            let (dbGetNewCenikValues, exnSql2) =                              
                                match selectValues getConnection closeConnection (insertOrUpdateError insertOrUpdate) IdNew with   
                                | Ok value  -> value, String.Empty                                            
                                | Error err -> errorMsgBoxS err

                            //********************************************************
                            let dbCenikValues = { dbGetNewCenikValues with Id = IdOld; ValueState = "old" }
                            let cond = dbCenikValues.Msgs.Msg1 = "First run"
                            let cenikValuesSend = cenikValuesTransferLayerSend dbCenikValues
                            let exnSql = errorMsgBoxIU (insertOrUpdate getConnection closeConnection cenikValuesSend) cond
                           
                            //********************************************************
                            let (dbSendOldCenikValues, exnSql3) =  
                                match selectValues getConnection closeConnection (insertOrUpdateError insertOrUpdate) IdOld with   
                                | Ok value  -> value, String.Empty                                            
                                | Error err -> errorMsgBoxS err

                            return { dbSendOldCenikValues with Msgs = { MessagesDomain.Default with Msg1 = exnSql; Msg2 = exnSql2; Msg3 = exnSql3 } }
                        }

            getDeserialisedCenikValues = 
               fun _ ->
                   async
                       {           
                           let IdNew = 2
                    
                           let (dbSendCenikValues, exnSql1) =                              
                               match selectValues getConnection closeConnection (insertOrUpdateError insertOrUpdate) IdNew with   
                               | Ok value  -> value, String.Empty                                            
                               | Error err -> errorMsgBoxS err
                                          
                           return { dbSendCenikValues with Msgs = { MessagesDomain.Default with Msg1 = exnSql1; Msg2 = errMsg } } 
                       }

            //************* testing XML ********************          
            sendKontaktValues =
                fun sendKontaktValues ->
                    async
                        {
                            let sendNewKontaktValues: KontaktValuesDomain = 
                                match verifyKontaktValues sendKontaktValues with
                                | Ok ()    ->   
                                            let exnJson =
                                                let f1 () = 
                                                    //failwith "Simulated exception10"
                                                    let sendKontaktValuesDtoXml = kontaktValuesTransferLayerDomainToXml sendKontaktValues
                                                    serializeToXml sendKontaktValuesDtoXml "xmlKontaktValues.xml"
                                                    String.Empty
                                                let f3 = sprintf"%s %s" "Zadané hodnoty nebyly uloženy, neb došlo k této chybě:"  
                                                tryWithResult1 f1 () f3
                                               
                                            { sendKontaktValues with Msgs = { MessagesDomain.Default with Msg1 = exnJson } }                                   
                                | Error _ ->
                                            KontaktValuesDomain.Default

                            return sendNewKontaktValues
                        }

            getOldKontaktValues =
                fun _ ->
                    async
                        {
                            let (getOldKontaktValues, exnJson) =
                                let f1 () = 
                                    copyFiles 
                                    <| "xmlKontaktValues.xml"
                                    <| "xmlKontaktValuesBackUp.xml"
                                    //failwith "Simulated exception12" 
                                    kontaktValuesTransferLayerXmlToDomain (deserializeFromXml<KontaktValuesDtoXml> "xmlKontaktValuesBackUp.xml"), String.Empty   
                                let f3 ex = KontaktValuesDomain.Default, sprintf"%s %s" "Pro zobrazování navrhovaných a předchozích hodnot kontaktů byly dosazeny defaultní hodnoty, neb došlo k této chybě:" ex 
                                tryWithResult1 f1 () f3      

                            return { getOldKontaktValues with Msgs = { MessagesDomain.Default with Msg1 = exnJson } }                 
                        } 

            getDeserialisedKontaktValues =
                fun _ ->
                    async
                        {                                              
                            let (getKontaktValues, exnJson1) =
                                let f1 () = 
                                   //failwith "Simulated exception13" 
                                    kontaktValuesTransferLayerXmlToDomain (deserializeFromXml<KontaktValuesDtoXml> "xmlKontaktValues.xml"), String.Empty 
                                let f3 ex = KontaktValuesDomain.Default, sprintf"%s %s" "Byly dosazeny defaultní hodnoty kontaktů, neb došlo k této chybě: " ex 
                                tryWithResult1 f1 () f3

                            return { getKontaktValues with Msgs = { MessagesDomain.Default with Msg1 = exnJson1 } }                   
                        }

            //************* testing JSON ********************                               
            sendLinkAndLinkNameValues =
               fun sendLinkAndLinkNameValues ->
                   async
                      {
                         let sendNewLinkAndLinkNameValues: LinkAndLinkNameValuesDomain = 
                             match verifyLinkAndLinkNameValues sendLinkAndLinkNameValues with
                             | Ok ()   ->  
                                        let exnJson =
                                            let f1 () = 
                                                //failwith "Simulated exception14"   
                                                let sendLinkAndLinkNameValuesDtoSend = linkAndLinkNameValuesTransferLayerSend sendLinkAndLinkNameValues
                                                serializeToJson sendLinkAndLinkNameValuesDtoSend "jsonLinkAndLinkNameValues.json"
                                                String.Empty
                                            let f3 = sprintf"%s %s" "Zadané hodnoty nebyly uloženy, neb došlo k této chybě:"  
                                            tryWithResult1 f1 () f3     
                                        { sendLinkAndLinkNameValues with Msgs = { MessagesDomain.Default with Msg1 = exnJson } }     
                             | Error _ ->
                                        LinkAndLinkNameValuesDomain.Default                        

                         return sendNewLinkAndLinkNameValues
                      }
           
            getOldLinkAndLinkNameValues =
                fun _ ->
                   async
                       {                         
                           let (getOldLinkAndLinkNameValues, exnJson) =
                               let f1 () = 
                                   copyFiles 
                                   <| "jsonLinkAndLinkNameValues.json"
                                   <| "jsonLinkAndLinkNameValuesBackUp.json"
                                   //failwith "Simulated exception15"                           
                                   linkAndLinkNameValuesTransferLayerGet (deserializeFromJson<LinkAndLinkNameValuesDtoGet> "jsonLinkAndLinkNameValuesBackUp.json"), String.Empty  
                               let f3 ex = LinkAndLinkNameValuesDomain.Default, sprintf"%s %s" "Pro zobrazování navrhovaných a předchozích hodnot kontaktů byly dosazeny defaultní hodnoty, neb došlo k této chybě:" ex 
                               tryWithResult1 f1 () f3                             

                           return { getOldLinkAndLinkNameValues with Msgs = { MessagesDomain.Default with Msg1 = exnJson } }  
                       } 

            getDeserialisedLinkAndLinkNameValues =
               fun _ ->
                   async
                       {

                           let (getLinkAndLinkNameValues, exnJson) =
                               let f1 () = 
                                   copyFiles 
                                   <| "jsonLinkAndLinkNameValues.json"
                                   <| "jsonLinkAndLinkNameValuesBackUp.json"
                                   //failwith "Simulated exception17"                           
                                   linkAndLinkNameValuesTransferLayerGet (deserializeFromJson<LinkAndLinkNameValuesDtoGet> "jsonLinkandLinkNameValues.json"), String.Empty  
                               let f3 ex = LinkAndLinkNameValuesDomain.Default, sprintf"%s %s" "Pro zobrazování navrhovaných a předchozích hodnot kontaktů byly dosazeny defaultní hodnoty, neb došlo k této chybě:" ex 
                               tryWithResult1 f1 () f3

                           return { getLinkAndLinkNameValues with Msgs = { MessagesDomain.Default with Msg1 = exnJson } }  
                       }

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
        }

    let webApp exnSql =
        Remoting.createApi ()
        |> Remoting.withRouteBuilder Route.builder
        |> Remoting.fromValue (IGetApi exnSql)
        |> Remoting.buildHttpHandler

    let app =
        //let exnSql = insertOrUpdate { GetCenikValues.Default with Msgs = { Messages.Default with Msg1 = "First run" } }
        let dbCenikValues = { CenikValuesDomain.Default with Msgs = { MessagesDomain.Default with Msg1 = "First run" } }
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