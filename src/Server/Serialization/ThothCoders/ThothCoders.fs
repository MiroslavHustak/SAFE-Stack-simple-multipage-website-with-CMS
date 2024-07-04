namespace Serialization.Coders.Server

//Compiler-independent template suitable for Shared as well

//************************************************

//Compiler directives
#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

//*********************************************** 
open DtoGet.Server.DtoGet
open DtoSend.Server.DtoSend

module ThothCoders =   
                   
    let encoder (link : LinkAndLinkNameValuesDtoSend) =
        Encode.object
            [
                "V001", Encode.string link.V001
                "V002", Encode.string link.V002
                "V003", Encode.string link.V003
                "V004", Encode.string link.V004
                "V005", Encode.string link.V005
                "V006", Encode.string link.V006
                "V001n", Encode.string link.V001n
                "V002n", Encode.string link.V002n
                "V003n", Encode.string link.V003n
                "V004n", Encode.string link.V004n
                "V005n", Encode.string link.V005n
                "V006n", Encode.string link.V006n
                "Msgs",
                    Encode.object
                        [
                            "Msg1", Encode.string link.Msgs.Msg1
                            "Msg2", Encode.string link.Msgs.Msg2
                            "Msg3", Encode.string link.Msgs.Msg3
                            "Msg4", Encode.string link.Msgs.Msg4
                            "Msg5", Encode.string link.Msgs.Msg5
                            "Msg6", Encode.string link.Msgs.Msg6
                        ]
            ]


    let decoder : Decoder<LinkAndLinkNameValuesDtoGet> =
        Decode.object
            (fun get ->
                {
                    V001 = get.Required.Field "V001" Decode.string
                    V002 = get.Required.Field "V002" Decode.string
                    V003 = get.Required.Field "V003" Decode.string
                    V004 = get.Required.Field "V004" Decode.string
                    V005 = get.Required.Field "V005" Decode.string
                    V006 = get.Required.Field "V006" Decode.string
                    V001n = get.Required.Field "V001n" Decode.string
                    V002n = get.Required.Field "V002n" Decode.string
                    V003n = get.Required.Field "V003n" Decode.string
                    V004n = get.Required.Field "V004n" Decode.string
                    V005n = get.Required.Field "V005n" Decode.string
                    V006n = get.Required.Field "V006n" Decode.string
                    Msgs =
                        get.Required.Field "Msgs"
                            (Decode.object
                                (fun get1
                                    ->
                                        {
                                            Msg1 = get1.Required.Field "Msg1" Decode.string
                                            Msg2 = get1.Required.Field "Msg2" Decode.string
                                            Msg3 = get1.Required.Field "Msg3" Decode.string
                                            Msg4 = get1.Required.Field "Msg4" Decode.string
                                            Msg5 = get1.Required.Field "Msg5" Decode.string
                                            Msg6 = get1.Required.Field "Msg6" Decode.string
                                        }
                                )
                            )
                }
            )