namespace TransLayerSend.Server

open System
open SharedTypes
open DtoSend.Server.DtoSend

module TransLayerSend =

    let private messagesTransferLayerSend (messagesDomain: MessagesDomain) : MessagesDtoSend =
        {
             Msg1 = messagesDomain.Msg1
             Msg2 = messagesDomain.Msg2   
             Msg3 = messagesDomain.Msg3  
             Msg4 = messagesDomain.Msg4
             Msg5 = messagesDomain.Msg5  
             Msg6 = messagesDomain.Msg6                   
        }

    let internal cenikValuesTransferLayerSend (cenikValuesDomain: CenikValuesDomain) : CenikValuesDtoSend =
        {
            Id = cenikValuesDomain.Id
            ValueState = cenikValuesDomain.ValueState
            V001 = cenikValuesDomain.V001
            V002 = cenikValuesDomain.V002
            V003 = cenikValuesDomain.V003
            V004 = cenikValuesDomain.V004
            V005 = cenikValuesDomain.V005
            V006 = cenikValuesDomain.V006
            V007 = cenikValuesDomain.V007
            V008 = cenikValuesDomain.V008
            V009 = cenikValuesDomain.V009
            Msgs = messagesTransferLayerSend cenikValuesDomain.Msgs
        }

    // Defined but currently unused; retained for potential future requirements or updates.    
    let internal kontaktValuesTransferLayerSend (kontaktValuesDomain: KontaktValuesDomain) : KontaktValuesDtoSend =
        {            
            V001 = kontaktValuesDomain.V001
            V002 = kontaktValuesDomain.V002
            V003 = kontaktValuesDomain.V003
            V004 = kontaktValuesDomain.V004
            V005 = kontaktValuesDomain.V005
            V006 = kontaktValuesDomain.V006
            V007 = kontaktValuesDomain.V007          
            Msgs = messagesTransferLayerSend kontaktValuesDomain.Msgs
        }

    let internal linkAndLinkNameValuesTransferLayerSend (linkAndLinkNameValuesDomain: LinkAndLinkNameValuesDomain) : LinkAndLinkNameValuesDtoSend =
        {            
            V001 = linkAndLinkNameValuesDomain.V001
            V002 = linkAndLinkNameValuesDomain.V002
            V003 = linkAndLinkNameValuesDomain.V003
            V004 = linkAndLinkNameValuesDomain.V004
            V005 = linkAndLinkNameValuesDomain.V005
            V006 = linkAndLinkNameValuesDomain.V006
            V001n = linkAndLinkNameValuesDomain.V001n
            V002n = linkAndLinkNameValuesDomain.V002n
            V003n = linkAndLinkNameValuesDomain.V003n
            V004n = linkAndLinkNameValuesDomain.V004n
            V005n = linkAndLinkNameValuesDomain.V005n
            V006n = linkAndLinkNameValuesDomain.V006n
            Msgs = messagesTransferLayerSend linkAndLinkNameValuesDomain.Msgs
        }