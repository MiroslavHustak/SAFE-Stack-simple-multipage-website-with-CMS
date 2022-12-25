namespace PublicPages

open Elmish
open Feliz
open Fable.Remoting.Client

open Shared

open HtmlFeliz.Layout
open Records.Client
open HtmlFeliz.ContentNenajdete

module Nenajdete = 

    type Model =
        {      
            Id: int
        }

    type Msg =
        | DummyMsg

    let init id: Model * Cmd<Msg> =
        let model = { Id = id }
        model, Cmd.none

    let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =  model, Cmd.none

    let view (model: Model) (dispatch: Msg -> unit) links =

        let nenajdeteRecord =
           {
               Home = prop.className "normal"
               Sluzby = prop.className "normal"
               Cenik = prop.className "normal"
               Nenajdete = prop.className "current"
               Kontakt = prop.className "normal"
           }
           
        layout <| contentNenajdete() <| nenajdeteRecord <| links



