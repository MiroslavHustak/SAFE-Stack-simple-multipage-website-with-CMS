module Login

open Elmish
open Feliz
open Fable.Remoting.Client

open Shared

open ContentLogin
open ContentCMSRozcestnik
open ContentMaintenance

open System
open Browser

open Router

open SharedTypes

type Model =
    {
      SecurityTokenFile: string
      SecurityToken: string
      LoginResult: string
      InputUsr: string
      InputPsw: string
      Id: int
    }

type Msg =
    | SetUsrInput of string
    | SetPswInput of string
    | SendUsrPswToServer
    | GetCredentials of GetCredentials
    | AskServerForDeletingSecurityTokenFile
    | Dummy of DeleteSecurityTokenFile
    
let getCredentialsApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IGetApi>

let deleteSecurityTokenFileApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IGetApi>

let init id : Model * Cmd<Msg> =
    let model = {
                  SecurityTokenFile = String.Empty
                  SecurityToken = String.Empty
                  LoginResult = String.Empty
                  InputUsr = String.Empty
                  InputPsw = String.Empty
                  Id = id
                }
    model, Cmd.none

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | SetUsrInput value -> { model with InputUsr = value }, Cmd.none
    | SetPswInput value -> { model with InputPsw = value }, Cmd.none

    | SendUsrPswToServer ->
        let buttonClickEvent = SharedCredentialValues.create model.LoginResult model.InputUsr model.InputPsw
        let cmd = Cmd.OfAsync.perform getCredentialsApi.getCredentials buttonClickEvent GetCredentials
        model, cmd

    | GetCredentials value -> {
                                 model with LoginResult = value.LoginResult; InputUsr = value.Usr; InputPsw = value.Psw
                              }, Cmd.none

    | AskServerForDeletingSecurityTokenFile ->
        let sendEvent = DeleteSecurityTokenFile.create true 
        let cmd = Cmd.OfAsync.perform deleteSecurityTokenFileApi.deleteSecurityTokenFile sendEvent Dummy
        model, cmd

    | Dummy _ -> model, Cmd.none

let view (model: Model) (dispatch: Msg -> unit) securityToken = 

    let errorMsg1 = "Buď uživatelské jméno anebo heslo je neplatné."
    let errorMsg2 = "Prosím zadej údaje znovu."

    let proponClick =
        prop.onClick (fun e -> e.preventDefault()
                               dispatch SendUsrPswToServer
                     )

    let submitInput =
        Html.input [
            prop.type' "submit"
            prop.value "Odeslat"
            prop.id "Button1"                                                                           
            proponClick
            prop.style
                [
                  style.width(200)
                  style.fontWeight.bold
                  style.fontSize(16) //font-size: large
                  style.color.blue
                  style.fontFamily "sans-serif"
                ]
        ]                      

    let inputElementUsr =
        Html.input [          
            prop.type' "text"
            prop.id "userNameID"
            prop.name "userName"            
            prop.placeholder "Uživatelské jméno"
            prop.value model.InputUsr
            prop.onChange (fun (ev: string) -> SetUsrInput ev |> dispatch)
            prop.style
                [
                  style.width(200)
                  style.fontFamily "sans-serif"
                ]
            prop.autoFocus true
        ]
        
    let inputElementPsw =
        Html.input [            
             prop.type' "password"
             prop.id "passWID"
             prop.name "passW"
             prop.placeholder "Heslo"
             prop.value model.InputPsw
             prop.onChange (fun (ev: string) -> SetPswInput ev |> dispatch)  
             prop.style
                 [
                   style.width(200)
                   style.fontFamily "sans-serif"
                 ]                   ]   

    let myLogin() =

        let deleteSecurityTokenFile askServerForDeletingSecurityTokenFile =
            Html.div [
               Html.form [
                   prop.action (toHash (Router.Home))
                   prop.children [
                       Html.input [
                           prop.type' "submit"
                           prop.value "Log-off a návrat na webové stránky"
                           prop.id "Button2"
                           prop.onClick (fun _ -> dispatch askServerForDeletingSecurityTokenFile)
                           prop.style
                               [
                                 style.width(300)
                                 style.height(30)
                                 style.fontWeight.bold
                                 style.fontSize(16) //font-size: large
                                 style.color.blue
                                 style.fontFamily "sans-serif"
                               ]
                       ]
                   ]                   
               ]


               (*
               Html.button [
                   prop.type' "button"
                   prop.text "Log-off"
                   prop.id "Button2"                                                                           
                   prop.onClick (fun _ -> dispatch askServerForDeletingSecurityTokenFile)
                   prop.style
                       [
                         style.height(50)
                         style.width(200)
                         style.fontWeight.bold
                         style.fontSize(16) //font-size: large
                         style.color.blue
                         style.fontFamily "sans-serif"
                       ]
               ]
               *)
            ]
            
        match model.LoginResult with
        | "CMSRozcestnik" -> contentCMSRozcestnik (deleteSecurityTokenFile AskServerForDeletingSecurityTokenFile)
        | "Invalid"       ->
                             contentLogin
                             <| submitInput
                             <| inputElementUsr 
                             <| inputElementPsw       
                             <| (errorMsg1, errorMsg2)
                             <| false
                             <| (dispatch: Msg -> unit)                         
        | _               ->
                            contentLogin
                            <| submitInput                        
                            <| inputElementUsr 
                            <| inputElementPsw                    
                            <| (String.Empty, String.Empty)
                            <| true
                            <| (dispatch: Msg -> unit)

    match securityToken with
    | "securityToken" -> contentMaintenance()
    | _               -> myLogin() 