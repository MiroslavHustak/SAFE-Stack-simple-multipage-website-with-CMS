namespace DtoSend.Server

open System

module DtoSend =

    type MessagesDtoSend =
        {
            Msg1: string
            Msg2: string
            Msg3: string
            Msg4: string
            Msg5: string
            Msg6: string
        }
        
    type CenikValuesDtoSend =
        {
            Id: int; ValueState: string;
            V001: string; V002: string; V003: string;
            V004: string; V005: string; V006: string;
            V007: string; V008: string; V009: string;
            Msgs: MessagesDtoSend
        }

    // Defined but currently unused; retained for potential future requirements or updates.  
    type KontaktValuesDtoSend =
        {
            V001: string; V002: string; V003: string;
            V004: string; V005: string; V006: string;
            V007: string; Msgs: MessagesDtoSend
        }     

    type LinkAndLinkNameValuesDtoSend  =
        {
            V001: string; V002: string; V003: string;
            V004: string; V005: string; V006: string;
            V001n: string; V002n: string; V003n: string;
            V004n: string; V005n: string; V006n: string;
            Msgs: MessagesDtoSend
        }
  
