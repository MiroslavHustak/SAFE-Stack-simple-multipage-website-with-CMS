namespace Shared

module CEBuilders = 
      
    [<Struct>]
    type internal Builder1 = Builder1 with            
        member _.Bind(condition, nextFunc) =
            match fst condition with
            | false -> snd condition
            | true  -> nextFunc() 
        member _.Using x = x
        member _.Return x = x

    let internal pyramidOfHell = Builder1