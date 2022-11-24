namespace SharedTypesAndRecords

type GetCredentials =
    {
        LoginResult: string
        Usr: string
        Psw: string
    }

type GetSecurityToken =
    {
        SecurityToken: string
    }

type DeleteSecurityTokenFile = { DeleteSecurityTokenFile: bool } 

type GetCenikValues =
    {
        V001: string; V002: string; V003: string;
        V004: string; V005: string; V006: string;
        V007: string; V008: string; V009: string
    }

type GetKontaktValues =
    {
        V001: string; V002: string; V003: string;
        V004: string; V005: string; V006: string;
        V007: string
    }

type GetLinkAndLinkNameValues =
    {
        V001: string; V002: string; V003: string;
        V004: string; V005: string; V006: string;
        V001n: string; V002n: string; V003n: string;
        V004n: string; V005n: string; V006n: string
    }

type IGetApi =
    {
      getCredentials: GetCredentials -> Async<GetCredentials>
      deleteSecurityTokenFile: DeleteSecurityTokenFile -> Async<DeleteSecurityTokenFile>
      sendSecurityToken: GetSecurityToken -> Async<GetSecurityToken>
      getCenikValues: GetCenikValues -> Async<GetCenikValues>
      sendOldCenikValues: GetCenikValues -> Async<GetCenikValues>
      sendDeserialisedCenikValues: GetCenikValues -> Async<GetCenikValues>
      getKontaktValues: GetKontaktValues -> Async<GetKontaktValues>
      sendOldKontaktValues: GetKontaktValues -> Async<GetKontaktValues>
      sendDeserialisedKontaktValues: GetKontaktValues -> Async<GetKontaktValues>
      getLinkAndLinkNameValues: GetLinkAndLinkNameValues -> Async<GetLinkAndLinkNameValues>
      sendOldLinkAndLinkNameValues: GetLinkAndLinkNameValues -> Async<GetLinkAndLinkNameValues>
      sendDeserialisedLinkAndLinkNameValues: GetLinkAndLinkNameValues -> Async<GetLinkAndLinkNameValues>
    }


