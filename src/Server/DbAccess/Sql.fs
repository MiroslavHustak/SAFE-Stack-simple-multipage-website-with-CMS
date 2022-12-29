namespace DbAccess

open System
open System.Data.SqlClient

open SharedTypes
open Queries.SqlQueries
open DiscriminatedUnions.Server
open Auxiliaries.Server.Connection
open Auxiliaries.Server.ROP_Functions

//SQL type providers did not work in this app

module Sql = 

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
    let insertOrUpdate (getCenikValues: GetCenikValues) =
    
        let insertOrUpdateNow x =

            //failwith "Simulated exception SqlInsertOrUpdate" 

            use connection = new SqlConnection(connStringSomee) //choice between Local and Somee
            connection.Open()  

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
            | None   -> 
                        cmdInsert.Parameters.AddWithValue("@valId", idInt) |> ignore
                        newParamList |> List.iter (fun item -> cmdInsert.Parameters.AddWithValue(item) |> ignore)
                        cmdInsert.ExecuteNonQuery() |> ignore

        let exnSql = (insertOrUpdateNow, (fun x -> ()), "ErrorSql1") |||> tryWith |> deconstructor1

        match getCenikValues.Msgs.Msg1 = "First run" with
        | true when exnSql <> String.Empty  -> sprintf"%s %s" "Byly dosazeny defaultní nebo předchozí hodnoty, neb došlo k této chybě:" exnSql
        | true when exnSql = String.Empty   -> String.Empty 
        | false when exnSql <> String.Empty -> sprintf"%s %s" "Zadané hodnoty nebyly nebo nebudou uloženy, neb došlo k této chybě:" exnSql
        | false when exnSql = String.Empty  -> String.Empty
        | _                                 -> exnSql        

    let selectValues idInt =

        let myErrMsg  = [| String.Empty; String.Empty |]
        let myErrType = [| Default; Default |]
    
        let selectValuesNow x = 

            //failwith "Simulated exception SqlSelectValues"

            use connection = new SqlConnection(connStringSomee) 
            connection.Open()  

            let whatIs (x: obj) =
                match x with
                | :? string as s -> s 
                | _              -> "error"        

            let whatIsInt (x: obj) =
                match x with
                | :? int as i ->  i 
                | _           -> -1 //Id cannot be -1 
                              
            let getValues =

                let idString = string idInt

                //**************** SqlCommands *****************
                use cmdExists = new SqlCommand(queryExists idString, connection)
                use cmdSelect = new SqlCommand(querySelect idString, connection)

                //**************** Read values from DB *****************
                let reader =            
                    match cmdExists.ExecuteScalar() |> Option.ofObj with 
                    | Some _ -> cmdSelect.ExecuteReader()
                    | None   -> let exnSql1 = insertOrUpdate GetCenikValues.Default
                                Array.set myErrMsg 0 exnSql1 //Error message can only be caught here, not further down the code
                                match myErrMsg |> Array.head with
                                | "" -> Array.set myErrType 0 NotFullDb 
                                | _  -> Array.set myErrType 0 OtherProblems                             
                                cmdSelect.ExecuteReader()

                let myRecord =                 
                    Seq.initInfinite (fun _ -> reader.Read())
                    |> Seq.takeWhile ((=) true) 
                    |> Seq.collect (fun _ ->  
                                            seq
                                                {
                                                yield    
                                                    {
                                                        Id = whatIsInt reader.["Id"]
                                                        ValueState = whatIs reader.["ValueState"]
                                                        V001 = whatIs reader.["V001"]
                                                        V002 = whatIs reader.["V002"]
                                                        V003 = whatIs reader.["V003"]
                                                        V004 = whatIs reader.["V004"]
                                                        V005 = whatIs reader.["V005"]
                                                        V006 = whatIs reader.["V006"]
                                                        V007 = whatIs reader.["V007"]
                                                        V008 = whatIs reader.["V008"]
                                                        V009 = whatIs reader.["V009"]
                                                        Msgs = Messages.Default
                                                    }
                                                } 
                                   ) |> Seq.head //the function only places data to the head of the collection (a function with while does the same)
                                                                                      
                let mySeq = seq { string myRecord.Id; myRecord.ValueState; myRecord.V001; myRecord.V002; myRecord.V003; myRecord.V004; myRecord.V005; myRecord.V006; myRecord.V007; myRecord.V008; myRecord.V009 }
                       
                match mySeq |> Seq.contains "error" || mySeq |> Seq.contains "-1" with
                | false -> myRecord                         
                | true  -> Array.set myErrMsg 1 "Chyba při načítání hodnot z databáze. Dosazeny defaultní hodnoty."
                           Array.set myErrType 1 ProblemsWithReader 
                           GetCenikValues.Default            
            getValues
 
        let (getValues, exnSql2) = (selectValuesNow, (fun x -> ()), "ErrorSql2") |||> tryWith |> deconstructor2 GetCenikValues.Default
        let exnSql =
            match myErrType |> Array.head with
            | NotFullDb -> //Not fully filled db, but InsetUpdate ended with success => ignoring exceptions on condition there are no problems with data reading
                           match myErrType |> Array.last with
                           | ProblemsWithReader -> sprintf "%s %s %s" exnSql2 (myErrMsg |> Array.head) (myErrMsg |> Array.last)
                           | OtherProblems      -> sprintf "%s %s %s" exnSql2 (myErrMsg |> Array.head) (myErrMsg |> Array.last)
                           | _                  -> String.Empty   
            | _         -> sprintf "%s %s %s" exnSql2 (myErrMsg |> Array.head) (myErrMsg |> Array.last)
        getValues, exnSql