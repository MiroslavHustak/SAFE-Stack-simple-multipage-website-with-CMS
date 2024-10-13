namespace Shared

open SharedTypes

type IGetApi =

    //unit -> Async<GetKontaktValues> etc is enough while transferring one way only, but I need error messages to be sent back to the client side

    {
        login : LoginValuesShared -> Async<LoginResult> 
        sendCenikValues : CenikValuesShared -> Async<CenikValuesShared> 
        getOldCenikValues : CenikValuesShared -> Async<CenikValuesShared>
        getDeserialisedCenikValues : CenikValuesShared -> Async<CenikValuesShared>
        sendKontaktValues : KontaktValuesShared -> Async<KontaktValuesShared> 
        getOldKontaktValues : KontaktValuesShared -> Async<KontaktValuesShared>
        getDeserialisedKontaktValues : KontaktValuesShared -> Async<KontaktValuesShared>
        sendLinkAndLinkNameValues : LinkValuesShared -> Async<LinkValuesShared> 
        getOldLinkValues : LinkValuesShared -> Async<LinkValuesShared>
        getDeserialisedLinkAndLinkNameValues : LinkValuesShared -> Async<LinkValuesShared>
    }

module Route =

    let internal builder typeName methodName = sprintf "/api/%s/%s" typeName methodName

