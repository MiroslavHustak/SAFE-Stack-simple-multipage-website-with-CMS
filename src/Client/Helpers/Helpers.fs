namespace Helpers.Client

open Shared

open System
    
    module Helper =

        let internal compare p1 p2 p3 = 

            match (p1, p2, p3) with
            | (a, b, c) when a = b -> (a, String.Empty, c)
            | (a, b, c) when a = c -> (a, String.Empty, String.Empty)
            | (a, b, c) when b = c -> (a, b, String.Empty)
            | (a, b, c)            -> (a, b, c)

        let internal removeSpaces (input: string) =

            let c = (char)32         

            match input.[0] with
            | x when x = c -> input.[1..]        
            | _            -> input                                               

        //let internal strContainsOnlySpace str = str |> Seq.forall (fun item -> item = (char)32) //for educational purposes, might come in handy...

        let internal javaScriptMessageBox errorMsg =

            errorMsg
            |> Option.ofNullEmptySpace
            |> function
                | Some value -> Browser.Dom.window.alert(errorMsg)
                | None       -> ()

              