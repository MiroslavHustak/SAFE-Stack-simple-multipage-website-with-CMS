namespace Shared

open System
open SharedTypes

module SharedLoginValues =

    let create username password = 
        {
            Username = username   //client -> server
            Password = password   //client -> server
        }
  
module SharedCenikValues =
        
    let create id valState v001 v002 v003 v004 v005 v006 v007 v008 v009 =
        {
            Id = id; ValueState = valState;
            V001 = v001; V002 = v002; V003 = v003;
            V004 = v004; V005 = v005; V006 = v006;
            V007 = v007; V008 = v008; V009 = v009;
            Msgs = MessagesDomain.Default
        }
   
module SharedDeserialisedCenikValues =
   
    let create (cenikInputValues: CenikValuesDomain) =
        {
            Id = cenikInputValues.Id; ValueState = cenikInputValues.ValueState;
            V001 = cenikInputValues.V001; V002 = cenikInputValues.V002; V003 = cenikInputValues.V003;
            V004 = cenikInputValues.V004; V005 = cenikInputValues.V005; V006 = cenikInputValues.V006;
            V007 = cenikInputValues.V007; V008 = cenikInputValues.V008; V009 = cenikInputValues.V009;
            Msgs = MessagesDomain.Default
        }

module SharedKontaktValues =
            
    let create v001 v002 v003 v004 v005 v006 v007 =
        {
            V001 = v001; V002 = v002; V003 = v003
            V004 = v004; V005 = v005;
            V006 = v006; V007 = v007;  Msgs = MessagesDomain.Default
        }

module SharedDeserialisedKontaktValues =
   
    let create (kontaktInputValues: KontaktValuesDomain) =
        {
            V001 = kontaktInputValues.V001; V002 = kontaktInputValues.V002; V003 = kontaktInputValues.V003;
            V004 = kontaktInputValues.V004; V005 = kontaktInputValues.V005;
            V006 = kontaktInputValues.V006; V007 = kontaktInputValues.V007; Msgs = MessagesDomain.Default           
        }

module SharedLinkAndLinkNameValues =
    
    let create v001 v002 v003 v004 v005 v006 v001n v002n v003n v004n v005n v006n =
        {
            V001 = v001; V002 = v002; V003 = v003;
            V004 = v004; V005 = v005; V006 = v006;
            V001n = v001n; V002n = v002n; V003n = v003n;
            V004n = v004n; V005n = v005n; V006n = v006n;
            Msgs = MessagesDomain.Default    
        }

module SharedDeserialisedLinkAndLinkNameValues =
   
    let create (linkAndLinkNameInputValues: LinkAndLinkNameValuesDomain) =
        {
            V001 = linkAndLinkNameInputValues.V001; V002 = linkAndLinkNameInputValues.V002; V003 = linkAndLinkNameInputValues.V003;
            V004 = linkAndLinkNameInputValues.V004; V005 = linkAndLinkNameInputValues.V005; V006 = linkAndLinkNameInputValues.V006;
            V001n = linkAndLinkNameInputValues.V001n; V002n = linkAndLinkNameInputValues.V002n; V003n = linkAndLinkNameInputValues.V003n;
            V004n = linkAndLinkNameInputValues.V004n; V005n = linkAndLinkNameInputValues.V005n; V006n = linkAndLinkNameInputValues.V006n;
            Msgs = MessagesDomain.Default    
        }

module Route =

    let builder typeName methodName = sprintf "/api/%s/%s" typeName methodName

