namespace Auxiliaries.Errors

open System
open System.Data.SqlClient

open FsToolkit.ErrorHandling

open SharedTypes

open DiscriminatedUnions.Server

open PatternBuilders.Server.PatternBuilders

module Errors =

    let errorMsgBox insertOrUpdate cond =
    
        let du =
            match insertOrUpdate with
            | Ok _     -> NoInsertError
            | Error _ -> InsertOrUpdateError
                
        //just testing active patterns... :-)
        let (|Cond1|Cond2|Cond3|) value =
        
            MyPatternBuilder    
                {    
                    let! _ = (<>) value NoInsertError, Cond1
                    let! _ = (=) value NoInsertError, Cond2                          
                    return Cond3
                }       
    
        //let cond = getCenikValues.Msgs.Msg1 = "First run"       
            
        match du with
        | Cond2 when cond = true  -> FirstRunError //sprintf"%s %s" "Byly dosazeny defaultní nebo předchozí hodnoty, neb došlo k této chybě:" s
        | Cond2 when cond = false -> InsertOrUpdateError//sprintf"%s %s" "Zadané hodnoty nebyly nebo nebudou uloženy, neb došlo k této chybě:" s
        | Cond1                   -> NoInsertError
        | Cond3 | _               -> du
    
   

