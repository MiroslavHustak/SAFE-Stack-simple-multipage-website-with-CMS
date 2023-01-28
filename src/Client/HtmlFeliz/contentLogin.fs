namespace HtmlFeliz

open Feliz
open SharedTypes

module ContentLogin = 

    let private br n = ( [ 1..n ] |> List.map (fun _ -> Html.br []) |> List.ofSeq ) |> List.map (fun item -> item)

    //complete html/Feliz code (no layout)
    let contentLogin submitInput inputElementUsr inputElementPsw (rcErrorMsg: SharedApi.LoginProblems) hiddenValue dispatch = 

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

