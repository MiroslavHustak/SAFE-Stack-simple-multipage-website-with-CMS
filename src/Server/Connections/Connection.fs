namespace Connections

open System
open System.IO
open System.Data.SqlClient

open Logging.Logging

module Connection = 

    //Switch between the databases (always comment out the option you will not use)
   
    //nutricniterapie.somee.com
    let [<Literal>] private connStringSomee = @"workstation id= ......"

    //nterapie.somee.com //testing website
    //let [<Literal>] internal connStringSomee = @"" 

    //localhost
    let [<Literal>] private connStringLocal = @"Data Source=Misa\SQLEXPRESS;Initial Catalog=nterapieLocal;Integrated Security=True"

    let internal getAsyncConnection () =

        async
            {
                try          
                    let connection = new SqlConnection(connStringLocal)
                    //let connection = new SqlConnection(connStringSomee)
                    do! connection.OpenAsync() |> Async.AwaitTask

                    return Ok connection   
                with 
                | ex ->
                     logInfoMsg <| sprintf "Error020W %s" (string ex.Message)
                     return Error <| string ex.Message
            }          

    let internal closeAsyncConnection (connection: Async<Result<SqlConnection, string>>) =  

        async
            {
                match! connection with
                | Ok connection
                    ->
                    try
                        try
                            do! connection.CloseAsync() |> Async.AwaitTask
                            return Ok ()

                        finally
                            async { return! connection.DisposeAsync().AsTask() |> Async.AwaitTask } |> Async.StartImmediate                      
                    with 
                    | ex -> return Error <| string ex.Message

                | Error err
                    ->
                    logInfoMsg <| sprintf "Error020W %s" err
                    return Error err
            }

    //***************** Sync variant **********************

    //shall be in a tryWith block
    let internal getConnection () =
        
        let connection = new SqlConnection(connStringLocal)
        //let connection = new SqlConnection(connStringSomee)
        connection.Open()
        connection
    
    let internal closeConnection (connection: SqlConnection) =

        connection.Close()
        connection.Dispose()

