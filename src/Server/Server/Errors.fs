namespace Server

module Errors =

    let internal tryWithResult f1 f2 err =
        try
            try          
                f1 ()
            finally
                f2 
        with
        | ex -> Error <| err ex.Message

    let internal tryWithResult1 f1 f2 f3 =
        try
            try          
                f1 ()
            finally
                f2 
        with
        | ex -> f3 ex.Message

    let internal tryWithVerify f2 err f1 =
        try
            try          
                f1 
            finally
                f2 
        with
        | _ -> err

 

    
