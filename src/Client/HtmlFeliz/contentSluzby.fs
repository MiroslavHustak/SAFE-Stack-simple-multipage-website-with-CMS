namespace HtmlFeliz

open Feliz

module ContentSluzby = 

    let contentSluzby() = 

        Html.div [
            prop.id "templatemo_content"
            prop.children [
                Html.h1 [
                    prop.id 13
                    prop.children [
                        Html.text "Nabízené služby"
                    ]
                ]
                Html.ul [
                    Html.li [
                        prop.id 6
                        prop.children [                        
                            Html.text "Výživa při redukci hmotnosti"
                        ]
                    ]
                    Html.li [
                        prop.id 7
                        prop.text "Racionální výživový plán pro klienty, kteří nepotřebují redukovat hmotnost, ale chtějí se stravovat zdravě"
                    ]
                    Html.li [
                        prop.id 8
                        prop.children [
                            Html.text "Individuální výživový plán při onemocněních (diabetes mellitus, dna, celiakie, anorexie a bulimie, chronické onemocnění střev, hyperlipoproteinemie, osteoporóza) a pro onkologické pacienty"                        
                        ]
                    ]
                    Html.li [
                        prop.id 9
                        prop.children [                      
                            Html.text "Výživa dětí"
                        ]
                    ]
                    Html.li [
                        prop.id 10
                        prop.children [
                            Html.text "Výživa v těhotenství a při kojení"                       
                        ]
                    ]
                    Html.li [
                        prop.id 11
                        prop.children [
                            Html.text "Výživa sportovců"
                        ]
                    ]
                    Html.li [
                        prop.id 12
                        prop.children [
                            Html.text "Bioimpendanční měření"
                        ]
                    ]
                ]
                Html.div [
                    Html.img [
                        prop.className "two"
                        prop.src "/Content/images/Srdicko.png"
                        prop.width 409
                        prop.height 307
                        prop.alt "Srdicko"
                    ]
                ]
                Html.div [
                    prop.className "cleaner_h60"
                ]
            ]
        ]

