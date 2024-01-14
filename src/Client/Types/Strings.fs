namespace Strings.Client

open System
open FsToolkit.ErrorHandling

module Strings =

    let (|StringNonN|) s = //for educational purposes, not used yet
        s 
        |> Option.ofNull 
        |> function 
            | Some value -> string value
            | None       -> String.Empty