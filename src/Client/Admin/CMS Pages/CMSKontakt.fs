module CMSKontakt

open Elmish
open Feliz
open Fable.Remoting.Client

open Shared
open SharedTypesAndRecords

open System

open Layout
open Router
open ContentCMSKontakt
open ContentCMSForbidden

type Model =
    {
      KontaktValues: GetKontaktValues
      OldKontaktValues: GetKontaktValues
      V001Input: string
      V002Input: string
      V003Input: string
      V004Input: string
      V005Input: string
      V006Input: string
      V007Input: string       
      Id: int
    }

type Msg =    
    | SetV001Input of string
    | SetV002Input of string
    | SetV003Input of string
    | SetV004Input of string
    | SetV005Input of string
    | SetV006Input of string
    | SetV007Input of string   
    | SendKontaktValuesToServer
    | SendOldKontaktValuesToServer
    | GetKontaktValues of GetKontaktValues
    | GetOldKontaktValues of GetKontaktValues
    
let getKontaktValuesApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IGetApi>

let init id : Model * Cmd<Msg> =
    let model = {
        KontaktValues =
            {
                V001 = ""; V002 = ""; V003 = "";
                V004 = ""; V005 = ""; V006 = "";
                V007 = ""
            }
        OldKontaktValues =
            {
                V001 = ""; V002 = ""; V003 = "";
                V004 = ""; V005 = ""; V006 = "";
                V007 = "" 
            }
        V001Input = ""
        V002Input = ""
        V003Input = ""
        V004Input = ""
        V005Input = ""
        V006Input = ""
        V007Input = ""       
        Id = id
      }
    model, Cmd.ofMsg SendOldKontaktValuesToServer

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    | SetV001Input value -> { model with V001Input = value }, Cmd.none
    | SetV002Input value -> { model with V002Input = value }, Cmd.none
    | SetV003Input value -> { model with V003Input = value }, Cmd.none
    | SetV004Input value -> { model with V004Input = value }, Cmd.none
    | SetV005Input value -> { model with V005Input = value }, Cmd.none
    | SetV006Input value -> { model with V006Input = value }, Cmd.none
    | SetV007Input value -> { model with V007Input = value }, Cmd.none    

    | SendOldKontaktValuesToServer ->
        let loadEvent = SharedDeserialisedKontaktValues.create model.OldKontaktValues
        let cmd = Cmd.OfAsync.perform getKontaktValuesApi.sendOldKontaktValues loadEvent GetOldKontaktValues
        model, cmd

    | SendKontaktValuesToServer ->
        let buttonClickEvent:GetKontaktValues = SharedKontaktValues.create
                                              <| model.V001Input <| model.V002Input <| model.V003Input 
                                              <| model.V004Input <| model.V005Input <| model.V006Input
                                              <| model.V007Input   
        let cmd = Cmd.OfAsync.perform getKontaktValuesApi.getKontaktValues buttonClickEvent GetKontaktValues
        model, cmd

    | GetKontaktValues valueNew ->
        {
            model with
                       KontaktValues =
                          {
                              V001 = valueNew.V001; V002 = valueNew.V002; V003 = valueNew.V003;
                              V004 = valueNew.V004; V005 = valueNew.V005; V006 = valueNew.V006;
                              V007 = valueNew.V007
                          }
        },  Cmd.none

    | GetOldKontaktValues valueOld ->
        {
            model with
                       OldKontaktValues =
                          {
                              V001 = valueOld.V001; V002 = valueOld.V002; V003 = valueOld.V003;
                              V004 = valueOld.V004; V005 = valueOld.V005; V006 = valueOld.V006;
                              V007 = valueOld.V007
                          }
        },  Cmd.none  
   
let view (model: Model) (dispatch: Msg -> unit) securityToken =

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
                                                        Html.h1 "Editace kontaktních údajů"
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
                                                    Html.td "Číslo řádku"     
                                                    Html.td "Současnost"
                                                    Html.td []  
                                                    Html.td "Nový údaj"
                                                ]
                                            ]
                                            Html.tr [                                                
                                                prop.children [
                                                    Html.td "Kontaktní údaj 1"   
                                                    Html.td
                                                        [
                                                        prop.style
                                                            [
                                                                style.fontWeight.bold
                                                                style.color.blue
                                                            ]
                                                        prop.children
                                                            [                                                            
                                                                 Html.text model.OldKontaktValues.V001 
                                                            ]   
                                                        ]
                                                    Html.td []    
                                                    Html.td
                                                        [
                                                            Html.input [
                                                                
                                                                prop.id "content"
                                                                prop.type' "text"
                                                                prop.id "001"
                                                                prop.name ""
                                                                prop.onChange (fun (ev: string) -> SetV001Input ev |> dispatch)                                                       
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
                                                    Html.td "Kontaktní údaj 2"  
                                                    Html.td
                                                        [
                                                        prop.style
                                                            [
                                                                style.fontWeight.bold
                                                                style.color.blue
                                                            ]
                                                        prop.children
                                                            [                                                            
                                                                 Html.text model.OldKontaktValues.V002 
                                                            ]   
                                                        ]
                                                    Html.td []    
                                                    Html.td
                                                        [
                                                            Html.input [
                                                                prop.type' "text"
                                                                prop.id "002"
                                                                prop.name ""
                                                                prop.onChange (fun (ev: string) -> SetV002Input ev |> dispatch)
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
                                                    Html.td "Kontaktní údaj 3" 
                                                    Html.td
                                                        [
                                                        prop.style
                                                            [
                                                                style.fontWeight.bold
                                                                style.color.blue
                                                            ]
                                                        prop.children
                                                            [                                                            
                                                                 Html.text model.OldKontaktValues.V003 
                                                            ]   
                                                        ]
                                                    Html.td []    
                                                    Html.td
                                                        [
                                                            Html.input [
                                                                prop.type' "text"
                                                                prop.id "003"
                                                                prop.name ""
                                                                prop.onChange (fun (ev: string) -> SetV003Input ev |> dispatch)
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
                                                    Html.td "Kontaktní údaj 4" 
                                                    Html.td
                                                        [
                                                        prop.style
                                                            [
                                                                style.fontWeight.bold
                                                                style.color.blue
                                                            ]
                                                        prop.children
                                                            [                                                            
                                                                Html.text model.OldKontaktValues.V004 
                                                            ]   
                                                        ]
                                                    Html.td []        
                                                    Html.td
                                                        [
                                                            Html.input [
                                                                prop.type' "text"
                                                                prop.id "004"
                                                                prop.name ""
                                                                prop.onChange (fun (ev: string) -> SetV004Input ev |> dispatch)
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
                                                    Html.td "Kontaktní údaj 5" 
                                                    Html.td
                                                        [
                                                        prop.style
                                                            [
                                                                style.fontWeight.bold
                                                                style.color.blue
                                                            ]
                                                        prop.children
                                                            [                                                            
                                                                Html.text model.OldKontaktValues.V005 
                                                            ]   
                                                        ]
                                                    Html.td []    
                                                    Html.td
                                                        [
                                                            Html.input [
                                                                prop.type' "text"
                                                                prop.id "005"
                                                                prop.name ""                                                           
                                                                prop.onChange (fun (ev: string) -> SetV005Input ev |> dispatch)                                                                                                                   
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
                                                    Html.td "Kontaktní údaj 6" 
                                                    Html.td
                                                        [
                                                        prop.style
                                                            [
                                                                style.fontWeight.bold
                                                                style.color.blue
                                                            ]
                                                        prop.children
                                                            [                                                            
                                                                 Html.text model.OldKontaktValues.V006 
                                                            ]   
                                                        ]                                                   
                                                    Html.td []    
                                                    Html.td
                                                        [
                                                            Html.input [
                                                                prop.type' "text"
                                                                prop.id "006"
                                                                prop.name ""                                                            
                                                                prop.onChange (fun (ev: string) -> SetV006Input ev |> dispatch)                                                                                                                
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
                                                    Html.td "Kontaktní údaj 7"
                                                    Html.td
                                                        [
                                                        prop.style
                                                            [
                                                                style.fontWeight.bold
                                                                style.color.blue
                                                            ]
                                                        prop.children
                                                            [                                                            
                                                                 Html.text model.OldKontaktValues.V007 
                                                            ]   
                                                        ]
                                                    Html.td []    
                                                    Html.td
                                                        [
                                                            Html.input [
                                                                prop.type' "text"
                                                                prop.id "007"
                                                                prop.name ""
                                                                prop.onChange (fun (ev: string) -> SetV007Input ev |> dispatch)
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
                                                    Html.td []                                                   
                                                ]
                                            ]                                      
                                            Html.tr [
                                                prop.style
                                                    [
                                                        //style.marginLeft(0)                                                       
                                                    ] 
                                                prop.children [    
                                                    Html.td []    
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
                                                                                   dispatch SendKontaktValuesToServer
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

    
    match securityToken.SecurityToken = "securityToken" with //model.GetCredentials.SecurityTokenFile with credentials.SecurityTokenFile
    | true -> completeContent
    | false -> contentCMSForbidden()

