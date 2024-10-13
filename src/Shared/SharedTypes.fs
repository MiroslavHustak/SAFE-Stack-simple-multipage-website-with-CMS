namespace Shared

open System

//https://stackoverflow.com/questions/59738472/struct-attribute-on-discriminated-unions

module SharedTypes = 

    //************** SCDUs for type-driven development (client + server) ***************
        
    type [<Struct>] Username = Username of string 
    type [<Struct>] Password = Password of string
    type [<Struct>] ErrorMsgLine1 = ErrorMsgLine1 of string
    type [<Struct>] ErrorMsgLine2 = ErrorMsgLine2 of string
    //type [<Struct>] AccessToken = AccessToken of string

    //*********************************************************************************
        
    [<Struct>]
    type LoginErrorMsgShared = //no DDD aplied for these data 
        {
            line1 : ErrorMsgLine1
            line2 : ErrorMsgLine2
        }    
    
    [<Struct>]
    type User =
        {
            Username : Username
            //AccessToken: AccessToken
        }         
                     
    //https://stackoverflow.com/questions/59738472/struct-attribute-on-discriminated-unions
    [<Struct>]
    type LoginResult =
        | UsernameOrPasswordIncorrect of UsernameOrPasswordIncorrect : LoginErrorMsgShared
        | LoggedIn of LoggedIn : User