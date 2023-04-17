namespace SharedTypes

open System

//https://stackoverflow.com/questions/59738472/struct-attribute-on-discriminated-unions

[<RequireQualifiedAccess>]
module SharedApi = 

    //[<Struct>]
    //type AccessToken = AccessToken of string

    [<Struct>]
    type Username = Username of string //See Isaac Abraham page 272 onwards

    [<Struct>]
    type Password = Password of string

    [<Struct>]
    type LoginProblems =
        {
            line1: string
            line2: string
        }    

    [<Struct>]
    type User =
        {
            Username: Username
            //AccessToken: AccessToken
        }         
                 
    //https://stackoverflow.com/questions/59738472/struct-attribute-on-discriminated-unions
    [<Struct>]
    type LoginResult =
        | UsernameOrPasswordIncorrect of UsernameOrPasswordIncorrect: LoginProblems
        | LoggedIn of LoggedIn: User


//**********************************************

(*

Automatic conversions:

F# OOP

type AccessToken(token: string) =
    member val Token = token with get

type Username(name: string) =
    member val Name = name with get

type Password(value: string) =
    member val Value = value with get

type LoginProblems(line1: string, line2: string) =
    member val Line1 = line1 with get
    member val Line2 = line2 with get

type LoginResult(loginProblems: option<LoginProblems>, user: option<User>) =
    member val LoginProblems = loginProblems with get
    member val User = user with get

type User(username: Username, accessToken: AccessToken) =
    member val Username = username with get
    member val AccessToken = accessToken with get

type Login(login: string, password: string) =
    member val Login = login with get
    member val Password = password with get

type Auth =
    static member Login(login: Login, password: Password) =
        match (login.Login, password.Value) with
        | ("admin", "password123") ->
            let accessToken = AccessToken("token")
            let username = Username(login.Login)
            let user = User(username, accessToken)
            LoginResult(None, Some(user))
        | _ ->
            let loginProblems = LoginProblems("Username or password is incorrect", "Please try again")
            LoginResult(Some(loginProblems), None)


C# OOP

using System;

namespace SharedApi
{
    public struct AccessToken
    {
        public string Token { get; }

        public AccessToken(string token)
        {
            Token = token;
        }
    }

    public struct Username
    {
        public string Name { get; }

        public Username(string name)
        {
            Name = name;
        }
    }

    public struct Password
    {
        public string Value { get; }

        public Password(string value)
        {
            Value = value;
        }
    }

    public struct LoginProblems
    {
        public string Line1 { get; }
        public string Line2 { get; }

        public LoginProblems(string line1, string line2)
        {
            Line1 = line1;
            Line2 = line2;
        }
    }

    public struct User
    {
        public Username Username { get; }
        public AccessToken AccessToken { get; }

        public User(Username username, AccessToken accessToken)
        {
            Username = username;
            AccessToken = accessToken;
        }
    }

    public enum LoginResultType
    {
        UsernameOrPasswordIncorrect,
        LoggedIn
    }

    public class LoginResult
    {
        public LoginResultType Type { get; }
        public object Result { get; }

        public LoginResult(LoginProblems loginProblems)
        {
            Type = LoginResultType.UsernameOrPasswordIncorrect;
            Result = loginProblems;
        }

        public LoginResult(User user)
        {
            Type = LoginResultType.LoggedIn;
            Result = user;
        }
    }
}

//nebo takto
//***************************************************************************
//druha varianta

using System;

namespace SharedApi
{    
    public class AccessToken
    {
        public string Token { get; }
    
        public AccessToken(string token)
        {
            Token = token;
        }
    }

    public class Username
    {
        public string Name { get; }
    
        public Username(string name)
        {
            Name = name;
        }
    }

    public class Password
    {
        public string Text { get; }
    
        public Password(string text)
        {
            Text = text;
        }
    }

    public class LoginProblems
    {
        public string Line1 { get; }
        public string Line2 { get; }
    
        public LoginProblems(string line1, string line2)
        {
            Line1 = line1;
            Line2 = line2;
        }
    }

    public class User
    {
        public Username Username { get; }
        public AccessToken AccessToken { get; }
    
        public User(Username username, AccessToken accessToken)
        {
            Username = username;
            AccessToken = accessToken;
        }
    }

    public abstract class LoginResult
    {
    }

    public class UsernameOrPasswordIncorrect : LoginResult
    {
        public LoginProblems Problems { get; }
    
        public UsernameOrPasswordIncorrect(LoginProblems problems)
        {
            Problems = problems;
        }
    }

    public class LoggedIn : LoginResult
    {
        public User User { get; }
    
        public LoggedIn(User user)
        {
            User = user;
        }
    }

}
// Factory Method pattern to create instances of LoginResult.
//The abstract LoginResult class is the product,
//while UsernameOrPasswordIncorrect and LoggedIn are concrete implementations of that product.

*)