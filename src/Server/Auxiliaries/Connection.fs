module Connection
open System
open System.IO
open System.Data.SqlClient

let [<Literal>] connStringSomee = @"workstation id=nterapie.mssql.somee.com;packet size=4096;user id=FSharpDeveloper_SQLLogin_1;pwd=1791iyi6tf;data source=nterapie.mssql.somee.com;persist security info=False;initial catalog=nterapie" 
let [<Literal>] connStringLocal = @"Data Source=Misa\SQLEXPRESS;Initial Catalog=nterapieLocal;Integrated Security=True"
