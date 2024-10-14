namespace Server

open System.IO

module Resources =

    let internal pathToResources path = 
        try
            //sprintf "%s%s%s" AppDomain.CurrentDomain.BaseDirectory "Resources" path //nefunguje, haze to do debug
            Path.Combine("Resources", path) //CopyAlways
        with
        | ex -> failwith (sprintf "Kontaktuj programátora, závažná chyba na serveru !!! %s" ex.Message)

module Settings =

    let internal pathToJsonBackup = Resources.pathToResources @"jsonLinkAndLinkNameValuesBackUp.json"
    let internal pathToJson = Resources.pathToResources @"jsonLinkAndLinkNameValues.json"

    let internal pathToXmlBackup = Resources.pathToResources @"xmlKontaktValuesBackUp.xml"
    let internal pathToXml = Resources.pathToResources @"xmlKontaktValues.xml"

    let internal pathToXmlBackup2 = Resources.pathToResources @"xmlKontaktValuesBackUp2.xml"
    let internal pathToXml2 = Resources.pathToResources @"xmlKontaktValues2.xml"

    let internal pathToXmlBackup3 = Resources.pathToResources @"xmlKontaktValuesBackUp3.xml"
    let internal pathToXml3 = Resources.pathToResources @"xmlKontaktValues3.xml"

    let internal pathToUberHashTxt = Resources.pathToResources @"uberHash.txt"

    let internal logFileName = Resources.pathToResources @"logs/app.log"