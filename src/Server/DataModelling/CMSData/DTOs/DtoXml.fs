namespace DtoXml.Server

open System
open System.Xml.Serialization

module DtoXml =

    //System.Runtime.Serialization 

    type MessagesDtoXml =
        {
            Msg1 : string
            Msg2 : string
            Msg3 : string
            Msg4 : string
            Msg5 : string
            Msg6 : string
        }
     
    type KontaktValuesDtoXml =
        {
            V001 : string
            V002 : string
            V003 : string
            V004 : string
            V005 : string
            V006 : string
            V007 : string
            Msgs : MessagesDtoXml
        }

    //*******************************************************

    //System.Xml.Serialization

    //https://dev.to/robmulpeter/xml-serialization-for-f-record-types-2p9l

    //The [<CLIMutable>] attribute instructs the compiler to add internal auto-generated mutable fields.
    //From a programming perspective, the type is still immutable as these internal fields can only be accessed by
    //automated processes or if you decided to expose the type to C# code.

    [<CLIMutable>]
    [<XmlRoot("MessagesDtoXml2")>]
    type MessagesDtoXml2 =
        {
            [<XmlElement("Msg1")>] Msg1 : string
            [<XmlElement("Msg2")>] Msg2 : string
            [<XmlElement("Msg3")>] Msg3 : string
            [<XmlElement("Msg4")>] Msg4 : string
            [<XmlElement("Msg5")>] Msg5 : string
            [<XmlElement("Msg6")>] Msg6 : string
        }

    [<CLIMutable>]
    [<XmlRoot("KontaktValuesDtoXml2")>]
    type KontaktValuesDtoXml2 =
        {
            [<XmlElement("V001")>] V001 : string
            [<XmlElement("V002")>] V002 : string
            [<XmlElement("V003")>] V003 : string
            [<XmlElement("V004")>] V004 : string
            [<XmlElement("V005")>] V005 : string
            [<XmlElement("V006")>] V006 : string
            [<XmlElement("V007")>] V007 : string
            [<XmlElement("Msgs")>] Msgs : MessagesDtoXml2
        }

    //*******************************************************    

    //System.Xml.Linq
    
    type MessagesDtoXml3 =
        {
            Msg1 : string
            Msg2 : string
            Msg3 : string
            Msg4 : string
            Msg5 : string
            Msg6 : string
        }
         
    type KontaktValuesDtoXml3 =
        {
            V001 : string
            V002 : string
            V003 : string
            V004 : string
            V005 : string
            V006 : string
            V007 : string
            Msgs : MessagesDtoXml3
        }
    
    //******************************************************* 
    