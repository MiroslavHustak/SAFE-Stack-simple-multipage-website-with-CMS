namespace SharedTypes

open System

[<Struct>]
type LoginInfo =
    {
        Username: SharedApi.Username
        Password: SharedApi.Password
    }

type MessagesDomain =
    {
        Msg1: string
        Msg2: string
        Msg3: string
        Msg4: string
        Msg5: string
        Msg6: string
    }
    static member Default =
           {
               Msg1 = String.Empty; Msg2 = String.Empty;
               Msg3 = String.Empty; Msg4 = String.Empty;
               Msg5 = String.Empty; Msg6 = String.Empty
           }

type CenikValuesDomain =
    {
        Id: int; ValueState: string;
        V001: string; V002: string; V003: string;
        V004: string; V005: string; V006: string;
        V007: string; V008: string; V009: string;
        Msgs: MessagesDomain
    }
    static member Default = //fixed values
        {
            Id = 1; ValueState = "fixed";
            V001 = "300"; V002 = "300"; V003 = "2 200";
            V004 = "250"; V005 = "230"; V006 = "400";
            V007 = "600"; V008 = "450"; V009 = "450";
            Msgs = MessagesDomain.Default
        }

type KontaktValuesDomain =
    {
        V001: string; V002: string; V003: string;
        V004: string; V005: string; V006: string;
        V007: string; Msgs: MessagesDomain
    }
    static member Default = 
        {
            V001 = "Mgr. Hana NOVÁKOVÁ"; V002 = "Nutriční teraupetka"; V003 = "Pohoří 247";
            V004 = "725 26 Ostrava-Krásné Pole"; V005 = "Tel.: 739 421 710"; V006 = "E-mail: nutricniterapie@centrum.cz";
            V007 = (char)32 |> string; Msgs = MessagesDomain.Default
        }

type LinkAndLinkNameValuesDomain =
    {
        V001: string; V002: string; V003: string;
        V004: string; V005: string; V006: string;
        V001n: string; V002n: string; V003n: string;
        V004n: string; V005n: string; V006n: string;
        Msgs: MessagesDomain
    }
    static member Default = 
        {
            V001 = "https://blog.kaloricketabulky.cz/2013/08/nutricni-terapeut-vs-vyzivovy-poradce-kdo-nam-muze-radit-s-vyzivou/";
            V002 = "http://www.aktivityprozdravi.cz/zdravotni-problemy/civilizacni-psychologicke-a-jine-nemoci/civilizacni-choroby-a-nas-zivotni-styl";
            V003 = "https://www.novinky.cz/zena/zdravi/403392-obezita-je-problem-ktery-lide-casto-prehlizeji.html";
            V004 = "https://www.euronabycerny.com/eshop/jetbar-tycinky"; V005 = "https://www.morevsrdcievropy.cz";
            V006 = "https://www.facebook.com/nutricniterapie/";
            V001n = "Kdo nám může radit s výživou?"; V002n = "Civilizační choroby"; V003n = "Problém obezity"; V004n = "Tyčinky Eurona JETBAR";
            V005n = "Moře v srdci Evropy"; V006n = "Facebook"; Msgs = MessagesDomain.Default
        }

type IGetApi =
    //unit -> Async<GetKontaktValues> etc is enough while transferring one way only, but I need error messages to be sent back to the client side
    {
        login : LoginInfo -> Async<SharedApi.LoginResult> 
        sendCenikValues: CenikValuesDomain -> Async<CenikValuesDomain> 
        getOldCenikValues: CenikValuesDomain -> Async<CenikValuesDomain>
        getDeserialisedCenikValues: CenikValuesDomain -> Async<CenikValuesDomain>
        sendKontaktValues: KontaktValuesDomain -> Async<KontaktValuesDomain> 
        getOldKontaktValues: KontaktValuesDomain -> Async<KontaktValuesDomain>
        getDeserialisedKontaktValues: KontaktValuesDomain -> Async<KontaktValuesDomain>
        sendLinkAndLinkNameValues: LinkAndLinkNameValuesDomain -> Async<LinkAndLinkNameValuesDomain> 
        getOldLinkAndLinkNameValues: LinkAndLinkNameValuesDomain -> Async<LinkAndLinkNameValuesDomain>
        getDeserialisedLinkAndLinkNameValues: LinkAndLinkNameValuesDomain -> Async<LinkAndLinkNameValuesDomain>
    }
    


