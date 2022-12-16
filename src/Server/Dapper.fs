module Dapper

open System

open Dapper
open FSharp.Control
open Dapper.FSharp.MSSQL
open System.Data.SqlClient

open SqlQueries
open Connection
open SharedTypes

//Dapper.FSharp
let private table = table'<GetCenikValues> "CENIK"

//**************** Sql queries - inner functions  *****************
let insertOrUpdate (getCenikValues: GetCenikValues) = //TODO trywith

    use connection = new SqlConnection(connStringLocal) 
    connection.Open()  

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
    | Some _ -> cmdUpdate().GetAwaiter().GetResult() |> ignore                      
    | None   -> cmdInsert().GetAwaiter().GetResult() |> ignore             
    
let selectValues idInt =

    use connection = new SqlConnection(connStringLocal) 
    connection.Open()

    //**************** SqlCommands ***************** 
    //plain Dapper
    let cmdExistsDapper = connection.ExecuteScalar(queryExists(string idInt))

     //Dapper.FSharp
    let cmdSelect() =       
        task
            {
                let! values =
                    select
                        {
                            for p in table do                            
                                where (p.Id = idInt)
                        } |> connection.SelectAsync<GetCenikValues> //Dapper.FSharp creators rendered the resulting type as a task
                        
                match values |> Option.ofObj with
                | Some values -> return Seq.head values
                | None        -> return GetCenikValues.Default  
            }

    //**************** Execute commands with business logic *****************
    match cmdExistsDapper |> Option.ofObj with 
    | Some _ -> cmdSelect().GetAwaiter().GetResult() //Gets to be done here, but in cases where not, transfer the task up the code
    | None   -> insertOrUpdate GetCenikValues.Default
                cmdSelect().GetAwaiter().GetResult() //Gets to be done here, but in cases where not, transfer the task up the code

    (*
      Result and RunSynchronously block the thread pool
      CPU is idle waiting for them to return
      *************************************************
      Using task expressions is preferred when interoperating extensively with .NET libraries that create or consume .NET tasks.
    *)