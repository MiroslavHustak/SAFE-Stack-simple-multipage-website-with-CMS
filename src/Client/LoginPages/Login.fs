namespace LoginPages

open System

open Feliz
open Elmish
open Fable.Remoting.Client

open Shared
open Shared

module Login =

    //ExternalMsg by Maxime Mangel
    //https://medium.com/@MangelMaxime/my-tips-for-working-with-elmish-ab8d193d52fd
    type ExternalMsg =
        | NoOp
        | SignedIn of SharedTypes.LoginResult

    type ApplicationUser =  
        | FirstTimeRunAnonymous
        | Anonymous
        | LoggedIn of SharedTypes.User

    type Model =
        {
            User: ApplicationUser
            Problem: SharedTypes.LoginErrorMsgShared

            //********** ClientDtoCredentials **********
            InputUsr: string
            InputPsw: string
            //******************************************

            Id: int
        }
        
    [<RequireQualifiedAccess>]
    type Page =       
        | CMSRozcestnik of CMSPages.CMSRozcestnik.Model 

    type Msg =
        | SetUsrInput of string
        | SetPswInput of string
        | SendUsrPswToServer
        | GetLoginResults of SharedTypes.LoginResult
        | LoginCompleted of SharedTypes.LoginResult
        | Logout
        | CMSRozcestnikMsg of CMSPages.CMSRozcestnik.Msg
        | CMSRozcestnikModel of CMSPages.CMSRozcestnik.Model 

    let private getLoginApi =
        Remoting.createApi ()
        |> Remoting.withRouteBuilder Route.builder
        |> Remoting.buildProxy<IGetApi>

    let init id : Model * Cmd<Msg> =

        let model =
            {
                User = FirstTimeRunAnonymous
                Problem = { line1 = String.Empty; line2 = String.Empty }
                InputUsr = String.Empty
                InputPsw = String.Empty
                Id = id
            }
        model, Cmd.none

    let update (msg: Msg) (model: Model): Model * Cmd<Msg> * ExternalMsg =

        match msg with
        | SetUsrInput value -> { model with InputUsr = value }, Cmd.none, NoOp
        | SetPswInput value -> { model with InputPsw = value }, Cmd.none, NoOp

        | SendUsrPswToServer
            ->
             //let buttonClickEvent = SharedLoginValues.create (SharedTypes.Username model.InputUsr) (SharedTypes.Password model.InputPsw)
             let buttonClickEvent = SharedLoginValues.transferLayer model.InputUsr model.InputPsw
             let cmd = Cmd.OfAsync.perform getLoginApi.login buttonClickEvent GetLoginResults 
             model, cmd, NoOp

        | GetLoginResults value
            -> 
             let model =           
                 match value with
                 | SharedTypes.UsernameOrPasswordIncorrect problem -> { model with User = ApplicationUser.Anonymous; Problem = problem } //potrebne pro na konci modulu uvedeny kod
                 | SharedTypes.LoggedIn user                       -> { model with User = ApplicationUser.LoggedIn user } //potrebne pro na konci modulu uvedeny kod    
             model, Cmd.ofMsg (LoginCompleted value), NoOp

        | LoginCompleted session -> model, Cmd.none, SignedIn session
        | Logout                 -> model, Cmd.none, NoOp
        | CMSRozcestnikMsg _     -> model, Cmd.none, NoOp
        | CMSRozcestnikModel _   -> model, Cmd.none, NoOp

    let view (model: Model) (dispatch: Msg -> unit) =

        let proponClick =
            prop.onClick (fun e ->
                                 e.preventDefault()
                                 dispatch SendUsrPswToServer
                         )

        let submitInput =
            Html.input [
                prop.type' "submit"
                prop.value "Odeslat"
                prop.id "Button1"                                                                           
                proponClick
                prop.style
                    [
                        style.width(200)
                        style.fontWeight.bold
                        style.fontSize(16) 
                        style.color.blue
                        style.fontFamily "sans-serif"
                    ]
            ]                      

        let inputElementUsr =
            Html.input [          
                prop.type' "text"
                prop.id "userNameID"
                prop.name "userName"            
                prop.placeholder "Uživatelské jméno"
                prop.value model.InputUsr
                // prop.onChange (fun (ev: string) -> SetUsrInput ev |> dispatch)
                prop.onChange (SetUsrInput >> dispatch)
                prop.style
                    [
                        style.width(200)
                        style.fontFamily "sans-serif"
                    ]
                prop.autoFocus true
            ]
        
        let inputElementPsw =
            Html.input [            
                    prop.type' "password"
                    prop.id "passWID"
                    prop.name "passW"
                    prop.placeholder "Heslo"
                    prop.value model.InputPsw
                    // prop.onChange (fun (ev: string) -> SetPswInput ev |> dispatch)
                    prop.onChange (SetPswInput >> dispatch)  
                    prop.style
                        [
                            style.width(200)
                            style.fontFamily "sans-serif"
                        ]
            ]   
         
        //************************************************************************
        let br n =
            (
                [ 1..n ]
                |> List.map (fun _ -> Html.br [])
                |> List.ofSeq
            )
            |> List.map (fun item -> item)
        
        //complete html/Feliz code (no layout)
        let contentLogin submitInput inputElementUsr inputElementPsw (rcErrorMsg: SharedTypes.LoginErrorMsgShared) hiddenValue dispatch = 
        
            Html.html [
                prop.xmlns "http://www.w3.org/1999/xhtml"
                prop.children [
                    Html.head [
                        //prop..runat "server"
                        prop.children [
                            Html.meta [
                                prop.name "viewport"
                                prop.content "width=device-width, initial-scale=1.0"
                            ]
                            Html.meta [                        
                                prop.content "text/html; charset=utf-8"
                            ]
                            Html.title "Nutriční terapie"
                            Html.meta [
                                prop.name "keywords"
                                prop.content "css templates, healthy living, diet, nutrition, fitness, web design"
                            ]
                            Html.meta [
                                prop.name "description"
                                prop.content "Healthy Living - free CSS template provided by templatemo.com"
                            ]
                            Html.link [
                                prop.rel "icon"
                                prop.href "/Content/images/favicon.ico"
                            ]
                            Html.link [
                                prop.href "/Content/css/login.css"
                                prop.rel "stylesheet"
                            ]                       
                        ]
                    ]
                    Html.body [
                        Html.form [
                            prop.id "loginForm"   
                            prop.method "get"
                            prop.children [
                                Html.div [
                                    prop.id "wrap"
                                    prop.children [
                                        Html.div [
                                            prop.id "contentwrap"
                                            prop.children [
                                                Html.div [
                                                    prop.id "content"
                                                    prop.children [
                                                        yield! br 2
                                                        Html.table [                                                                                            
                                                            prop.style
                                                                [
                                                                    //style.marginLeft auto //neexistuje
                                                                    //style.marginRight auto //neexistuje
                                                                    //vycentrovano pres css
                                                                ]
                                                            prop.children [
                                                                Html.tr [
                                                                    Html.td [
                                                                        prop.style
                                                                            [
                                                                                style.height(100)                                                                 
                                                                            ]
                                                                        prop.children [
                                                                            Html.div [
                                                                                prop.id "apple"
                                                                                prop.children [
                                                                                    yield! br 2
                                                                                    Html.img [
                                                                                        prop.style
                                                                                            [
                                                                                                style.marginLeft(40) //prop.width 80 / 2                                                              
                                                                                            ]                                                                             
                                                                                        prop.src "/Content/images/apple.png"
                                                                                        prop.alt "Downloading apple..."
                                                                                        prop.id "apple"
                                                                                        prop.className "unhidden"
                                                                                        prop.width 80
                                                                                        prop.height 80
                                                                                    ]
                                                                                    yield! br 3
                                                                                ]
                                                                            ]
                                                                        ]
                                                                    ]
                                                                ]
                                                                Html.tr [
                                                                    Html.td [                                                                
                                                                        prop.style
                                                                            [
                                                                                style.height(100)                                                                 
                                                                            ]
                                                                        prop.children [
                                                                            Html.div [                                                                        
                                                                                Html.br []
                                                                                Html.label [
                                                                                    prop.for' "Label1"                                                                            
                                                                                    prop.style
                                                                                        [
                                                                                            style.width(200)
                                                                                            style.fontWeight.bold
                                                                                            style.fontSize(18) //font-size: large
                                                                                            style.color.blue
                                                                                            style.fontFamily "sans-serif"
                                                                                        ]
                                                                                    prop.children [
                                                                                        Html.text "Uživatelské jméno"                                                                                
                                                                                    ]
                                                                                ]                                                                        
                                                                                Html.br []
                                                                                inputElementUsr                                                                  
                                                                                Html.br []
                                                                                Html.br []
                                                                                Html.label [
                                                                                    prop.for' "Label2"
                                                                                    prop.style
                                                                                        [
                                                                                            style.width(200)
                                                                                            style.fontWeight.bold
                                                                                            style.fontSize(18) //font-size: large neni
                                                                                            style.color.blue
                                                                                            style.fontFamily "sans-serif"
                                                                                        ]
                                                                                    prop.text "Heslo"
                                                                                ]
                                                                                Html.br []
                                                                                inputElementPsw                                                                
                                                                            ]
                                                                        ]
                                                                    ]
                                                                ]
                                                                Html.tr [
                                                                    Html.td [
                                                                        prop.style
                                                                            [
                                                                                style.height(100)                                                                 
                                                                            ]
                                                                        prop.children [
                                                                            Html.div [
                                                                                Html.br []
                                                                                Html.br []
                                                                                Html.br []
                                                                                submitInput                                                      
                                                                                Html.br []
                                                                                Html.label [
                                                                                    prop.hidden hiddenValue
                                                                                    prop.for' "Label3"                                                                            
                                                                                    prop.style
                                                                                        [
                                                                                            style.width(200)
                                                                                            style.fontWeight.bold
                                                                                            style.fontSize(12) //font-size: large
                                                                                            style.color.blue
                                                                                            style.fontFamily "sans-serif"
                                                                                        ]
                                                                                    prop.children [
                                                                                        Html.text rcErrorMsg.line1
                                                                                        Html.br []
                                                                                        Html.text rcErrorMsg.line2 
                                                                                    ]
                                                                                ]          
                                                                                Html.br []
                                                                                Html.br []
                                                                                Html.div [
                                                                                    Html.a [
                                                                                        prop.style
                                                                                            [                                                                                      
                                                                                                style.fontFamily "sans-serif"
                                                                                                style.fontWeight.bold
                                                                                            ]
                                                                                        prop.href "/"
                                                                                        prop.children [
                                                                                            Html.text "Návrat na stránky ambulance nutriční terapie"
                                                                                        ]
                                                                                    ]
                                                                                ]
                                                                            ]
                                                                        ]
                                                                    ]
                                                                ]
                                                            ]
                                                        ]
                                                        Html.br []
                                                    ]
                                                ]
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
            ]   

        let fnError rcErrorMsg =
            contentLogin
            <| submitInput                        
            <| inputElementUsr 
            <| inputElementPsw  
            <| rcErrorMsg
            <| false //related to hiding
            <| (dispatch: Msg -> unit)

        let fnFirstRun rcErrorMsg =
            contentLogin
            <| submitInput                        
            <| inputElementUsr 
            <| inputElementPsw                    
            <| rcErrorMsg
            <| true //related to hiding
            <| (dispatch: Msg -> unit)

        match model.User with      
        | Anonymous             -> fnError model.Problem
        | FirstTimeRunAnonymous -> fnFirstRun model.Problem
        | LoggedIn user         -> CMSPages.CMSRozcestnik.view CMSRozcestnikModel user (CMSRozcestnikMsg >> dispatch) //it is not strictly necessary for the model and user to be here, but I left them here to keep things tidy

                   
//Redundant code - for learning purposes only!!!
//Code by Maxime Mangel
//https://medium.com/@MangelMaxime/my-tips-for-working-with-elmish-ab8d193d52fd
module Parent =

    open Shared

    type Model =
        {
            Session : SharedTypes.LoginResult option
            Login : Login.Model
        }        

    type Msg =
        | LoginMsg of Login.Msg

    let private update (msg : Msg) (model : Model) =
        match msg with
        | LoginMsg loginMsg ->        
            let (loginModel, loginCmd, loginExtraMsg) = Login.update loginMsg model.Login

            // Here we can match over loginExtraMsg to do something
            let newModel =
                match loginExtraMsg with
                | Login.ExternalMsg.NoOp -> model
                | Login.ExternalMsg.SignedIn session -> { model with Session = Some session }

            { newModel with Login = loginModel }, Cmd.map LoginMsg loginCmd