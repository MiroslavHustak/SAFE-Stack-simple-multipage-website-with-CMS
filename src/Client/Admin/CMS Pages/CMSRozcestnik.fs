module CMSRozcestnik

open Elmish
open Feliz
open Fable.Remoting.Client

open Shared
open SharedTypes

open Layout
open Records
open ContentCMSRozcestnik
open ContentCMSForbidden
open Login

type Model =
    {        
      Id: int    
      //Dummy: unit         
    }

type Msg =
    | AskServerForDeletingSecurityTokenFile
    | DeleteSecurityTokenFileMsg of unit   

let private deleteSecurityTokenFileApi = Remoting.createApi ()
                                         |> Remoting.withRouteBuilder Route.builder
                                         |> Remoting.buildProxy<IGetApi>

let init id: Model * Cmd<Msg> =
    let model = {
                    Id = id                
                }
    //let model = { Dummy = () } 
    model, Cmd.none

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | AskServerForDeletingSecurityTokenFile ->
        let sendEvent = DeleteSecurityTokenFile.create () 
        let cmd = Cmd.OfAsync.perform deleteSecurityTokenFileApi.deleteSecurityTokenFile sendEvent DeleteSecurityTokenFileMsg
        model, cmd
                               
    | DeleteSecurityTokenFileMsg _ -> model, Cmd.none // value je unit, takze staci placement   

let view (model: Model) (dispatch: Msg -> unit) =

    //druhy rozcestnik
    let returnButtonDiv askServerForDeletingSecurityTokenFile =
         Html.div [            
            Html.form [
                prop.action (RouterM.toHash (RouterM.Home))
                prop.children [
                    Html.input [
                        prop.type' "submit"
                        prop.value "Logout a návrat na webové stránky" //druhy rozcestnik
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

    contentCMSRozcestnik (returnButtonDiv AskServerForDeletingSecurityTokenFile) 
   