namespace Client

module AssemblyInfo =

    open System.Runtime.CompilerServices

    [<assembly: InternalsVisibleTo("Shared")>]
    [<assembly: InternalsVisibleTo("Server")>]
    [<assembly: InternalsVisibleTo("Client.Tests")>]
    do ()



