namespace Shared

open System

//https://stackoverflow.com/questions/59738472/struct-attribute-on-discriminated-unions

[<RequireQualifiedAccess>]
module SharedTypes = 

    //************** SCDUs for type-driven development (client + server) ***************

    //[<Struct>]
    //type AccessToken = AccessToken of string

    [<Struct>]
    type Username = Username of string //See Isaac Abraham page 272 onwards

    [<Struct>]
    type Password = Password of string

    // = LoginValuesServerDto 
    // = LoginValuesClientDomainModel 
    [<Struct>]
    type LoginValuesShared =
       {
           Username: Username
           Password: Password
       }

    // = LoginErrorMsgServerDomainModel 
    // = LoginErrorMsgClientDto
    [<Struct>]
    type LoginErrorMsgShared =
        {
            line1: string
            line2: string
        }    

    [<Struct>]
    type User =
        {
            Username: Username
            //AccessToken: AccessToken
        }         
                 
    //https://stackoverflow.com/questions/59738472/struct-attribute-on-discriminated-unions
    [<Struct>]
    type LoginResult =
        | UsernameOrPasswordIncorrect of UsernameOrPasswordIncorrect: LoginErrorMsgShared
        | LoggedIn of LoggedIn: User

