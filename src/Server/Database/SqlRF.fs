namespace Database

open System
open System.Data.SqlClient

open FsToolkit.ErrorHandling

open SharedTypes
open Queries.SqlQueries
open DiscriminatedUnions.Server

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
    let insertOrUpdate getConnection closeConnection getCenikValues =

        try
            //failwith "Simulated exception SqlInsertOrUpdate"
                
            //use connection = new SqlConnection(connStringSomee) //choose between LocalHost and Somee
            //connection.Open()
            let connection = getConnection()

            try
                let idInt = getCenikValues.Id //idInt = Primary Key for new/old/fixed value state
                let valState = getCenikValues.ValueState
                let idString = string idInt        
       
                //**************** Parameters for command.Parameters.AddWithValue("@val", some value) *****************
                let newParamList =
                    [
                        ("@valState", valState); ("@val01", getCenikValues.V001); ("@val02", getCenikValues.V002);
                        ("@val03", getCenikValues.V003); ("@val04", getCenikValues.V004); ("@val05", getCenikValues.V005);
                        ("@val06", getCenikValues.V006); ("@val07", getCenikValues.V007); ("@val08", getCenikValues.V008); ("@val09", getCenikValues.V009)
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
                       
                let getValues: CenikValues*SelectErrorOptions =

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
                                            match insertOrUpdate getConnection closeConnection CenikValues.Default with
                                            | Ok _    -> Error InsertOrUpdateError1
                                            | Error _ -> Error InsertOrUpdateError2                                                                  
                       
                            match reader with
                            | Ok reader ->                                     
                                        let inline downCast (x: obj) = // ^a -> statically resolved generic type parameter 
                                            match x with
                                            | :? ^a as value -> Some value 
                                            | _              -> None

                                        let extractValue fn defaultValue =
                                            match fn with     
                                            | Some value -> value, false
                                            | None       -> defaultValue, true           
                                                                                                     
                                        let getValues =                                                
                                            Seq.initInfinite (fun _ -> reader.Read())
                                            |> Seq.takeWhile ((=) true) 
                                            |> Seq.collect (fun _ ->  
                                                                    seq
                                                                        {                                                                               
                                                                        yield    
                                                                            {                                                       
                                                                                IdRF = extractValue (downCast reader.["Id"]) CenikValues.Default.Id
                                                                                ValueStateRF = extractValue (downCast reader.["ValueState"]) CenikValues.Default.ValueState
                                                                                V001RF = extractValue (downCast reader.["V001"]) CenikValues.Default.V001                                                                                   
                                                                                V002RF = extractValue (downCast reader.["V002"]) CenikValues.Default.V002
                                                                                V003RF = extractValue (downCast reader.["V003"]) CenikValues.Default.V003
                                                                                V004RF = extractValue (downCast reader.["V004"]) CenikValues.Default.V004
                                                                                V005RF = extractValue (downCast reader.["V005"]) CenikValues.Default.V005
                                                                                V006RF = extractValue (downCast reader.["V006"]) CenikValues.Default.V006
                                                                                V007RF = extractValue (downCast reader.["V007"]) CenikValues.Default.V007
                                                                                V008RF = extractValue (downCast reader.["V008"]) CenikValues.Default.V008
                                                                                V009RF = extractValue (downCast reader.["V009"]) CenikValues.Default.V009
                                                                                MsgsRF = Messages.Default
                                                                            }
                                                                        } 
                                                            ) |> Seq.head //the function only places data to the head of the collection (a function with "while" does the same)
                                        reader.Close()
                                        reader.Dispose()

                                        let convertToRegularRc getValues =
                                            {
                                                Id = fst getValues.IdRF; ValueState = fst getValues.ValueStateRF;
                                                V001 = fst getValues.V001RF; V002 = fst getValues.V002RF; V003 = fst getValues.V003RF;
                                                V004 = fst getValues.V004RF; V005 = fst getValues.V005RF; V006 = fst getValues.V006RF;
                                                V007 = fst getValues.V007RF; V008 = fst getValues.V008RF; V009 = fst getValues.V009RF;
                                                Msgs = getValues.MsgsRF
                                            }
                                      
                                        let anySndTrue (rc: CenikValuesRF) =

                                            match rc with
                                            |
                                                {
                                                    IdRF = (_, flag1); ValueStateRF = (_, flag2);
                                                    V001RF = (_, flag3); V002RF = (_, flag4); V003RF = (_, flag5);
                                                    V004RF = (_, flag6); V005RF = (_, flag7); V006RF = (_, flag8);
                                                    V007RF = (_, flag9); V008RF = (_, flag10); V009RF = (_, flag11)
                                                }
                                                ->
                                                    flag1 || flag2 || flag3 || flag4 || flag5 ||
                                                    flag6 || flag7 || flag8 || flag9 ||
                                                    flag10 || flag11
                                                                                               
                                        match anySndTrue getValues with 
                                        | true  -> CenikValues.Default, ReadingDbError
                                        | false -> convertToRegularRc getValues, NoSelectError                                               
                                       
                            | Error du -> CenikValues.Default, du
                        finally
                            closeConnection connection                        
                    with
                    | _ -> CenikValues.Default, ReadingDbError 

                getValues

            with
            | _ -> CenikValues.Default, ConnectionError 
       
 
        


