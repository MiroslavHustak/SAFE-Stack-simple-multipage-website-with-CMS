namespace PublicPages

open System

open Feliz
open Elmish
open Fable.Remoting.Client

open Shared

open Records.Client
open HtmlFeliz.Layout
open Helpers.Client.Helper

open Feliz
open Elmish
open Fable.Remoting.Client

open NotUsedPages.ContentMaintenance

module ErrorPage = 

    type Model =
        {        
          Dummy : unit      
        }

    type Msg =
        | DummyMsgText   

    let internal init () : Model * Cmd<Msg> =
        let model = { Dummy = () } 
        model, Cmd.none

    let internal update (msg : Msg) (model : Model) : Model * Cmd<Msg> = model, Cmd.none

    let internal view (model : Model) (dispatch : Msg -> unit) = contentMaintenance()




    

