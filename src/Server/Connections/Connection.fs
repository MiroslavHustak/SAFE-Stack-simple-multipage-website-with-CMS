namespace Auxiliaries.Connections

open System
open System.IO
open System.Data.SqlClient

module Connection = 

    //Switch between the databases (always comment out the option you will not use)
   
    //nutricniterapie.somee.com
    //let [<Literal>] connStringSomee = @"" 

    //nterapie.somee.com //testing website
    let [<Literal>] internal connStringSomee = @"workstation id=nterapie.mssql.somee.com;packet size=4096;user id=FSharpDeveloper_SQLLogin_1;pwd=1791iyi6tf;data source=nterapie.mssql.somee.com;persist security info=False;initial catalog=nterapie" 

    //localhost
    //let [<Literal>] internal connStringLocal = @"Data Source=Misa\SQLEXPRESS;Initial Catalog=nterapieLocal;Integrated Security=True"

    let internal getConnection () =
        let connection = new SqlConnection(connStringSomee)
        connection.Open()
        connection
    
    let internal closeConnection (connection: SqlConnection) =
        connection.Close()
        connection.Dispose()

