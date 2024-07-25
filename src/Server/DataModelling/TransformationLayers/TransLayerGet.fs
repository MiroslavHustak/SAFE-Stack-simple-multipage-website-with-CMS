namespace TransLayerGet.Server

open System

open Shared
open Helpers.Server
open ErrorTypes.Server
open DtoGet.Server.DtoGet
open Helpers.Server.CEBuilders

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

    let private messagesTransferLayerGetDefault : MessagesDtoGet =
        {
            Msg1 = String.Empty
            Msg2 = String.Empty
            Msg3 = String.Empty
            Msg4 = String.Empty
            Msg5 = String.Empty   
            Msg6 = String.Empty           
        } 

    let internal cenikValuesTransferLayerGet (cenikValuesDtoGet: CenikValuesDtoGet) =

        pyramidOfDoom
            {
                let! id = cenikValuesDtoGet.IdDtoGet, Error ReadingDbError  
                let! valueState = cenikValuesDtoGet.ValueStateDtoGet, Error ReadingDbError   
                let! v001 = cenikValuesDtoGet.V001DtoGet, Error ReadingDbError  
                let! v002 = cenikValuesDtoGet.V002DtoGet, Error ReadingDbError  
                let! v003 = cenikValuesDtoGet.V003DtoGet, Error ReadingDbError  
                let! v004 = cenikValuesDtoGet.V004DtoGet, Error ReadingDbError  
                let! v005 = cenikValuesDtoGet.V005DtoGet, Error ReadingDbError  
                let! v006 = cenikValuesDtoGet.V006DtoGet, Error ReadingDbError  
                let! v007 = cenikValuesDtoGet.V007DtoGet, Error ReadingDbError  
                let! v008 = cenikValuesDtoGet.V008DtoGet, Error ReadingDbError  
                let! v009 = cenikValuesDtoGet.V009DtoGet, Error ReadingDbError  

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
                                messagesTransferLayerGet
                                    (
                                        match cenikValuesDtoGet.MsgsDtoGet with
                                        | Some value -> value
                                        | None       -> messagesTransferLayerGetDefault
                                    )
                        }
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

        