module Server.Tests

open System
open System.IO

open Expecto

open Errors
open Server
open Shared

open ErrorTypes.Server
open Auxiliaries.Server.Security2

let private server =
    testList "Server"
        [
            //just testing a test :-), no real benefit out of this unit test
            testCase "testingExpectoServer" <| fun _ ->

                let expected = 5
                Expect.isLessThanOrEqual expected (2+3) "2+3 = 5"//test description

            //real unit test 
            testCase "uberHashServer" <| fun _ ->

                let expected =

                    let uberHashError uberHash credential seqFn = 
                        match uberHash with
                        | Ok uberHash ->
                                        match verify (uberHash |> seqFn) credential with 
                                        | true  -> LegitimateTrue
                                        | false -> LegitimateFalse
                        | Error _     -> Exception

                        |> tryWithVerify () Exception  
                          
                    let uberHash =
                   
                        let f1 () = 
                            match File.Exists(Path.GetFullPath(@"e:\SAFE Stack\SAFE-Nutricni-terapie4\tests\Server\uberHash.txt")) with
                            | false ->
                                    Error String.Empty                                
                            | true  ->                              
                                    match File.ReadAllLines(@"e:\SAFE Stack\SAFE-Nutricni-terapie4\tests\Server\uberHash.txt") |> Option.ofObj with 
                                    | Some value -> Ok (value |> Seq.ofArray) 
                                    | None       -> Error String.Empty                                                     

                        tryWithResult f1 () (sprintf"%s")                        
                    uberHash

                Expect.isOk expected "secret credential"  
                Expect.equal (expected.OkValue |> Seq.last) "....." "secret credential" 
        ]

let private all =
    testList "All"
        [
            Shared.Tests.shared
            server
        ]

[<EntryPoint>]
let internal main _ = runTestsWithCLIArgs [] [||] all