namespace TransLayerXml.Server

open System
open SharedTypes
open DtoXml.Server.DtoXml

module TransLayerXml =

    let private messagesTransferLayerXmlToDomain (messagesXml: MessagesDtoXml) : MessagesDomain =
        {
             Msg1 = messagesXml.Msg1
             Msg2 = messagesXml.Msg2   
             Msg3 = messagesXml.Msg3  
             Msg4 = messagesXml.Msg4
             Msg5 = messagesXml.Msg5  
             Msg6 = messagesXml.Msg6                   
        }

    let private messagesTransferLayerDomainToXml (messagesXml: MessagesDomain) : MessagesDtoXml =
        {
             Msg1 = messagesXml.Msg1
             Msg2 = messagesXml.Msg2   
             Msg3 = messagesXml.Msg3  
             Msg4 = messagesXml.Msg4
             Msg5 = messagesXml.Msg5  
             Msg6 = messagesXml.Msg6                   
        } 
     
    let internal kontaktValuesTransferLayerXmlToDomain (kontaktValuesXmlToDomain: KontaktValuesDtoXml) : KontaktValuesDomain =
        {            
            V001 = kontaktValuesXmlToDomain.V001
            V002 = kontaktValuesXmlToDomain.V002
            V003 = kontaktValuesXmlToDomain.V003
            V004 = kontaktValuesXmlToDomain.V004
            V005 = kontaktValuesXmlToDomain.V005
            V006 = kontaktValuesXmlToDomain.V006
            V007 = kontaktValuesXmlToDomain.V007          
            Msgs = messagesTransferLayerXmlToDomain kontaktValuesXmlToDomain.Msgs
        }

    let internal kontaktValuesTransferLayerDomainToXml (kontaktValuesDomainToXml: KontaktValuesDomain) : KontaktValuesDtoXml =
           {            
               V001 = kontaktValuesDomainToXml.V001
               V002 = kontaktValuesDomainToXml.V002
               V003 = kontaktValuesDomainToXml.V003
               V004 = kontaktValuesDomainToXml.V004
               V005 = kontaktValuesDomainToXml.V005
               V006 = kontaktValuesDomainToXml.V006
               V007 = kontaktValuesDomainToXml.V007          
               Msgs = messagesTransferLayerDomainToXml kontaktValuesDomainToXml.Msgs
           }
    

   