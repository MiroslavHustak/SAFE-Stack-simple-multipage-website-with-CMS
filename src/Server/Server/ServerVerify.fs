namespace Server

open System
open System.IO

open Saturn

open Fable.Remoting.Server
open Fable.Remoting.Giraffe

open Shared
open SharedTypes

open ErrorTypes.Server
open PatternBuilders.Server.PatternBuilders

open Auxiliaries.Server.Security2
open Auxiliaries.Server.ROP_Functions

module ServerVerify =

    let private strContainsOnlySpace str = str |> Seq.forall (fun item -> item = (char)32)  //A string is a sequence of characters => use Seq.forall to test directly //(char)32 = space

    let private isValidLogin inputUsrString inputPswString = not (strContainsOnlySpace inputUsrString || strContainsOnlySpace inputPswString)

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
            new StreamWriter(Path.GetFullPath("uberHash.txt")) 
            |> Option.ofObj
            |> function
                | Some value -> value
                | None       -> //Check the "uberHash.txt" file manually 
                                new StreamWriter(Path.GetFullPath("")) 
        mySeq |> Seq.iter (fun item -> do sw.WriteLine(item)) 
    //************************************************************************

    let internal verifyLogin (login: LoginInfo) =   // LoginInfo -> Async<LoginResult>>

        MyPatternBuilder    
            {
                let rc1 = { SharedApi.LoginProblems.line1 = "Závažná chyba na serveru !!!"; SharedApi.LoginProblems.line2 = "Chybí soubor pro ověření uživatelského jména a hesla" }
                let rc2 = { SharedApi.LoginProblems.line1 = "Závažná chyba na serveru !!!"; SharedApi.LoginProblems.line2 = "Problém s ověřením uživatelského jména a hesla" }
                let rc3 = { SharedApi.LoginProblems.line1 = "Buď uživatelské jméno anebo heslo je neplatné."; SharedApi.LoginProblems.line2 = "Prosím zadej údaje znovu." }  

                let usr = login.Username |> function SharedApi.Username value -> value //unwrapping SCDU
                let psw = login.Password |> function SharedApi.Password value -> value

                let uberHash x =
                    //File.Exists will not throw an exception, but GetFullPath will do so
                    match File.Exists(Path.GetFullPath("uberHash.txt")) with
                    | false -> Seq.empty //No need of any action, Seq.empty will do its job here                                
                    | true  ->                              
                               match File.ReadAllLines("uberHash.txt") |> Option.ofObj with //StreamReader refused to work here, that is why File.ReadAllLines was used 
                               | Some value -> value |> Seq.ofArray 
                               | None       -> Seq.empty //No need of any action, Seq.empty will do its job here
             
                let uberHash = (uberHash, (fun x -> ()), String.Empty) |||> tryWith |> deconstructor0 

                let! _ = (<>) uberHash Seq.empty, SharedApi.UsernameOrPasswordIncorrect rc1
                
                let! _ = isValidLogin usr psw, SharedApi.UsernameOrPasswordIncorrect rc3

                let verify1 x = verify (uberHash |> Seq.head) usr
                let verify1 = (verify1, (fun x -> ()), String.Empty) |||> tryWith |> deconstructor4

                let! _ = (<>) verify1 Exception, SharedApi.UsernameOrPasswordIncorrect rc2 

                let verify2 x = verify (uberHash |> Seq.last) psw
                let verify2 = (verify2, (fun x -> ()), String.Empty) |||> tryWith |> deconstructor4

                let! _ = (<>) verify2 Exception, SharedApi.UsernameOrPasswordIncorrect rc2
                let! _ = (&&) (verify1 = LegitimateTrue) (verify2 = LegitimateTrue), SharedApi.UsernameOrPasswordIncorrect rc3 
                                                                        
                return SharedApi.LoggedIn { Username = login.Username } //{ Username = login.Username; AccessToken = SharedApi.AccessToken accessToken }
            }
    
      //TODO validation upon request from the user 
    let verifyCenikValues (cenikValues: CenikValuesDomain) =
        match isValidCenik () with
        | ()  -> Success ()        
        //| _ -> Failure ()

     //TODO validation upon request from the user 
    let verifyKontaktValues (kontaktValues: KontaktValuesDomain) =
       match isValidKontakt () with
       | ()  -> Success ()        
       //| _ -> Failure ()

    //TODO validation upon request from the user 
    let verifyLinkAndLinkNameValues (linkValues: LinkAndLinkNameValuesDomain) =
       match isValidLink () with
       | ()  -> Success ()        
       //| _ -> Failure ()

 