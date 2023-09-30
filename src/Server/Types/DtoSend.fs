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
        static member Default =
               {
                   Msg1 = String.Empty; Msg2 = String.Empty;
                   Msg3 = String.Empty; Msg4 = String.Empty;
                   Msg5 = String.Empty; Msg6 = String.Empty
               }

    type CenikValuesDtoSend =
        {
            Id: int; ValueState: string;
            V001: string; V002: string; V003: string;
            V004: string; V005: string; V006: string;
            V007: string; V008: string; V009: string;
            Msgs: MessagesDtoSend
        }
      
