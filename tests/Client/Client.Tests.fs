module Client.Tests

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

open System

//dotnet run Runtests (Client + Server + Shared) //reliable
//Client tests alone -> npm run test:live //not reliable, TODO find out why

//Client/Shared test results -> to http://localhost:8081/ in a web browser.

let private client =
    testList "Client"
        [
            testCase "testingMochaClient" <| fun _ ->

                //just testing a testCase :-), no real benefit out of this unit test
                let expected = 
                    let strContainsOnlySpace str = 
                        str |> Seq.forall (fun item -> item = (char)32)

                    let current = "testString1"
                    let old = "testString2"     

                    let input current old =                  
                        match strContainsOnlySpace current || current = String.Empty with
                        | true  -> old
                        | false -> current

                    input current old

                Expect.stringContains expected "test" "testingMochaClient"//test description     
        ]

let private allTests =
    testList "All"
        [
#if FABLE_COMPILER // This preprocessor directive makes editor happy
            Shared.Tests.shared
#endif
            client
        ]

[<EntryPoint>]
let internal main args =
#if FABLE_COMPILER
    Mocha.runTests allTests
#else
    runTestsWithArgs defaultConfig args allTests
#endif