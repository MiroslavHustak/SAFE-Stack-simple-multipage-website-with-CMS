namespace DiscriminatedUnions.Server

type ResultSF<'TSuccess,'TFailure> =
   | Success of 'TSuccess
   | Failure of 'TFailure

[<Struct>]
type SelectErrorOptions =   
   | InsertOrUpdateError1
   | InsertOrUpdateError2
   | ReadingDbError
   | ConnectionError
   | NoSelectError

[<Struct>]
type InsertErrorOptions =   
   | FirstRunError
   | InsertOrUpdateError
   | NoInsertError

[<Struct>]
type ErrorVerifyOptions = 
   | LegitimateTrue 
   | LegitimateFalse 
   | Exception
