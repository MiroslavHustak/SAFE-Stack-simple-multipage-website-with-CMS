namespace PublicPages

open System

open Elmish
open Feliz
open Fable.Remoting.Client

open Shared
open SharedTypes

open Records.Client
open HtmlFeliz.Layout
open Auxiliaries.Client.Helper

module Kontakt =

    type Model =
        {
            KontaktValues: KontaktValuesDomain
            KontaktInputValues: KontaktValuesDomain
            ErrorMsg: string
            Id: int
        }

    type Msg =   
        | AskServerForKontaktValues 
        | NewKontaktValues of KontaktValuesDomain    

    let private getDeserialisedKontaktValuesApi =
        Remoting.createApi ()
        |> Remoting.withRouteBuilder Route.builder
        |> Remoting.buildProxy<IGetApi>

    let init id : Model * Cmd<Msg> =
       
        let model =        
            {
                KontaktValues = KontaktValuesDomain.Default        
                KontaktInputValues = KontaktValuesDomain.Default
                ErrorMsg = String.Empty
                Id = id
            }
        model, Cmd.ofMsg AskServerForKontaktValues

    let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
        
        match msg with       
            | AskServerForKontaktValues ->
                 let loadEvent = SharedDeserialisedKontaktValues.create model.KontaktInputValues
                 let cmd = Cmd.OfAsync.perform getDeserialisedKontaktValuesApi.getDeserialisedKontaktValues loadEvent NewKontaktValues
                 model, cmd            
            | NewKontaktValues value -> { model with KontaktValues =
                                                        {
                                                            V001 = value.V001; V002 = value.V002; V003 = value.V003;
                                                            V004 = value.V004; V005 = value.V005; V006 = value.V006;
                                                            V007 = value.V007; Msgs = value.Msgs 
                                                        }
                                                     ErrorMsg = sprintf "%s %s %s" value.Msgs.Msg1 value.Msgs.Msg2 value.Msgs.Msg3
                                        }, Cmd.none    
 
    let view (model: Model) (dispatch: Msg -> unit) links =
    
        let kontaktRecord =
           {
               Home = prop.className "normal"
               Sluzby = prop.className "normal"
               Cenik = prop.className "normal"
               Nenajdete = prop.className "normal"
               Kontakt = prop.className "current"
           }

        let kontaktHtml =

            javaScriptMessageBox model.ErrorMsg
    
            Html.div [
                prop.id "templatemo_content"
                prop.children [
                    Html.h2 [
                        prop.id 16
                        prop.children [
                            Html.text "Kontakt na ambulanci nutriční terapie"
                        ]
                    ]
                    Html.div [
                        Html.p [
                            prop.id 17
                            prop.style
                                [
                                  style.padding(0)
                                ]
                            prop.children [
                                Html.text model.KontaktValues.V001
                            ]
                        ]
                        Html.p [
                            prop.id 18
                            prop.style
                                [
                                  style.padding(0)
                                ]
                            prop.children [
                                Html.text model.KontaktValues.V002
                            ]
                        ]
                        Html.p [
                            prop.id 19
                            prop.style
                                [
                                  style.padding(0)
                                ]
                            prop.children [
                                Html.text model.KontaktValues.V003                    
                            ]
                        ]
                        Html.p [
                            prop.id 20
                            prop.style
                                [
                                  style.padding(0)
                                ]
                            prop.children [
                                Html.text model.KontaktValues.V004                
                            ]
                        ]
                        Html.p [
                            prop.id 21
                            prop.style
                                [
                                  style.padding(0)
                                ]
                            prop.children [
                                Html.text model.KontaktValues.V005                            
                            ]
                        ]
                        Html.p [
                            prop.id 21
                            prop.style
                                [
                                  style.padding(0)
                                ]
                            prop.children [
                                Html.text model.KontaktValues.V006                            
                            ]
                        ]
                        Html.p [
                            prop.id 21
                            prop.style
                               [
                                 style.padding(0)
                               ]
                            prop.children [
                               Html.text model.KontaktValues.V007                            
                            ]
                        ]
                    ]
                    Html.div [
                        prop.className "body2"
                        prop.children [
                            Html.p [
                                prop.className "editable"
                                prop.id 24
                                prop.style
                                    [
                                      style.padding(0)
                                    ]
                                prop.children [
                                    Html.text "-----------------------------------------------------------------------------------------"
                                ]
                            ]
                            Html.p [
                                prop.className "editable"
                                prop.id 25
                                prop.style
                                    [
                                      style.padding(0)
                                    ]
                                prop.children [
                                     Html.text "IČ: 02021820"
                                     Html.br []
                                ]
                            ]
                            Html.p [
                                prop.className "editable"
                                prop.id 26
                                prop.style
                                    [
                                        style.padding(0)
                                    ]
                                prop.children [
                                    Html.text "Fyzická osoba zapsaná v Živnostenském rejstříku od 10.10.2012"
                                    Html.br []
                                ]
                            ]
                            Html.p [
                                prop.className "editable"
                                prop.id 27
                                prop.style
                                    [
                                        style.padding(0)
                                    ]
                                prop.children [
                                    Html.text "Předmět podnikání: Výroba, obchod a služby neuvedené v přílohách 1 až 3 živnostenského zákona"       
                                    Html.br []
                                ]
                            ]
                            Html.p [
                                prop.className "editable"
                                prop.id 28
                                prop.style
                                    [
                                      style.padding(0)
                                    ]
                                prop.children [
                                    Html.text "Vznik oprávnění dne 21.08.2013"
                                    Html.br []
                                ]
                            ]
                        ]
                    ]
                    Html.div [
                        prop.className "cleaner_h50"
                    ]
                    Html.div [
                        Html.img [
                            prop.src "/Content/images/Telefon.png"
                            prop.width 340
                            prop.height 279
                            prop.alt "telefon"
                        ]
                    ]
                ]
            ]     
    
        layout <| kontaktHtml <| kontaktRecord <| links