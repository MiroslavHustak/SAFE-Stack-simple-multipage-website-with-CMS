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

[<Struct>]
type ErrorVerifyOptions = 
   | LegitimateTrue 
   | LegitimateFalse 
   | Exception
