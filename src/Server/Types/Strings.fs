namespace Strings.Server

module Strings = 

    open System

    let internal (|StringNonN|) s = 
        s 
        |> Option.ofObj 
        |> function 
            | Some value -> string value
            | None       -> String.Empty