namespace Shared

open System

open CEBuilders

module Option =    

    let internal ofNullEmpty (value: 'nullableValue) = //NullOrEmpty

        pyramidOfHell
            {
                let!_ = not <| System.Object.ReferenceEquals(value, null), None 
                let value = string value 
                let! _ = not <| String.IsNullOrEmpty(value), None 

                return Some value
            }

    let internal ofNullEmptySpace (value: 'nullableValue) = //NullOrEmpty, NullOrWhiteSpace
    
        pyramidOfHell
            {
                let!_ = not <| System.Object.ReferenceEquals(value, null), None 
                let value = string value 
                let! _ = not <| (String.IsNullOrEmpty(value) || String.IsNullOrWhiteSpace(value)), None
    
                return Some value
            }

module Miscellaneous = 

    open System

    let internal strContainsOnlySpace str =
        str |> Seq.forall (fun item -> item = (char)32)  //A string is a sequence of characters => use Seq.forall to test directly //(char)32 = space*    
    
        
       
          



