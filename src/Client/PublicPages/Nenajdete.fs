namespace PublicPages

open Feliz
open Elmish
open Fable.Remoting.Client

open Records.Client
open HtmlFeliz.Layout

module Nenajdete = 

    type Model =
        {      
            Id: int
        }

    type Msg =
        | DummyMsg

    let internal init id: Model * Cmd<Msg> =

        let model = { Id = id }
        model, Cmd.none

    let internal update (msg: Msg) (model: Model) : Model * Cmd<Msg> = model, Cmd.none

    let internal view (model: Model) (dispatch: Msg -> unit) links =

        let nenajdeteRecord =
           {
               Home = prop.className "normal"
               Sluzby = prop.className "normal"
               Cenik = prop.className "normal"
               Nenajdete = prop.className "current"
               Kontakt = prop.className "normal"
           }

        let contentNenajdete() =
        
            Html.div [
                prop.id "templatemo_content"
                prop.children [
                    Html.h1 [
                        prop.id 14
                        prop.text "Co u mě nenajdete"
                    ]
                    Html.p [
                        prop.id 15
                        prop.children [
                            Html.text "Nepracuji při hubnutí s žádnými podpůrnými prostředky a doplňky typu \"zázračných\" pilulek a koktejlů, které se v dnešní době na trhu vyskytují ve velkém množství. Tato nutriční ambulance pracuje s běžnou a dostupnou stravou."                    
                        ]
                    ]
                    Html.br []
                    Html.div [
                        Html.img [
                            prop.src "/Content/images/Tabletky.jpg"
                            prop.width 398
                            prop.height 280
                            prop.alt "tabletky"
                        ]
                    ]
                ]
            ]  
           
        layout <| contentNenajdete() <| nenajdeteRecord <| links



