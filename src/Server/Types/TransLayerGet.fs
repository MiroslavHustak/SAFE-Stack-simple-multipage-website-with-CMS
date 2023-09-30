namespace TransLayerGet.Server

open System
open SharedTypes
open DtoGet.Server.DtoGet

module TransLayerGet =

    let private messagesTransferLayerGet (messagesDto: MessagesDtoGet) : MessagesDomain =
        {
             Msg1 = messagesDto.Msg1
             Msg2 = messagesDto.Msg2   
             Msg3 = messagesDto.Msg3  
             Msg4 = messagesDto.Msg4
             Msg5 = messagesDto.Msg5  
             Msg6 = messagesDto.Msg6                   
        }

    let internal cenikValuesTransferLayerGet (cenikValuesDtoGet: CenikValuesDtoGet) : CenikValuesDomain =
        {
            Id = fst cenikValuesDtoGet.IdDtoGet
            ValueState = fst cenikValuesDtoGet.ValueStateDtoGet
            V001 = fst cenikValuesDtoGet.V001DtoGet
            V002 = fst cenikValuesDtoGet.V002DtoGet
            V003 = fst cenikValuesDtoGet.V003DtoGet
            V004 = fst cenikValuesDtoGet.V004DtoGet
            V005 = fst cenikValuesDtoGet.V005DtoGet
            V006 = fst cenikValuesDtoGet.V006DtoGet
            V007 = fst cenikValuesDtoGet.V007DtoGet
            V008 = fst cenikValuesDtoGet.V008DtoGet
            V009 = fst cenikValuesDtoGet.V009DtoGet
            Msgs = messagesTransferLayerGet cenikValuesDtoGet.MsgsDtoGet
        }

        