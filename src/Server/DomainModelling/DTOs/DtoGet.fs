namespace DtoGet.Server

open System

module DtoGet =

    type MessagesDtoGet =
        {
            Msg1: string
            Msg2: string
            Msg3: string
            Msg4: string
            Msg5: string
            Msg6: string
        }
       
    type CenikValuesDtoGet =
        {
            IdDtoGet: obj; ValueStateDtoGet: obj;
            V001DtoGet: obj; V002DtoGet: obj; V003DtoGet: obj;
            V004DtoGet: obj; V005DtoGet: obj; V006DtoGet: obj;
            V007DtoGet: obj; V008DtoGet: obj; V009DtoGet: obj;
            MsgsDtoGet: MessagesDtoGet
        }

    // Defined but currently unused; retained for potential future requirements or updates.     
    type KontaktValuesDtoGet =
        {
            V001: string; V002: string; V003: string;
            V004: string; V005: string; V006: string;
            V007: string; Msgs: MessagesDtoGet
        }

    type LinkAndLinkNameValuesDtoGet  =
        {
            V001: string; V002: string; V003: string;
            V004: string; V005: string; V006: string;
            V001n: string; V002n: string; V003n: string;
            V004n: string; V005n: string; V006n: string;
            Msgs: MessagesDtoGet
        }

   