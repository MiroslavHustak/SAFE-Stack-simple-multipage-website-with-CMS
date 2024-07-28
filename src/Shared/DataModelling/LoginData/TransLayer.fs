namespace Shared

open System
open Shared

module SharedLoginValues =

    let internal transferLayer username password = 
        {
            //Option type for string not necessary here (string -> string or String.Empty, sufficient here)
            Username = SharedTypes.Username (string username)   
            Password = SharedTypes.Password (string password)   
        }