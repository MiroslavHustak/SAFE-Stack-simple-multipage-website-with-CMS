namespace TransLayerSend.Server

open System
open DtoSend.Server.DtoSend
open SharedTypes

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