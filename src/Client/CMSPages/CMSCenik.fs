namespace CMSPages

open System

open Feliz
open Elmish
open FSharp.Control
open Fable.Remoting.Client

open Shared

open Helpers.Client.Helper

module CMSCenik = 

    //Common model to view / from view
    type Model =
        {
            //***** ClientDtoToView *********
            CenikValues: CenikValuesShared
            OldCenikValues: CenikValuesShared
            //******************************

            //***** ClientDtoFromView *********
            V001Input: string
            V002Input: string
            V003Input: string
            V004Input: string
            V005Input: string
            V006Input: string
            V007Input: string
            V008Input: string
            V009Input: string
            //******************************

            Id: int
            DelayMsg: string
            ErrorMsg: string
        }

    type Msg =
        | SetV001Input of string
        | SetV002Input of string
        | SetV003Input of string
        | SetV004Input of string
        | SetV005Input of string
        | SetV006Input of string
        | SetV007Input of string
        | SetV008Input of string
        | SetV009Input of string   
        | SendCenikValuesToServer
        | SendOldCenikValuesToServer
        | NewCenikValues of CenikValuesShared
        | OldCenikValues of CenikValuesShared
        | AsyncWorkIsComplete 
    
    let private sendCenikValuesApi =
        Remoting.createApi ()
        |> Remoting.withRouteBuilder Route.builder
        |> Remoting.buildProxy<IGetApi>

    let init id : Model * Cmd<Msg> =

        let model =
            {
                CenikValues = SharedCenikValues.cenikValuesDomainDefault           
                OldCenikValues = SharedCenikValues.cenikValuesDomainDefault
                V001Input = String.Empty
                V002Input = String.Empty
                V003Input = String.Empty
                V004Input = String.Empty
                V005Input = String.Empty
                V006Input = String.Empty
                V007Input = String.Empty
                V008Input = String.Empty
                V009Input = String.Empty 
                Id = id
                DelayMsg = String.Empty
                ErrorMsg = String.Empty
            }
        model, Cmd.ofMsg SendOldCenikValuesToServer

    let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =

        match msg with
        //SetInput takhle nelze - viz poznamka ve view
        //| SetInput value -> { model with CenikInputValues = { V001 = value.V001; ....atd. } }, Cmd.none
        | SetV001Input value -> { model with V001Input = value }, Cmd.none
        | SetV002Input value -> { model with V002Input = value }, Cmd.none
        | SetV003Input value -> { model with V003Input = value }, Cmd.none
        | SetV004Input value -> { model with V004Input = value }, Cmd.none
        | SetV005Input value -> { model with V005Input = value }, Cmd.none
        | SetV006Input value -> { model with V006Input = value }, Cmd.none
        | SetV007Input value -> { model with V007Input = value }, Cmd.none
        | SetV008Input value -> { model with V008Input = value }, Cmd.none
        | SetV009Input value -> { model with V009Input = value }, Cmd.none
    
        | SendOldCenikValuesToServer
            ->   
             let loadEvent = SharedDeserialisedCenikValues.transformLayer model.OldCenikValues
             let cmd = Cmd.OfAsync.perform sendCenikValuesApi.getOldCenikValues loadEvent OldCenikValues  
             model, cmd

        | AsyncWorkIsComplete -> { model with DelayMsg = String.Empty }, Cmd.none 

        | SendCenikValuesToServer
            ->
             try
                 try
                     let buttonClickEvent: CenikValuesShared =                   
                         
                         SharedCenikValues.transformLayer  //sending model in the parameter would mean defining Model in Shared what would distort the MVU model
                         <| SharedCenikValues.cenikValuesDomainDefault.Id
                         <| SharedCenikValues.cenikValuesDomainDefault.ValueState //whatever Id and Value State
                         <| model.V001Input
                         <| model.V002Input
                         <| model.V003Input
                         <| model.V004Input
                         <| model.V005Input
                         <| model.V006Input
                         <| model.V007Input
                         <| model.V008Input
                         <| model.V009Input

                     //Cmd.OfAsyncImmediate instead of Cmd.OfAsync
                     let cmd = Cmd.OfAsyncImmediate.perform sendCenikValuesApi.sendCenikValues buttonClickEvent NewCenikValues 
                     let cmd2 (cmd: Cmd<Msg>) delayedCmd = Cmd.batch <| seq { cmd; Cmd.ofSub delayedCmd }               

                     let delayedCmd (dispatch: Msg -> unit): unit =                    
                         let delayedDispatch: Async<unit> =                      
                             async
                                 {
                                     let! completor = Async.StartChild (async { return dispatch SendOldCenikValuesToServer } ) 
                                     let! result = completor
                                     do! Async.Sleep 1000 //see the Elmish Book
                                     dispatch AsyncWorkIsComplete           
                                 }
                         Async.StartImmediate delayedDispatch
                     { model with DelayMsg = "Probíhá načítání ... " }, cmd2 cmd delayedCmd  //cmd shall be performed first, delayedCmd shall be performed second; hence Cmd.OfAsyncImmediate instead of Cmd.OfAsync         
                 finally
                 ()   
             with
             | ex -> { model with ErrorMsg = sprintf "Nedošlo k načtení hodnot. Popis chyby: %s " (string ex) }, Cmd.none   

        | NewCenikValues valueNew
            ->
             {
                 model with
                           CenikValues =
                              {
                                  Id = valueNew.Id; ValueState = valueNew.ValueState;
                                  V001 = valueNew.V001; V002 = valueNew.V002; V003 = valueNew.V003;
                                  V004 = valueNew.V004; V005 = valueNew.V005; V006 = valueNew.V006;
                                  V007 = valueNew.V007; V008 = valueNew.V008; V009 = valueNew.V009;
                                  Msgs = valueNew.Msgs
                              }                           
                           ErrorMsg =
                               let (p1, p2, p3) = compare valueNew.Msgs.Msg1 valueNew.Msgs.Msg2 valueNew.Msgs.Msg3   
                               removeSpaces <| sprintf "%s %s %s" p1 p2 p3
             },  Cmd.none

        | OldCenikValues valueOld
            ->    
             {
                 model with
                           OldCenikValues =
                              {
                                  Id = valueOld.Id; ValueState = valueOld.ValueState;
                                  V001 = valueOld.V001; V002 = valueOld.V002; V003 = valueOld.V003;
                                  V004 = valueOld.V004; V005 = valueOld.V005; V006 = valueOld.V006;
                                  V007 = valueOld.V007; V008 = valueOld.V008; V009 = valueOld.V009;
                                  Msgs = valueOld.Msgs 
                              }
                           ErrorMsg =
                               let (p1, p2, p3) = compare valueOld.Msgs.Msg1 valueOld.Msgs.Msg2 valueOld.Msgs.Msg3  
                               removeSpaces <| sprintf "%s %s %s" p1 p2 p3                      
             },  Cmd.none

    let view (model: Model) (dispatch: Msg -> unit) =        

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
                            Html.link [
                                prop.href "/Content/css_modal/modaldialog.css"
                                prop.rel "stylesheet"
                            ]
                        ]
                    ]
                    Html.body [
                        Html.form [                    
                            prop.method "get"
                            prop.action (MaximeRouter.Router.toHash (MaximeRouter.Router.CMSCenik 7))                        
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
                                                            Html.h1 "Editace hodnot v ceníku"
                                                            Html.h3 "Hodnoty jsou v Kč"
                                                        ]
                                                    ]
                                                ]                                                                             
                                                Html.tr [                                              
                                                    prop.children
                                                        [
                                                            //let studyIt = yield! [ for item in 1..7 do Html.td [] ] |> List.map (fun item -> item)
                                                            //let studyIt = [ yield! ( [ 1..7 ] |> List.map (fun _ -> Html.td []) |> List.ofSeq ) ] //zajimava konstrukce quli yield
                                                            yield! td 7 
                                                        ]  
                                                        //zkusebni kod
                                                        (*
                                                        let myList = [ Html.text "001"; Html.text "002"; Html.text "003"; Html.text "004"; Html.text "005" ]
                                                        yield! [
                                                            for item in myList do
                                                                Html.text ", "
                                                                item
                                                        ] |> List.tail //quli carky, kera je prvni                                                   
                                                        *)                                                    
                                                ]                                        
                                                Html.tr [
                                                    prop.style
                                                        [
                                                            style.fontWeight.bold
                                                        ] 
                                                    prop.children [
                                                        Html.td "Název položky v ceníku"                                                    
                                                        Html.td "Prvotní hodnoty"                                               
                                                        Html.td []
                                                        Html.td "Současnost"
                                                        Html.td []
                                                        Html.td []
                                                        Html.td "Zadání nových hodnot"
                                                    ]
                                                ]
                                                Html.tr [                                                
                                                    prop.children [
                                                        Html.td "Vstupní vyšetření"
                                                    
                                                        Html.td "500"                                              
                                                        Html.td []
                                                        Html.td
                                                            [
                                                            prop.style
                                                                [
                                                                    style.fontWeight.bold
                                                                    style.color.blue
                                                                ]
                                                            prop.children
                                                                [                                                            
                                                                     Html.text model.OldCenikValues.V001 
                                                                ]   
                                                            ]
                                                        Html.td []
                                                        Html.td []
                                                        Html.td
                                                            [
                                                                Html.input [
                                                                    prop.id "content"
                                                                    prop.type' "text"
                                                                    prop.id "001"
                                                                    prop.name ""
                                                                    prop.placeholder model.OldCenikValues.V001 
                                                                    prop.onChange (SetV001Input >> dispatch) 
                                                                    //prop.onChange (fun (ev: string) -> SetV001Input ev |> dispatch)    
                                                                    //nasledujici nelze, bo event nemoze byt takeho typu, bohuzel
                                                                    //prop.onChange (fun (ev: GetCenikValues) ->  SetInput ev.V001 |> dispatch)                                                                
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
                                                        Html.td "Asistovaný nákup"
                                                        Html.td "300"                                                  
                                                        Html.td []
                                                        Html.td
                                                            [
                                                            prop.style
                                                                [
                                                                    style.fontWeight.bold
                                                                    style.color.blue
                                                                ]
                                                            prop.children
                                                                [                                                            
                                                                     Html.text model.OldCenikValues.V002 
                                                                ]   
                                                            ]
                                                        Html.td []
                                                        Html.td []
                                                        Html.td
                                                            [
                                                                Html.input [
                                                                    prop.type' "text"
                                                                    prop.id "002"
                                                                    prop.name ""
                                                                    prop.placeholder model.OldCenikValues.V002
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
                                                        Html.td "Redukční balíček"
                                                        Html.td "2500"                                                    
                                                        Html.td []
                                                        Html.td
                                                            [
                                                            prop.style
                                                                [
                                                                    style.fontWeight.bold
                                                                    style.color.blue
                                                                ]
                                                            prop.children
                                                                [                                                            
                                                                     Html.text model.OldCenikValues.V003 
                                                                ]   
                                                            ]
                                                        Html.td []
                                                        Html.td []
                                                        Html.td
                                                            [
                                                                Html.input [
                                                                    prop.type' "text"
                                                                    prop.id "003"
                                                                    prop.name ""
                                                                    prop.placeholder model.OldCenikValues.V003 
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
                                                        Html.td "Kontrolní konzultace"
                                                        Html.td "250"                                                    
                                                        Html.td []
                                                        Html.td
                                                            [
                                                            prop.style
                                                                [
                                                                    style.fontWeight.bold
                                                                    style.color.blue
                                                                ]
                                                            prop.children
                                                                [                                                            
                                                                     Html.text model.OldCenikValues.V004 
                                                                ]   
                                                            ]
                                                        Html.td []
                                                        Html.td []
                                                        Html.td
                                                            [
                                                                Html.input [
                                                                    prop.type' "text"
                                                                    prop.id "004"
                                                                    prop.name ""
                                                                    prop.placeholder model.OldCenikValues.V004
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
                                                        Html.td "Sestavení jídelního lístku na týden"
                                                        Html.td @"N/A"                                                     
                                                        Html.td []
                                                        Html.td
                                                            [
                                                            prop.style
                                                                [
                                                                    style.fontWeight.bold
                                                                    style.color.blue
                                                                ]
                                                            prop.children
                                                                [                                                            
                                                                     Html.text model.OldCenikValues.V005 
                                                                ]   
                                                            ]
                                                        Html.td []
                                                        Html.td []
                                                        Html.td
                                                            [
                                                                Html.input [
                                                                    prop.type' "text"
                                                                    prop.id "005"
                                                                    prop.name ""
                                                                    prop.placeholder model.OldCenikValues.V005
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
                                                        Html.td "Sestavení jídelního lístku na 2 týdny"
                                                        Html.td @"N/A"
                                                        Html.td []
                                                        Html.td
                                                            [
                                                            prop.style
                                                                [
                                                                    style.fontWeight.bold
                                                                    style.color.blue
                                                                ]
                                                            prop.children
                                                                [                                                            
                                                                     Html.text model.OldCenikValues.V006 
                                                                ]   
                                                            ]
                                                        Html.td []
                                                        Html.td []
                                                        Html.td
                                                            [
                                                                Html.input [
                                                                    prop.type' "text"
                                                                    prop.id "006"
                                                                    prop.name ""
                                                                    prop.placeholder model.OldCenikValues.V006
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
                                                        Html.td "Sestavení jídelního lístku na 3 týdny"
                                                        Html.td @"N/A" 
                                                        Html.td []
                                                        Html.td
                                                            [
                                                            prop.style
                                                                [
                                                                    style.fontWeight.bold
                                                                    style.color.blue
                                                                ]
                                                            prop.children
                                                                [                                                            
                                                                     Html.text model.OldCenikValues.V007 
                                                                ]   
                                                            ]
                                                        Html.td []
                                                        Html.td []
                                                        Html.td
                                                            [
                                                                Html.input [
                                                                    prop.type' "text"
                                                                    prop.id "007"
                                                                    prop.name ""
                                                                    prop.placeholder model.OldCenikValues.V007
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
                                                            //style.fontWeight.bold                                                       
                                                        ] 
                                                    prop.children [
                                                        Html.td "Edukace diety (diabetologie)"
                                                        Html.td "450" 
                                                        Html.td []
                                                        Html.td
                                                            [
                                                            prop.style
                                                                [
                                                                    style.fontWeight.bold
                                                                    style.color.blue
                                                                ]
                                                            prop.children
                                                                [                                                            
                                                                     Html.text model.OldCenikValues.V008 
                                                                ]   
                                                            ]
                                                        Html.td []
                                                        Html.td []
                                                        Html.td
                                                            [
                                                                Html.input [
                                                                    prop.type' "text"
                                                                    prop.id "008"
                                                                    prop.name ""
                                                                    prop.placeholder model.OldCenikValues.V008
                                                                    prop.onChange (SetV008Input >> dispatch)
                                                                    //prop.onChange (fun (ev: string) -> SetV008Input ev |> dispatch)
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
                                                        Html.td "Edukace diety (dietologie)"
                                                        Html.td "450"                                                    
                                                        Html.td []
                                                        Html.td
                                                            [
                                                            prop.style
                                                                [
                                                                    style.fontWeight.bold
                                                                    style.color.blue
                                                                ]
                                                            prop.children
                                                                [                                                            
                                                                     Html.text model.OldCenikValues.V009  
                                                                ]   
                                                            ]
                                                        Html.td []
                                                        Html.td []
                                                        Html.td
                                                            [
                                                                Html.input [
                                                                    prop.type' "text"
                                                                    prop.id "009"
                                                                    prop.name ""
                                                                    prop.placeholder model.OldCenikValues.V009
                                                                    prop.onChange (SetV009Input >> dispatch)
                                                                    //prop.onChange (fun (ev: string) -> SetV009Input ev |> dispatch)
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
                                                        yield! td 3                                                        
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
                                                        yield! td 3
                                                    ]
                                                ]                                      
                                                Html.tr [
                                                    prop.style
                                                        [
                                                            //style.marginLeft(0)                                                       
                                                        ] 
                                                    prop.children [
                                                        Html.td [ ]                                                   
                                                        Html.td [
                                                            Html.a [
                                                                prop.style
                                                                    [
                                                                        style.width(200)
                                                                        style.fontFamily "sans-serif"
                                                                        style.fontWeight.bold
                                                                    ]
                                                                prop.href (MaximeRouter.Router.toHash (MaximeRouter.Router.Route.CMSRozcestnik 6)) //"#cmsRozcestnik/6"
                                                                prop.children [
                                                                    Html.text "Návrat na rozcestník"
                                                                ]
                                                            ]
                                                        ]
                                                        Html.td []
                                                        Html.td [
                                                            prop.style
                                                                [
                                                                    style.fontWeight.bold
                                                                    style.fontSize(14) 
                                                                    style.color.blue
                                                                    style.fontFamily "sans-serif"
                                                                    style.visibility.hidden
                                                                ]
                                                            prop.children
                                                                [                                                            
                                                                    Html.text "**********************" 
                                                                ]                                                                                                                
                                                                ]
                                                        Html.td [
                                                            prop.style
                                                                [
                                                                    style.visibility.hidden
                                                                ]
                                                            prop.children
                                                                [                                                            
                                                                     Html.text "******" 
                                                                ]                                                                                                                
                                                        ]
                                                        Html.td []
                                                        Html.td [
                                                            Html.input [
                                                                prop.type' "submit"
                                                                prop.value "Uložit nové hodnoty"
                                                                prop.id "Button1"
                                                                prop.onClick (fun e -> e.preventDefault()
                                                                                       dispatch SendCenikValuesToServer
                                                                             )
                                                                prop.style
                                                                    [
                                                                      style.width(200)
                                                                      style.fontWeight.bold
                                                                      style.fontSize(16) 
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