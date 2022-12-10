module Dapper

open System

open Dapper
open FSharp.Control
open Dapper.FSharp.MSSQL
open System.Data.SqlClient


open SQLQueries
open Connection
open SharedTypes

//Dapper.FSharp
let private table = table'<GetCenikValues> "CENIK"

//**************** Sql queries - inner functions  *****************
let private insertOrUpdate (connection: SqlConnection) (getCenikValues: GetCenikValues) = //TODO trywith

    let idInt = getCenikValues.Id //idInt = Primary Key for new/old/fixed value state
               
    //**************** SqlCommands *****************    
    //plain Dapper
    let cmdExistsDapper = connection.ExecuteScalar(queryExists(string idInt))

    //Dapper.FSharp
    let cmdInsert() = 
        insert
            {
                into table
                value getCenikValues
            } |> connection.InsertAsync

    //Dapper.FSharp        
    let cmdUpdate() = 
        update
            {
                for p in table do
                    set getCenikValues
                    where (p.Id = getCenikValues.Id)
            } |> connection.UpdateAsync   

    //**************** Execute commands with business logic *****************
    match cmdExistsDapper |> Option.ofObj with
    | Some _ -> cmdUpdate().Wait() |> ignore                      
    | None   -> cmdInsert().Wait() |> ignore             
    
let private selectValues (connection: SqlConnection) idInt =

    //**************** SqlCommands ***************** 
    //plain Dapper
    let cmdExistsDapper = connection.ExecuteScalar(queryExists(string idInt))
    
    let reader =
        //Dapper.FSharp
        let cmdSelect() =
            select
                {
                    for p in table do                            
                        where (p.Id = idInt)
                } |> connection.SelectAsync<GetCenikValues>       

        //**************** Execute commands with business logic (read values from DB) *****************
        //match cmdExists.ExecuteScalar() |> Option.ofObj with
        match cmdExistsDapper |> Option.ofObj with 
        | Some _ -> cmdSelect() 
        | None   -> insertOrUpdate connection GetCenikValues.Default
                    cmdSelect() 
    reader.Wait()

    match reader.IsCompletedSuccessfully with
    | true  -> reader.Result |> Seq.head             
    | false -> GetCenikValues.Default
              

//**************** Sql queries - executions *****************
//TODO  vsecko try with

let insertOrUpdateUniversal dbCenikValues=
    use connection = new SqlConnection(connStringLocal) 
    connection.Open()  
    insertOrUpdate connection dbCenikValues

let selectValuesUniversal idInt =
    use connection = new SqlConnection(connStringLocal) 
    connection.Open()
    selectValues connection idInt

 






