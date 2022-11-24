module Sluzby

open Elmish
open Feliz
open Fable.Remoting.Client

open Shared

open Layout
open Records
open ContentSluzby
open ContentMaintenance

type Model =
    {      
        Id: int       
    }

type Msg =
    | DummyMsg

let init id : Model * Cmd<Msg> =
    let model = { Id = id }
    model, Cmd.none

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =  model, Cmd.none
   

let view (model: Model) (dispatch: Msg -> unit) links securityToken =

    let sluzbyRecord =
       {
         Home = prop.className "normal"
         Sluzby = prop.className "current"
         Cenik = prop.className "normal"
         Nenajdete = prop.className "normal"
         Kontakt = prop.className "normal"
       }

    match securityToken with
    | "securityToken" -> layout <| contentSluzby() <| sluzbyRecord <| links //contentMaintenance()
    | _ -> layout <| contentSluzby() <| sluzbyRecord <| links
    
