namespace TransLayerGet.Server

open System

open SharedTypes
open Helpers.Server
open ErrorTypes.Server
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

    let internal cenikValuesTransferLayerGet (cenikValuesDtoGet: CenikValuesDtoGet) =              

        let listInt = 
            [
                Casting.castAs<int> cenikValuesDtoGet.IdDtoGet;               
            ]

        let listString =    
            [
                Casting.castAs<string> cenikValuesDtoGet.ValueStateDtoGet;
                Casting.castAs<string> cenikValuesDtoGet.V001DtoGet;
                Casting.castAs<string> cenikValuesDtoGet.V002DtoGet;
                Casting.castAs<string> cenikValuesDtoGet.V003DtoGet;
                Casting.castAs<string> cenikValuesDtoGet.V004DtoGet;
                Casting.castAs<string> cenikValuesDtoGet.V005DtoGet;
                Casting.castAs<string> cenikValuesDtoGet.V006DtoGet;
                Casting.castAs<string> cenikValuesDtoGet.V007DtoGet;
                Casting.castAs<string> cenikValuesDtoGet.V008DtoGet;
                Casting.castAs<string> cenikValuesDtoGet.V009DtoGet
            ]

        (listInt |> List.contains None || listString |> List.contains None)
        |> function
            | false  ->
                      let listInt = listInt |> List.choose id
                      let listString = listString |> List.choose id
                 
                      Ok
                          {
                              Id = listInt.Head
                              ValueState = listString.Head
                              V001 = listString |> List.item 1
                              V002 = listString |> List.item 2
                              V003 = listString |> List.item 3
                              V004 = listString |> List.item 4
                              V005 = listString |> List.item 5
                              V006 = listString |> List.item 6
                              V007 = listString |> List.item 7
                              V008 = listString |> List.item 8
                              V009 = listString |> List.item 9
                              Msgs = messagesTransferLayerGet cenikValuesDtoGet.MsgsDtoGet
                          }
                          
            | true ->
                    Error ReadingDbError      

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

        