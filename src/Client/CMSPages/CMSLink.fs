namespace CMSPages

open System

open Feliz
open Elmish
open FSharp.Control
open Fable.Remoting.Client

open Shared
open Helpers.Client.Helper

module CMSLink = 

     //Common model to view / from view
    type Model =
        {
            //***** ClientDtoToView *********
            LinkValues: LinkValuesShared
            OldLinkValues: LinkValuesShared
            //******************************

            //***** ClientDtoFromView *********            
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
            //******************************

            Id: int
            DelayMsg: string
            ErrorMsg: string
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
        | SendLinkValuesToServer
        | SendOldLinkValuesToServer
        | NewLinkValues of LinkValuesShared
        | OldLinkValues of LinkValuesShared
        | AsyncWorkIsComplete 

    let private sendLinkValuesApi =
        Remoting.createApi ()
        |> Remoting.withRouteBuilder Route.builder
        |> Remoting.buildProxy<IGetApi>

    let internal init id : Model * Cmd<Msg> =

        let model =
            {
                LinkValues = SharedLinkValues.linkValuesDomainDefault           
                OldLinkValues = SharedLinkValues.linkValuesDomainDefault           
                V001LinkInput = String.Empty
                V002LinkInput = String.Empty
                V003LinkInput = String.Empty
                V004LinkInput = String.Empty
                V005LinkInput = String.Empty
                V006LinkInput = String.Empty       
                V001LinkNameInput = String.Empty
                V002LinkNameInput = String.Empty
                V003LinkNameInput = String.Empty
                V004LinkNameInput = String.Empty
                V005LinkNameInput = String.Empty
                V006LinkNameInput = "Facebook"           
                Id = id
                DelayMsg = String.Empty
                ErrorMsg = String.Empty
            }
        model, Cmd.ofMsg SendOldLinkValuesToServer

    let internal update (msg: Msg) (model: Model) : Model * Cmd<Msg> =

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

        | SendOldLinkValuesToServer
            ->
             let loadEvent = SharedDeserialisedValues.transferLayer model.OldLinkValues
             let cmd = Cmd.OfAsync.perform sendLinkValuesApi.getOldLinkValues loadEvent OldLinkValues
             model, cmd

        | AsyncWorkIsComplete -> { model with DelayMsg = String.Empty }, Cmd.none 
    
        | SendLinkValuesToServer
            ->
             try
                 try
                     let buttonClickEvent: LinkValuesShared =   
                         let input current old =
                             match current = String.Empty with //String.IsNullOrWhiteSpace current || String.IsNullOrEmpty current
                             | true  -> old
                             | false -> current 
                         SharedLinkValues.transferLayer
                         <| input model.V001LinkInput model.OldLinkValues.V001 <| input model.V002LinkInput model.OldLinkValues.V002 <| input model.V003LinkInput model.OldLinkValues.V003 
                         <| input model.V004LinkInput model.OldLinkValues.V004 <| input model.V005LinkInput model.OldLinkValues.V005 <| input model.V006LinkInput model.OldLinkValues.V006
                         <| input model.V001LinkNameInput model.OldLinkValues.V001n <| input model.V002LinkNameInput model.OldLinkValues.V002n <| input model.V003LinkNameInput model.OldLinkValues.V003n 
                         <| input model.V004LinkNameInput model.OldLinkValues.V004n <| input model.V005LinkNameInput model.OldLinkValues.V005n <| input model.V006LinkNameInput model.OldLinkValues.V006n

                     //Cmd.OfAsyncImmediate instead of Cmd.OfAsync
                     let cmd = Cmd.OfAsyncImmediate.perform sendLinkValuesApi.sendLinkAndLinkNameValues buttonClickEvent NewLinkValues
                     let cmd2 (cmd: Cmd<Msg>) delayedDispatch = Cmd.batch <| seq { cmd; Cmd.ofSub delayedDispatch }    

                     let delayedCmd (dispatch: Msg -> unit): unit =                                                  
                         let delayedDispatch: Async<unit> =
                             async
                                 {
                                     let! completor = Async.StartChild (async { return dispatch SendOldLinkValuesToServer })
                                     let! result = completor
                                     do! Async.Sleep 1000

                                     dispatch AsyncWorkIsComplete
                                 }                                   
                         Async.StartImmediate delayedDispatch                                                            
                                                         
                     { model with DelayMsg = "Probíhá načítání..."; ErrorMsg = String.Empty }, cmd2 cmd delayedCmd        
                 finally
                 ()   
             with
             | ex -> { model with ErrorMsg = "Nedošlo k načtení hodnot." }, Cmd.none  

        | NewLinkValues valueNew
            ->
             {
                 model with
                           LinkValues =
                              {
                                  V001 = valueNew.V001; V002 = valueNew.V002; V003 = valueNew.V003;
                                  V004 = valueNew.V004; V005 = valueNew.V005; V006 = valueNew.V006;
                                  V001n = valueNew.V001n; V002n = valueNew.V002n; V003n = valueNew.V003n;
                                  V004n = valueNew.V004n; V005n = valueNew.V005n; V006n = "Facebook";
                                  Msgs = valueNew.Msgs
                              }
                           ErrorMsg = 
                               let (p1, p2, p3) = compare valueNew.Msgs.Msg1 valueNew.Msgs.Msg2 valueNew.Msgs.Msg3
                               removeSpaces <| sprintf "%s %s %s" p1 p2 p3 
             },  Cmd.none

        | OldLinkValues valueOld
            ->
             {
                 model with
                            OldLinkValues =
                                {
                                    V001 = valueOld.V001; V002 = valueOld.V002; V003 = valueOld.V003;
                                    V004 = valueOld.V004; V005 = valueOld.V005; V006 = valueOld.V006;
                                    V001n = valueOld.V001n; V002n = valueOld.V002n; V003n = valueOld.V003n;
                                    V004n = valueOld.V004n; V005n = valueOld.V005n; V006n = "Facebook";
                                    Msgs = valueOld.Msgs
                                }
                            ErrorMsg = 
                                let (p1, p2, p3) = compare valueOld.Msgs.Msg1 valueOld.Msgs.Msg2 valueOld.Msgs.Msg3
                                removeSpaces <| sprintf "%s %s %s" p1 p2 p3 
             },  Cmd.none
   
    let internal view (model: Model) (dispatch: Msg -> unit) =

        let completeContent() =

            let td n =
                (
                    [ 1..n ]
                    |> List.map (fun _ -> Html.td [])
                    |> List.ofSeq
                )
                |> List.map (fun item -> item)

            javaScriptMessageBox model.ErrorMsg

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
                            prop.action (MaximeRouter.Router.toHash (MaximeRouter.Router.CMSLink 9)) 
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
                                                        yield! td 3      
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
                                                        Html.td model.OldLinkValues.V001n   
                                                        Html.td
                                                            [
                                                                Html.input [
                                                            
                                                                    prop.id "content"
                                                                    prop.type' "text"
                                                                    prop.id "001a"
                                                                    prop.name ""
                                                                    prop.placeholder model.OldLinkValues.V001n 
                                                                    prop.onChange (SetV001LinkNameInput >> dispatch)
                                                                    //prop.onChange (fun (ev: string) -> SetV001LinkNameInput ev |> dispatch) 
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
                                                                    prop.placeholder model.OldLinkValues.V001
                                                                    prop.onChange (SetV001LinkInput >> dispatch)
                                                                    //prop.onChange (fun (ev: string) -> SetV001LinkInput ev |> dispatch)    
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
                                                        Html.td model.OldLinkValues.V002n  
                                                   
                                                        Html.td
                                                            [
                                                                Html.input [
                                                                    prop.type' "text"
                                                                    prop.id "002a"
                                                                    prop.name ""
                                                                    prop.placeholder model.OldLinkValues.V002n
                                                                    prop.onChange (SetV002LinkNameInput >> dispatch)
                                                                    //prop.onChange (fun (ev: string) -> SetV002LinkNameInput ev |> dispatch)
                                                                    prop.autoFocus true
                                                                ]    
                                                            ]
                                                        Html.td
                                                            [
                                                                Html.input [
                                                                    prop.type' "text"
                                                                    prop.id "002b"
                                                                    prop.name ""
                                                                    prop.placeholder model.OldLinkValues.V002
                                                                    prop.onChange (SetV002LinkInput >> dispatch)
                                                                    //prop.onChange (fun (ev: string) -> SetV002LinkInput ev |> dispatch)
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
                                                        Html.td model.OldLinkValues.V003n 
                                                        Html.td
                                                            [
                                                                Html.input [
                                                                    prop.type' "text"
                                                                    prop.id "003a"
                                                                    prop.name ""
                                                                    prop.placeholder model.OldLinkValues.V003n
                                                                    prop.onChange (SetV003LinkNameInput >> dispatch)
                                                                    //prop.onChange (fun (ev: string) -> SetV003LinkNameInput ev |> dispatch)
                                                                    prop.autoFocus true
                                                                ]    
                                                            ]
                                                        Html.td
                                                            [
                                                                Html.input [
                                                                    prop.type' "text"
                                                                    prop.id "003b"
                                                                    prop.name ""
                                                                    prop.placeholder model.OldLinkValues.V003
                                                                    prop.onChange (SetV003LinkInput >> dispatch)
                                                                    //prop.onChange (fun (ev: string) -> SetV003LinkInput ev |> dispatch)
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
                                                        Html.td model.OldLinkValues.V004n 
                                                        Html.td
                                                            [
                                                                Html.input [
                                                                    prop.type' "text"
                                                                    prop.id "004a"
                                                                    prop.name ""
                                                                    prop.placeholder model.OldLinkValues.V004n
                                                                    prop.onChange (SetV004LinkNameInput >> dispatch)
                                                                    //prop.onChange (fun (ev: string) -> SetV004LinkNameInput ev |> dispatch)
                                                                    prop.autoFocus true
                                                                ]    
                                                            ]           
                                                        Html.td
                                                            [
                                                                Html.input [
                                                                    prop.type' "text"
                                                                    prop.id "004b"
                                                                    prop.name ""
                                                                    prop.placeholder model.OldLinkValues.V004
                                                                    prop.onChange (SetV004LinkInput >> dispatch)
                                                                    //prop.onChange (fun (ev: string) -> SetV004LinkInput ev |> dispatch)
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
                                                        Html.td model.OldLinkValues.V005n  
                                                        Html.td
                                                            [
                                                                Html.input [
                                                                    prop.type' "text"
                                                                    prop.id "005a"
                                                                    prop.name ""
                                                                    prop.placeholder model.OldLinkValues.V005n
                                                                    prop.onChange (SetV005LinkNameInput >> dispatch)
                                                                    //prop.onChange (fun (ev: string) -> SetV005LinkNameInput ev |> dispatch)   
                                                                    prop.autoFocus true
                                                                ]    
                                                            ]      
                                                        Html.td
                                                            [
                                                                Html.input [
                                                                    prop.type' "text"
                                                                    prop.id "005b"
                                                                    prop.name ""
                                                                    prop.placeholder model.OldLinkValues.V005
                                                                    prop.onChange (SetV005LinkInput >> dispatch)
                                                                    //prop.onChange (fun (ev: string) -> SetV005LinkInput ev |> dispatch)
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
                                                        Html.td model.OldLinkValues.V006n  
                                                        Html.td model.OldLinkValues.V006n 
                                                        Html.td
                                                            [
                                                                Html.input [
                                                                    prop.type' "text"
                                                                    prop.id "006"
                                                                    prop.name ""
                                                                    prop.placeholder model.OldLinkValues.V006
                                                                    prop.onChange (SetV006LinkInput >> dispatch)
                                                                    //prop.onChange (fun (ev: string) -> SetV006LinkInput ev |> dispatch)  
                                                                    prop.autoFocus true
                                                                ]    
                                                            ]      
                                                    ]
                                                ]
                                                Html.tr [
                                                    prop.style
                                                        [
                                                            //style.marginLeft(0)
                                                            style.height(15)  
                                                        ] 
                                                    prop.children [
                                                        yield! td 3                                                                                                  
                                                    ]
                                                ]        
                                                Html.tr [
                                                    prop.style
                                                        [
                                                            //style.marginLeft(0)                                                       
                                                        ] 
                                                    prop.children [    
                                                        Html.td [                                                       
                                                            prop.style
                                                                [                                                                   
                                                                    style.fontFamily "sans-serif"
                                                                    style.fontWeight.bold
                                                                ]                                                           
                                                            prop.children [
                                                                Html.text "Použij mezerník pro generování prázdného řádku"
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
                                                        ]                                                  
                                                    ]
                                                ]
                                                Html.tr [
                                                    prop.style
                                                        [
                                                            //style.marginLeft(0)
                                                            style.height(15)  
                                                        ] 
                                                    prop.children [
                                                        Html.td []    
                                                        Html.td
                                                            [
                                                                prop.style
                                                                    [
                                                                        style.fontWeight.bold
                                                                        style.fontSize(14) 
                                                                        style.color.red
                                                                        style.fontFamily "sans-serif"
                                                                    ]
                                                                prop.children
                                                                    [                                                            
                                                                        Html.text model.DelayMsg
                                                                    ]                                                                                                                
                                                            ]
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
                                                                prop.href (MaximeRouter.Router.toHash (MaximeRouter.Router.CMSRozcestnik 6))
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
                                                                                       dispatch SendLinkValuesToServer
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

        completeContent()