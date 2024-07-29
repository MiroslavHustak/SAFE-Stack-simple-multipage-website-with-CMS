namespace TransLayerFromStorage.Server

open System

open Shared
open ErrorTypes.Server

open Helpers.Server
open Helpers.Server.CEBuilders
open DtoFromStorage.Server.DtoFromStorage

module TransLayerFromStorage =

    let private messagesTransformLayerFromStorage (messagesDto: MessagesDtoFromStorage) : MessagesShared =
        {
             Msg1 = messagesDto.Msg1
             Msg2 = messagesDto.Msg2
             Msg3 = messagesDto.Msg3 
             Msg4 = messagesDto.Msg4 
             Msg5 = messagesDto.Msg5   
             Msg6 = messagesDto.Msg6             
        }

    let private messagesTransformLayerFromStorageDefault : MessagesDtoFromStorage =
        {
            Msg1 = String.Empty
            Msg2 = String.Empty
            Msg3 = String.Empty
            Msg4 = String.Empty
            Msg5 = String.Empty   
            Msg6 = String.Empty           
        } 

    let internal cenikValuesTransformLayerFromStorage (cenikValuesDtoFromStorage: CenikValuesDtoFromStorage) =

        pyramidOfDoom
            {
                let! id = cenikValuesDtoFromStorage.IdDtoGet, Error ReadingDbError  
                let! valueState = cenikValuesDtoFromStorage.ValueStateDtoGet, Error ReadingDbError   
                let! v001 = cenikValuesDtoFromStorage.V001DtoGet, Error ReadingDbError  
                let! v002 = cenikValuesDtoFromStorage.V002DtoGet, Error ReadingDbError  
                let! v003 = cenikValuesDtoFromStorage.V003DtoGet, Error ReadingDbError  
                let! v004 = cenikValuesDtoFromStorage.V004DtoGet, Error ReadingDbError  
                let! v005 = cenikValuesDtoFromStorage.V005DtoGet, Error ReadingDbError  
                let! v006 = cenikValuesDtoFromStorage.V006DtoGet, Error ReadingDbError  
                let! v007 = cenikValuesDtoFromStorage.V007DtoGet, Error ReadingDbError  
                let! v008 = cenikValuesDtoFromStorage.V008DtoGet, Error ReadingDbError  
                let! v009 = cenikValuesDtoFromStorage.V009DtoGet, Error ReadingDbError  

                return
                    Ok
                        {
                            Id = id
                            ValueState = valueState
                            V001 = v001
                            V002 = v002
                            V003 = v003
                            V004 = v004
                            V005 = v005
                            V006 = v006
                            V007 = v007
                            V008 = v008
                            V009 = v009
                            Msgs =
                                messagesTransformLayerFromStorage
                                    (
                                        match cenikValuesDtoFromStorage.MsgsDtoGet with
                                        | Some value -> value
                                        | None       -> messagesTransformLayerFromStorageDefault
                                    )
                        }
            }       

    // Defined but currently unused; retained for potential future requirements or updates.     
    let internal kontaktValuesTransformLayerFromStorage (kontaktValuesDtoFromStorage: KontaktValuesDtoFromStorage) : KontaktValuesShared  =
        {            
            V001 = kontaktValuesDtoFromStorage.V001
            V002 = kontaktValuesDtoFromStorage.V002
            V003 = kontaktValuesDtoFromStorage.V003
            V004 = kontaktValuesDtoFromStorage.V004
            V005 = kontaktValuesDtoFromStorage.V005
            V006 = kontaktValuesDtoFromStorage.V006
            V007 = kontaktValuesDtoFromStorage.V007          
            Msgs = messagesTransformLayerFromStorage kontaktValuesDtoFromStorage.Msgs
        }

    let internal linkValuesTransformLayerFromStorage (linkValuesDtoFromStorage: LinkValuesDtoFromStorage) : LinkValuesShared  =
        {            
            V001 = linkValuesDtoFromStorage.V001
            V002 = linkValuesDtoFromStorage.V002
            V003 = linkValuesDtoFromStorage.V003
            V004 = linkValuesDtoFromStorage.V004
            V005 = linkValuesDtoFromStorage.V005
            V006 = linkValuesDtoFromStorage.V006
            V001n = linkValuesDtoFromStorage.V001n
            V002n = linkValuesDtoFromStorage.V002n
            V003n = linkValuesDtoFromStorage.V003n
            V004n = linkValuesDtoFromStorage.V004n
            V005n = linkValuesDtoFromStorage.V005n
            V006n = linkValuesDtoFromStorage.V006n
            Msgs = messagesTransformLayerFromStorage linkValuesDtoFromStorage.Msgs
        }

        