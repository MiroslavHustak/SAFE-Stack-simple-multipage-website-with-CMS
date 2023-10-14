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
 
    let internal selectValues getConnection closeConnection insertDefaultValues idInt =
        
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
                             match cmdExists.ExecuteScalar() |> Option.ofNull with 
                             | Some _ -> Ok <| cmdSelect.ExecuteReader()
                             | None   -> Error <| insertDefaultValues 

                         match reader with
                         | Ok reader ->                                      
                                     let getValues: CenikValuesDtoGet option =                                                
                                         Seq.initInfinite (fun _ -> reader.Read())
                                         |> Seq.takeWhile ((=) true)  //compare |> Seq.skipWhile ((=) false)
                                         |> Seq.collect
                                             (fun _ ->
                                                 seq
                                                     {                                                                               
                                                     yield    
                                                         {                                                       
                                                             IdDtoGet = Casting.castAs<int> reader.["Id"] 
                                                             ValueStateDtoGet = Casting.castAs<string> reader.["ValueState"]
                                                             V001DtoGet = Casting.castAs<string> reader.["V001"]                                                                               
                                                             V002DtoGet = Casting.castAs<string> reader.["V002"]
                                                             V003DtoGet = Casting.castAs<string> reader.["V003"]
                                                             V004DtoGet = Casting.castAs<string> reader.["V004"]
                                                             V005DtoGet = Casting.castAs<string> reader.["V005"]
                                                             V006DtoGet = Casting.castAs<string> reader.["V006"]
                                                             V007DtoGet = Casting.castAs<string> reader.["V007"]
                                                             V008DtoGet = Casting.castAs<string> reader.["V008"]
                                                             V009DtoGet = Casting.castAs<string> reader.["V009"]
                                                             MsgsDtoGet = MessagesDtoGet.Default
                                                         }
                                                     } 
                                             ) |> List.ofSeq |> List.tryHead //the function only places data to the head of the collection (a function with "while" does the same)
                                   
                                     reader.Close()
                                     reader.Dispose()

                                     match getValues with
                                     | Some getValues ->                                                                                                          
                                                       [
                                                           getValues.IdDtoGet |> Option.isNone; getValues.ValueStateDtoGet |> Option.isNone;
                                                           getValues.V001DtoGet |> Option.isNone; getValues.V002DtoGet |> Option.isNone; getValues.V003DtoGet |> Option.isNone;
                                                           getValues.V004DtoGet |> Option.isNone; getValues.V005DtoGet |> Option.isNone; getValues.V006DtoGet |> Option.isNone;
                                                           getValues.V007DtoGet |> Option.isNone; getValues.V008DtoGet |> Option.isNone; getValues.V009DtoGet |> Option.isNone
                                                       ]
                                                       |> List.contains true
                                                       |> function
                                                           | true  -> Error ReadingDbError 
                                                           | false -> Ok <| cenikValuesTransferLayerGet getValues
                                     | None           ->
                                                       Error ReadingDbError                                         
                         | Error err ->
                                     Error err
                     finally
                         closeConnection connection                        
                 with
                 | _ -> Error ReadingDbError                          

             getValues

         with
         | _ -> Error ConnectionError  