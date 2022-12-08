module Sql

open System
open System.Data.SqlClient

open Connection
open SharedTypes
open Microsoft.FSharp.Quotations

//**************** Sql query strings *****************
let private queryCreateDatabase = "CREATE DATABASE nterapieLocal" //nelze pro connStringSomee
let private queryDeleteAll = "DELETE FROM CENIK" //Deleting all data from the relevant table
let private queryDelete id = sprintf "%s%s" "DELETE FROM CENIK WHERE Id = " id 
let private queryExists id = sprintf "%s%s%s" "SELECT Id FROM CENIK WHERE EXISTS (SELECT Id FROM CENIK WHERE CENIK.Id = " id ")"
let private querySelect id = sprintf "%s%s" "SELECT * FROM CENIK WHERE Id = " id

let private queryUpdate id = sprintf "%s%s" "UPDATE CENIK
                                             SET ValueState = @valState, CenikValuesV001 = @val01, CenikValuesV002 = @val02,
                                                 CenikValuesV003 = @val03, CenikValuesV004 = @val04, CenikValuesV005 = @val05,
                                                 CenikValuesV006 = @val06, CenikValuesV007 = @val07, CenikValuesV008 = @val08, CenikValuesV009 = @val09
                                             WHERE Id = " id

let private queryCreateTable = "
    CREATE TABLE CENIK (
                       Id int NOT NULL PRIMARY KEY,
                       ValueState varchar(255),
                       CenikValuesV001 varchar(255),
                       CenikValuesV002 varchar(255),
                       CenikValuesV003 varchar(255),
                       CenikValuesV004 varchar(255),
                       CenikValuesV005 varchar(255),
                       CenikValuesV006 varchar(255),
                       CenikValuesV007 varchar(255),
                       CenikValuesV008 varchar(255),
                       CenikValuesV009 varchar(255)   
                       )"  

let private queryInsert = "
    INSERT INTO CENIK (Id,
                      [ValueState],
                      [CenikValuesV001],
                      [CenikValuesV002],
                      [CenikValuesV003],
                      [CenikValuesV004],
                      [CenikValuesV005],
                      [CenikValuesV006],
                      [CenikValuesV007],
                      [CenikValuesV008],
                      [CenikValuesV009]
                      )
    VALUES (@valId, @valState, @val01, @val02, @val03, @val04, @val05, @val06, @val07, @val08, @val09)"

//**************** Sql commands - templates *****************
(*
use cmdExists = new SqlCommand(queryExists idString, connection)
use cmdInsert = new SqlCommand(queryInsert, connection)
use cmdUpdate = new SqlCommand(queryUpdate idString, connection)
use cmdDeleteAll = new SqlCommand(queryDeleteAll, connection)
use cmdDelete = new SqlCommand(queryDelete idString, connection) 
*)  

//**************** Sql queries - inner functions  *****************
let private insertOrUpdate connection valState idInt (getCenikValues: GetCenikValues) = //TODO trywith
     
    let idString = string idInt //idInt = Primary Key for new/old/fixed value state

    let val01 = getCenikValues.V001
    let val02 = getCenikValues.V002
    let val03 = getCenikValues.V003
    let val04 = getCenikValues.V004
    let val05 = getCenikValues.V005
    let val06 = getCenikValues.V006
    let val07 = getCenikValues.V007
    let val08 = getCenikValues.V008
    let val09 = getCenikValues.V009
       
    //**************** Parameters for command.Parameters.AddWithValue("@val", nejake hodnota) *****************
    let newParamList = [
                           ("@valState", valState); ("@val01", val01); ("@val02", val02);
                           ("@val03", val03); ("@val04", val04); ("@val05", val05);
                           ("@val06", val06); ("@val07", val07); ("@val08", val08); ("@val09", val09)
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

let private selectValues connection idInt =

    let whatIs(x: obj) =
        match x with
        | :? string as str -> str  //aby nedoslo k nerizene chybe behem runtime
        | _                -> //error4 "error 4 - x :?> string"   //TODO                           
                              x :?> string
    let result = 
        let idString = string idInt

        //**************** SqlCommands *****************
        use cmdExists = new SqlCommand(queryExists idString, connection)
        use cmdSelect = new SqlCommand(querySelect idString, connection)

        //**************** Add values to parameters and execute commands with business logic *****************
        let reader =
            let e = "error"
            match cmdExists.ExecuteScalar() |> Option.ofObj with //cmdExists.ExecuteScalar() |> Option.ofObj
            | Some _ -> cmdSelect.ExecuteReader() 
            | None   -> //insertOrUpdateOld e e e e e e e e e e //TODO
                        cmdSelect.ExecuteReader()

        //seq { while reader.Read() do yield { //filling in a record } } |> Seq.head 
        Seq.initInfinite (fun _ -> reader.Read())
        |> Seq.takeWhile ((=) true) 
        |> Seq.collect (fun _ ->  
                                seq
                                    {
                                    yield    
                                        {
                                            V001 = whatIs reader.["CenikValuesV001"]
                                            V002 = whatIs reader.["CenikValuesV002"]
                                            V003 = whatIs reader.["CenikValuesV003"]
                                            V004 = whatIs reader.["CenikValuesV004"]
                                            V005 = whatIs reader.["CenikValuesV005"]
                                            V006 = whatIs reader.["CenikValuesV006"]
                                            V007 = whatIs reader.["CenikValuesV007"]
                                            V008 = whatIs reader.["CenikValuesV008"]
                                            V009 = whatIs reader.["CenikValuesV009"]
                                        }
                                    } 
                        ) |> Seq.head
    connection.Close()
    connection.Dispose()
    result

//**************** Sql queries - executions *****************
  //TODO  vsecko try with
let insertOrUpdateFixed () = 
    let connection = new SqlConnection(connStringLocal) 
    connection.Open()
    let valState = "fixed"
    let idInt = 1 //Primary Key for fixed value state   
    insertOrUpdate connection valState idInt GetCenikValues.Default
    connection.Close()
    connection.Dispose()

let insertOrUpdateNew newDbCenikValues = 
    let connection = new SqlConnection(connStringLocal) 
    connection.Open()
    let valState = "new"
    let idInt = 2 //Primary Key for new value state
    insertOrUpdate connection valState idInt newDbCenikValues    
    connection.Close()
    connection.Dispose()

let insertOrUpdateOld newDbCenikValues = //new vales transpiled into old values
    let connection = new SqlConnection(connStringLocal) 
    connection.Open()
    let valState = "old"
    let idInt = 3 //Primary Key for old value state
    insertOrUpdate connection valState idInt newDbCenikValues
    connection.Close()
    connection.Dispose()

let selectDeserValues () =
    let connection = new SqlConnection(connStringLocal) 
    connection.Open()
    let idInt = 2 //Primary Key for new value state
    selectValues connection idInt   

let selectNewValues () =
    let connection = new SqlConnection(connStringLocal) 
    connection.Open()    
    let idInt = 2 //Primary Key for new value state
    selectValues connection idInt //TODO  try with

let selectOldValues () =
    let connection = new SqlConnection(connStringLocal) 
    connection.Open()       
    let idInt = 3 //Primary Key for old value state
    selectValues connection idInt 