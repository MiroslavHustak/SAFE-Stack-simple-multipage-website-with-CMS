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

type CenikValuesDomain =
    {
        Id: int; ValueState: string;
        V001: string; V002: string; V003: string;
        V004: string; V005: string; V006: string;
        V007: string; V008: string; V009: string;
        Msgs: MessagesDomain
    }   

type KontaktValuesDomain =
    {
        V001: string; V002: string; V003: string;
        V004: string; V005: string; V006: string;
        V007: string; Msgs: MessagesDomain
    }    

type LinkAndLinkNameValuesDomain =
    {
        V001: string; V002: string; V003: string;
        V004: string; V005: string; V006: string;
        V001n: string; V002n: string; V003n: string;
        V004n: string; V005n: string; V006n: string;
        Msgs: MessagesDomain
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
    


