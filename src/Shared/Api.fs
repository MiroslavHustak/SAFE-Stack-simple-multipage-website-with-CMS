namespace Shared

type IGetApi =

    //unit -> Async<GetKontaktValues> etc is enough while transferring one way only, but I need error messages to be sent back to the client side

    {
        login : SharedTypes.LoginInfo -> Async<SharedTypes.LoginResult> 
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

