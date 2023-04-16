namespace DbAccess

open System
open System.Data.SqlClient

open SharedTypes
open Queries.SqlQueries
open DiscriminatedUnions.Server

open Auxiliaries.Server.DapperHelper
open Auxiliaries.Server.ROP_Functions
open Auxiliaries.Connections.Connection
open PatternBuilders.Server.PatternBuilders

//SQL type providers did not work in this app, they block the database

module Sql =
      
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
    let insertOrUpdate (getCenikValues: GetCenikValues) =
    
        let insertOrUpdateNow x =

            //failwith "Simulated exception SqlInsertOrUpdate" 

            use connection = new SqlConnection(connStringSomee) //choose between LocalHost and Somee
            connection.Open()  

            let idInt = getCenikValues.Id //idInt = Primary Key for new/old/fixed value state
            let valState = getCenikValues.ValueState
            let idString = string idInt        
       
            //**************** Parameters for command.Parameters.AddWithValue("@val", some value) *****************
            let newParamList =
                [
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
                        cmdUpdate.ExecuteNonQuery() |> ignore           
            | None   -> 
                        cmdInsert.Parameters.AddWithValue("@valId", idInt) |> ignore
                        newParamList |> List.iter (fun item -> cmdInsert.Parameters.AddWithValue(item) |> ignore)
                        cmdInsert.ExecuteNonQuery() |> ignore

        let exnSql = (insertOrUpdateNow, (fun x -> ()), "ErrorSql1") |||> tryWith |> deconstructor1
            
        //just testing active patterns... :-)
        let (|Cond1|Cond2|Cond3|) (value:string) =
    
            MyPatternBuilder    
                {    
                    let! _ = (<>) value String.Empty, Cond1
                    let! _ = (=) value String.Empty, Cond2                          
                    return Cond3
                }       

        let cond4 = getCenikValues.Msgs.Msg1 = "First run" 

        match exnSql with
        | Cond2 when cond4 = true  -> sprintf"%s %s" "Byly dosazeny defaultní nebo předchozí hodnoty, neb došlo k této chybě:" exnSql
        | Cond2 when cond4 = false -> sprintf"%s %s" "Zadané hodnoty nebyly nebo nebudou uloženy, neb došlo k této chybě:" exnSql
        | Cond1                    -> String.Empty 
        | Cond3 | _                -> exnSql   
      
        (*
        match getCenikValues.Msgs.Msg1 = "First run" with
        | true when exnSql <> String.Empty        -> sprintf"%s %s" "Byly dosazeny defaultní nebo předchozí hodnoty, neb došlo k této chybě:" exnSql
        | true | false when exnSql = String.Empty -> String.Empty 
        | false when exnSql <> String.Empty       -> sprintf"%s %s" "Zadané hodnoty nebyly nebo nebudou uloženy, neb došlo k této chybě:" exnSql
        | _                                       -> exnSql        
        *) 
        
    let selectValues idInt =

        let myErrMsg  = [| String.Empty; String.Empty |]
        let myErrType = [| Default; Default |]
    
        let selectValuesNow x = 

            //failwith "Simulated exception SqlSelectValues"

            use connection = new SqlConnection(connStringSomee) //Tohle uzavre ve vhodne dobe a navic je mozne toto mit v try with bloku. Vzpomen si, co to delalo pri samostatnem connection.Close() 
            connection.Open()
                       
            let getValues =

                let idString = string idInt

                //**************** SqlCommands *****************
                use cmdExists = new SqlCommand(queryExists idString, connection)
                use cmdSelect = new SqlCommand(querySelect idString, connection)

                //**************** Read values from DB *****************
                use reader =            
                    match cmdExists.ExecuteScalar() |> Option.ofObj with 
                    | Some _ -> cmdSelect.ExecuteReader()
                    | None   ->
                                let exnSql1 = insertOrUpdate GetCenikValues.Default
                                Array.set myErrMsg 0 exnSql1 //Error message can only be caught here, not further down the code
                                match myErrMsg |> Array.head with
                                | "" -> Array.set myErrType 0 NotFullDb 
                                | _  -> Array.set myErrType 0 OtherProblems                             
                                cmdSelect.ExecuteReader()

                let myRecord =       

                    (* ^a -> statically resolved generic type parameter *)

                    let inline whatIsG (x: obj) = //downcast
                        match x with
                        | :? ^a as gen -> Some gen 
                        | _            -> None                        
                                           
                    let extractValue fn defaultValue =
                        match fn with     
                        | Some value -> value
                        | None       ->
                                        Array.set myErrMsg 1 "Chyba při načítání hodnot z databáze. Dosazeny defaultní hodnoty místo chybných hodnot."
                                        Array.set myErrType 1 ProblemsWithReader 
                                        defaultValue                                                           

                    Seq.initInfinite (fun _ -> reader.Read())
                    |> Seq.takeWhile ((=) true) 
                    |> Seq.collect (fun _ ->  
                                            seq
                                                {
                                                yield    
                                                    {                                                       
                                                        Id = int (extractValue (whatIsG reader.["Id"]) GetCenikValues.Default.Id) 
                                                        ValueState = string (extractValue (whatIsG reader.["ValueState"]) GetCenikValues.Default.ValueState) 
                                                        V001 = string (extractValue (whatIsG reader.["V001"]) GetCenikValues.Default.V001) 
                                                        V002 = string (extractValue (whatIsG reader.["V002"]) GetCenikValues.Default.V002)
                                                        V003 = string (extractValue (whatIsG reader.["V003"]) GetCenikValues.Default.V003)
                                                        V004 = string (extractValue (whatIsG reader.["V004"]) GetCenikValues.Default.V004)
                                                        V005 = string (extractValue (whatIsG reader.["V005"]) GetCenikValues.Default.V005)
                                                        V006 = string (extractValue (whatIsG reader.["V006"]) GetCenikValues.Default.V006)
                                                        V007 = string (extractValue (whatIsG reader.["V007"]) GetCenikValues.Default.V007)
                                                        V008 = string (extractValue (whatIsG reader.["V008"]) GetCenikValues.Default.V008)
                                                        V009 = string (extractValue (whatIsG reader.["V009"]) GetCenikValues.Default.V009)
                                                        Msgs = Messages.Default
                                                    }
                                                } 
                                   ) |> Seq.head //the function only places data to the head of the collection (a function with "while" does the same)                       
                myRecord        
            getValues
 
        let (getValues, exnSql2) = (selectValuesNow, (fun x -> ()), "ErrorSql2") |||> tryWith |> deconstructor2 GetCenikValues.Default
        let exnSql =
            match myErrType |> Array.head with
            | NotFullDb -> //Not fully filled db, but InsetUpdate ended with success => ignoring exceptions on condition there are no problems with data reading
                           match myErrType |> Array.last with
                           | ProblemsWithReader -> sprintf "%s %s %s" exnSql2 (myErrMsg |> Array.head) (myErrMsg |> Array.last)
                           | OtherProblems      -> sprintf "%s %s %s" exnSql2 (myErrMsg |> Array.head) (myErrMsg |> Array.last)
                           | _                  -> String.Empty   
            | _         -> sprintf "%s %s %s" exnSql2 (myErrMsg |> Array.head) (myErrMsg |> Array.last)
        getValues, exnSql