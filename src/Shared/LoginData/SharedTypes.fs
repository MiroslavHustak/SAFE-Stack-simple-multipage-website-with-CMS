namespace Shared

open System

//https://stackoverflow.com/questions/59738472/struct-attribute-on-discriminated-unions

module SharedTypes = 

    //************** SCDUs for type-driven development (client + server) ***************

    //[<Struct>]
    //type AccessToken = AccessToken of string

    [<Struct>]
    type Username = Username of string //See Isaac Abraham page 272 onwards

    [<Struct>]
    type Password = Password of string