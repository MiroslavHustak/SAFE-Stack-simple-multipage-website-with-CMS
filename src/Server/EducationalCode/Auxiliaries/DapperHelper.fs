namespace Auxiliaries.Server

(*

open SharedTypes

//for testing Dapper and Dapper.FSharp
//for learning purposes only

module DapperHelper = 

    type DapperGetCenikValues =
        {
            Id: int; ValueState: string;
            V001: string; V002: string; V003: string;
            V004: string; V005: string; V006: string;
            V007: string; V008: string; V009: string
        }
        static member Default = //fixed values
            {
                Id = 1; ValueState = "fixed";
                V001 = "300"; V002 = "300"; V003 = "2 200";
                V004 = "250"; V005 = "230"; V006 = "400";
                V007 = "600"; V008 = "450"; V009 = "450"           
            }        

    let dapperGetCenikValues (getCenikValues: GetCenikValues) : DapperGetCenikValues =
        {
            Id = getCenikValues.Id; ValueState = getCenikValues.ValueState;
            V001 = getCenikValues.V001; V002 = getCenikValues.V002; V003 = getCenikValues.V003;
            V004 = getCenikValues.V004; V005 = getCenikValues.V005; V006 = getCenikValues.V006;
            V007 = getCenikValues.V007; V008 = getCenikValues.V008; V009 = getCenikValues.V009
        }

    let getCenikValues (dapperGetCenikValues: DapperGetCenikValues) : GetCenikValues =
        {
            Id = dapperGetCenikValues.Id; ValueState = dapperGetCenikValues.ValueState;
            V001 = dapperGetCenikValues.V001; V002 = dapperGetCenikValues.V002; V003 = dapperGetCenikValues.V003;
            V004 = dapperGetCenikValues.V004; V005 = dapperGetCenikValues.V005; V006 = dapperGetCenikValues.V006;
            V007 = dapperGetCenikValues.V007; V008 = dapperGetCenikValues.V008; V009 = dapperGetCenikValues.V009;
            Msgs = Messages.Default
        }    

*)