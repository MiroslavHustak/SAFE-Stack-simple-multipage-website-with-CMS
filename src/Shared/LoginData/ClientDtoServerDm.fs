namespace Shared

open System

//https://stackoverflow.com/questions/59738472/struct-attribute-on-discriminated-unions

// = LoginValuesServerDto 
// = LoginValuesClientDomainModel 
[<Struct>]
type LoginValuesShared =
    {
        Username: SharedTypes.Username
        Password: SharedTypes.Password
    }

// TODO
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
        Username: SharedTypes.Username
        //AccessToken: AccessToken
    }         
                 
//https://stackoverflow.com/questions/59738472/struct-attribute-on-discriminated-unions
[<Struct>]
type LoginResult =
    | UsernameOrPasswordIncorrect of UsernameOrPasswordIncorrect: LoginErrorMsgShared
    | LoggedIn of LoggedIn: User

