namespace Database

open System
open System.Data.SqlClient

open FsToolkit.ErrorHandling

open SharedTypes
open Queries.SqlQueries
open DiscriminatedUnions.Server

open Auxiliaries.Server.ROP_Functions
open Auxiliaries.Connections.Connection
open PatternBuilders.Server.PatternBuilders

//SQL type providers did not work in this app, they block the database

module SqlRF =
      
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
    let insertOrUpdate getCenikValues =
              
        let insertOrUpdate: Result<unit, string> =

            try

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
                            Ok () 
                | None   -> 
                            cmdInsert.Parameters.AddWithValue("@valId", idInt) |> ignore
                            newParamList |> List.iter (fun item -> cmdInsert.Parameters.AddWithValue(item) |> ignore)
                            cmdInsert.ExecuteNonQuery() |> ignore
                            Ok () 
            with
            | _ -> Error String.Empty //uzivatel nepotrebuje znat popis chyby

        (*
        let errorMsgBox =

            let s =
                match insertOrUpdate with
                | Ok _     -> String.Empty
                | Error ex -> ex
            
            //just testing active patterns... :-)
            let (|Cond1|Cond2|Cond3|) (value:string) =
    
                MyPatternBuilder    
                    {    
                        let! _ = (<>) value String.Empty, Cond1
                        let! _ = (=) value String.Empty, Cond2                          
                        return Cond3
                    }       

            let cond4 = getCenikValues.Msgs.Msg1 = "First run"       
        
            match s with
            | Cond2 when cond4 = true  -> sprintf"%s %s" "Byly dosazeny defaultní nebo předchozí hodnoty, neb došlo k této chybě:" s
            | Cond2 when cond4 = false -> sprintf"%s %s" "Zadané hodnoty nebyly nebo nebudou uloženy, neb došlo k této chybě:" s
            | Cond1                    -> String.Empty 
            | Cond3 | _                -> s

        errorMsgBox
        *)
        insertOrUpdate
            
    let selectValues idInt =
           
            try
                //failwith "Simulated exception SqlSelectValues"
                use connection = new SqlConnection(connStringSomee) //Tohle uzavre ve vhodne dobe a navic je mozne toto mit v try with bloku. Vzpomen si, co to delalo pri samostatnem connection.Close() 
                connection.Open()
                       
                let getValues: GetCenikValues*SelectErrorOptions =

                    try
                        let idString = string idInt
                        failwith "Simulated exception SqlSelectValues" 

                        //**************** SqlCommands *****************
                        use cmdExists = new SqlCommand(queryExists idString, connection)
                        use cmdSelect = new SqlCommand(querySelect idString, connection)

                        //**************** Read values from DB *****************
                        let reader =            
                            match cmdExists.ExecuteScalar() |> Option.ofObj with 
                            | Some _ -> Ok <| cmdSelect.ExecuteReader()
                            | None   ->
                                        let exnSql = insertOrUpdate GetCenikValues.Default
                                        match exnSql.Equals(String.Empty) with
                                        | true  -> Error InsertOrUpdateError1 //"Byly dosazeny defaultní nebo předchozí hodnoty, neb došlo k chybě při načítání hodnot z databáze." 
                                        | false -> Error InsertOrUpdateError2   //"Došlo k chybě při načítání hodnot z databáze a dosazování defaultních hodnot. Zobrazované hodnoty mohou být chybné."                             
                       
                        match reader with
                        | Ok reader ->                                     
                                    let inline downCast (x: obj) = // ^a -> statically resolved generic type parameter 
                                        match x with
                                        | :? ^a as value -> Some value 
                                        | _              -> None

                                    let extractValue fn defaultValue =
                                        match fn with     
                                        | Some value -> value, false
                                        | None       -> defaultValue, true           
                                                                                                             
                                    let getValues =                                                
                                        Seq.initInfinite (fun _ -> reader.Read())
                                        |> Seq.takeWhile ((=) true) 
                                        |> Seq.collect (fun _ ->  
                                                                seq
                                                                    {                                                                               
                                                                    yield    
                                                                        {                                                       
                                                                            IdRF = extractValue (downCast reader.["Id"]) GetCenikValues.Default.Id
                                                                            ValueStateRF = extractValue (downCast reader.["ValueState"]) GetCenikValues.Default.ValueState
                                                                            V001RF = extractValue (downCast reader.["V001"]) GetCenikValues.Default.V001                                                                                   
                                                                            V002RF = extractValue (downCast reader.["V002"]) GetCenikValues.Default.V002
                                                                            V003RF = extractValue (downCast reader.["V003"]) GetCenikValues.Default.V003
                                                                            V004RF = extractValue (downCast reader.["V004"]) GetCenikValues.Default.V004
                                                                            V005RF = extractValue (downCast reader.["V005"]) GetCenikValues.Default.V005
                                                                            V006RF = extractValue (downCast reader.["V006"]) GetCenikValues.Default.V006
                                                                            V007RF = extractValue (downCast reader.["V007"]) GetCenikValues.Default.V007
                                                                            V008RF = extractValue (downCast reader.["V008"]) GetCenikValues.Default.V008
                                                                            V009RF = extractValue (downCast reader.["V009"]) GetCenikValues.Default.V009
                                                                            MsgsRF = Messages.Default
                                                                        }
                                                                    } 
                                                        ) |> Seq.head //the function only places data to the head of the collection (a function with "while" does the same)
                                    reader.Close()
                                    reader.Dispose()

                                    let convertToRegularRc getValues : GetCenikValues =

                                        {
                                            Id = fst getValues.IdRF; ValueState = fst getValues.ValueStateRF;
                                            V001 = fst getValues.V001RF; V002 = fst getValues.V002RF; V003 = fst getValues.V003RF;
                                            V004 = fst getValues.V004RF; V005 = fst getValues.V005RF; V006 = fst getValues.V006RF;
                                            V007 = fst getValues.V007RF; V008 = fst getValues.V008RF; V009 = fst getValues.V009RF;
                                            Msgs = getValues.MsgsRF
                                        }
                                      
                                    let anySndTrue (rc: GetCenikValuesRF) =

                                        match rc with
                                        |
                                            {
                                                IdRF = (_, flag1); ValueStateRF = (_, flag2);
                                                V001RF = (_, flag3); V002RF = (_, flag4); V003RF = (_, flag5);
                                                V004RF = (_, flag6); V005RF = (_, flag7); V006RF = (_, flag8);
                                                V007RF = (_, flag9); V008RF = (_, flag10); V009RF = (_, flag11)
                                            }
                                            ->
                                                flag1 || flag2 || flag3 || flag4 || flag5 ||
                                                flag6 || flag7 || flag8 || flag9 ||
                                                flag10 || flag11
                                                                                               
                                    match anySndTrue getValues with //TODO cemu su chybove hlasky 2x v redakcnim systemu?
                                    | true  -> GetCenikValues.Default, ReadingDbError
                                    | false -> convertToRegularRc getValues, NoSelectError                                               
                                       
                        | Error du  -> GetCenikValues.Default, du
                                    //match InsertOrUpdateError with th                               
                                    //| NotFullDb    -> GetCenikValues.Default, "NotFullDb. Dosazeny defaultní hodnoty místo chybných hodnot."
                                    //| SelectError -> GetCenikValues.Default, "UnknownError. Hmm, neco zvlastniho se stane."
                                      
                    with
                    | _ -> GetCenikValues.Default, ReadingDbError //"Reading db problem. Dosazeny defaultní hodnoty místo chybných hodnot."

                getValues

            with
            | _ -> GetCenikValues.Default, ConnectionError //"Connection problem. Dosazeny defaultní hodnoty místo chybných hodnot."

       
 
        


