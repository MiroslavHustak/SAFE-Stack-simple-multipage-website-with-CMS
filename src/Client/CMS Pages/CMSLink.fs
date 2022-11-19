module CMSLink

open Elmish
open Feliz
open Fable.Remoting.Client

open Shared
open SharedRecords

open System

open Layout
open Router
open ContentCMSKontakt

type Model =
    {
      LinkAndLinkNameValues: GetLinkAndLinkNameValues
      OldLinkAndLinkNameValues: GetLinkAndLinkNameValues
      V001LinkInput: string
      V002LinkInput: string
      V003LinkInput: string
      V004LinkInput: string
      V005LinkInput: string
      V006LinkInput: string
      V001LinkNameInput: string
      V002LinkNameInput: string
      V003LinkNameInput: string
      V004LinkNameInput: string
      V005LinkNameInput: string
      V006LinkNameInput: string
      Id: int
    }

type Msg =
    | SetV001LinkInput of string
    | SetV002LinkInput of string
    | SetV003LinkInput of string
    | SetV004LinkInput of string
    | SetV005LinkInput of string
    | SetV006LinkInput of string
    | SetV001LinkNameInput of string
    | SetV002LinkNameInput of string
    | SetV003LinkNameInput of string
    | SetV004LinkNameInput of string
    | SetV005LinkNameInput of string
    | SetV006LinkNameInput of string
    | SendLinkAndLinkNameValuesToServer
    | SendOldLinkAndLinkNameValuesToServer
    | GetLinkAndLinkNameValues of GetLinkAndLinkNameValues
    | GetOldLinkAndLinkNameValues of GetLinkAndLinkNameValues

let getLinkAndLinkNameValuesApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IGetApi>

let init id : Model * Cmd<Msg> =
    let model = {
        LinkAndLinkNameValues =
            {
                V001 = ""; V002 = ""; V003 = "";
                V004 = ""; V005 = ""; V006 = ""
                V001n = ""; V002n = ""; V003n = "";
                V004n = ""; V005n = ""; V006n = "Facebook"
            }
        OldLinkAndLinkNameValues =
            {
                V001 = ""; V002 = ""; V003 = "";
                V004 = ""; V005 = ""; V006 = ""
                V001n = ""; V002n = ""; V003n = "";
                V004n = ""; V005n = ""; V006n = "Facebook"
            }      
        V001LinkInput = ""
        V002LinkInput = ""
        V003LinkInput = ""
        V004LinkInput = ""
        V005LinkInput = ""
        V006LinkInput = ""       
        V001LinkNameInput = ""
        V002LinkNameInput = ""
        V003LinkNameInput = ""
        V004LinkNameInput = ""
        V005LinkNameInput = ""
        V006LinkNameInput = "Facebook"           
        Id = id
      }
    model, Cmd.ofMsg SendOldLinkAndLinkNameValuesToServer

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | SetV001LinkInput value -> { model with V001LinkInput = value }, Cmd.none
    | SetV002LinkInput value -> { model with V002LinkInput = value }, Cmd.none
    | SetV003LinkInput value -> { model with V003LinkInput = value }, Cmd.none
    | SetV004LinkInput value -> { model with V004LinkInput = value }, Cmd.none
    | SetV005LinkInput value -> { model with V005LinkInput = value }, Cmd.none
    | SetV006LinkInput value -> { model with V006LinkInput = value }, Cmd.none
    | SetV001LinkNameInput value -> { model with V001LinkNameInput = value }, Cmd.none
    | SetV002LinkNameInput value -> { model with V002LinkNameInput = value }, Cmd.none
    | SetV003LinkNameInput value -> { model with V003LinkNameInput = value }, Cmd.none
    | SetV004LinkNameInput value -> { model with V004LinkNameInput = value }, Cmd.none
    | SetV005LinkNameInput value -> { model with V005LinkNameInput = value }, Cmd.none
    | SetV006LinkNameInput value -> { model with V006LinkNameInput = value }, Cmd.none

    | SendOldLinkAndLinkNameValuesToServer ->
        let loadEvent = SharedDeserialisedLinkAndLinkNameValues.create model.OldLinkAndLinkNameValues
        let cmd = Cmd.OfAsync.perform getLinkAndLinkNameValuesApi.sendOldLinkAndLinkNameValues loadEvent GetOldLinkAndLinkNameValues
        model, cmd

    | SendLinkAndLinkNameValuesToServer ->
        let buttonClickEvent:GetLinkAndLinkNameValues =                                        
            SharedLinkAndLinkNameValues.create
            <| model.V001LinkInput <| model.V002LinkInput <| model.V003LinkInput 
            <| model.V004LinkInput <| model.V005LinkInput <| model.V006LinkInput
            <| model.V001LinkNameInput <| model.V002LinkNameInput <| model.V003LinkNameInput 
            <| model.V004LinkNameInput <| model.V005LinkNameInput <| model.V006LinkNameInput

        let cmd = Cmd.OfAsync.perform getLinkAndLinkNameValuesApi.getLinkAndLinkNameValues buttonClickEvent GetLinkAndLinkNameValues
        model, cmd

    | GetLinkAndLinkNameValues valueNew ->
        {
            model with
                       LinkAndLinkNameValues =
                          {
                            V001 = valueNew.V001; V002 = valueNew.V002; V003 = valueNew.V003;
                            V004 = valueNew.V004; V005 = valueNew.V005; V006 = valueNew.V006;
                            V001n = valueNew.V001n; V002n = valueNew.V002n; V003n = valueNew.V003n;
                            V004n = valueNew.V004n; V005n = valueNew.V005n; V006n = "Facebook"
                          }
        },  Cmd.none

    | GetOldLinkAndLinkNameValues valueOld ->
        {
            model with
                        OldLinkAndLinkNameValues =
                            {
                              V001 = valueOld.V001; V002 = valueOld.V002; V003 = valueOld.V003;
                              V004 = valueOld.V004; V005 = valueOld.V005; V006 = valueOld.V006;
                              V001n = valueOld.V001n; V002n = valueOld.V002n; V003n = valueOld.V003n;
                              V004n = valueOld.V004n; V005n = valueOld.V005n; V006n = "Facebook"
                            }
        },  Cmd.none
   
let view (model: Model) (dispatch: Msg -> unit) = 

    let completeContent = 
        Html.html [
            prop.xmlns "http://www.w3.org/1999/xhtml"
            prop.children [
                Html.head [
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
                            prop.href "/Content/images/apple.png"
                        ]
                        Html.link [
                            prop.href "/Content/css/cms.css"
                            prop.rel "stylesheet"
                        ]
                    ]
                ]
                Html.body [
                    Html.form [                    
                        prop.method "get"
                        prop.action (toHash (Router.CMSKontakt 8)) 
                        prop.children [                        
                            Html.br []
                            Html.br []
                            Html.div [
                                prop.id "content"
                                prop.children [                                       
                                    Html.table [                                   
                                        prop.children [                                       
                                            Html.tr [
                                                Html.td [                                               
                                                    prop.children [
                                                        Html.div [
                                                            prop.id "apple"
                                                            prop.children [                                                                            
                                                                Html.img [
                                                                    prop.style
                                                                        [
                                                                            style.marginLeft(10)                                                       
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
                                                Html.td [
                                                    Html.div [
                                                        Html.h1 "Editace odkazů a jejich názvů"
                                                        //Html.h3 ""
                                                    ]
                                                ]
                                            ]                                                                             
                                            Html.tr [
                                                prop.children [
                                                    Html.td []
                                                    Html.td []
                                                    Html.td []                                  
                                               
                                                    (*
                                                    //zkusebni kod
                                                    //let myList = [ Html.td []; Html.td []; Html.td []; Html.td []; Html.td [] ]
                                                    let myList = [ Html.text "001"; Html.text "002"; Html.text "003"; Html.text "004"; Html.text "005" ]
                                                    yield! [
                                                        for item in myList do
                                                            Html.text ", "
                                                            item
                                                    ] |> List.tail //quli carky, kera je prvni
                                                    *) 
                                                ]
                                            ]                                        
                                            Html.tr [
                                                prop.style
                                                    [
                                                        style.fontWeight.bold
                                                    ] 
                                                prop.children [
                                                    Html.td "Současný název odkazu"     
                                                    Html.td "Nový název"
                                                    Html.td "Nový odkaz (link)"
                                                ]
                                            ]
                                            Html.tr [                                                
                                                prop.children [
                                                    Html.td model.OldLinkAndLinkNameValues.V001n   
                                                    Html.td
                                                        [
                                                            Html.input [
                                                            
                                                                prop.id "content"
                                                                prop.type' "text"
                                                                prop.id "001a"
                                                                prop.name ""
                                                                prop.onChange (fun (ev: string) -> SetV001LinkNameInput ev |> dispatch)                                                       
                                                                prop.autoFocus true
                                                            ]    
                                                        ]
                                                    Html.td
                                                        [
                                                            Html.input [
                                                            
                                                                prop.id "content"
                                                                prop.type' "text"
                                                                prop.id "001b"
                                                                prop.name ""
                                                                prop.onChange (fun (ev: string) -> SetV001LinkInput ev |> dispatch)                                                       
                                                                prop.autoFocus true
                                                            ]    
                                                        ]        
                                                ]
                                            ]
                                            Html.tr [
                                                prop.style
                                                    [
                                                        //style.fontWeight.bold                                                        
                                                    ] 
                                                prop.children [
                                                    Html.td model.OldLinkAndLinkNameValues.V002n  
                                                   
                                                    Html.td
                                                        [
                                                            Html.input [
                                                                prop.type' "text"
                                                                prop.id "002a"
                                                                prop.name ""
                                                                prop.onChange (fun (ev: string) -> SetV002LinkNameInput ev |> dispatch)
                                                                prop.autoFocus true
                                                            ]    
                                                        ]
                                                    Html.td
                                                        [
                                                            Html.input [
                                                                prop.type' "text"
                                                                prop.id "002b"
                                                                prop.name ""
                                                                prop.onChange (fun (ev: string) -> SetV002LinkInput ev |> dispatch)
                                                                prop.autoFocus true
                                                            ]    
                                                        ]      
                                                ]
                                            ]
                                            Html.tr [
                                                prop.style
                                                    [
                                                        //style.fontWeight.bold                                                        
                                                    ] 
                                                prop.children [
                                                    Html.td model.OldLinkAndLinkNameValues.V003n 
                                                    Html.td
                                                        [
                                                            Html.input [
                                                                prop.type' "text"
                                                                prop.id "003a"
                                                                prop.name ""
                                                                prop.onChange (fun (ev: string) -> SetV003LinkNameInput ev |> dispatch)
                                                                prop.autoFocus true
                                                            ]    
                                                        ]
                                                    Html.td
                                                        [
                                                            Html.input [
                                                                prop.type' "text"
                                                                prop.id "003b"
                                                                prop.name ""
                                                                prop.onChange (fun (ev: string) -> SetV003LinkInput ev |> dispatch)
                                                                prop.autoFocus true
                                                            ]    
                                                        ]      
                                                ]
                                            ]
                                            Html.tr [
                                                prop.style
                                                    [
                                                        //style.fontWeight.bold                                                        
                                                    ] 
                                                prop.children [
                                                    Html.td model.OldLinkAndLinkNameValues.V004n 
                                                    Html.td
                                                        [
                                                            Html.input [
                                                                prop.type' "text"
                                                                prop.id "004a"
                                                                prop.name ""
                                                                prop.onChange (fun (ev: string) -> SetV004LinkNameInput ev |> dispatch)
                                                                prop.autoFocus true
                                                            ]    
                                                        ]           
                                                    Html.td
                                                        [
                                                            Html.input [
                                                                prop.type' "text"
                                                                prop.id "004b"
                                                                prop.name ""
                                                                prop.onChange (fun (ev: string) -> SetV004LinkInput ev |> dispatch)
                                                                prop.autoFocus true
                                                            ]    
                                                        ]      
                                                ]
                                            ]
                                            Html.tr [
                                                prop.style
                                                    [
                                                        //style.fontWeight.bold                                                        
                                                    ] 
                                                prop.children [
                                                    Html.td model.OldLinkAndLinkNameValues.V005n  
                                                    Html.td
                                                        [
                                                            Html.input [
                                                                prop.type' "text"
                                                                prop.id "005a"
                                                                prop.name ""                                                           
                                                                prop.onChange (fun (ev: string) -> SetV005LinkNameInput ev |> dispatch)                                                                                                                   
                                                                prop.autoFocus true
                                                            ]    
                                                        ]      
                                                    Html.td
                                                        [
                                                            Html.input [
                                                                prop.type' "text"
                                                                prop.id "005b"
                                                                prop.name ""                                                           
                                                                prop.onChange (fun (ev: string) -> SetV005LinkInput ev |> dispatch)                                                                                                                   
                                                                prop.autoFocus true
                                                            ]    
                                                        ]      
                                                ]
                                            ]
                                            Html.tr [
                                                prop.style
                                                    [
                                                        //style.fontWeight.bold                                                        
                                                    ] 
                                                prop.children [
                                                    Html.td model.OldLinkAndLinkNameValues.V006n  
                                                    Html.td model.OldLinkAndLinkNameValues.V006n 
                                                    Html.td
                                                        [
                                                            Html.input [
                                                                prop.type' "text"
                                                                prop.id "006"
                                                                prop.name ""                                                            
                                                                prop.onChange (fun (ev: string) -> SetV006LinkNameInput ev |> dispatch)                                                                                                                
                                                                prop.autoFocus true
                                                            ]    
                                                        ]      
                                                ]
                                            ]                                                                                                      
                                            Html.tr [
                                                prop.style
                                                    [
                                                        //style.marginLeft(0)
                                                        style.height(50)  
                                                    ] 
                                                prop.children [
                                                    Html.td []    
                                                    Html.td []
                                                    Html.td []
                                                                                             
                                                ]
                                            ]                                      
                                            Html.tr [
                                                prop.style
                                                    [
                                                        //style.marginLeft(0)                                                       
                                                    ] 
                                                prop.children [    
                                                    Html.td [
                                                        Html.a [
                                                            prop.style
                                                                [
                                                                    style.width(200)
                                                                    style.fontFamily "sans-serif"
                                                                    style.fontWeight.bold
                                                                ]
                                                            prop.href "#cmsRozcestnik/6"
                                                            prop.children [
                                                                Html.text "Návrat na rozcestník"
                                                            ]
                                                        ]
                                                    ]                                                  
                                                    Html.td [
                                                        prop.style
                                                            [
                                                                style.visibility.hidden
                                                            ]
                                                        prop.children
                                                            [                                                            
                                                                 Html.text "****" 
                                                            ]                                                                                                                
                                                    ]  
                                                    Html.td [
                                                        Html.input [
                                                            prop.type' "submit"
                                                            prop.value "Uložit nové údaje"
                                                            prop.id "Button1"
                                                            prop.onClick (fun e -> e.preventDefault()
                                                                                   dispatch SendLinkAndLinkNameValuesToServer
                                                                         )
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

    completeContent 

   