namespace DtoDefault.Server

open System

open DtoGet.Server.DtoGet
open DtoXml.Server.DtoXml
open DtoSend.Server.DtoSend

module DtoDefault =  

    // Defined but currently unused; retained for potential future requirements or updates. 
    let MessagesDtoSendDefault =        
        {
            DtoSend.Server.DtoSend.Msg1 = String.Empty
            DtoSend.Server.DtoSend.Msg2 = String.Empty
            DtoSend.Server.DtoSend.Msg3 = String.Empty
            DtoSend.Server.DtoSend.Msg4 = String.Empty
            DtoSend.Server.DtoSend.Msg5 = String.Empty
            DtoSend.Server.DtoSend.Msg6 = String.Empty
        }
     
    let MessagesDtoGetDefault =        
        {
            DtoGet.Server.DtoGet.Msg1 = String.Empty
            DtoGet.Server.DtoGet.Msg2 = String.Empty
            DtoGet.Server.DtoGet.Msg3 = String.Empty
            DtoGet.Server.DtoGet.Msg4 = String.Empty
            DtoGet.Server.DtoGet.Msg5 = String.Empty
            DtoGet.Server.DtoGet.Msg6 = String.Empty
        }

    // Defined but currently unused; retained for potential future requirements or updates.     
    let MessagesDtoXmlDefault =       
        {
            DtoXml.Server.DtoXml.Msg1 = String.Empty
            DtoXml.Server.DtoXml.Msg2 = String.Empty
            DtoXml.Server.DtoXml.Msg3 = String.Empty
            DtoXml.Server.DtoXml.Msg4 = String.Empty
            DtoXml.Server.DtoXml.Msg5 = String.Empty
            DtoXml.Server.DtoXml.Msg6 = String.Empty
        }