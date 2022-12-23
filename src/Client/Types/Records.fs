namespace Records

open System

open Feliz

[<Struct>]
type MyCssClass =
    {
        Home: IReactProperty
        Sluzby: IReactProperty
        Cenik: IReactProperty
        Nenajdete: IReactProperty
        Kontakt: IReactProperty
    }