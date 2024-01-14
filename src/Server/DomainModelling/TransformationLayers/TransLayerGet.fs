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
    
    let private extractValue p =
        match p with
        | Some value -> value
        | None       -> failwith "Already checked, an exception should not happen"

    let internal cenikValuesTransferLayerGet (cenikValuesDtoGet: CenikValuesDtoGet) : CenikValuesDomain =
        {
            Id = extractValue cenikValuesDtoGet.IdDtoGet
            ValueState = extractValue cenikValuesDtoGet.ValueStateDtoGet
            V001 = extractValue cenikValuesDtoGet.V001DtoGet
            V002 = extractValue cenikValuesDtoGet.V002DtoGet
            V003 = extractValue cenikValuesDtoGet.V003DtoGet
            V004 = extractValue cenikValuesDtoGet.V004DtoGet
            V005 = extractValue cenikValuesDtoGet.V005DtoGet
            V006 = extractValue cenikValuesDtoGet.V006DtoGet
            V007 = extractValue cenikValuesDtoGet.V007DtoGet
            V008 = extractValue cenikValuesDtoGet.V008DtoGet
            V009 = extractValue cenikValuesDtoGet.V009DtoGet
            Msgs = messagesTransferLayerGet cenikValuesDtoGet.MsgsDtoGet
        }  

    // Defined but currently unused; retained for potential future requirements or updates.     
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

        