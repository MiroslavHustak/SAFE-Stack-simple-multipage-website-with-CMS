module CMSRozcestnik

open Elmish
open Feliz
open Fable.Remoting.Client

open Shared

open Layout
open Records
open ContentCMSRozcestnik

type Model =
    {        
      Id: int
      //Dummy: unit         
    }

type Msg =
    | DummyMsgText 

let todosApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IGetApi>

let init id : Model * Cmd<Msg> =
    let model = { Id = id }
    //let model = { Dummy = () } 
    model, Cmd.none

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =  model, Cmd.none

let view (model: Model) (dispatch: Msg -> unit) = contentCMSRozcestnik() 

