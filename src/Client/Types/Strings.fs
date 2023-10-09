namespace Strings.Client

open FsToolkit.ErrorHandling

module Strings = 

    open System

    let (|StringNonN|) s = 
        s 
        |> Option.ofNull 
        |> function 
            | Some value -> string value
            | None       -> String.Empty