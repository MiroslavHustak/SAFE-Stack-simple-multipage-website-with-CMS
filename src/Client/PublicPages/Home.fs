module Home

open System

open Feliz
open Elmish
open Fable.Remoting.Client

open Records

open Layout
open ContentHome

type Model =
    {        
        Dummy: unit
        ErrorMsg: string
    }

type Msg =
    | DummyMsgText   

let init () : Model * Cmd<Msg> =
    let model =
        {
            Dummy = ()
            ErrorMsg = String.Empty
        } 
    model, Cmd.none

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> = model, Cmd.none

let view (model: Model) (dispatch: Msg -> unit) links =

   let homeRecord =
      {
          Home = prop.className "current"
          Sluzby = prop.className "normal"
          Cenik = prop.className "normal"
          Nenajdete = prop.className "normal"
          Kontakt = prop.className "normal"
      }

   layout <| contentHome() <| homeRecord <| links
   