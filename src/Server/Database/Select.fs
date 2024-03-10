namespace Database

open System
open System.Data.SqlClient
open FsToolkit.ErrorHandling

open InsertOrUpdate

open SharedTypes
open ErrorTypes.Server
open Queries.SqlQueries

open Helpers.Server
open Helpers.Server.CEBuilders

open DtoGet.Server.DtoGet
open DtoDefault.Server.DtoDefault
open TransLayerGet.Server.TransLayerGet

open Connections


//SQL type providers did not work in this app (they blocked the Somee database)
module Select = 

    //******************************************************************************************************************
    let internal selectValues getConnection closeConnection insertDefaultValues idInt =
        
         try
             //failwith "Simulated exception SqlSelectValues"            
             let connection = getConnection()
                    
             let getValues: Result<CenikValuesDomain, SelectErrorOptions> =

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
                                                          yield    
                                                              {                                                       
                                                                  IdDtoGet = reader.["Id"] 
                                                                  ValueStateDtoGet = reader.["ValueState"]
                                                                  V001DtoGet = reader.["V001"]                                                                               
                                                                  V002DtoGet = reader.["V002"]
                                                                  V003DtoGet = reader.["V003"]
                                                                  V004DtoGet = reader.["V004"]
                                                                  V005DtoGet = reader.["V005"]
                                                                  V006DtoGet = reader.["V006"]
                                                                  V007DtoGet = reader.["V007"]
                                                                  V008DtoGet = reader.["V008"]
                                                                  V009DtoGet = reader.["V009"]
                                                                  MsgsDtoGet = MessagesDtoGetDefault
                                                              }
                                                      } 
                                        )
                                        |> List.ofSeq
                                        |> List.tryHead
                                        |> function
                                            | Some value -> cenikValuesTransferLayerGet value
                                            | None       -> Error ReadingDbError
                         | Error err ->
                                      Error err     
                                                        
                     finally
                         closeConnection connection  //just in case :-)                      
                 with
                 | _ -> Error ReadingDbError                          

             getValues

         with
         | _ -> Error ConnectionError
                              
