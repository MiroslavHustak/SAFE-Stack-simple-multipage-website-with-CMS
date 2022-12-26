namespace Auxiliaries

open System
open System.IO
open System.Data.SqlClient

module Connection = 

    //Choose between the databases (always comment out the option you will not use)
   
    //nutricniterapie.somee.com
    //let [<Literal>] connStringSomee = @"" 

    //nterapie.somee.com //testing website
    let [<Literal>] connStringSomee = @"workstation id=nterapie.mssql.somee.com;packet size=4096;user id=FSharpDeveloper_SQLLogin_1;pwd=1791iyi6tf;data source=nterapie.mssql.somee.com;persist security info=False;initial catalog=nterapie" 

    //localhost
    //let [<Literal>] connStringLocal = @"Data Source=Misa\SQLEXPRESS;Initial Catalog=nterapieLocal;Integrated Security=True"

