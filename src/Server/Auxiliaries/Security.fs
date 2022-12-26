/// Provides functions for encoding and decoding Json web tokens

namespace Auxiliaries.Server

open System
open System.IO
open System.Text
open Newtonsoft.Json
open PasswordGenerator
open System.Security.Cryptography

    module Security = 

        //not in use  //TODO implement it if possible
        //This code taken from the SAFE Stack learning pages 

        //  Learn about JWT https://jwt.io/introduction/
        //  This module uses the JOSE-JWT library https://github.com/dvsekhvalnov/jose-jwt
        let private createRandomKey() =
            let generator = System.Security.Cryptography.RandomNumberGenerator.Create()
            let randomKey = Array.init 32 byte
            generator.GetBytes(randomKey)
            randomKey

        let tokenGenerator() = 
            //https://www.nuget.org/packages/PasswordGenerator/
            let pwd = new Password(includeLowercase = false, includeUppercase = false, includeNumeric = true, includeSpecial = false, passwordLength = 1024)
            pwd.Next()    

        /// A pass phrase you create only once and save to a file on the server
        /// The next time the server runs, the pass phrase is read and used
        let private passPhrase =
            let securityPassFile = FileInfo("securityPassFile.txt")
            if not securityPassFile.Exists then 
                let passPhrase = createRandomKey()
                File.WriteAllBytes(securityPassFile.FullName, passPhrase)
            File.ReadAllBytes(securityPassFile.FullName)

        let private encodeString (payload : string) =
            Jose.JWT.Encode(payload, passPhrase, Jose.JweAlgorithm.A256KW, Jose.JweEncryption.A256CBC_HS512)
        let private decodeString (jwt : string) =
            Jose.JWT.Decode(jwt, passPhrase, Jose.JweAlgorithm.A256KW, Jose.JweEncryption.A256CBC_HS512)

        /// Encodes an object as a JSON web token.
        let encodeJwt token = JsonConvert.SerializeObject token |> encodeString 

        /// Decodes a JSON web token to an object.
        let private decodeJwt<'a> (jwt : string) : 'a = decodeString jwt |> JsonConvert.DeserializeObject<'a>

        /// Returns true if the JSON Web Token is successfully decoded and the signature is verified.
        let validateJwt (jwt : string) : string option =
            try 
                let token = decodeJwt jwt
                Some token
            with _ -> None




