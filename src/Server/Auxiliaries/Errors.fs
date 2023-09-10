namespace Auxiliaries.Errors

open System
open DiscriminatedUnions.Server
open PatternBuilders.Server.PatternBuilders

module Errors =

    let errorMsgBox insertOrUpdate cond =
    
        let du =
            match insertOrUpdate with
            | Ok _    -> NoInsertError
            | Error _ -> InsertOrUpdateError
                
        //just testing active patterns... :-)
        let (|Cond1|Cond2|Cond3|) value =
        
            MyPatternBuilder    
                {    
                    let! _ = (<>) value NoInsertError, Cond1
                    let! _ = (=) value NoInsertError, Cond2                          
                    return Cond3
                } 
            
        match du with
        | Cond2 when cond = true  -> FirstRunError 
        | Cond2 when cond = false -> InsertOrUpdateError
        | Cond1                   -> NoInsertError
        | Cond3 | _               -> du
    
   

