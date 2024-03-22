namespace DtoXml.Server

open System

module DtoXml =

    type MessagesDtoXml =
        {
            Msg1: string
            Msg2: string
            Msg3: string
            Msg4: string
            Msg5: string
            Msg6: string
        }
     
    type KontaktValuesDtoXml =
        {
            V001: string; V002: string; V003: string;
            V004: string; V005: string; V006: string;
            V007: string; Msgs: MessagesDtoXml
        }