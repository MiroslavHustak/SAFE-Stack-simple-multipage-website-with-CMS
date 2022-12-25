namespace HtmlFeliz

open Feliz

module ContentHome =

    let contentHome () =     

       //render ***********************************
       Html.div [
           prop.id "templatemo_content"
           prop.children [
               Html.h1 [
                   prop.id 1
                   prop.children [
                       Html.text "Ambulance nutriční terapie"
                   ]
               ]
               Html.div [
                   prop.classes [ "image_wrapper"; "fl_img" ]
                   prop.children [
                       Html.img [
                           prop.src "/Content/images/Uvodni foto.png"
                           prop.width 246
                           prop.height 270
                           prop.alt "image 1"
                       ]
                   ]
               ]
               Html.p [
                   prop.id 2
                   prop.children [
                       Html.text "Jmenuji se Hana Nováková a jsem nutriční terapeutka a diplomovaná všeobecná sestra. Výživou člověka se zabývám již mnoho let."                                      
                   ]
               ]
               Html.p [
                   prop.id 3
                   prop.children [
                       Html.text "Výživa je propojena se zdravotním stavem každého člověka. Špatná výživa nese sebou řadu onemocnění, např. kardiovaskulární onemocnění, zhoubné nádory, obezitu a cukrovku. Obor nutriční terapie je velmi rozšířený a dokáže pomoci klientům všech věkových kategorií."                                      
                   ]
               ]
               Html.div [
                   prop.className "cleaner_h60"
               ]
               Html.h2 [
                   prop.id 4
                   prop.children [
                       Html.text "Odborná způsobilost"                                       
                   ]
               ]
               Html.p [
                   prop.id 5
                   prop.children [
                       Html.text "Jsem registrovaná na ministerstvu zdravotnictví jakožto zdravotnický pracovník. Praxi v oboru nutriční terapie mám již 13 let. Neustále se vzdělávám v nutriční terapií pomocí konvenčních zdravotnických certifikovaných kurzů, např. v oblasti diabetologie, obezitologie, celiakie, výživy těhotných a kojicích žen, anorexie a bulimie. Dále se věnuji a vzdělávám se v alternativní medicíně co se týče vyváženosti a energií každé potraviny."                                        
                   ]
               ]
           ]
       ]
