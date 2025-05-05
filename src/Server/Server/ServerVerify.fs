namespace Server

open System
open System.IO

open FsToolkit.ErrorHandling

//open Errors
//open Saturn
//open Fable.Remoting.Giraffe

open Shared
open Shared.SharedTypes

open Settings
open Logging.Logging
open ErrorTypes.Server

open Helpers.Server
open Helpers.Server.Security2
open Helpers.Server.CEBuilders
open Helpers.Server.Miscellaneous

module ServerVerify =

    let internal verifyLogin () (login : LoginValuesShared) =   // LoginInfo -> Async<LoginResult>>

        let isValidLogin inputUsrString inputPswString = not (strContainsOnlySpace inputUsrString || strContainsOnlySpace inputPswString)            

        let uberHashError uberHash credential seqFn =

            try
                pyramidOfDoom  // pyramidOfHell and Result.isOk not possible to use
                    {
                        let! uberHash = uberHash |> Result.toOption, ExceptionError
                        let! _ = not (uberHash |> Seq.isEmpty) |> Option.ofBool, ExceptionError
                        let! _ = verify (uberHash |> seqFn) credential |> Option.ofBool, LegitimateFalse
                    
                        return LegitimateTrue
                    }
            with
            | ex
                ->
                logInfoMsg <| sprintf "Error012 %s" (string ex.Message)
                ExceptionError                   

        pyramidOfHell  
            {
                let rc1 =
                    {
                        LoginErrorMsgShared.line1 = ErrorMsgLine1 "Závažná chyba na serveru !!!"
                        LoginErrorMsgShared.line2 = ErrorMsgLine2 "Chybí soubor pro ověření uživatelského jména a hesla"
                    }

                let rc2 =
                    {
                        LoginErrorMsgShared.line1 = ErrorMsgLine1 "Závažná chyba na serveru !!!"
                        LoginErrorMsgShared.line2 = ErrorMsgLine2 "Problém s ověřením uživatelského jména a hesla"
                    }

                let rc3 =
                    {
                        LoginErrorMsgShared.line1 = ErrorMsgLine1 "Buď uživatelské jméno anebo heslo je neplatné."
                        LoginErrorMsgShared.line2 = ErrorMsgLine2 "Prosím zadej údaje znovu."
                    }  

                let usr = login.Username |> function SharedTypes.Username value -> value 
                let psw = login.Password |> function SharedTypes.Password value -> value 

                let uberHash =
                    try
                        pyramidOfDoom 
                            {
                                let! _ = File.Exists(Path.GetFullPath pathToUberHashTxt) |> Option.ofBool, Error String.Empty
                                let value = File.ReadAllLines pathToUberHashTxt |> Seq.ofArray //non-nullable, ex caught with tryWith
                                let! _ =
                                    not
                                        (
                                            value
                                            |> Seq.map (fun item -> item |> Option.ofNull)
                                            |> Seq.exists (fun item -> item.IsNone)
                                        )
                                        |> Option.ofBool, Error String.Empty

                                return Ok value 
                            } 
                    with
                    | ex
                        ->
                        logInfoMsg <| sprintf "Error013 %s" (string ex.Message)
                        Error (string ex.Message)

                let! _ = uberHash |> Result.isOk, UsernameOrPasswordIncorrect rc1                
                let! _ = isValidLogin usr psw, UsernameOrPasswordIncorrect rc3

                let verify1 = uberHashError uberHash usr Seq.head 
                let! _ = (<>) verify1 ExceptionError, UsernameOrPasswordIncorrect rc2

                let verify2 = uberHashError uberHash psw Seq.last 
                let! _ = (<>) verify2 ExceptionError, UsernameOrPasswordIncorrect rc2
                let! _ = (&&) (verify1 = LegitimateTrue) (verify2 = LegitimateTrue), UsernameOrPasswordIncorrect rc3 
                                                                        
                return LoggedIn { Username = login.Username } //{ Username = login.Username; AccessToken = SharedApi.AccessToken accessToken }
            }

//************** TODO validation upon request from the user *************************

    let private isValidCenik param = ()   
    let private isValidKontakt param = () 
    let private isValidLink param = ()

    let verifyCenikValues (cenikValues: CenikValuesShared) =
        match isValidCenik () with
        | ()  -> Ok ()        
        //| _ -> Error _

    let verifyKontaktValues (kontaktValues: KontaktValuesShared) =
       match isValidKontakt () with
       | ()  -> Ok ()        
       //| _ -> Error _

    let verifyLinkAndLinkNameValues (linkValues: LinkValuesShared) =
       match isValidLink () with
       | ()  -> Ok ()        
       //| _ -> Error _

    (*
    // applicatives

    type Validation<'err, 'ok> =
        | Ok of 'ok
        | Error of 'err list
    
    module Validation =
        let map f x =
            match x with
            | Ok x -> Ok (f x)
            | Error errs -> Error errs
    
        let apply f x =
            match f, x with
            | Ok f, Ok x -> Ok (f x)
            | Error e1, Error e2 -> Error (e1 @ e2)
            | Error e, _ | _, Error e -> Error e
    
        let ofResult = function
            | Result.Ok x -> Ok x
            | Result.Error e -> Error [e]

    let validateCenik v =
        match isValidCenik v with
        | true  -> Ok ()
        | false -> Error ["Ceník invalid"]
    
    let validateKontakt v =
        match isValidKontakt v with
        | true  -> Ok ()
        | false -> Error ["Kontakt invalid"]
    
    let validateLink v =
        match isValidLink v with
        | true  -> Ok ()
        | false -> Error ["Link invalid"]
    
    let validateAll cenik kontakt link =
        Ok (fun _ _ _ -> ()) // dummy combiner
        |> Validation.apply (validateCenik cenik)
        |> Validation.apply (validateKontakt kontakt)
        |> Validation.apply (validateLink link)

    *)