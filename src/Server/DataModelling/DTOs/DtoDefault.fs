namespace DtoDefault.Server

open System

open DtoFromStorage.Server.DtoFromStorage
open DtoXml.Server.DtoXml
open DtoToStorage.Server.DtoToStorage

module DtoDefault =  

    // Defined but currently unused; retained for potential future requirements or updates. 
    let MessagesDtoSendDefault =        
        {
            DtoToStorage.Server.DtoToStorage.Msg1 = String.Empty
            DtoToStorage.Server.DtoToStorage.Msg2 = String.Empty
            DtoToStorage.Server.DtoToStorage.Msg3 = String.Empty
            DtoToStorage.Server.DtoToStorage.Msg4 = String.Empty
            DtoToStorage.Server.DtoToStorage.Msg5 = String.Empty
            DtoToStorage.Server.DtoToStorage.Msg6 = String.Empty
        }
     
    let MessagesDtoFromStorageDefault =        
        {
            DtoFromStorage.Server.DtoFromStorage.Msg1 = String.Empty 
            DtoFromStorage.Server.DtoFromStorage.Msg2 = String.Empty
            DtoFromStorage.Server.DtoFromStorage.Msg3 = String.Empty
            DtoFromStorage.Server.DtoFromStorage.Msg4 = String.Empty
            DtoFromStorage.Server.DtoFromStorage.Msg5 = String.Empty
            DtoFromStorage.Server.DtoFromStorage.Msg6 = String.Empty
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