namespace TransLayerLoginValues.Server

open System

open Shared
open LoginValuesDomain.Server.LoginValuesDomain

module TransLayerLoginValues =

    let internal loginValuesTransferLayer (login: LoginValuesShared) =
        {
            username =
                login.Username |> function SharedTypes.Username value -> value 
            password =
                login.Password |> function SharedTypes.Password value -> value
        }

        
     