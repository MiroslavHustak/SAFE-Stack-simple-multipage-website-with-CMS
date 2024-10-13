namespace Logging

open System

open Newtonsoft.Json
open NReco.Logging.File
open Microsoft.Extensions.Logging

//**********************************

open Helpers.Server
open Server.Settings

module Logging =     

    // Function to format log entry as JSON array
    let private formatLogEntry (msg : LogMessage) =
        try
            let sb = System.Text.StringBuilder()

            use sw = new System.IO.StringWriter(sb) 
            use jsonWriter = new JsonTextWriter(sw) 

            try     
                jsonWriter.WriteValue(string DateTime.Now)
                //jsonWriter.WriteValue(string msg.LogLevel)
                jsonWriter.WriteStartArray()

                //jsonWriter.WriteValue(msg.LogName)
                //jsonWriter.WriteValue(msg.EventId.Id)

                jsonWriter.WriteValue(msg.Message)
                jsonWriter.WriteEndArray()

                Option.ofNullEmpty >> Result.fromOption <| sb             

            finally
                ()
                //sw.Dispose()
                //jsonWriter.Close()

        with
        | ex -> Error <| string ex.Message
                                   
        |> function
            | Ok value  -> value  
            | Error err -> String.Empty //There is nothing I can do with it, so the app will silently continue....
                           

    //***************************Log files******************************       
    
    let private loggerFactory = 
        LoggerFactory.Create(
            fun builder ->                                        
                         builder.AddFile(
                             logFileName, 
                             fun fileLoggerOpts
                                 ->     
                                  fileLoggerOpts.FileSizeLimitBytes <- 52428800
                                  fileLoggerOpts.MaxRollingFiles <- 10   
                                  fileLoggerOpts.FormatLogEntry <- formatLogEntry
                             ) 
                             |> ignore
        )
       
    let private logger = 
        loggerFactory.CreateLogger("SafeStackNutricniTerapie4")
                
    let internal logInfoMsg msg = 
        logger.LogInformation(msg)
        loggerFactory.Dispose()

