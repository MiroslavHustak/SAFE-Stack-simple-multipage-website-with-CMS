namespace Database

open System
open System.Data.SqlClient
open FsToolkit.ErrorHandling

open InsertOrUpdate

open SharedTypes
open ErrorTypes.Server
open Queries.SqlQueries

open Auxiliaries.Server
open Auxiliaries.Server.CEBuilders

open DtoGet.Server.DtoGet
open DtoDefault.Server.DtoDefault
open TransLayerGet.Server.TransLayerGet

//SQL type providers did not work in this app (they blocked the Somee database)
module Select = 

    //******************************************************************************************************************
    //for learning and testing purposes
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
                         
                         use cmdExists = new SqlCommand(queryExists idString, connection) //non-nullable, ex caught with tryWith                                     
                         use cmdSelect = new SqlCommand(querySelect idString, connection) //non-nullable, ex caught with tryWith                                                   
                        
                         //**************** Read values from DB *****************

                         let reader =  
                             pyramidOfDoom
                                 {
                                     //Objects handled with extra care due to potential type-related concerns (you can call it paranoia :-)). 
                                     let! _ = cmdExists.ExecuteScalar() |> Option.ofNull, Error insertDefaultValues
                                     let reader = cmdSelect.ExecuteReader()  //non-nullable, ex caught with tryWith (monadic operation discontinued)   

                                     return Ok reader
                                 }
                                      
                         match reader with
                         | Ok reader ->
                                      let getValues: CenikValuesDtoGet option = //Seq not strictly necessary here, but retained for potential future requirements or updates.                                             
                                          Seq.initInfinite (fun _ -> reader.Read() && reader.HasRows = true)
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
                                                                  MsgsDtoGet = MessagesDtoGetDefault
                                                              }
                                                          } 
                                              ) |> List.ofSeq |> List.tryHead 
                                                 
                                      reader.Close()
                                      reader.Dispose()

                                      match getValues with
                                      | Some getValues ->                                                                                                          
                                                        [
                                                            getValues.IdDtoGet |> Option.isSome; getValues.ValueStateDtoGet |> Option.isSome;
                                                            getValues.V001DtoGet |> Option.isSome; getValues.V002DtoGet |> Option.isSome; getValues.V003DtoGet |> Option.isSome;
                                                            getValues.V004DtoGet |> Option.isSome; getValues.V005DtoGet |> Option.isSome; getValues.V006DtoGet |> Option.isSome;
                                                            getValues.V007DtoGet |> Option.isSome; getValues.V008DtoGet |> Option.isSome; getValues.V009DtoGet |> Option.isSome
                                                        ]
                                                        |> List.forall (fun item -> (=) item true)
                                                        |> function
                                                            | true  -> Ok <| cenikValuesTransferLayerGet getValues
                                                            | false -> Error ReadingDbError 
                                      | None           ->
                                                        Error ReadingDbError

                         | Error err ->
                                      Error err     
                                                        
                     finally
                         closeConnection connection  //just in case :-)                      
                 with
                 | _ -> Error ReadingDbError                          

             getValues

         with
         | _ -> Error ConnectionError
