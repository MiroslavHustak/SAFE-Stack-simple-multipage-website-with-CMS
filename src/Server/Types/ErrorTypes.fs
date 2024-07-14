namespace ErrorTypes.Server

[<Struct>]
type SelectErrorOptions =   
   | InsertOrUpdateError1
   | InsertOrUpdateError2
   | ReadingDbError
   | ConnectionError

[<Struct>]
type InsertErrorOptions =   
   | FirstRunError
   | InsertOrUpdateError
   | NoError
   | InsertConnectionError

[<Struct>]
type ErrorVerifyOptions = 
   | LegitimateTrue 
   | LegitimateFalse 
   | Exception

[<Struct>]
type ErrorCopyingFiles = 
   | LegitimateTrue 
   | LegitimateFalse 
   | Exception
