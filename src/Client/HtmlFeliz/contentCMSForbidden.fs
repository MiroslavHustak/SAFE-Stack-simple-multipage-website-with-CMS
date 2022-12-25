namespace HtmlFeliz

open Feliz

module ContentCMSForbidden = 

    //not in use 

    //complete html/Feliz code (no layout)
    let contentCMSForbidden() = 

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
                            prop.href "/Content/css/cms.css"
                            prop.rel "stylesheet"
                        ]                       
                    ]
                ]
                Html.body [
                    Html.div [
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
                                                    Html.br []
                                                    Html.br []
                                                
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
                                                                                Html.br []
                                                                                Html.br []
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
                                                                            ]
                                                                        ]
                                                                    ]
                                                                ]
                                                            ]
                                                            Html.tr [
                                                                prop.style
                                                                    [
                                                                        style.marginLeft(0)
                                                                        style.height(100)  
                                                                    ] 
                                                                prop.children [
                                                                    Html.td [
                                                                        Html.h1 [                                                                        
                                                                            prop.id 1
                                                                            prop.children [
                                                                                Html.text "K této stránce lze přistupovat pouze po přihlášení."                                                        
                                                                            ]
                                                                        ]
                                                                    ]                                                       
                                                                ]
                                                            ]   
                                                            Html.tr [
                                                                Html.td [
                                                                    prop.style
                                                                        [
                                                                          style.height(80)                                                                 
                                                                        ]
                                                                    prop.children [
                                                                        Html.div [                                                                 
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
                                                                                        Html.text "Návrat na hlavní stránku" //TODO zrobit skutecny log-off
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







