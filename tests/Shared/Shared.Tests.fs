module Shared.Tests

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

open Shared

//Expecto unit test for Shared performed together with the Server test
let shared =
    testList "Shared"
        [
            //just testing a test :-)
            testCase "testingExpecto" <| fun _ ->

                let expected = 5
                Expect.equal expected (2+3) "2+3 = 5"//test description      
        ]