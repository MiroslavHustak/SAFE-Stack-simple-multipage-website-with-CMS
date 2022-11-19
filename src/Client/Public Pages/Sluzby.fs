module Sluzby

open Elmish
open Feliz
open Fable.Remoting.Client

open Shared

open Layout
open Records
open ContentSluzby

type Model =
    {      
        Id: int       
    }

type Msg =
    | DummyMsg

let todosApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IGetApi>

let init id : Model * Cmd<Msg> =
    let model = { Id = id }
    model, Cmd.none

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =  model, Cmd.none
   

let view (model: Model) (dispatch: Msg -> unit) links =

    let sluzbyRecord =
       {
         Home = prop.className "normal"
         Sluzby = prop.className "current"
         Cenik = prop.className "normal"
         Nenajdete = prop.className "normal"
         Kontakt = prop.className "normal"
       }

    layout <| contentSluzby() <| sluzbyRecord <| links
