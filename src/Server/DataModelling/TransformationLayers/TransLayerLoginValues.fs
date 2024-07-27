namespace TransLayerLoginValues.Server

open System

open Shared

module TransLayerLoginValues =

    let internal loginValuesTransferLayer (login: SharedTypes.LoginValuesShared) =

        let usr = login.Username |> function SharedTypes.Username value -> value //unwrapping SCDU
        let psw = login.Password |> function SharedTypes.Password value -> value
        usr, psw
       