namespace Auxiliaries.Client

open System
    
    module Helper =

        let compare p1 p2 p3 =

            match (p1, p2, p3) with
            | (a, b, c) when a = b -> (a, String.Empty, c)
            | (a, b, c) when a = c -> (a, String.Empty, String.Empty)
            | (a, b, c) when b = c -> (a, b, String.Empty)
            | (a, b, c)            -> (a, b, c)

        let removeSpaces (input: string) =

            let c = string <| (char)32
            let d = sprintf "%s%s" c c 

            match input with
            | x when x = c -> String.Empty
            | x when x = d -> String.Empty
            | _            ->
                            match String.length input, input.[0] with
                            | length, c when length >= 2 && input.[1] = c -> input.[2..]
                            | length, c when length >= 1                  -> input.[1..]
                            | _                                           -> input                                 

        let strContainsOnlySpace str = str |> Seq.forall (fun item -> item = (char)32) 

        let javaScriptMessageBox errorMsg =

            match not (strContainsOnlySpace errorMsg || errorMsg = String.Empty) with
            | true  -> Browser.Dom.window.alert(errorMsg)
            | false -> ()
              