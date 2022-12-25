namespace HtmlFeliz

open Feliz

module ContentNenajdete = 

    let contentNenajdete() =

        Html.div [
            prop.id "templatemo_content"
            prop.children [
                Html.h1 [
                    prop.id 14
                    prop.text "Co u mě nenajdete"
                ]
                Html.p [
                    prop.id 15
                    prop.children [
                        Html.text "Nepracuji při hubnutí s žádnými podpůrnými prostředky a doplňky typu \"zázračných\" pilulek a koktejlů, které se v dnešní době na trhu vyskytují ve velkém množství. Tato nutriční ambulance pracuje s běžnou a dostupnou stravou."                    
                    ]
                ]
                Html.br []
                Html.div [
                    Html.img [
                        prop.src "/Content/images/Tabletky.jpg"
                        prop.width 398
                        prop.height 280
                        prop.alt "tabletky"
                    ]
                ]
            ]
        ]

