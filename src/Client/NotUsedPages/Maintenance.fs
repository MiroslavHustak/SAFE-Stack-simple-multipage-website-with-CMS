namespace NotUsedPages

open Feliz
open Elmish
open Fable.Remoting.Client

open NotUsedPages.ContentMaintenance

module Maintenance = 

    type Model =
        {        
          Dummy: unit      
        }

    type Msg =
        | DummyMsgText   

    let internal init () : Model * Cmd<Msg> =
        let model = { Dummy = () } 
        model, Cmd.none

    let internal update (msg : Msg) (model : Model) : Model * Cmd<Msg> = model, Cmd.none

    let internal view (model : Model) (dispatch : Msg -> unit) = contentMaintenance()


    