module Shared.Tests

#if FABLE_COMPILER
open Fable.Mocha //Mocha unit test for Shared performed together with the Client test
#else
open Expecto //Expecto unit test for Shared performed together with the Server test
#endif

open Shared

let shared =
    testList "Shared"
        [           
            testCase "testingExpectoMochaShared" <| fun _ ->

                let x = SharedDeserialisedCenikValues.transformLayer SharedCenikValues.cenikValuesDomainDefault

                let expected = x.Msgs
                let actual = Shared.SharedMessageDefaultValues.messageDefault

                Expect.equal expected (actual) "Msgs"//test description      
        ]