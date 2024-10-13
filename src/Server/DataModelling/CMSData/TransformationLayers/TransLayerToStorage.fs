namespace TransLayerSend.Server

open System
open Shared
open DtoToStorage.Server.DtoToStorage

module TransLayerSend =

    let private messagesTransformLayerToStorage (messagesDomain : MessagesShared) : MessagesDtoToStorage =
        {
             Msg1 = messagesDomain.Msg1
             Msg2 = messagesDomain.Msg2   
             Msg3 = messagesDomain.Msg3  
             Msg4 = messagesDomain.Msg4
             Msg5 = messagesDomain.Msg5  
             Msg6 = messagesDomain.Msg6                   
        }

    let internal cenikValuesTransformLayerToStorage (cenikValuesDomain : CenikValuesShared) : CenikValuesDtoToStorage =
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
            Msgs = messagesTransformLayerToStorage cenikValuesDomain.Msgs
        }

    // Defined but currently unused; retained for potential future requirements or updates.    
    let internal kontaktValuesTransformLayerToStorage (kontaktValuesDomain : KontaktValuesShared) : KontaktValuesDtoToStorage =
        {            
            V001 = kontaktValuesDomain.V001
            V002 = kontaktValuesDomain.V002
            V003 = kontaktValuesDomain.V003
            V004 = kontaktValuesDomain.V004
            V005 = kontaktValuesDomain.V005
            V006 = kontaktValuesDomain.V006
            V007 = kontaktValuesDomain.V007          
            Msgs = messagesTransformLayerToStorage kontaktValuesDomain.Msgs
        }

    let internal linkValuesTransformLayerToStorage (linkValuesDomain : LinkValuesShared) : LinkValuesDtoToStorage =
        {            
            V001 = linkValuesDomain.V001
            V002 = linkValuesDomain.V002
            V003 = linkValuesDomain.V003
            V004 = linkValuesDomain.V004
            V005 = linkValuesDomain.V005
            V006 = linkValuesDomain.V006
            V001n = linkValuesDomain.V001n
            V002n = linkValuesDomain.V002n
            V003n = linkValuesDomain.V003n
            V004n = linkValuesDomain.V004n
            V005n = linkValuesDomain.V005n
            V006n = linkValuesDomain.V006n
            Msgs = messagesTransformLayerToStorage linkValuesDomain.Msgs
        }