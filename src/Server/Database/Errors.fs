namespace Database.Errors

open System

open Shared

open ErrorTypes.Server
open Connections.Connection
open Helpers.Server.CEBuilders
open TransLayerSend.Server.TransLayerSend

module Errors =

    let internal insertDefaultValues insertOrUpdate =     
        
        let cenikValuesDtoSendDefault = cenikValuesTransferLayerToStorage SharedCenikValues.cenikValuesDomainDefault

        match insertOrUpdate getConnection closeConnection cenikValuesDtoSendDefault with
        | Ok _    -> InsertOrUpdateError1
        | Error _ -> InsertOrUpdateError2

    let internal errorMsgBoxIU insertOrUpdate cond =

        //just having fun with active patterns... :-)
        let (|Cond1|Cond2|Cond3|) value =
        
            pyramidOfHell    
                {    
                    let! _ = (<>) value NoError, Cond1
                    let! _ = (=) value NoError, Cond2
                    
                    return Cond3
                }
    
        let err =
            match insertOrUpdate with
            | Ok _    -> NoError
            | Error _ -> InsertOrUpdateError    
                
        match err with
        | Cond2 when cond = true  -> FirstRunError 
        | Cond2 when cond = false -> InsertOrUpdateError
        | Cond1                   -> NoError
        | Cond3 | _               -> err

        |> function 
            | FirstRunError         -> "Byly dosazeny defaultní nebo předchozí hodnoty, neb došlo k chybě při načítání hodnot z databáze."
            | InsertOrUpdateError   -> "Zadané hodnoty nebyly nebo nebudou uloženy, neb došlo k chybě při načítání hodnot z databáze." 
            | NoError               -> String.Empty
            | InsertConnectionError -> "Chyba připojení k databázi. Zadané hodnoty nebyly nebo nebudou do databáze uloženy."

    let internal errorMsgBoxS =
        
        function
        | InsertOrUpdateError1 -> SharedCenikValues.cenikValuesDomainDefault, "Byly dosazeny defaultní nebo předchozí hodnoty, neb došlo k chybě při ověřování existující databáze."
        | InsertOrUpdateError2 -> SharedCenikValues.cenikValuesDomainDefault, "Došlo k chybě při načítání hodnot z databáze a dosazování defaultních hodnot. Zobrazované hodnoty mohou být chybné."
        | ReadingDbError       -> SharedCenikValues.cenikValuesDomainDefault, "Chyba při načítání hodnot z databáze. Dosazeny defaultní hodnoty místo chybných hodnot."
        | ConnectionError      -> SharedCenikValues.cenikValuesDomainDefault, "Chyba připojení k databázi. Dosazeny defaultní hodnoty místo chybných hodnot."