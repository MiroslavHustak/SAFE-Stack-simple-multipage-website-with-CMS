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
            
    let init id: Model * Cmd<Msg> =
        let model = {
                        Id = id                
                    }
        model, Cmd.none

    let update (msg: Msg) (model: Model): Model * Cmd<Msg> =
        match msg with
        | Dummy -> model, Cmd.none 

    let view model (user: SharedApi.User) (dispatch: Msg -> unit) = 

        let userText = sprintf"%s%s" "Uživatel byl právě přihlášen jako : " user.Username
                
        let hiddenValue =
            match user.Username with
            | "" -> true
            | _  -> false

        let returnButtonDiv = 
             Html.div [            
                Html.form [
                    prop.action (MaximeRouter.Router.toHash (MaximeRouter.Router.Home))
                    prop.children [
                        Html.td [
                            Html.h4 [                                                                        
                                prop.id 100
                                prop.hidden hiddenValue
                                prop.children [
                                    Html.text userText                                                       
                                ]
                            ]
                        ]         
                        Html.input [
                            prop.type' "submit"
                            prop.value "Logout a návrat na webové stránky" 
                            prop.id "Button2"
                            prop.onClick (fun _ -> dispatch Dummy) //Main.fs contains "Session = None" 
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
             ]      

        contentCMSRozcestnik returnButtonDiv 
   