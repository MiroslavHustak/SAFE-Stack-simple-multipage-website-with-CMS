namespace LoginValuesDomain.Server

open System

module LoginValuesDomain =

    [<Struct>]
    type LoginValuesDomain =
        {
            username: string
            password: string           
        }

    (*
    [<Struct>]
    type LoginValuesDomain<'a, 'b> = 
        {
            Username: 'a
            Password: 'b
        }
    *)

    
       