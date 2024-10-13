namespace Shared

open System

//Common (shared) client/server domain model

[<Struct>]
type LoginValuesShared =
    {
        Username : SharedTypes.Username
        Password : SharedTypes.Password
    }