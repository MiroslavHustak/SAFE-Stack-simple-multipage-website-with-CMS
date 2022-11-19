module Login

open Elmish
open Feliz
open Fable.Remoting.Client

open Shared

open ContentLogin
open ContentCMSRozcestnik

open System
open Browser

open Router

open SharedRecords

type Model =
    {
      Route: string
      InputUsr: string
      InputPsw: string
      Id: int
    }

type Msg =
    | SetUsrInput of string
    | SetPswInput of string
    | SendUsrPswToServer
    | GetRouteUsrPsw of GetRouteUsrPsw    
    
let getCredentialsApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IGetApi>

let init id : Model * Cmd<Msg> =
    let model = {
                  Route = String.Empty
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
        let buttonClickEvent = SharedValues.create model.Route model.InputUsr model.InputPsw
        let cmd = Cmd.OfAsync.perform getCredentialsApi.getCredentials buttonClickEvent GetRouteUsrPsw
        model, cmd

    | GetRouteUsrPsw value -> { model with Route = value.Route; InputUsr = value.Usr; InputPsw = value.Psw }, Cmd.none
 

let view (model: Model) (dispatch: Msg -> unit) = 

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

    let inputElementUsr proponChange =
        Html.input [          
            prop.type' "text"
            prop.id "userNameID"
            prop.name "userName"            
            prop.placeholder "Uživatelské jméno"
            prop.value model.InputUsr
            proponChange
            prop.style
                [
                  style.width(200)
                  style.fontFamily "sans-serif"
                ]
            prop.autoFocus true
        ]
        
    let inputElementPsw proponChange =
        Html.input [            
             prop.type' "password"
             prop.id "passWID"
             prop.name "passW"
             prop.placeholder "Heslo"
             prop.value model.InputPsw
             proponChange 
             prop.style
                 [
                   style.width(200)
                   style.fontFamily "sans-serif"
                 ]                   ]   

    match model.Route with
    | "CMSRozcestnik" -> contentCMSRozcestnik()
    | "Invalid"       -> 
                         contentLogin
                         <| submitInput
                         <| inputElementUsr (prop.onChange (fun (ev: string) -> SetUsrInput ev |> dispatch)) 
                         <| inputElementPsw (prop.onChange (fun (ev: string) -> SetPswInput ev |> dispatch))       
                         <| (errorMsg1, errorMsg2)
                         <| false
                         <| (dispatch: Msg -> unit)                         
    | _               ->
                        contentLogin
                        <| submitInput                        
                        <| inputElementUsr (prop.onChange (fun (ev: string) -> SetUsrInput ev |> dispatch)) 
                        <| inputElementPsw (prop.onChange (fun (ev: string) -> SetPswInput ev |> dispatch))                       
                        <| (String.Empty, String.Empty)
                        <| true
                        <| (dispatch: Msg -> unit)