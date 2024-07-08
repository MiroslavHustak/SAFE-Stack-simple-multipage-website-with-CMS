namespace Connections

open System
open System.IO
open System.Data.SqlClient

module Connection = 

    //Switch between the databases (always comment out the option you will not use)
   
    //nutricniterapie.somee.com
    //let [<Literal>] connStringSomee = @"workstation id= ..........." 

    //nterapie.somee.com //testing website
    //let [<Literal>] internal connStringSomee = @"" 

    //localhost
    let [<Literal>] internal connStringLocal = @"Data Source=Misa\SQLEXPRESS;Initial Catalog=nterapieLocal;Integrated Security=True"

    //shall be in a tryWith block
    let internal getConnection () =        
        let connection = new SqlConnection(connStringLocal)
        //let connection = new SqlConnection(connStringSomee)
        connection.Open()
        connection
    
    let internal closeConnection (connection: SqlConnection) =
        connection.Close()
        connection.Dispose()