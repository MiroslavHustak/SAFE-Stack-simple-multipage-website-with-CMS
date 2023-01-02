namespace CMSPages

open Feliz
open Elmish
open Fable.Remoting.Client

open Shared
open SharedTypes

open HtmlFeliz
open ContentCMSRozcestnik

module CMSRozcestnik = 

    type Model =
        {        
          Id: int   
        }

    type Msg =
         | Dummy  

    let private deleteSecurityTokenFileApi =
        Remoting.createApi ()
        |> Remoting.withRouteBuilder Route.builder
        |> Remoting.buildProxy<IGetApi>

    let init id: Model * Cmd<Msg> =
        let model = {
                        Id = id                
                    }
        model, Cmd.none

    let update (msg: Msg) (model: Model): Model * Cmd<Msg> =
        match msg with
        | Dummy -> model, Cmd.none 

    let view (model: Model) (dispatch: Msg -> unit) =

        //second "rozcestnik" 
        let returnButtonDiv =
             Html.div [            
                Html.form [
                    prop.action (MaximeRouter.Router.toHash (MaximeRouter.Router.Home))
                    prop.children [
                        Html.input [
                            prop.type' "submit"
                            prop.value "Logout a návrat na webové stránky" //second "rozcestnik" 
                            prop.id "Button2"
                            prop.onClick (fun _ -> dispatch Dummy) //Main.fs contains "Session = None" for the second "rozcestnik"
                            prop.style
                                [
                                  style.width(300)
                                  style.height(30)
                                  style.fontWeight.bold
                                  style.fontSize(16) 
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

        contentCMSRozcestnik returnButtonDiv 
   