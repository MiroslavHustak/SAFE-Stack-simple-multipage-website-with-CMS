namespace CMSPages

open Feliz
open Elmish
open Fable.Remoting.Client

open Shared
open SharedTypes

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

        let usr = user.Username |> function SharedApi.Username value -> value

        let userText = sprintf"%s%s" "Uživatel byl právě přihlášen jako : " usr   
        let userTextTime = sprintf"%s%s" "Okamžik přihlášení : " (string <| System.DateTime.Now.ToLongTimeString())
               
        let hiddenValue =
            match usr with
            | "" -> true
            | _  -> false

        let returnButtonDiv = 
             Html.div [            
                Html.form [
                    prop.action (MaximeRouter.Router.toHash (MaximeRouter.Router.Home))
                    prop.children [
                        Html.td [
                            Html.h5 [                                                                        
                                prop.id 100
                                prop.hidden hiddenValue
                                prop.children [
                                    Html.text userText
                                    Html.br[]
                                    Html.text userTextTime
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
        
        let contentCMSRozcestnik returnButtonDiv = 
        
                Html.html [
                    prop.xmlns "http://www.w3.org/1999/xhtml"
                    prop.children [
                        Html.head [
                            //prop..runat "server"
                            prop.children [
                                Html.meta [
                                    prop.name "viewport"
                                    prop.content "width=device-width, initial-scale=1.0"
                                ]
                                Html.meta [                        
                                    prop.content "text/html; charset=utf-8"
                                ]
                                Html.title "Nutriční terapie"
                                Html.meta [
                                    prop.name "keywords"
                                    prop.content "css templates, healthy living, diet, nutrition, fitness, web design"
                                ]
                                Html.meta [
                                    prop.name "description"
                                    prop.content "Healthy Living - free CSS template provided by templatemo.com"
                                ]
                                Html.link [
                                    prop.rel "icon"
                                    prop.href "/Content/images/favicon.ico"
                                ]
                                Html.link [
                                    prop.href "/Content/css/cms.css"
                                    prop.rel "stylesheet"
                                ]                       
                            ]
                        ]
                        Html.body [
                            Html.div [
                                prop.method "get"
                                prop.children [
                                    Html.div [
                                        prop.id "wrap"
                                        prop.children [
                                            Html.div [
                                                prop.id "contentwrap"
                                                prop.children [
                                                    Html.div [
                                                        prop.id "content"
                                                        prop.children [
                                                            Html.br []
                                                            Html.br []
                                                        
                                                            Html.table [                                                                                            
                                                                prop.style
                                                                    [
                                                                      //style.marginLeft auto //neexistuje
                                                                      //style.marginRight auto //neexistuje
                                                                      //vycentrovano pres css
                                                                    ]
                                                                prop.children [                                                                 
                                                                    Html.tr [
                                                                        Html.td [
                                                                            prop.style
                                                                                [
                                                                                  style.height(100)                                                                 
                                                                                ]
                                                                            prop.children [
                                                                                Html.div [
                                                                                    prop.id "apple"
                                                                                    prop.children [
                                                                                        Html.br []
                                                                                        Html.br []
                                                                                        Html.img [
                                                                                            prop.style
                                                                                                [
                                                                                                  style.marginLeft(40) //prop.width 80 / 2                                                              
                                                                                                ]                                                                             
                                                                                            prop.src "/Content/images/apple.png"
                                                                                            prop.alt "Downloading apple..."
                                                                                            prop.id "apple"
                                                                                            prop.className "unhidden"
                                                                                            prop.width 80
                                                                                            prop.height 80
                                                                                        ]                                                                                                                                       
                                                                                    ]
                                                                                ]
                                                                            ]
                                                                        ]
                                                                    ]
                                                                    Html.tr [
                                                                        prop.style
                                                                            [
                                                                                style.marginLeft(0)
                                                                                style.height(100)  
                                                                            ] 
                                                                        prop.children [
                                                                            Html.td [
                                                                                Html.h1 [                                                                        
                                                                                    prop.id 1
                                                                                    prop.children [
                                                                                        Html.text "Rozcestník"                                                        
                                                                                    ]
                                                                                ]
                                                                            ]                                                       
                                                                        ]
                                                                    ]   
                                                                    Html.tr [
                                                                        Html.td [
                                                                            prop.style
                                                                                [
                                                                                  style.height(80)                                                                 
                                                                                ]
                                                                            prop.children [
                                                                                Html.div [                                                                        
                                                                                    Html.div [
                                                                                        Html.a [
                                                                                            prop.style
                                                                                                [                                                                                      
                                                                                                  style.fontFamily "sans-serif"
                                                                                                  style.fontWeight.bold
                                                                                                  style.color.blue
                                                                                                ]
                                                                                            prop.href (MaximeRouter.Router.toHash (MaximeRouter.Router.CMSCenik 7))
                                                                                            prop.children [
                                                                                                Html.text "Editace ceníku"
                                                                                            ]
                                                                                        ]    
                                                                                    ]
                                                                                    Html.div [
                                                                                        Html.a [
                                                                                            prop.style
                                                                                                [                                                                                      
                                                                                                  style.fontFamily "sans-serif"
                                                                                                  style.fontWeight.bold
                                                                                                  style.color.blue
                                                                                                ]
                                                                                            prop.href (MaximeRouter.Router.toHash (MaximeRouter.Router.CMSKontakt 8))
                                                                                            prop.children [
                                                                                                Html.text "Editace kontaktních údajů"
                                                                                            ]
                                                                                        ]    
                                                                                    ]
                                                                                    Html.div [
                                                                                        Html.a [
                                                                                            prop.style
                                                                                                [                                                                                      
                                                                                                    style.fontFamily "sans-serif"
                                                                                                    style.fontWeight.bold
                                                                                                    style.color.blue
                                                                                                ]
                                                                                            prop.href (MaximeRouter.Router.toHash (MaximeRouter.Router.CMSLink 9))
                                                                                            prop.children [
                                                                                                Html.text "Editace odkazů na webové stránky"
                                                                                            ]
                                                                                        ]    
                                                                                    ]                                                                        
                                                                                    Html.br []
                                                                                    Html.br []
                                                                                    returnButtonDiv
                                                                                    (*
                                                                                    Html.div [                                                                                 
                                                                                        Html.button [                                                                           
                                                                                            prop.text "Log-off"
                                                                                            prop.id "Button2"                                                                           
                                                                                            proponClick
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
                                                                                    ]
                                                                                    *)
                                                                                ]
                                                                            ]
                                                                        ]
                                                                    ]
                                                                ]
                                                            ]
                                                            Html.br []
                                                        ]
                                                    ]
                                                ]
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]

        contentCMSRozcestnik returnButtonDiv
   