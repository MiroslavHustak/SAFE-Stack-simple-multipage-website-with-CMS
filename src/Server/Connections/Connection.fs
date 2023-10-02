namespace Auxiliaries.Connections

open System
open System.IO
open System.Data.SqlClient

module Connection = 

    //Switch between the databases (always comment out the option you will not use)
   
    //nutricniterapie.somee.com
    //let [<Literal>] connStringSomee = @"" 

    //nterapie.somee.com //testing website
    let [<Literal>] connStringSomee = @"*************************" 

    //localhost
    //let [<Literal>] connStringLocal = @"***********************"

    let getConnection () =
        let connection = new SqlConnection(connStringSomee)
        connection.Open()
        connection
    
    let closeConnection (connection: SqlConnection) =
        connection.Close()
        connection.Dispose()

