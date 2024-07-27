namespace Records.Client

open System

open Feliz
           
[<Struct>]
type ReactPage =
    {
        Home: IReactProperty
        Sluzby: IReactProperty
        Cenik: IReactProperty
        Nenajdete: IReactProperty
        Kontakt: IReactProperty
    }