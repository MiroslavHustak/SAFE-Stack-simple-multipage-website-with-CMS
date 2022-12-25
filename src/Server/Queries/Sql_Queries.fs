namespace Queries

open System

module SqlQueries = 

    let queryCreateDatabase = "CREATE DATABASE nterapieLocal" //not possible for connStringSomee
    let queryDeleteAll = "DELETE FROM CENIK" //Deleting all data from the relevant table
    let queryDelete id = sprintf "%s%s" "DELETE FROM CENIK WHERE Id = " id 
    let queryExists id = sprintf "%s%s%s" "SELECT Id FROM CENIK WHERE EXISTS (SELECT Id FROM CENIK WHERE CENIK.Id = " id ")"
    let querySelect id = sprintf "%s%s" "SELECT * FROM CENIK WHERE Id = " id

    let queryUpdate id = sprintf "%s%s" "UPDATE CENIK
                                                 SET ValueState = @valState,
                                                     V001 = @val01, V002 = @val02, V003 = @val03,
                                                     V004 = @val04, V005 = @val05, V006 = @val06,
                                                     V007 = @val07, V008 = @val08, V009 = @val09
                                                 WHERE Id = " id

    let queryCreateTable = "
        CREATE TABLE CENIK (
                           Id int NOT NULL PRIMARY KEY,
                           ValueState varchar(255),
                           V001 varchar(255),
                           V002 varchar(255),
                           V003 varchar(255),
                           V004 varchar(255),
                           V005 varchar(255),
                           V006 varchar(255),
                           V007 varchar(255),
                           V008 varchar(255),
                           V009 varchar(255)   
                           )"  

    let queryInsert = "
        INSERT INTO CENIK (Id,
                          [ValueState],
                          [V001],
                          [V002],
                          [V003],
                          [V004],
                          [V005],
                          [V006],
                          [V007],
                          [V008],
                          [V009]
                          )
        VALUES (@valId, @valState, @val01, @val02, @val03, @val04, @val05, @val06, @val07, @val08, @val09)"

