namespace CMSPages

open System

open Feliz
open Elmish
open FSharp.Control
open Fable.Remoting.Client

open Shared
open Helpers.Client.Helper

module CMSKontakt = 

     //Common model to view / from view
    type Model =
        {
            //***** ClientDtoToView *********
            KontaktValues : KontaktValuesShared
            OldKontaktValues : KontaktValuesShared
            //******************************

            //***** ClientDtoFromView *********
            V001Input : string
            V002Input : string
            V003Input : string
            V004Input : string
            V005Input : string
            V006Input : string
            V007Input : string
            //******************************

            Id: int
            DelayMsg : string
            ErrorMsg : string
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
        | NewKontaktValues of KontaktValuesShared
        | OldKontaktValues of KontaktValuesShared
        | AsyncWorkIsComplete 
    
    let private sendKontaktValuesApi =
        Remoting.createApi ()
        |> Remoting.withRouteBuilder Route.builder
        |> Remoting.buildProxy<IGetApi>

    let internal init id : Model * Cmd<Msg> =
        
        let model =
            {
                KontaktValues = SharedKontaktValues.kontaktValuesDomainDefault        
                OldKontaktValues = SharedKontaktValues.kontaktValuesDomainDefault      
                V001Input = String.Empty
                V002Input = String.Empty
                V003Input = String.Empty
                V004Input = String.Empty
                V005Input = String.Empty
                V006Input = String.Empty
                V007Input = String.Empty       
                Id = id
                DelayMsg = String.Empty
                ErrorMsg = String.Empty
            }
        model, Cmd.ofMsg SendOldKontaktValuesToServer

    let internal update (msg : Msg) (model : Model) : Model * Cmd<Msg> =

        match msg with
        | SetV001Input value -> { model with V001Input = value }, Cmd.none
        | SetV002Input value -> { model with V002Input = value }, Cmd.none
        | SetV003Input value -> { model with V003Input = value }, Cmd.none
        | SetV004Input value -> { model with V004Input = value }, Cmd.none
        | SetV005Input value -> { model with V005Input = value }, Cmd.none
        | SetV006Input value -> { model with V006Input = value }, Cmd.none
        | SetV007Input value -> { model with V007Input = value }, Cmd.none    

        | SendOldKontaktValuesToServer
            ->
             let loadEvent = SharedDeserialisedKontaktValues.transformLayer model.OldKontaktValues
             let cmd = Cmd.OfAsync.perform sendKontaktValuesApi.getOldKontaktValues loadEvent OldKontaktValues
             model, cmd

        | AsyncWorkIsComplete -> { model with DelayMsg = String.Empty }, Cmd.none 
        
        | SendKontaktValuesToServer
            ->
             try
                 try
                     let buttonClickEvent: KontaktValuesShared =

                         let input newValue oldValue = //I cannot enforce the value of the placeholder in any other way, TODO find a better way
                             match newValue with
                             | value
                                 when
                                     value = String.Empty -> oldValue
                             | _                          -> newValue
                                                
                         SharedKontaktValues.transformLayer   //sending model in the parameter would mean defining Model in Shared what would distort the MVU model
                         <| input model.V001Input model.OldKontaktValues.V001
                         <| input model.V002Input model.OldKontaktValues.V002
                         <| input model.V003Input model.OldKontaktValues.V003
                         <| input model.V004Input model.OldKontaktValues.V004
                         <| input model.V005Input model.OldKontaktValues.V005
                         <| input model.V006Input model.OldKontaktValues.V006
                         <| input model.V007Input model.OldKontaktValues.V007                              

                     //Cmd.OfAsyncImmediate instead of Cmd.OfAsync
                     let cmd = Cmd.OfAsyncImmediate.perform sendKontaktValuesApi.sendKontaktValues buttonClickEvent NewKontaktValues
                     let cmd2 (cmd : Cmd<Msg>) delayedDispatch = Cmd.batch <| seq { cmd; Cmd.ofSub delayedDispatch }

                     let delayedCmd (dispatch : Msg -> unit) : unit =                                                  
                         let delayedDispatch : Async<unit> =
                             async
                                 {
                                     let! completor = Async.StartChild (async { return dispatch SendOldKontaktValuesToServer })
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
                  
        | NewKontaktValues valueNew
            ->
             {
                 model with
                     KontaktValues =
                         {
                             V001 = valueNew.V001; V002 = valueNew.V002; V003 = valueNew.V003
                             V004 = valueNew.V004; V005 = valueNew.V005; V006 = valueNew.V006
                             V007 = valueNew.V007; Msgs = valueNew.Msgs
                         }
                     ErrorMsg = 
                         let (p1, p2, p3) = compare valueNew.Msgs.Msg1 valueNew.Msgs.Msg2 valueNew.Msgs.Msg3
                         removeSpaces <| sprintf "%s %s %s" p1 p2 p3 
             }, Cmd.none

        | OldKontaktValues valueOld
            ->
             {
                 model with
                     OldKontaktValues =
                         {
                             V001 = valueOld.V001; V002 = valueOld.V002; V003 = valueOld.V003
                             V004 = valueOld.V004; V005 = valueOld.V005; V006 = valueOld.V006
                             V007 = valueOld.V007; Msgs = valueOld.Msgs
                         }
                     ErrorMsg =
                         let (p1, p2, p3) = compare valueOld.Msgs.Msg1 valueOld.Msgs.Msg2 valueOld.Msgs.Msg3  
                         removeSpaces <| sprintf "%s %s %s" p1 p2 p3  
             }, Cmd.none  
   
    let internal view (model : Model) (dispatch : Msg -> unit) =        

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
                            prop.action (MaximeRouter.Router.toHash (MaximeRouter.Router.CMSKontakt 8)) 
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
                                                                    prop.placeholder model.OldKontaktValues.V001
                                                                    prop.onChange (SetV001Input >> dispatch)
                                                                    //prop.onChange (fun (ev: string) -> SetV001Input ev |> dispatch)     
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
                                                                    prop.placeholder model.OldKontaktValues.V002
                                                                    prop.onChange (SetV002Input >> dispatch)
                                                                    //prop.onChange (fun (ev: string) -> SetV002Input ev |> dispatch)
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
                                                                    prop.placeholder model.OldKontaktValues.V003
                                                                    prop.onChange (SetV003Input >> dispatch)
                                                                    //prop.onChange (fun (ev: string) -> SetV003Input ev |> dispatch)
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
                                                                    prop.placeholder model.OldKontaktValues.V004
                                                                    prop.onChange (SetV004Input >> dispatch)
                                                                    //prop.onChange (fun (ev: string) -> SetV004Input ev |> dispatch)
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
                                                                    prop.placeholder model.OldKontaktValues.V005
                                                                    prop.onChange (SetV005Input >> dispatch)
                                                                    //prop.onChange (fun (ev: string) -> SetV005Input ev |> dispatch) 
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
                                                                    prop.placeholder model.OldKontaktValues.V006
                                                                    prop.onChange (SetV006Input >> dispatch)
                                                                    //prop.onChange (fun (ev: string) -> SetV006Input ev |> dispatch)
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
                                                                    prop.placeholder model.OldKontaktValues.V007 
                                                                    prop.onChange (SetV007Input >> dispatch)
                                                                    //prop.onChange (fun (ev: string) -> SetV007Input ev |> dispatch)
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
                                                        Html.td [
                                                            prop.style
                                                                [                                                               
                                                                    style.fontFamily "sans-serif"
                                                                    style.fontWeight.bold
                                                                ]
                                                            prop.children
                                                                [
                                                                     Html.text "Použij mezerník pro generování prázdného řádku"
                                                                ]
                                                        ]    
                                                        yield! td 3                                     
                                                    ]
                                                ]                                      
                                                Html.tr [
                                                    prop.style
                                                        [
                                                            //style.marginLeft(0)                                                       
                                                        ] 
                                                    prop.children [    
                                                        Html.td
                                                            [
                                                                prop.style
                                                                    [                                                                    
                                                                        style.visibility.hidden
                                                                    ]
                                                                prop.children
                                                                    [                                                            
                                                                        Html.text "*********"
                                                                    ]                                                                                                                
                                                            ]    
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
                                                        Html.td
                                                            [
                                                                prop.style
                                                                    [                                                                    
                                                                        style.visibility.hidden
                                                                    ]
                                                                prop.children
                                                                    [                                                            
                                                                        Html.text "********************"
                                                                    ]                                                                                            
                                                            ]    
                                                        Html.td [
                                                            Html.input [
                                                                prop.type' "submit"
                                                                prop.value "Uložit nové údaje"
                                                                prop.id "Button1"
                                                                prop.onClick
                                                                    <|
                                                                    fun e ->
                                                                           e.preventDefault()
                                                                           dispatch SendKontaktValuesToServer
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