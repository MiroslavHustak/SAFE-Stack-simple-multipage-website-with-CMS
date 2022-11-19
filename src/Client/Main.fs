//Code for routing taken from StackOverflow:
//https://stackoverflow.com/questions/54970180/how-can-i-do-a-simple-elmish-router
//Created by Maxime Mangel:
//https://stackoverflow.com/users/2911775/maxime-mangel

module App

open Feliz
open Elmish
open Fable.React

open Layout
open Router
open ContentHome
open SharedRecords
open Fable.Remoting.Client
open Shared

type Page =
    | Home of Home.Model
    | Sluzby of Sluzby.Model
    | Cenik of Cenik.Model
    | Nenajdete of Nenajdete.Model
    | Kontakt of Kontakt.Model
    | Login of Login.Model    
    | CMSRozcestnik of CMSRozcestnik.Model
    | CMSCenik of CMSCenik.Model
    | CMSKontakt of CMSKontakt.Model
    | CMSLink of CMSLink.Model
    | NotFound

type Model =
    {
        ActivePage : Page
        CurrentRoute : Router.Route option
        LinkAndLinkNameValues: GetLinkAndLinkNameValues
        LinkAndLinkNameInputValues: GetLinkAndLinkNameValues   
    }

type Msg =
    | HomeMsg of Home.Msg
    | SluzbyMsg of Sluzby.Msg
    | CenikMsg of Cenik.Msg
    | NenajdeteMsg of Nenajdete.Msg
    | KontaktMsg of Kontakt.Msg
    | LoginMsg of Login.Msg    
    | CMSRozcestnikMsg of CMSRozcestnik.Msg
    | CMSCenikMsg of CMSCenik.Msg
    | CMSKontaktMsg of CMSKontakt.Msg
    | CMSLinkMsg of CMSLink.Msg
    | SendLinkAndLinkNameValuesToServer 
    | GetLinkAndLinkNameValues of GetLinkAndLinkNameValues

let sendDeserialisedLinkAndLinkNameValues =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IGetApi>

let private setRoute (optRoute: Router.Route option) model =

    let model = { model with CurrentRoute = optRoute }

    //pokud potrebujeme aktivovat neco uz pri spusteni stranek
    let cmd fn cmd msg = Cmd.batch <| seq { Cmd.map fn cmd; Cmd.ofMsg msg }

    match optRoute with
    | None ->
        { model with ActivePage = Page.NotFound }, Cmd.none

    | Some Router.Route.Home ->
        let (homeModel, homeCmd) = Home.init ()
        { model with ActivePage = Page.Home homeModel }, cmd HomeMsg homeCmd SendLinkAndLinkNameValuesToServer

    | Some (Router.Route.Sluzby sluzbyId) ->
        let (sluzbyModel, sluzbyCmd) = Sluzby.init sluzbyId
        { model with ActivePage = Page.Sluzby sluzbyModel }, cmd SluzbyMsg sluzbyCmd SendLinkAndLinkNameValuesToServer

    | Some (Router.Route.Cenik cenikId) ->
        let (cenikModel, cenikCmd) = Cenik.init cenikId
        { model with ActivePage = Page.Cenik cenikModel }, cmd CenikMsg cenikCmd SendLinkAndLinkNameValuesToServer

    | Some (Router.Route.Nenajdete nenajdeteId) ->
        let (nenajdeteModel, nenajdeteCmd) = Nenajdete.init nenajdeteId
        { model with ActivePage = Page.Nenajdete nenajdeteModel }, cmd NenajdeteMsg nenajdeteCmd SendLinkAndLinkNameValuesToServer

    | Some (Router.Route.Kontakt kontaktId) ->
        let (kontaktModel, kontaktCmd) = Kontakt.init kontaktId
        { model with ActivePage = Page.Kontakt kontaktModel }, cmd KontaktMsg kontaktCmd SendLinkAndLinkNameValuesToServer

    | Some (Router.Route.Login loginId) ->
        let (loginModel, loginCmd) = Login.init loginId
        { model with ActivePage = Page.Login loginModel }, cmd LoginMsg loginCmd SendLinkAndLinkNameValuesToServer

    | Some (Router.Route.CMSRozcestnik cmsRozcestnikId) ->
        let (cmsRozcestnikModel, cmsRozcestnikCmd) = CMSRozcestnik.init cmsRozcestnikId
        { model with ActivePage = Page.CMSRozcestnik cmsRozcestnikModel }, Cmd.map CMSRozcestnikMsg cmsRozcestnikCmd

    | Some (Router.Route.CMSCenik cmsCenikId) ->
        let (cmsCenikModel, cmsCenikCmd) = CMSCenik.init cmsCenikId
        { model with ActivePage = Page.CMSCenik cmsCenikModel }, Cmd.map CMSCenikMsg cmsCenikCmd

    | Some (Router.Route.CMSKontakt cmsKontaktId) ->
        let (cmsKontaktModel, cmsKontaktCmd) = CMSKontakt.init cmsKontaktId
        { model with ActivePage = Page.CMSKontakt cmsKontaktModel }, Cmd.map CMSKontaktMsg cmsKontaktCmd

    | Some (Router.Route.CMSLink cmsLinkId) ->
        let (cmsLinkModel, cmsLinkCmd) = CMSLink.init cmsLinkId
        { model with ActivePage = Page.CMSLink cmsLinkModel }, Cmd.map CMSLinkMsg cmsLinkCmd

let init (location : Router.Route option) =
   
    setRoute location
        {
            ActivePage = Page.NotFound
            CurrentRoute = None          
            LinkAndLinkNameValues =
                {
                    V001 = ""; V002 = ""; V003 = "";
                    V004 = ""; V005 = ""; V006 = ""
                    V001n = ""; V002n = ""; V003n = "";
                    V004n = ""; V005n = ""; V006n = "Facebook"
                }
            LinkAndLinkNameInputValues =
                {
                    V001 = ""; V002 = ""; V003 = "";
                    V004 = ""; V005 = ""; V006 = ""
                    V001n = ""; V002n = ""; V003n = "";
                    V004n = ""; V005n = ""; V006n = "Facebook"
                }    
        }    

let update (msg : Msg) (model : Model) =
    match model.ActivePage, msg with
    | Page.NotFound, _ ->
        // Nothing to do here
        model, Cmd.none

    | Page.Home homeModel, HomeMsg homeMsg ->
        let (homeModel, homeCmd) = Home.update homeMsg homeModel       
        { model with ActivePage = Page.Home homeModel }, Cmd.map HomeMsg homeCmd

    | Page.Sluzby sluzbyModel, SluzbyMsg sluzbyMsg ->
        let (sluzbyModel, sluzbyCmd) = Sluzby.update sluzbyMsg sluzbyModel
        { model with ActivePage = Page.Sluzby sluzbyModel }, Cmd.map SluzbyMsg sluzbyCmd

    | Page.Cenik cenikModel, CenikMsg cenikMsg ->
        let (cenikModel, cenikCmd) = Cenik.update cenikMsg cenikModel
        { model with ActivePage = Page.Cenik cenikModel }, Cmd.map CenikMsg cenikCmd

    | Page.Nenajdete nenajdeteModel, NenajdeteMsg nenajdeteMsg ->
        let (nenajdeteModel, nenajdeteCmd) = Nenajdete.update nenajdeteMsg nenajdeteModel
        { model with ActivePage = Page.Nenajdete nenajdeteModel }, Cmd.map NenajdeteMsg nenajdeteCmd

    | Page.Kontakt kontaktModel, KontaktMsg kontaktMsg ->
        let (kontaktModel, kontaktCmd) = Kontakt.update kontaktMsg kontaktModel
        { model with ActivePage = Page.Kontakt kontaktModel }, Cmd.map KontaktMsg kontaktCmd

    | Page.Login loginModel, LoginMsg loginMsg ->
         let (loginModel, loginCmd) = Login.update loginMsg loginModel
         { model with ActivePage = Page.Login loginModel }, Cmd.map LoginMsg loginCmd    

    | Page.CMSRozcestnik cmsRozcestnikModel, CMSRozcestnikMsg cmsRozcestnikMsg ->
        let (cmsRozcestnikModel, cmsRozcestnikCmd) = CMSRozcestnik.update cmsRozcestnikMsg cmsRozcestnikModel
        { model with ActivePage = Page.CMSRozcestnik cmsRozcestnikModel }, Cmd.map CMSRozcestnikMsg cmsRozcestnikCmd

    | Page.CMSCenik cmsCenikModel, CMSCenikMsg cmsCenikMsg ->
        let (cmsCenikModel, cmsCenikCmd) = CMSCenik.update cmsCenikMsg cmsCenikModel
        { model with ActivePage = Page.CMSCenik cmsCenikModel }, Cmd.map CMSCenikMsg cmsCenikCmd

    | Page.CMSKontakt cmsKontaktModel, CMSKontaktMsg cmsKontaktMsg ->
        let (cmsKontaktModel, cmsKontaktCmd) = CMSKontakt.update cmsKontaktMsg cmsKontaktModel
        { model with ActivePage = Page.CMSKontakt cmsKontaktModel }, Cmd.map CMSKontaktMsg cmsKontaktCmd

    | Page.CMSLink cmsLinkModel, CMSLinkMsg cmsLinkMsg ->
            let (cmsLinkModel, cmsLinkCmd) = CMSLink.update cmsLinkMsg cmsLinkModel
            { model with ActivePage = Page.CMSLink cmsLinkModel }, Cmd.map CMSLinkMsg cmsLinkCmd

    //pokud potrebujeme aktivovat hodnoty uz pri spusteni stranek        
    | _, SendLinkAndLinkNameValuesToServer ->
        let loadEvent = SharedDeserialisedLinkAndLinkNameValues.create model.LinkAndLinkNameInputValues
        let cmd = Cmd.OfAsync.perform sendDeserialisedLinkAndLinkNameValues.sendDeserialisedLinkAndLinkNameValues loadEvent GetLinkAndLinkNameValues
        model, cmd
        
    | _, GetLinkAndLinkNameValues value -> { model with LinkAndLinkNameValues =
                                                                                {
                                                                                    V001 = value.V001; V002 = value.V002; V003 = value.V003;
                                                                                    V004 = value.V004; V005 = value.V005; V006 = value.V006;
                                                                                    V001n = value.V001n; V002n = value.V002n; V003n = value.V003n;
                                                                                    V004n = value.V004n; V005n = value.V005n; V006n = value.V006n;
                                                                                }
                                           }, Cmd.none    
        
    | _, msg ->
        //Browser.console.warn("Message discarded:\n", string msg)
            
                
    model, Cmd.none     

let view (model : Model) (dispatch : Dispatch<Msg>) =
    match model.ActivePage with
    | Page.NotFound -> str "404 Page not found"       
    | Page.Home homeModel -> Home.view homeModel (HomeMsg >> dispatch) model.LinkAndLinkNameValues
    | Page.Sluzby sluzbyModel -> Sluzby.view sluzbyModel (SluzbyMsg >> dispatch) model.LinkAndLinkNameValues
    | Page.Cenik cenikModel -> Cenik.view cenikModel (CenikMsg >> dispatch) model.LinkAndLinkNameValues
    | Page.Nenajdete nenajdeteModel -> Nenajdete.view nenajdeteModel (NenajdeteMsg >> dispatch) model.LinkAndLinkNameValues
    | Page.Kontakt kontaktModel -> Kontakt.view kontaktModel (KontaktMsg >> dispatch) model.LinkAndLinkNameValues
    | Page.Login loginModel -> Login.view loginModel (LoginMsg >> dispatch)     
    | Page.CMSRozcestnik cmsRozcestnikModel -> CMSRozcestnik.view cmsRozcestnikModel (CMSRozcestnikMsg >> dispatch)
    | Page.CMSCenik cmsCenikModel -> CMSCenik.view cmsCenikModel (CMSCenikMsg >> dispatch)
    | Page.CMSKontakt cmsKontaktModel -> CMSKontakt.view cmsKontaktModel (CMSKontaktMsg >> dispatch)
    | Page.CMSLink cmsLinkModel -> CMSLink.view cmsLinkModel (CMSLinkMsg >> dispatch)

open Elmish.UrlParser
open Elmish.Navigation
open Elmish.React

Program.mkProgram init update view
|> Program.toNavigable (parseHash Router.routeParser) setRoute
|> Program.withReactSynchronous "elmish-app"
|> Program.run


