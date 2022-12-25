namespace DiscriminatedUnions.Server

type Result<'TSuccess,'TFailure> =
   | Success of 'TSuccess
   | Failure of 'TFailure

[<Struct>]
type ErrorOptions = 
   | Default
   | NotFullDb
   | ProblemsWithReader
   | OtherProblems