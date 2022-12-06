/// Provides functions for encoding and decoding Json web tokens
module Security

//zatim nevyuzivano //TODO -> implement the code
//kod prevzat z vyukovych stranek SAFE Stack

open System
open System.IO
open System.Text
open Newtonsoft.Json
open System.Security.Cryptography
open SharedTypes
open PasswordGenerator

open System.Data.SqlClient

let [<Literal>] connStringSomee = @"workstation id=nterapie.mssql.somee.com;packet size=4096;user id=FSharpDeveloper_SQLLogin_1;pwd=1791iyi6tf;data source=nterapie.mssql.somee.com;persist security info=False;initial catalog=nterapie" 
let [<Literal>] connStringLocal = @"Data Source=Misa\SQLEXPRESS;Initial Catalog=nterapieLocal;Integrated Security=True"

let neco() =
    //**************** SqlQueries *****************
    let queryExists = "SELECT Id FROM CENIK WHERE EXISTS (SELECT Id FROM CENIK WHERE CENIK.Id = 1)"
    let queryInsert = "
                      INSERT INTO CENIK (Id,
                                        [ValueState],
                                        [CenikValuesV001],
                                        [CenikValuesV002],
                                        [CenikValuesV003],
                                        [CenikValuesV004],
                                        [CenikValuesV005],
                                        [CenikValuesV006],
                                        [CenikValuesV007],
                                        [CenikValuesV008],
                                        [CenikValuesV009]
                                        )
                      VALUES (@valId, @valState, @val01, @val02, @val03, @val04, @val05, @val06, @val07, @val08, @val09) 
                      "   
  
    //**************** Parameters for command.Parameters.AddWithValue("@val", nejake hodnota) *****************
    let fixedParamList = [
                            ("@valState", "fixed"); ("@val01", "300"); ("@val02", "300");
                            ("@val03", "2 200"); ("@val04", "250"); ("@val05", "230");
                            ("@val06", "400"); ("@val07", "600"); ("@val08", "450"); ("@val09", "450")
                         ]   

    //**************** SqlConnectionOpen *****************
    use connection = new SqlConnection(connStringLocal)
    connection.Open()

    //**************** SqlCommands *****************
    use cmdExists = new SqlCommand(queryExists, connection)
    use cmdInsert = new SqlCommand(queryInsert, connection)

    //**************** Add values to parameters and execute commands with business logic *****************  
    match cmdExists.ExecuteScalar() |> Option.ofObj with
    | Some _ -> //connection.Close()
                //connection.Dispose()
                ()
    | None   ->                                 
                cmdInsert.Parameters.AddWithValue("@valId", 2) |> ignore
                fixedParamList |> List.iter (fun item -> cmdInsert.Parameters.AddWithValue(item) |> ignore)
                cmdInsert.ExecuteNonQuery() |> ignore
                //connection.Close()
                //connection.Dispose()

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




