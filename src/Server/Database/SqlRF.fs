namespace Database

open System
open System.Data.SqlClient

open FsToolkit.ErrorHandling

open SharedTypes
open Queries.SqlQueries
open DiscriminatedUnions.Server

open Auxiliaries.Server

open DtoGet.Server.DtoGet
open DtoSend.Server.DtoSend
open TransLayerGet.Server.TransLayerGet
open TransLayerSend.Server.TransLayerSend

//SQL type providers did not work in this app, they block the database
module SqlRF =
      
    //**************** Sql query strings *****************
    //See the file SQL Queries.fs

    //**************** Sql commands - templates *****************
    (*
    use cmdExists = new SqlCommand(queryExists idString, connection)
    use cmdInsert = new SqlCommand(queryInsert, connection)
    use cmdUpdate = new SqlCommand(queryUpdate idString, connection)
    use cmdDeleteAll = new SqlCommand(queryDeleteAll, connection)
    use cmdDelete = new SqlCommand(queryDelete idString, connection) 
    *)

    //**************** Sql queries - inner functions  *****************
    let insertOrUpdate getConnection closeConnection (sendCenikValues : CenikValuesDtoSend) =

        try
            //failwith "Simulated exception SqlInsertOrUpdate"
                
            //use connection = new SqlConnection(connStringSomee) //choose between LocalHost and Somee
            //connection.Open()
            let connection = getConnection()

            try
                let idInt = sendCenikValues.Id //idInt = Primary Key for new/old/fixed value state
                let valState = sendCenikValues.ValueState
                let idString = string idInt        
       
                //**************** Parameters for command.Parameters.AddWithValue("@val", some value) *****************
                let newParamList =
                    [
                        ("@valState", valState); ("@val01", sendCenikValues.V001); ("@val02", sendCenikValues.V002);
                        ("@val03", sendCenikValues.V003); ("@val04", sendCenikValues.V004); ("@val05", sendCenikValues.V005);
                        ("@val06", sendCenikValues.V006); ("@val07", sendCenikValues.V007); ("@val08", sendCenikValues.V008); ("@val09", sendCenikValues.V009)
                    ]       

                //**************** SqlCommands *****************
                use cmdExists = new SqlCommand(queryExists idString, connection)
                use cmdInsert = new SqlCommand(queryInsert, connection)
                use cmdUpdate = new SqlCommand(queryUpdate idString, connection)

                //**************** Add values to parameters and execute commands with business logic *****************
                match cmdExists.ExecuteScalar() |> Option.ofObj with
                | Some _ -> 
                            newParamList |> List.iter (fun item -> cmdUpdate.Parameters.AddWithValue(item) |> ignore) 
                            cmdUpdate.ExecuteNonQuery() |> ignore
                            Ok () 
                | None   -> 
                            cmdInsert.Parameters.AddWithValue("@valId", idInt) |> ignore
                            newParamList |> List.iter (fun item -> cmdInsert.Parameters.AddWithValue(item) |> ignore)
                            cmdInsert.ExecuteNonQuery() |> ignore
                            Ok ()
            finally
                closeConnection connection
        with
        | _ -> Error InsertOrUpdateError //uzivatel nepotrebuje znat detailni popis chyby       
            
    let selectValues getConnection closeConnection idInt =
           
            try
                //failwith "Simulated exception SqlSelectValues"
                //use connection = new SqlConnection(connStringSomee) //Tohle uzavre ve vhodne dobe a navic je mozne toto mit v try with bloku. Vzpomen si, co to delalo pri samostatnem connection.Close() 
                //connection.Open()
                let connection = getConnection()
                       
                let getValues: CenikValuesDomain*SelectErrorOptions =

                    try
                        try
                            let idString = string idInt
                            //failwith "Simulated exception SqlSelectValues" 

                            //**************** SqlCommands *****************
                            use cmdExists = new SqlCommand(queryExists idString, connection)
                            use cmdSelect = new SqlCommand(querySelect idString, connection)

                            //**************** Read values from DB *****************
                            let reader =            
                                match cmdExists.ExecuteScalar() |> Option.ofObj with 
                                | Some _ -> Ok <| cmdSelect.ExecuteReader()
                                | None   ->
                                            let cenikValuesDtoSendDefault = cenikValuesTransferLayerSend CenikValuesDomain.Default
                                            match insertOrUpdate getConnection closeConnection cenikValuesDtoSendDefault with
                                            | Ok _    -> Error InsertOrUpdateError1
                                            | Error _ -> Error InsertOrUpdateError2                                                                  
                       
                            match reader with
                            | Ok reader ->   
                                        let extractValue fn defaultValue =
                                            match fn with     
                                            | Some value -> value, false
                                            | None       -> defaultValue, true                                      
                                      
                                        let getValues =                                                
                                            Seq.initInfinite (fun _ -> reader.Read())
                                            |> Seq.takeWhile ((=) true)  //compare |> Seq.skipWhile ((=) false)
                                            |> Seq.collect (fun _ ->  
                                                                    seq
                                                                        {                                                                               
                                                                        yield    
                                                                            {                                                       
                                                                                IdDtoGet = extractValue (Casting.downCast reader.["Id"]) CenikValuesDomain.Default.Id
                                                                                ValueStateDtoGet = extractValue (Casting.downCast reader.["ValueState"]) CenikValuesDomain.Default.ValueState
                                                                                V001DtoGet = extractValue (Casting.downCast reader.["V001"]) CenikValuesDomain.Default.V001                                                                                   
                                                                                V002DtoGet = extractValue (Casting.downCast reader.["V002"]) CenikValuesDomain.Default.V002
                                                                                V003DtoGet = extractValue (Casting.downCast reader.["V003"]) CenikValuesDomain.Default.V003
                                                                                V004DtoGet = extractValue (Casting.downCast reader.["V004"]) CenikValuesDomain.Default.V004
                                                                                V005DtoGet = extractValue (Casting.downCast reader.["V005"]) CenikValuesDomain.Default.V005
                                                                                V006DtoGet = extractValue (Casting.downCast reader.["V006"]) CenikValuesDomain.Default.V006
                                                                                V007DtoGet = extractValue (Casting.downCast reader.["V007"]) CenikValuesDomain.Default.V007
                                                                                V008DtoGet = extractValue (Casting.downCast reader.["V008"]) CenikValuesDomain.Default.V008
                                                                                V009DtoGet = extractValue (Casting.downCast reader.["V009"]) CenikValuesDomain.Default.V009
                                                                                MsgsDtoGet = MessagesDtoGet.Default
                                                                            }
                                                                        } 
                                                            ) |> Seq.head //the function only places data to the head of the collection (a function with "while" does the same)
                                        reader.Close()
                                        reader.Dispose()
                                       
                                        let anySndTrue (rc: CenikValuesDtoGet) =

                                            match rc with
                                            |
                                                {
                                                    IdDtoGet = (_, flag1); ValueStateDtoGet = (_, flag2);
                                                    V001DtoGet = (_, flag3); V002DtoGet = (_, flag4); V003DtoGet = (_, flag5);
                                                    V004DtoGet = (_, flag6); V005DtoGet = (_, flag7); V006DtoGet = (_, flag8);
                                                    V007DtoGet = (_, flag9); V008DtoGet = (_, flag10); V009DtoGet = (_, flag11)
                                                }
                                                ->
                                                    flag1 || flag2 || flag3 || flag4 || flag5 ||
                                                    flag6 || flag7 || flag8 || flag9 ||
                                                    flag10 || flag11
                                                                                               
                                        match anySndTrue getValues with 
                                        | true  -> CenikValuesDomain.Default, ReadingDbError
                                        | false -> cenikValuesTransferLayerGet getValues, NoSelectError                                               
                                       
                            | Error du -> CenikValuesDomain.Default, du
                        finally
                            closeConnection connection                        
                    with
                    | _ -> CenikValuesDomain.Default, ReadingDbError 

                getValues

            with
            | _ -> CenikValuesDomain.Default, ConnectionError 
       
 
        


