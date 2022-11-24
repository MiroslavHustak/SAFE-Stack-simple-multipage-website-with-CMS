module CMSRozcestnik

open Elmish
open Feliz
open Fable.Remoting.Client

open Shared
open SharedTypesAndRecords

open Layout
open Records
open ContentCMSRozcestnik
open ContentCMSForbidden
open Router

type Model =
    {        
      Id: int
      //Dummy: unit         
    }

type Msg =
    | AskServerForDeletingSecurityTokenFile
    | Dummy of DeleteSecurityTokenFile

let deleteSecurityTokenFileApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IGetApi>

let init id : Model * Cmd<Msg> =
    let model = { Id = id }
    //let model = { Dummy = () } 
    model, Cmd.none

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | AskServerForDeletingSecurityTokenFile ->
        let sendEvent = DeleteSecurityTokenFile.create true 
        let cmd = Cmd.OfAsync.perform deleteSecurityTokenFileApi.deleteSecurityTokenFile sendEvent Dummy
        model, cmd
    | Dummy _ -> model, Cmd.none

let view (model: Model) (dispatch: Msg -> unit) (securityToken: GetSecurityToken) =

    let deleteSecurityTokenFile askServerForDeletingSecurityTokenFile =
         Html.div [
            Html.form [
                prop.action (toHash (Router.Home))
                prop.children [
                    Html.input [
                        prop.type' "submit"
                        prop.value "Log-off"
                        prop.id "Button2"
                        prop.onClick (fun _ -> dispatch askServerForDeletingSecurityTokenFile)
                        prop.style
                            [
                              style.width(200)
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


    match securityToken.SecurityToken = "securityToken" with 
    | true ->  //dispatch AskServerForDeletingSecurityTokenFile 
               contentCMSRozcestnik (deleteSecurityTokenFile AskServerForDeletingSecurityTokenFile)
    | false -> contentCMSForbidden()

