/// Proviedes functions for encoding and decoding Json web tokens
module Security

open System
open System.IO
open System.Text
open Newtonsoft.Json
open System.Security.Cryptography
open Shared
open PasswordGenerator


type AuthToken = SecurityToken of string

type SecureRequest<'t> = {
    Token : string
    Body : 't
}

type AuthError = 
    | TokenInvalid
    | UserUnauthorized 

type SecureResponse<'t> = Async<Result<'t, AuthError>> 

type UserInfo =
    { Username : string
      Claims : string [] }

//  Learn about JWT https://jwt.io/introduction/
//  This module uses the JOSE-JWT library https://github.com/dvsekhvalnov/jose-jwt
let private createRandomKey() =
    let generator = System.Security.Cryptography.RandomNumberGenerator.Create()
    let randomKey = Array.init 32 byte
    generator.GetBytes(randomKey)
    randomKey

/// A pass phrase you create only once and save to a file on the server
/// The next time the server runs, the pass phrase is read and used
let private passPhrase =
    //https://www.nuget.org/packages/PasswordGenerator/
    let pwd = new Password(includeLowercase = true, includeUppercase = true, includeNumeric = true, includeSpecial = true, passwordLength = 128)

    let securityToken = pwd.Next()

    if securityToken = String.Empty then 
        let passPhrase = createRandomKey()
        File.WriteAllBytes(securityToken, passPhrase)
    File.ReadAllBytes(securityToken)

let private encodeString (payload : string) =
    Jose.JWT.Encode(payload, passPhrase, Jose.JweAlgorithm.A256KW, Jose.JweEncryption.A256CBC_HS512)
let private decodeString (jwt : string) =
    Jose.JWT.Decode(jwt, passPhrase, Jose.JweAlgorithm.A256KW, Jose.JweEncryption.A256CBC_HS512)

/// Encodes an object as a JSON web token.
let encodeJwt token = JsonConvert.SerializeObject token |> encodeString //*************************************

/// Decodes a JSON web token to an object.
let private decodeJwt<'a> (jwt : string) : 'a = decodeString jwt |> JsonConvert.DeserializeObject<'a>

/// Returns true if the JSON Web Token is successfully decoded and the signature is verified.
let private validateJwt (jwt : string) : UserInfo option =
    try 
        let token = decodeJwt jwt
        Some token
    with _ -> None




