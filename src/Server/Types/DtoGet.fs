namespace DtoGet.Server

open System
open SharedTypes

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
        static member Default =
               {
                   Msg1 = String.Empty; Msg2 = String.Empty;
                   Msg3 = String.Empty; Msg4 = String.Empty;
                   Msg5 = String.Empty; Msg6 = String.Empty
               }

    type CenikValuesDtoGet =
        {
            IdDtoGet: int*bool; ValueStateDtoGet: string*bool;
            V001DtoGet: string*bool; V002DtoGet: string*bool; V003DtoGet: string*bool;
            V004DtoGet: string*bool; V005DtoGet: string*bool; V006DtoGet: string*bool;
            V007DtoGet: string*bool; V008DtoGet: string*bool; V009DtoGet: string*bool;
            MsgsDtoGet: MessagesDtoGet
        }

   