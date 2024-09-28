namespace ErrorTypes.Server

[<Struct>]
type SelectErrorOptions =   
   | InsertOrUpdateError1
   | InsertOrUpdateError2
   | ReadingDbError
   | SelectConnectionError

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
   | ExceptionError

[<Struct>]
type ErrorCopyingFiles = 
   | LegitimateTrue 
   | LegitimateFalse 
   | ExceptionError
