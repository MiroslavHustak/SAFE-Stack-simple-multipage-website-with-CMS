namespace DtoFromStorage.Server

open System

module DtoFromStorage =   

    type MessagesDtoFromStorage =
        {
            Msg1 : string
            Msg2 : string
            Msg3 : string
            Msg4 : string
            Msg5 : string
            Msg6 : string
        }
       
    type CenikValuesDtoFromStorage =
        {
            IdDtoGet : int option
            ValueStateDtoGet : string option
            V001DtoGet : string option
            V002DtoGet : string option
            V003DtoGet : string option
            V004DtoGet : string option
            V005DtoGet : string option
            V006DtoGet : string option
            V007DtoGet : string option
            V008DtoGet : string option
            V009DtoGet : string option
            MsgsDtoGet : MessagesDtoFromStorage option
        }

    // Defined but currently unused; retained for potential future requirements or updates.     
    type KontaktValuesDtoFromStorage =
        {
            V001 : string
            V002 : string
            V003 : string
            V004 : string
            V005 : string
            V006 : string
            V007 : string
            Msgs : MessagesDtoFromStorage
        }

    type LinkValuesDtoFromStorage  =
        {
            V001 : string
            V002 : string
            V003 : string
            V004 : string
            V005 : string
            V006 : string
            V001n : string
            V002n : string
            V003n : string
            V004n : string
            V005n : string
            V006n : string
            Msgs : MessagesDtoFromStorage
        }

   