namespace Database

open System
open System.Data.SqlClient
open FsToolkit.ErrorHandling

open Shared
open ErrorTypes.Server
open Queries.SqlQueries

open Helpers.Server.CEBuilders

open DtoDefault.Server.DtoDefault
open DtoFromStorage.Server.DtoFromStorage
open TransLayerFromStorage.Server.TransLayerFromStorage

//SQL type providers did not work in this app (they blocked the Somee database)
module Select = 

    //******************************************************************************************************************
    let internal selectValues (createConnection: unit -> SqlConnection) insertDefaultValues idInt =
        
         try
             //failwith "Simulated exception SqlSelectValues"            
             let connection = createConnection()
                    
             let getValues: Result<CenikValuesShared, SelectErrorOptions> =

                 try
                     try
                         //failwith "Simulated exception SqlSelectValues" 

                         //**************** SqlCommands *****************
                         
                         use cmdExists = new SqlCommand(queryExists, connection) //non-nullable, ex caught with tryWith                                     
                         use cmdSelect = new SqlCommand(querySelect, connection) //non-nullable, ex caught with tryWith                                                   
                        
                         //**************** Read values from DB *****************

                         let reader =  
                             pyramidOfDoom
                                 {
                                     cmdExists.Parameters.AddWithValue("@Id", idInt) |> ignore
                                     cmdSelect.Parameters.AddWithValue("@Id", idInt) |> ignore

                                     //Objects handled with extra care due to potential type-related concerns (you can call it "paranoia" :-)). 
                                     let! _ = cmdExists.ExecuteScalar() |> Option.ofNull, Error insertDefaultValues
                                     let reader = cmdSelect.ExecuteReader()  //non-nullable, ex caught with tryWith (monadic operation discontinued)   

                                     return Ok reader
                                 }
                                      
                         match reader with
                         | Ok reader ->
                                      //Seq not strictly necessary here, but retained for potential future requirements or updates.                                             
                                      Seq.initInfinite (fun _ -> reader.Read() && reader.HasRows = true)
                                      |> Seq.takeWhile ((=) true)  //compare |> Seq.skipWhile ((=) false)
                                      |> Seq.collect
                                          (fun _ ->
                                                  seq 
                                                      {
                                                          
                                                          let indexId = reader.GetOrdinal("Id")                                                                  
                                                          let indexValueState = reader.GetOrdinal("ValueState")
                                                          let v001Index = reader.GetOrdinal("V001")
                                                          let v001Index = reader.GetOrdinal("V001")
                                                          let v002Index = reader.GetOrdinal("V002")
                                                          let v003Index = reader.GetOrdinal("V003")
                                                          let v004Index = reader.GetOrdinal("V004")
                                                          let v005Index = reader.GetOrdinal("V005")
                                                          let v006Index = reader.GetOrdinal("V006")
                                                          let v007Index = reader.GetOrdinal("V007")
                                                          let v008Index = reader.GetOrdinal("V008")
                                                          let v009Index = reader.GetOrdinal("V009")

                                                          yield    
                                                              {
                                                                  IdDtoGet = reader.GetInt32(indexId) |> Option.ofNull
                                                                  ValueStateDtoGet = reader.GetString(indexValueState) |> Option.ofNull
                                                                  V001DtoGet = reader.GetString(v001Index) |> Option.ofNull
                                                                  V002DtoGet = reader.GetString(v002Index) |> Option.ofNull
                                                                  V003DtoGet = reader.GetString(v003Index) |> Option.ofNull
                                                                  V004DtoGet = reader.GetString(v004Index) |> Option.ofNull 
                                                                  V005DtoGet = reader.GetString(v005Index) |> Option.ofNull
                                                                  V006DtoGet = reader.GetString(v006Index) |> Option.ofNull
                                                                  V007DtoGet = reader.GetString(v007Index) |> Option.ofNull
                                                                  V008DtoGet = reader.GetString(v008Index) |> Option.ofNull
                                                                  V009DtoGet = reader.GetString(v009Index) |> Option.ofNull

                                                                  MsgsDtoGet = MessagesDtoFromStorageDefault |> Option.ofNull
                                                              }
                                                      } 
                                        )
                                        |> List.ofSeq
                                        |> List.tryHead
                                        |> function
                                            | Some value -> cenikValuesTransformLayerFromStorage value
                                            | None       -> Error ReadingDbError
                         | Error err ->
                                      Error err     
                                                        
                     finally
                         () // closeConnection connection                      
                 with
                 | _ -> Error ReadingDbError                          

             getValues

         with
         | _ -> Error ConnectionError
                              
