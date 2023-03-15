module Server.Tests

open Expecto

open Shared
open Server

open Auxiliaries.Server.Security2
open Auxiliaries.Server.ROP_Functions

open System
open System.IO

let server =
    testList "Server"
        [
            //just testing a test :-), no real benefit out of this unit test
            testCase "testingExpectoServer" <| fun _ ->

                let expected = 5
                Expect.isLessThanOrEqual expected (2+3) "2+3 = 5"//test description

            //real unit test 
            testCase "uberHashServer" <| fun _ ->

                let expected =

                    let uberHash x =
                    
                        match File.Exists(Path.GetFullPath(@"e:\SAFE Stack\SAFE-Nutricni-terapie4\tests\Server\uberHash.txt")) with
                        | false -> Seq.empty                               
                        | true  ->                              
                                   match File.ReadAllLines(@"e:\SAFE Stack\SAFE-Nutricni-terapie4\tests\Server\uberHash.txt") |> Option.ofObj with 
                                   | Some value -> value |> Seq.ofArray 
                                   | None       -> Seq.empty 
             
                    let result = (uberHash, (fun x -> ()), String.Empty) |||> tryWith |> deconstructor0

                    (verify (result |> Seq.head) "......"), (verify (result |> Seq.last) "......")

                Expect.equal (fst expected) true "secret usr" 
                Expect.equal (snd expected) true "secret psw" 
        ]

let all =
    testList "All"
        [
            Shared.Tests.shared
            server
        ]

[<EntryPoint>]
let main _ = runTestsWithCLIArgs [] [||] all