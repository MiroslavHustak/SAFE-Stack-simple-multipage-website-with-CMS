namespace Database

open System
open System.Data.SqlClient
open FsToolkit.ErrorHandling

open InsertOrUpdate

open SharedTypes
open Queries.SqlQueries
open ErrorTypes.Server

open Auxiliaries.Server
open Auxiliaries.Errors.Errors

open DtoGet.Server.DtoGet
open TransLayerGet.Server.TransLayerGet

//SQL type providers did not work in this app, they block the database
module Select =    
 
    let internal selectValues getConnection closeConnection insertOrUpdateError idInt =
        
         try
             //failwith "Simulated exception SqlSelectValues"            
             let connection = getConnection()
                    
             let getValues: Result<CenikValuesDomain, SelectErrorOptions> =

                 try
                     try
                         let idString = string idInt
                         //failwith "Simulated exception SqlSelectValues" 

                         //**************** SqlCommands *****************
                         use cmdExists = new SqlCommand(queryExists idString, connection)
                         use cmdSelect = new SqlCommand(querySelect idString, connection)

                         //**************** Read values from DB *****************
                         let reader =            
                             match cmdExists.ExecuteScalar() |> Option.ofObj with 
                             | Some _ -> Ok <| cmdSelect.ExecuteReader()
                             | None   -> Error <| insertOrUpdateError 

                         match reader with
                         | Ok reader ->   
                                     let extractValue fn defaultValue =
                                         match fn with     
                                         | Some value -> value, false
                                         | None       -> defaultValue, true                                      
                                   
                                     let getValues =                                                
                                         Seq.initInfinite (fun _ -> reader.Read())
                                         |> Seq.takeWhile ((=) true)  //compare |> Seq.skipWhile ((=) false)
                                         |> Seq.collect (fun _ ->  
                                                                 seq
                                                                     {                                                                               
                                                                     yield    
                                                                         {                                                       
                                                                             IdDtoGet = extractValue (Casting.downCast reader.["Id"]) CenikValuesDomain.Default.Id
                                                                             ValueStateDtoGet = extractValue (Casting.downCast reader.["ValueState"]) CenikValuesDomain.Default.ValueState
                                                                             V001DtoGet = extractValue (Casting.downCast reader.["V001"]) CenikValuesDomain.Default.V001                                                                                   
                                                                             V002DtoGet = extractValue (Casting.downCast reader.["V002"]) CenikValuesDomain.Default.V002
                                                                             V003DtoGet = extractValue (Casting.downCast reader.["V003"]) CenikValuesDomain.Default.V003
                                                                             V004DtoGet = extractValue (Casting.downCast reader.["V004"]) CenikValuesDomain.Default.V004
                                                                             V005DtoGet = extractValue (Casting.downCast reader.["V005"]) CenikValuesDomain.Default.V005
                                                                             V006DtoGet = extractValue (Casting.downCast reader.["V006"]) CenikValuesDomain.Default.V006
                                                                             V007DtoGet = extractValue (Casting.downCast reader.["V007"]) CenikValuesDomain.Default.V007
                                                                             V008DtoGet = extractValue (Casting.downCast reader.["V008"]) CenikValuesDomain.Default.V008
                                                                             V009DtoGet = extractValue (Casting.downCast reader.["V009"]) CenikValuesDomain.Default.V009
                                                                             MsgsDtoGet = MessagesDtoGet.Default
                                                                         }
                                                                     } 
                                                         ) |> Seq.head //the function only places data to the head of the collection (a function with "while" does the same)
                                     reader.Close()
                                     reader.Dispose()

                                     getValues
                                     |> function                                        
                                         |
                                             {
                                                 IdDtoGet = (_, flag1); ValueStateDtoGet = (_, flag2);
                                                 V001DtoGet = (_, flag3); V002DtoGet = (_, flag4); V003DtoGet = (_, flag5);
                                                 V004DtoGet = (_, flag6); V005DtoGet = (_, flag7); V006DtoGet = (_, flag8);
                                                 V007DtoGet = (_, flag9); V008DtoGet = (_, flag10); V009DtoGet = (_, flag11)
                                             }
                                             ->
                                                 flag1 || flag2 || flag3 || flag4 || flag5 ||
                                                 flag6 || flag7 || flag8 || flag9 ||
                                                 flag10 || flag11
                                                                                            
                                     |> function
                                         | true  -> Error ReadingDbError 
                                         | false -> Ok <| cenikValuesTransferLayerGet getValues                                                
                                    
                         | Error du -> Error du
                     finally
                         closeConnection connection                        
                 with
                 | _ -> Error ReadingDbError 

             getValues

         with
         | _ -> Error ConnectionError  
    
 
         


