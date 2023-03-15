module Client.Tests

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

//dotnet run Runtests (Client + Server + Shared)
//Client tests alone -> npm run test:live

//Client/Shared test results -> to http://localhost:8081/ in a web browser.

let client =
    testList "Client"
        [
            testCase "testingMochaClient" <| fun _ -> 
                //just testing a test :-)
                let expected = 5
                Expect.equal expected (2+8) "2+3 = 5"//test description     
        ]

let allTests =
    testList "All"
        [
#if FABLE_COMPILER // This preprocessor directive makes editor happy
            Shared.Tests.shared
#endif
            client
        ]

[<EntryPoint>]
let main args =
#if FABLE_COMPILER
    Mocha.runTests allTests
#else
    runTestsWithArgs defaultConfig args allTests
#endif