namespace Server

open System
open System.IO

open FsToolkit
open FsToolkit.ErrorHandling

open Saturn

open Fable.Remoting.Server
open Fable.Remoting.Giraffe

open Shared
open SharedTypes

open Errors
open ErrorTypes.Server

open Auxiliaries.Server
open Auxiliaries.Server.Resources
open Auxiliaries.Server.Security2
open Auxiliaries.Server.PatternBuilders

module ServerVerify =

    let private pathToUberHashTxt = pathToResources @"\uberHash.txt"
    
    let private strContainsOnlySpace str =
        str |> Seq.forall (fun item -> item = (char)32)  //A string is a sequence of characters => use Seq.forall to test directly //(char)32 = space

    let private isValidLogin inputUsrString inputPswString =
        not (strContainsOnlySpace inputUsrString || strContainsOnlySpace inputPswString)

    let private isValidCenik param = ()   //TODO validation upon request from the user 
    let private isValidKontakt param = () //TODO validation upon request from the user 
    let private isValidLink param = ()    //TODO validation upon request from the user

    //************************************************************************
    //TODO create a separate solution and include a try with block
    let private pswHash() = //to be used only once before bundling             
        
        let usr = uberHash "" //delete username before bundling
        let psw = uberHash "" //delete password before bundling
        let mySeq = seq { usr; psw }
        
        use sw =
            new StreamWriter(Path.GetFullPath(pathToUberHashTxt)) 
            |> Option.ofNull
            |> function
                | Some value -> value
                | None       -> //Check the "uberHash.txt" file manually 
                                new StreamWriter(Path.GetFullPath("")) 
        mySeq |> Seq.iter (fun item -> do sw.WriteLine(item)) 
    //************************************************************************

    let internal verifyLogin (login: LoginInfo) =   // LoginInfo -> Async<LoginResult>>

        let uberHashError uberHash credential seqFn =
            
            pyramidOfDoom  //nelze Builder1 (pyramidOfHell) a Result.isOk
                {
                    let! uberHash = uberHash |> Result.toOption, Exception
                    let! _ = not (uberHash |> Seq.isEmpty) |> Option.ofBool, Exception
                    let! _ = verify (uberHash |> seqFn) credential |> Option.ofBool, LegitimateFalse
                    
                    return LegitimateTrue
                }

            |> tryWithVerify () Exception                   

        pyramidOfHell  
            {
                let rc1 = { SharedApi.LoginProblems.line1 = "Závažná chyba na serveru !!!"; SharedApi.LoginProblems.line2 = "Chybí soubor pro ověření uživatelského jména a hesla" }
                let rc2 = { SharedApi.LoginProblems.line1 = "Závažná chyba na serveru !!!"; SharedApi.LoginProblems.line2 = "Problém s ověřením uživatelského jména a hesla" }
                let rc3 = { SharedApi.LoginProblems.line1 = "Buď uživatelské jméno anebo heslo je neplatné."; SharedApi.LoginProblems.line2 = "Prosím zadej údaje znovu." }  

                let usr = login.Username |> function SharedApi.Username value -> value //unwrapping SCDU
                let psw = login.Password |> function SharedApi.Password value -> value

                let uberHash =
                    let f1 () =

                        pyramidOfDoom
                            {
                                let! _ = File.Exists(Path.GetFullPath(pathToUberHashTxt)) |> Option.ofBool, Error String.Empty
                                let! value = File.ReadAllLines(pathToUberHashTxt) |> Option.ofNull, Error String.Empty

                                return Ok (value |> Seq.ofArray) 
                            }                       

                    tryWithResult f1 () (sprintf"%s")

                let! _ = uberHash |> Result.isOk, SharedApi.UsernameOrPasswordIncorrect rc1                
                let! _ = isValidLogin usr psw, SharedApi.UsernameOrPasswordIncorrect rc3

                let verify1 = uberHashError uberHash usr Seq.head 
                let! _ = (<>) verify1 Exception, SharedApi.UsernameOrPasswordIncorrect rc2

                let verify2 = uberHashError uberHash psw Seq.last 
                let! _ = (<>) verify2 Exception, SharedApi.UsernameOrPasswordIncorrect rc2
                let! _ = (&&) (verify1 = LegitimateTrue) (verify2 = LegitimateTrue), SharedApi.UsernameOrPasswordIncorrect rc3 
                                                                        
                return SharedApi.LoggedIn { Username = login.Username } //{ Username = login.Username; AccessToken = SharedApi.AccessToken accessToken }
            }
    
      //TODO validation upon request from the user 
    let verifyCenikValues (cenikValues: CenikValuesDomain) =
        match isValidCenik () with
        | ()  -> Ok ()        
        //| _ -> Error _

     //TODO validation upon request from the user 
    let verifyKontaktValues (kontaktValues: KontaktValuesDomain) =
       match isValidKontakt () with
       | ()  -> Ok ()        
       //| _ -> Error _

    //TODO validation upon request from the user 
    let verifyLinkAndLinkNameValues (linkValues: LinkAndLinkNameValuesDomain) =
       match isValidLink () with
       | ()  -> Ok ()        
       //| _ -> Error _

 