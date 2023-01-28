namespace SharedTypes

open System

[<RequireQualifiedAccess>]
module SharedApi = 

    //[<Struct>]
    //type AccessToken = AccessToken of string

    [<Struct>]
    type LoginProblems =
        {
            line1: string
            line2: string
        }    

    [<Struct>]
    type User =
        {
            Username : string
            //AccessToken : AccessToken
        }         
                 
    //https://stackoverflow.com/questions/59738472/struct-attribute-on-discriminated-unions
    [<Struct>]
    type LoginResult =
        | UsernameOrPasswordIncorrect of UsernameOrPasswordIncorrect: LoginProblems
        | LoggedIn of LoggedIn: User
