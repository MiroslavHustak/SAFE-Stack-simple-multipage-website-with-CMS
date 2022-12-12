[<RequireQualifiedAccess>]
module SharedApi

type AccessToken = AccessToken of string

type User =
    { Username : string
      AccessToken : AccessToken }

type LoginResult =
    | UsernameOrPasswordIncorrect
    | LoggedIn of User
