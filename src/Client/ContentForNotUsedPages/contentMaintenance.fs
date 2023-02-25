namespace NotUsedPages

open Feliz

module ContentMaintenance = 

    //not in use 

    let contentMaintenance() =

        Html.html [
            prop.lang "en"
            prop.children [
                Html.head [
                    Html.title "The Website is under construction ...."
                    Html.meta [
                        prop.charset "UTF-8"
                    ]
                    Html.meta [
                        prop.name "description"
                        prop.content "Coming soon template"
                    ]
                    Html.meta [
                        prop.name "author"
                        prop.content "https://downloadfort.com/"
                    ]
                    Html.meta [
                        prop.name "viewport"
                        prop.content "width=device-width, initial-scale=1.0"
                    ]
                    Html.link [
                        prop.href "/Content/css/CSS.css"
                        prop.rel "stylesheet"
                    ]
                ]
                Html.body [
                    prop.classes [ "body"; "bluebg" ]
                    prop.children [
                        Html.img [
                            prop.src "/Content/images/under-maintenance.jpg"
                            prop.className "imgcenter"
                        ]
                        Html.p [
                            prop.className "txtwhite"
                            prop.children [
                                Html.text "The website is "
                                Html.a [
                                    prop.className "txtlightyellow"
                                    prop.text "under maintenance."
                                ]
                            ]
                        ]
                        Html.p [
                            prop.className "txtwhite"
                            prop.children [
                                Html.text "Kindly visit after some time."
                                Html.text " "
                                Html.strong " Thank you!"
                            ]
                        ]
                    ]
                ]
            ]
        ]

    
