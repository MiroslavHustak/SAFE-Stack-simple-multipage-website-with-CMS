namespace Shared

module AssemblyInfo =

    open System.Runtime.CompilerServices

    [<assembly: InternalsVisibleTo("Client")>]
    [<assembly: InternalsVisibleTo("Server")>]
    [<assembly: InternalsVisibleTo("Shared.Tests")>]
    do ()



