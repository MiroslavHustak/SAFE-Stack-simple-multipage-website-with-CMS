module Sql

open System
open System.Data.SqlClient

open SqlQueries
open Connection
open SharedTypes

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
let insertOrUpdate (getCenikValues: GetCenikValues) = //TODO trywith

    use connection = new SqlConnection(connStringLocal) 
    connection.Open()  

    let idInt = getCenikValues.Id //idInt = Primary Key for new/old/fixed value state
    let valState = getCenikValues.ValueState
    let idString = string idInt        
       
    //**************** Parameters for command.Parameters.AddWithValue("@val", nejaka hodnota) *****************
    let newParamList = [
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
                cmdUpdate.ExecuteNonQuery() |> ignore //for learning purposes              
    | None   -> 
                cmdInsert.Parameters.AddWithValue("@valId", idInt) |> ignore
                newParamList |> List.iter (fun item -> cmdInsert.Parameters.AddWithValue(item) |> ignore)
                cmdInsert.ExecuteNonQuery() |> ignore       

let selectValues idInt =

    use connection = new SqlConnection(connStringLocal) 
    connection.Open()  

    let whatIs (x: obj) =
        match x with
        | :? string as str -> str  //aby nedoslo k nerizene chybe behem runtime
        | _                -> //error4 "error 4 - x :?> string"   //TODO            
                              String.Empty //whatever of the string type

    let whatIsInt (x: obj) =
        match x with
        | :? int as i -> i 
        | _           -> //error4 "error 4 - x :?> int"   //TODO            
                         -1 //whatever of the int type
                              
    let getValues = 
        let idString = string idInt

        //**************** SqlCommands *****************
        use cmdExists = new SqlCommand(queryExists idString, connection)
        use cmdSelect = new SqlCommand(querySelect idString, connection)

        //**************** Read values from DB *****************
        let reader =            
            match cmdExists.ExecuteScalar() |> Option.ofObj with 
            | Some _ -> cmdSelect.ExecuteReader() 
            | None   -> insertOrUpdate GetCenikValues.Default 
                        cmdSelect.ExecuteReader() 

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
                                        }
                                    } 
                       ) |> Seq.head
    getValues