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

    //nevyuzito, ale ponechavam pro pripad zmeny    
    let internal kontaktValuesTransferLayerGet (kontaktValuesDtoGet: KontaktValuesDtoGet) : KontaktValuesDomain  =
        {            
            V001 = kontaktValuesDtoGet.V001
            V002 = kontaktValuesDtoGet.V002
            V003 = kontaktValuesDtoGet.V003
            V004 = kontaktValuesDtoGet.V004
            V005 = kontaktValuesDtoGet.V005
            V006 = kontaktValuesDtoGet.V006
            V007 = kontaktValuesDtoGet.V007          
            Msgs = messagesTransferLayerGet kontaktValuesDtoGet.Msgs
        }

    let internal linkAndLinkNameValuesTransferLayerGet (linkAndLinkNameValuesDtoGet: LinkAndLinkNameValuesDtoGet) : LinkAndLinkNameValuesDomain  =
        {            
            V001 = linkAndLinkNameValuesDtoGet.V001
            V002 = linkAndLinkNameValuesDtoGet.V002
            V003 = linkAndLinkNameValuesDtoGet.V003
            V004 = linkAndLinkNameValuesDtoGet.V004
            V005 = linkAndLinkNameValuesDtoGet.V005
            V006 = linkAndLinkNameValuesDtoGet.V006
            V001n = linkAndLinkNameValuesDtoGet.V001n
            V002n = linkAndLinkNameValuesDtoGet.V002n
            V003n = linkAndLinkNameValuesDtoGet.V003n
            V004n = linkAndLinkNameValuesDtoGet.V004n
            V005n = linkAndLinkNameValuesDtoGet.V005n
            V006n = linkAndLinkNameValuesDtoGet.V006n
            Msgs = messagesTransferLayerGet linkAndLinkNameValuesDtoGet.Msgs
        }

        