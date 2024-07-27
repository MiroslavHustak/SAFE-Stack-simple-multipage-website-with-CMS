namespace DtoToStorage.Server

open System

module DtoToStorage =
      
    type MessagesDtoToStorage =
        {
            Msg1: string
            Msg2: string
            Msg3: string
            Msg4: string
            Msg5: string
            Msg6: string
        }
        
    type CenikValuesDtoToStorage =
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
            Msgs: MessagesDtoToStorage
        }

    // Defined but currently unused; retained for potential future requirements or updates.  
    type KontaktValuesDtoToStorage =
        {
            V001: string
            V002: string
            V003: string
            V004: string
            V005: string
            V006: string
            V007: string
            Msgs: MessagesDtoToStorage
        }     

    type LinkValuesDtoToStorage  =
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
            Msgs: MessagesDtoToStorage
        }
  
