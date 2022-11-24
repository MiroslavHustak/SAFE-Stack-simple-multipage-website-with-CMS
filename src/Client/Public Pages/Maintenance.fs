module Maintenance

open Elmish
open Feliz
open Fable.Remoting.Client

open Shared

open Layout
open Records
open ContentMaintenance

type Model =
    {        
      Dummy: unit      
    }

type Msg =
    | DummyMsgText   

let init () : Model * Cmd<Msg> =
    let model = { Dummy = () } 
    model, Cmd.none

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =  model, Cmd.none

let view (model: Model) (dispatch: Msg -> unit) = contentMaintenance()


    