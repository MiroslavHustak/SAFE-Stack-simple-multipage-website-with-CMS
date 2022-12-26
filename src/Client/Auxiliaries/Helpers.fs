namespace Auxiliaries.Client

open System
    
    module SpaceChecker =

        let strContainsOnlySpace str = 
            str |> Seq.forall (fun item -> item = (char)32) 

        let javaScriptMessage errorMsg = 

            match not (strContainsOnlySpace errorMsg || errorMsg = String.Empty) with
                   | true  -> Browser.Dom.window.alert(errorMsg)
                   | false -> ()
              
       
       
          



