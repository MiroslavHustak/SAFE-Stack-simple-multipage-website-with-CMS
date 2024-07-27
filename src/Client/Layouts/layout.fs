namespace HtmlFeliz

open System

open Feliz

open Shared
open Records.Client
open Helpers.Client.Helper

module Layout = 

    let private myBody render (pageRecord: MyCssClass) (links: LinkValuesShared) =

        let errorMsg = sprintf "%s %s %s" links.Msgs.Msg1 links.Msgs.Msg2 links.Msgs.Msg3

        javaScriptMessageBox errorMsg

        Html.div [
            Html.div [
                prop.id "templatemo_wrapper"
                prop.children [
                    Html.div [
                        prop.id "templatemo_header"
                        prop.children [
                            Html.div [
                                prop.id "site_title"
                                prop.children [
                                    Html.h1 [
                                        Html.a [
                                            prop.href (MaximeRouter.Router.toHash MaximeRouter.Router.Home)
                                            prop.children [
                                                Html.img [
                                                    prop.src "/Content/images/templatemo_logo.png"
                                                ]
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                            Html.div [
                                prop.id "banner"
                                prop.children [ 
                                    Html.h3 [
                                        Html.text "Kéž se naše potraviny" 
                                        Html.br[]
                                        Html.span [
                                             Html.text "stanou vaším lékem"    
                                        ]                               
                                    ]
                                ]
                            ]
                        ]
                    ]
                    Html.div [
                        Html.div [
                            prop.id "templatemo_menu"
                            prop.children [
                                Html.ul [
                                    Html.li [
                                        Html.a [
                                            pageRecord.Home                           
                                            prop.href (MaximeRouter.Router.toHash MaximeRouter.Router.Home)
                                            prop.text "O mně"
                                        ]
                                    ]
                                    Html.li [
                                        Html.a [
                                            pageRecord.Sluzby 
                                            prop.href (MaximeRouter.Router.toHash (MaximeRouter.Router.Sluzby 1))                             
                                            prop.text "Služby"
                                        ]
                                    ]
                                    Html.li [
                                        Html.a [
                                            pageRecord.Cenik 
                                            prop.href (MaximeRouter.Router.toHash (MaximeRouter.Router.Cenik 2))   
                                            prop.text "Ceník"                                    
                                        ]
                                    ]
                                    Html.li [
                                        Html.a [
                                            pageRecord.Nenajdete
                                            prop.href (MaximeRouter.Router.toHash (MaximeRouter.Router.Nenajdete 3))  
                                            prop.text "Co u mě nenajdete"
                                        ]
                                    ]
                                    Html.li [
                                        Html.a [
                                            pageRecord.Kontakt
                                            prop.href (MaximeRouter.Router.toHash (MaximeRouter.Router.Kontakt 4))  
                                            prop.text "Kontakt"
                                        ]
                                    ]
                                    Html.li [
                                        Html.a [
                                            prop.className "last"
                                            prop.href (MaximeRouter.Router.toHash (MaximeRouter.Router.Login 5))                                       
                                            prop.text "Login"
                                        ]
                                    ]
                                ]
                            ]
                        ]                
                        Html.div [
                            prop.id "templatemo_main"
                            prop.children [
                                //****************************************
                                render
                                //****************************************
                                Html.div [
                                    prop.id "templatemo_sidebar"
                                    prop.children [
                                        Html.div [
                                            prop.className "sidebar_box"
                                            prop.children [
                                                Html.div [
                                                    prop.className "header"
                                                    prop.children [
                                                        Html.h3 "Užitečné odkazy"
                                                    ]
                                                ]
                                                Html.div [
                                                    prop.className "content"
                                                    prop.children [
                                                        Html.ul [
                                                            prop.className "categories_list"
                                                            prop.children [
                                                                Html.li [
                                                                    Html.a [
                                                                        prop.href links.V001//"https://blog.kaloricketabulky.cz/2013/08/nutricni-terapeut-vs-vyzivovy-poradce-kdo-nam-muze-radit-s-vyzivou/"
                                                                        prop.style
                                                                          [
                                                                            style.fontWeight.bold
                                                                            style.fontSize(14)
                                                                          ]
                                                                        prop.target "_blank"
                                                                        prop.text links.V001n //"Kdo nám může radit s výživou?"
                                                                    ]
                                                                ]
                                                                Html.li [
                                                                    Html.a [
                                                                        prop.href links.V002//"http://www.aktivityprozdravi.cz/zdravotni-problemy/civilizacni-psychologicke-a-jine-nemoci/civilizacni-choroby-a-nas-zivotni-styl"
                                                                        prop.style
                                                                            [
                                                                              style.fontWeight.bold
                                                                              style.fontSize(14)
                                                                            ]
                                                                        prop.target "_blank"
                                                                        prop.text links.V002n//"Civilizační choroby"
                                                                    ]
                                                                ]
                                                                Html.li [
                                                                    Html.a [
                                                                        prop.href links.V003//"https://www.novinky.cz/zena/zdravi/403392-obezita-je-problem-ktery-lide-casto-prehlizeji.html"
                                                                        prop.style
                                                                            [
                                                                              style.fontWeight.bold
                                                                              style.fontSize(14)
                                                                            ]
                                                                        prop.target "_blank"
                                                                        prop.text links.V003n//"Problém obezity"
                                                                    ]
                                                                ]
                                                                Html.li [
                                                                    Html.a [
                                                                        prop.href links.V004//"https://www.euronabycerny.com/eshop/jetbar-tycinky"
                                                                        prop.style
                                                                            [
                                                                              style.fontWeight.bold
                                                                              style.fontSize(14)
                                                                            ]
                                                                        prop.target "_blank"
                                                                        prop.text links.V004n//"Tyčinky Eurona JETBAR"
                                                                    ]
                                                                ]
                                                                Html.li [
                                                                    Html.a [
                                                                        prop.href links.V005//"https://www.morevsrdcievropy.cz"
                                                                        prop.style
                                                                           [
                                                                             style.fontWeight.bold
                                                                             style.fontSize(14)
                                                                           ]
                                                                        prop.target "_blank"
                                                                        prop.text links.V005n//"Moře v srdci Evropy"
                                                                    ]
                                                                ]
                                                            ]
                                                        ]
                                                        Html.br []
                                                        Html.div [
                                                            Html.a [
                                                                prop.className "facebook"
                                                                prop.href links.V006//"https://www.facebook.com/nutricniterapie/"
                                                                prop.rel "nofollow"
                                                                prop.target "_blank"
                                                                prop.children [
                                                                    Html.img [                                                                
                                                                        prop.style
                                                                            [
                                                                              style.width(20)
                                                                              style.height(20)
                                                                            ]
                                                                        prop.alt "Facebook (Nutriční terapie Ostrava)"
                                                                        prop.title "Odkaz na Facebook (Nutriční terapie Ostrava)"
                                                                        prop.src "/Content/images/flogo-HexRBG-Wht-72.png"
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
                                Html.div [
                                    prop.className "cleaner"
                                ]
                            ]
                        ]
                    ]
                    Html.div [
                        prop.id "templatemo_footer"
                        prop.children [
                            Html.text "Copyright © 2018 Hana Nováková | "
                            Html.a [
                                prop.href "http://www.templatemo.com"
                                prop.target "_blank"
                                prop.text "Website Templates"
                            ]
                            Html.text " by "
                            Html.a [
                                prop.href "http://www.templatemo.com"
                                prop.target "_blank"
                                prop.text "Free CSS Templates"
                            ]
                        ]
                    ]
                    Html.br []
                    Html.div [
                        prop.id "templatemo_footer1"
                        prop.children [
                            Html.text "This website is based on an HTML/CSS template downloaded from "
                            Html.a [
                                prop.href "http://all-free-download.com/free-website-templates/"
                                prop.target "_blank"
                                prop.text "Free Website Templates"
                                prop.fontSize(12)
                            ]
                            Html.text " and adapted by "
                            Html.a [
                                prop.href "http://hustak.somee.com"
                                prop.target "_blank"
                                prop.text "Miroslav Husťák"
                            ]
                            Html.text " with the help of "
                            Html.a [
                                prop.href "https://safe-stack.github.io"
                                prop.target "_blank"
                                prop.text "SAFE Stack"
                            ]
                            Html.text ", " 
                            Html.a [
                                prop.href "https://zaid-ajaj.github.io/the-elmish-book/#/"
                                prop.target "_blank"
                                prop.text "Elmish"
                                   ]
                            Html.text " and "
                            Html.a [
                                prop.href "https://fsharp.org"
                                prop.target "_blank"
                                prop.text "F#"
                                   ]
                            Html.text "." 
                        ]
                    ]
                    Html.div [
                        prop.id "templatemo_footer1"
                        prop.children [
                            Html.text "Icons made by "
                            Html.a [
                                prop.href "https://www.flaticon.com/authors/smashicons"
                                prop.target "_blank"
                                prop.title "Smashicons"
                                prop.text "Smashicons"
                            ]
                            Html.text " that were downloaded from "
                            Html.a [
                                prop.href "https://www.flaticon.com/"
                                prop.title "Flaticon"
                                prop.text "www.flaticon.com"
                            ]
                            Html.text " are licensed under "
                            Html.a [
                                prop.href "http://creativecommons.org/licenses/by/3.0/"
                                prop.target "_blank"
                                prop.title "Creative Commons BY 3.0"
                                prop.text "Creative Commons BY 3.0."
                            ]
                        ]
                    ]
                    Html.div [
                        prop.id "templatemo_footer1"
                        prop.children [
                            Html.a [
                                prop.href "https://stackoverflow.com/questions/54970180/how-can-i-do-a-simple-elmish-router"
                                prop.target "_blank"
                                prop.title "F#/Elmish code"
                                prop.text "F#/Elmish code"
                            ]   
                            Html.text " for client-side routing by "
                            Html.a [
                                prop.href "https://stackoverflow.com/users/2911775/maxime-mangel"
                                prop.target "_blank"
                                prop.title "Maxime Mangel"
                                prop.text "Maxime Mangel"
                            ]
                            Html.text "."
                        ]
                    ]
                ]
            ]     
        ]

    let layout render (pageRecord: MyCssClass) links =

        Html.html [
            Html.head [
                Html.meta [
                    prop.name "viewport"
                    prop.content "width=device-width, initial-scale=1.0"
                ]
                Html.meta [
                    //prop.httpEquiv "Content-Type"
                    prop.content "text/html; charset=utf-8"
                ]
                Html.title "Nutriční terapie Ostrava"
                Html.meta [
                    prop.name "keywords"
                    prop.content "nutrition, fitness, Nutriční terapie Ostrava, nutriční poradenství, výživové poradenství"
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
                    prop.href "/Content/css/templatemo_style.css"
                    prop.rel "stylesheet"
                ]
            ]
            Html.body [
                Html.div [
                    prop.id "elmish-app"
                ]
                myBody render (pageRecord: MyCssClass) links
            ]
        ]