namespace Shared

open System

//Common (shared) client/server domain model

type MessagesShared =
    {
        Msg1: string
        Msg2: string
        Msg3: string
        Msg4: string
        Msg5: string
        Msg6: string
    }  

type CenikValuesShared =
    {
        Id: int
        ValueState: string
        V001: string
        V002: string
        V003: string
        V004: string
        V005: string
        V006: string
        V007: string
        V008: string
        V009: string
        Msgs: MessagesShared
    }   

type KontaktValuesShared =
    {
        V001: string
        V002: string
        V003: string
        V004: string
        V005: string
        V006: string
        V007: string
        Msgs: MessagesShared
    }    

type LinkValuesShared = 
    {
        V001: string
        V002: string
        V003: string
        V004: string
        V005: string
        V006: string
        V001n: string
        V002n: string
        V003n: string
        V004n: string
        V005n: string
        V006n: string
        Msgs: MessagesShared
    }
