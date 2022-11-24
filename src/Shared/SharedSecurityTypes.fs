module SharedSecurityTypes

open System

type AuthToken = SecurityToken of string

type SecureRequest<'t> =
    {
        Token: string
        Body: 't
    }

type NecoNevimCo =
    {
        Neco: string
    }

type AuthError =
    | TokenInvalid
    | UserUnauthorized

type SecureResponse<'t> = Async<Result<'t, AuthError>> 

let routes typeName methodName = 
 sprintf "/api/%s/%s" typeName methodName
 
type IBlogSecurityApi = 
    {
       publishNewPost : SecureRequest<NecoNevimCo> -> SecureResponse<NecoNevimCo> 
    }