namespace Shared

open System
open Shared

module SharedLoginValues =

    let transferLayer username password = 
        {
            SharedTypes.Username = SharedTypes.Username username   
            SharedTypes.Password = SharedTypes.Password password   
        }