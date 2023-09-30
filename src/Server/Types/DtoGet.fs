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

    type CenikValuesDtoGet1 =
        {
            IdDtoGet1: Result<int, int>; ValueStateDtoGet1: Result<string, string>;
            V001DtoGet1: Result<string, string>; V002DtoGet1: Result<string, string>; V003DtoGet1: Result<string, string>;
            V004DtoGet1: Result<string, string>; V005DtoGet1: Result<string, string>; V006DtoGet1: Result<string, string>;
            V007DtoGet1: Result<string, string>; V008DtoGet1: Result<string, string>; V009DtoGet1: Result<string, string>;
            MsgsDtoGet1: MessagesDtoGet
        }        
      

