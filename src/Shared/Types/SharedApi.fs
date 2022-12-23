[<RequireQualifiedAccess>]
module SharedApi

[<Struct>]
type AccessToken = AccessToken of string

[<Struct>]
type User =
    {
        Username : string
        AccessToken : AccessToken
    }

[<Struct>]
type LoginResult =
    | UsernameOrPasswordIncorrect
    | LoggedIn of User
