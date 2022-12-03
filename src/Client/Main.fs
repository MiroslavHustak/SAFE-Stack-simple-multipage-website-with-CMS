//Code for routing taken from StackOverflow:
//https://stackoverflow.com/questions/54970180/how-can-i-do-a-simple-elmish-router
//Created by Maxime Mangel:
//https://stackoverflow.com/users/2911775/maxime-mangel

[<RequireQualifiedAccess>]
module App

open System

open Elmish
open Fable.React
open Feliz.Router //zatim nevyuzito, moznost pouziti Cmd.Navigation
open Fable.Remoting.Client

open Shared
open SharedTypes
open LoginExtended

type ApplicationUser =
    | FirstTimeRunAnonymous
    | Anonymous
    | LoggedIn of SharedApi.User

[<RequireQualifiedAccess>]
type Page =
    | Home of Home.Model
    | Sluzby of Sluzby.Model
    | Cenik of Cenik.Model
    | Nenajdete of Nenajdete.Model
    | Kontakt of Kontakt.Model
    | Login of Login.Model 
    | Maintenance of Maintenance.Model 
    | CMSRozcestnik of CMSRozcestnik.Model
    | CMSCenik of CMSCenik.Model
    | CMSKontakt of CMSKontakt.Model
    | CMSLink of CMSLink.Model
    | NotFound
    //| Logout of Home.Model

type Model =
    {
        ActivePage: Page      
        CurrentRoute: RouterM.Route option 
        User: ApplicationUser 
        user: SharedApi.User

        Session: SharedApi.LoginResult option

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
    | MaintenanceMsg of Maintenance.Msg    
    | CMSRozcestnikMsg of CMSRozcestnik.Msg
    | CMSCenikMsg of CMSCenik.Msg
    | CMSKontaktMsg of CMSKontakt.Msg
    | CMSLinkMsg of CMSLink.Msg

    | AskServerForLinkAndLinkNameValues 
    | GetLinkAndLinkNameValues of GetLinkAndLinkNameValues

let private api() = Remoting.createApi ()
                    |> Remoting.withRouteBuilder Route.builder
                    |> Remoting.buildProxy<IGetApi>

let private sendDeserialisedLinkAndLinkNameValuesApi = api()

let private cmd1 fn cmd msg = Cmd.batch <| seq { Cmd.map fn cmd; Cmd.ofMsg msg }
let private cmd2 fn cmd msg msg' = Cmd.batch <| seq { Cmd.map fn cmd; Cmd.ofMsg msg; Cmd.ofMsg msg' }

let private setRoute (optRoute: RouterM.Route option) model = 
                                                 
    let model =
                          
        let applicationUser = 
            match model.Session with
            | Some value ->
                            match value with
                            | SharedApi.UsernameOrPasswordIncorrect -> Anonymous
                            | SharedApi.LoggedIn user               -> LoggedIn user  
            | None       -> Anonymous

        {
            model with CurrentRoute = optRoute //currentRoute //neni treba, User = applicationUser se v patter matching postara o spravny routing
                                      User = applicationUser                                                                                                                              
        }    
    
    (*
        //What's happening here is that F# allows you to deconstruct function arguments inline using arbitrarily complex pattern matching.
        //This is often mentioned when introducing single case DU's, but it's rarely followed to the conclusion, which leads
        //people to believe single case DU's are somehow special that way.

        type Composite = Composite of int 
        let unwrap (Composite a) = a

        which corresponds to:
        let unwrap x = 
        match x with
        | Composite a -> a
    *)

    //test zkraceneho match pro deconstruction single case DU
    let accessToken = match model.user.AccessToken with SharedApi.AccessToken value -> value
                      
    //test pro deconstruction single case DU
    let unwrap (SharedApi.AccessToken x) = x
    let accessToken = unwrap model.user.AccessToken

    //v pripade vlozeni do rozcestniku je treba jednou tady v setRoute dat dalsi parametry do RouterM.Route.CMSRozcestnik cmsRozcestnikId (druhy rozcestnik)
    //a pak jeste to same z loginu (prvni rozcestnik) 
    
    match optRoute with
    | None ->
        { model with ActivePage = Page.NotFound }, Cmd.none

    | Some RouterM.Route.Home ->    
       let (homeModel, homeCmd) = Home.init ()       
       { model with ActivePage = Page.Home homeModel }, cmd1 HomeMsg homeCmd AskServerForLinkAndLinkNameValues 

    | Some (RouterM.Route.Sluzby sluzbyId) ->
        let (sluzbyModel, sluzbyCmd) = Sluzby.init sluzbyId
        { model with ActivePage = Page.Sluzby sluzbyModel }, cmd1 SluzbyMsg sluzbyCmd AskServerForLinkAndLinkNameValues 

    | Some (RouterM.Route.Cenik cenikId) ->
        let (cenikModel, cenikCmd) = Cenik.init cenikId
        { model with ActivePage = Page.Cenik cenikModel }, cmd1 CenikMsg cenikCmd AskServerForLinkAndLinkNameValues 

    | Some (RouterM.Route.Nenajdete nenajdeteId) ->
        let (nenajdeteModel, nenajdeteCmd) = Nenajdete.init nenajdeteId 
        { model with ActivePage = Page.Nenajdete nenajdeteModel }, cmd1 NenajdeteMsg nenajdeteCmd AskServerForLinkAndLinkNameValues 

    | Some (RouterM.Route.Kontakt kontaktId) ->
        let (kontaktModel, kontaktCmd) = Kontakt.init kontaktId
        { model with ActivePage = Page.Kontakt kontaktModel }, cmd1 KontaktMsg kontaktCmd AskServerForLinkAndLinkNameValues 

    | Some (RouterM.Route.Login loginId) ->
        let (loginModel, loginCmd) = Login.init loginId
        { model with ActivePage = Page.Login loginModel }, Cmd.map LoginMsg loginCmd 

    //zatim nevyuzito
    | Some (RouterM.Route.Logout) ->
        let (homeModel, homeCmd) = Home.init () //or Login.init
        { model with ActivePage = Page.Home homeModel }, cmd1 HomeMsg homeCmd AskServerForLinkAndLinkNameValues 

    //zatim nevyuzito
    | Some (RouterM.Route.Maintenance) ->
        let (maintenanceModel, maintenanceCmd) = Maintenance.init ()
        { model with ActivePage = Page.Maintenance maintenanceModel }, Cmd.map MaintenanceMsg maintenanceCmd 

    | Some (RouterM.Route.CMSRozcestnik cmsRozcestnikId) ->               
        match model.User with
        | Anonymous ->  
                   let (homeModel, homeCmd) = Home.init () //or Login.init      
                   { model with ActivePage = Page.Home homeModel }, cmd1 HomeMsg homeCmd AskServerForLinkAndLinkNameValues 
        | LoggedIn user  ->
                   let (cmsRozcestnikModel, cmsRozcestnikCmd) = CMSRozcestnik.init cmsRozcestnikId 
                   { model with ActivePage = Page.CMSRozcestnik cmsRozcestnikModel }, Cmd.map CMSRozcestnikMsg cmsRozcestnikCmd
         //FirstTimeRunAnonymous je moc dluhe, takze podrzitko...
        | _     -> let (homeModel, homeCmd) = Home.init () //or Login.init      
                   { model with ActivePage = Page.Home homeModel }, cmd1 HomeMsg homeCmd AskServerForLinkAndLinkNameValues                  
                     
    | Some (RouterM.Route.CMSCenik cmsCenikId) ->  
        match model.User with
        | Anonymous ->  
                    let (homeModel, homeCmd) = Home.init () //or Login.init      
                    { model with ActivePage = Page.Home homeModel }, cmd1 HomeMsg homeCmd AskServerForLinkAndLinkNameValues 
        | LoggedIn user ->
                    let (cmsCenikModel, cmsCenikCmd) = CMSCenik.init cmsCenikId
                    { model with ActivePage = Page.CMSCenik cmsCenikModel }, Cmd.map CMSCenikMsg cmsCenikCmd 
        | _      -> let (homeModel, homeCmd) = Home.init () //or Login.init      
                    { model with ActivePage = Page.Home homeModel }, cmd1 HomeMsg homeCmd AskServerForLinkAndLinkNameValues 

    | Some (RouterM.Route.CMSKontakt cmsKontaktId) ->
        match model.User with
        | Anonymous ->  
                   let (homeModel, homeCmd) = Home.init () //or Login.init     
                   { model with ActivePage = Page.Home homeModel }, cmd1 HomeMsg homeCmd AskServerForLinkAndLinkNameValues 
        | LoggedIn user ->
                   let (cmsKontaktModel, cmsKontaktCmd) = CMSKontakt.init cmsKontaktId
                   { model with ActivePage = Page.CMSKontakt cmsKontaktModel }, Cmd.map CMSKontaktMsg cmsKontaktCmd 
        | _ ->     let (homeModel, homeCmd) = Home.init () //or Login.init      
                   { model with ActivePage = Page.Home homeModel }, cmd1 HomeMsg homeCmd AskServerForLinkAndLinkNameValues 

    | Some (RouterM.Route.CMSLink cmsLinkId) ->    
        match model.User with
        | Anonymous ->  
                   let (homeModel, homeCmd) = Home.init () //or Login.init      
                   { model with ActivePage = Page.Home homeModel }, cmd1 HomeMsg homeCmd AskServerForLinkAndLinkNameValues 
        | LoggedIn user ->
                   let (cmsLinkModel, cmsLinkCmd) = CMSLink.init cmsLinkId
                   { model with ActivePage = Page.CMSLink cmsLinkModel }, Cmd.map CMSLinkMsg cmsLinkCmd 
        | _     -> let (homeModel, homeCmd) = Home.init () //or Login.init       
                   { model with ActivePage = Page.Home homeModel }, cmd1 HomeMsg homeCmd AskServerForLinkAndLinkNameValues

let init (location: RouterM.Route option) =

    setRoute location
        {
            ActivePage = Page.NotFound 
            CurrentRoute = None

            User = Anonymous
            user = { Username = ""; AccessToken = SharedApi.AccessToken ""}

            Session = None
          
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

let update (msg: Msg) (model: Model) =
       
    match model.ActivePage, msg with
    | Page.NotFound, _ ->
        // Nothing to do here
        model, Cmd.none

    | Page.Home homeModel, HomeMsg homeMsg ->
        let (homeModel, homeCmd) = Home.update homeMsg homeModel       
        { model with ActivePage = Page.Home homeModel; Session = None }, Cmd.map HomeMsg homeCmd

    | Page.Sluzby sluzbyModel, SluzbyMsg sluzbyMsg ->
        let (sluzbyModel, sluzbyCmd) = Sluzby.update sluzbyMsg sluzbyModel
        { model with ActivePage = Page.Sluzby sluzbyModel; Session = None }, Cmd.map SluzbyMsg sluzbyCmd

    | Page.Cenik cenikModel, CenikMsg cenikMsg ->
        let (cenikModel, cenikCmd) = Cenik.update cenikMsg cenikModel
        { model with ActivePage = Page.Cenik cenikModel; Session = None }, Cmd.map CenikMsg cenikCmd

    | Page.Nenajdete nenajdeteModel, NenajdeteMsg nenajdeteMsg ->
        let (nenajdeteModel, nenajdeteCmd) = Nenajdete.update nenajdeteMsg nenajdeteModel
        { model with ActivePage = Page.Nenajdete nenajdeteModel; Session = None }, Cmd.map NenajdeteMsg nenajdeteCmd

    | Page.Kontakt kontaktModel, KontaktMsg kontaktMsg ->
        let (kontaktModel, kontaktCmd) = Kontakt.update kontaktMsg kontaktModel
        { model with ActivePage = Page.Kontakt kontaktModel; Session = None }, Cmd.map KontaktMsg kontaktCmd

    | Page.Login loginModel, LoginMsg loginMsg  ->
        let (loginModel, loginCmd, loginExtraMsg) = Login.update loginMsg loginModel        
        match loginExtraMsg with
        | Login.ExternalMsg.NoOp -> { model with ActivePage = Page.Login loginModel }, Cmd.map LoginMsg loginCmd
        | Login.ExternalMsg.SignedIn session -> { model with ActivePage = Page.Login loginModel; Session = Some session }, Cmd.map LoginMsg loginCmd

    | Page.CMSRozcestnik cmsRozcestnikModel, CMSRozcestnikMsg cmsRozcestnikMsg ->
        let (cmsRozcestnikModel, cmsRozcestnikCmd) = CMSRozcestnik.update cmsRozcestnikMsg cmsRozcestnikModel
        { model with ActivePage = Page.CMSRozcestnik cmsRozcestnikModel; Session = None }, Cmd.map CMSRozcestnikMsg cmsRozcestnikCmd //to None bude pri druhem rozcestniku

    | Page.CMSCenik cmsCenikModel, CMSCenikMsg cmsCenikMsg ->
        let (cmsCenikModel, cmsCenikCmd) = CMSCenik.update cmsCenikMsg cmsCenikModel 
        { model with ActivePage = Page.CMSCenik cmsCenikModel }, Cmd.map CMSCenikMsg cmsCenikCmd 

    | Page.CMSKontakt cmsKontaktModel, CMSKontaktMsg cmsKontaktMsg ->
        let (cmsKontaktModel, cmsKontaktCmd) = CMSKontakt.update cmsKontaktMsg cmsKontaktModel 
        { model with ActivePage = Page.CMSKontakt cmsKontaktModel }, Cmd.map CMSKontaktMsg cmsKontaktCmd 

    | Page.CMSLink cmsLinkModel, CMSLinkMsg cmsLinkMsg ->
        let (cmsLinkModel, cmsLinkCmd) = CMSLink.update cmsLinkMsg cmsLinkModel 
        { model with ActivePage = Page.CMSLink cmsLinkModel }, Cmd.map CMSLinkMsg cmsLinkCmd 
                                                                  
         //potrebujeme aktivovat hodnoty uz pri spusteni public stranek        
    | _, AskServerForLinkAndLinkNameValues ->
            let loadEvent = SharedDeserialisedLinkAndLinkNameValues.create model.LinkAndLinkNameInputValues
            let cmd = Cmd.OfAsync.perform sendDeserialisedLinkAndLinkNameValuesApi.sendDeserialisedLinkAndLinkNameValues loadEvent GetLinkAndLinkNameValues
            model, cmd
        
    | _, GetLinkAndLinkNameValues value -> { model with LinkAndLinkNameValues =
                                                                                {
                                                                                    V001 = value.V001; V002 = value.V002; V003 = value.V003;
                                                                                    V004 = value.V004; V005 = value.V005; V006 = value.V006;
                                                                                    V001n = value.V001n; V002n = value.V002n; V003n = value.V003n;
                                                                                    V004n = value.V004n; V005n = value.V005n; V006n = value.V006n;
                                                                                }
                                           }, Cmd.none
                                            
    | _, msg -> model, Cmd.none
        //Browser.console.warn("Message discarded:\n", string msg)    

let view (model: Model) (dispatch: Dispatch<Msg>) =
         
    match model.ActivePage with
    | Page.NotFound -> str "404 Page not found"       
    | Page.Home homeModel -> Home.view homeModel (HomeMsg >> dispatch) model.LinkAndLinkNameValues 
    | Page.Sluzby sluzbyModel -> Sluzby.view sluzbyModel (SluzbyMsg >> dispatch) model.LinkAndLinkNameValues 
    | Page.Cenik cenikModel -> Cenik.view cenikModel (CenikMsg >> dispatch) model.LinkAndLinkNameValues 
    | Page.Nenajdete nenajdeteModel -> Nenajdete.view nenajdeteModel (NenajdeteMsg >> dispatch) model.LinkAndLinkNameValues 
    | Page.Kontakt kontaktModel -> Kontakt.view kontaktModel (KontaktMsg >> dispatch) model.LinkAndLinkNameValues
    | Page.Maintenance maintenanceModel -> Maintenance.view maintenanceModel (MaintenanceMsg >> dispatch) //zatim nevyuzito
    | Page.Login loginModel ->  Login.view loginModel (LoginMsg >> dispatch)                              
    | Page.CMSRozcestnik cmsRozcestnikModel -> CMSRozcestnik.view cmsRozcestnikModel (CMSRozcestnikMsg >> dispatch)  
    | Page.CMSCenik cmsCenikModel -> CMSCenik.view cmsCenikModel (CMSCenikMsg >> dispatch) 
    | Page.CMSKontakt cmsKontaktModel -> CMSKontakt.view cmsKontaktModel (CMSKontaktMsg >> dispatch) 
    | Page.CMSLink cmsLinkModel -> CMSLink.view cmsLinkModel (CMSLinkMsg >> dispatch)
   // | Page.Logout homeModel -> Home.view homeModel (HomeMsg >> dispatch) model.LinkAndLinkNameValues //zatim nevyuzito

open Elmish.UrlParser
open Elmish.Navigation
open Elmish.React

Program.mkProgram init update view
|> Program.toNavigable (parseHash RouterM.routeParser) setRoute
|> Program.withReactSynchronous "elmish-app"
|> Program.run


