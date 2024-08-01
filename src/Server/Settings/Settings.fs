namespace Server

open Helpers.Server.Resources

module Settings =

    let internal pathToJsonBackup = pathToResources @"jsonLinkAndLinkNameValuesBackUp.json"
    let internal pathToJson = pathToResources @"jsonLinkAndLinkNameValues.json"

    let internal pathToXmlBackup = pathToResources @"xmlKontaktValuesBackUp.xml"
    let internal pathToXml = pathToResources @"xmlKontaktValues.xml"

    let internal pathToXmlBackup2 = pathToResources @"xmlKontaktValuesBackUp2.xml"
    let internal pathToXml2 = pathToResources @"xmlKontaktValues2.xml"

    let internal pathToXmlBackup3 = pathToResources @"xmlKontaktValuesBackUp3.xml"
    let internal pathToXml3 = pathToResources @"xmlKontaktValues3.xml"

    let internal pathToUberHashTxt = pathToResources @"uberHash.txt"