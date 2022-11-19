module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn

open Shared
open SharedRecords

open System

open Router
open Helpers.CopyingFiles
open Helpers.Serialisation
open Helpers.Deserialisation

let private verifyCredentials (credentials: GetRouteUsrPsw) =
        match SharedValues.isValid credentials.Usr credentials.Psw with
        | true  ->                 
                   let route = 
                       match credentials.Usr, credentials.Psw with
                       | "Hanka", "qwe" -> "CMSRozcestnik"                       
                       | _              -> "Invalid"
                   Ok(), route
        | false -> Error "", "Invalid"                  

 //TODO pripadne pouziti validace dle potreby klienta //TODO konzultovat s klientem
let private verifyCenikValues (cenikValues: GetCenikValues) =
    match SharedCenikValues.isValid () with
    | () -> Ok ()        
   // | _  -> Error "" 

//TODO pripadne pouziti validace dle potreby klienta //TODO konzultovat s klientem
let private verifyKontaktValues (kontaktValues: GetKontaktValues) =
   match SharedCenikValues.isValid () with
   | () -> Ok ()        
  // | _  -> Error ""

//TODO pripadne pouziti validace dle potreby klienta //TODO konzultovat s klientem
let private verifyLinkAndLinkNameValues (linkValues: GetLinkAndLinkNameValues) =
   match SharedLinkAndLinkNameValues.isValid () with
   | () -> Ok ()        
  // | _  -> Error ""
  
let IGetApi =
    {
      getCredentials =
          fun getCredentials ->
              async
                  {
                     let result = 
                        match verifyCredentials getCredentials with                
                        | Ok (), route -> { Route = route; Usr = getCredentials.Usr; Psw = getCredentials.Psw }
                        // e by se dalo vyuzit pro chybovu hlasku - viz errorMsg, ale tahat to do Loginu je pracne.... proto vyuzivam hodnotu v route
                        | (Error e), route -> { Route = route; Usr = String.Empty; Psw = String.Empty }
                     return result
                  }

      getCenikValues =
          fun getCenikValues ->
              async
                  {
                    let getNewCenikValues: GetCenikValues = 
                        match verifyCenikValues getCenikValues with                
                        | Ok () -> serialize getCenikValues "jsonCenikValues.xml" 
                                   {
                                       V001 = getCenikValues.V001; V002 = getCenikValues.V002;
                                       V003 = getCenikValues.V003; V004 = getCenikValues.V004;
                                       V005 = getCenikValues.V005; V006 = getCenikValues.V006;
                                       V007 = getCenikValues.V007; V008 = getCenikValues.V008;
                                       V009 = getCenikValues.V009
                                   }
                        | _    ->
                                   {
                                       V001 = String.Empty; V002 = String.Empty;
                                       V003 = String.Empty; V004 = String.Empty;
                                       V005 = String.Empty; V006 = String.Empty;
                                       V007 = String.Empty; V008 = String.Empty;
                                       V009 = String.Empty
                                   }

                    return getNewCenikValues
                  }

      sendOldCenikValues =
          fun _ ->
              async
                  {
                     copyFiles 
                     <| "jsonCenikValues.xml"
                     <| "jsonCenikValuesBackUp.xml"

                     let sendOldCenikValues = deserialize "jsonCenikValuesBackUp.xml"
                     return sendOldCenikValues
                  } 

      sendDeserialisedCenikValues =
         fun _ ->
             async
                 {
                    //vzpomen si na problem s records s odlisnymi fields :-)
                    let sendCenikValues = deserialize "jsonCenikValues.xml"
                    return sendCenikValues
                 }

      getKontaktValues =
          fun getKontaktValues ->
              async
                  {
                    let getNewKontaktValues: GetKontaktValues = 
                        match verifyKontaktValues getKontaktValues with                
                        | Ok () -> serialize getKontaktValues "jsonKontaktValues.xml" 
                                   {
                                       V001 = getKontaktValues.V001; V002 = getKontaktValues.V002;
                                       V003 = getKontaktValues.V003; V004 = getKontaktValues.V004;
                                       V005 = getKontaktValues.V005; V006 = getKontaktValues.V006;
                                       V007 = getKontaktValues.V007 
                                   }
                        | _     ->
                                   {
                                       V001 = String.Empty; V002 = String.Empty;
                                       V003 = String.Empty; V004 = String.Empty;
                                       V005 = String.Empty; V006 = String.Empty;
                                       V007 = String.Empty
                                   }

                    return getNewKontaktValues
                  }

      sendOldKontaktValues =
         fun _ ->
             async
                 {
                    copyFiles 
                    <| "jsonKontaktValues.xml"
                    <| "jsonKontaktValuesBackUp.xml"

                    let sendOldKontaktValues = deserialize "jsonKontaktValuesBackUp.xml"
                    return sendOldKontaktValues
                 } 

      sendDeserialisedKontaktValues =
          fun _ ->
             async
                {
                    let sendKontaktValues = deserialize "jsonKontaktValues.xml"
                    return sendKontaktValues
                }

      getLinkAndLinkNameValues =
          fun getLinkAndLinkNameValues ->
              async
                  {
                    let getNewLinkAndLinkNameValues: GetLinkAndLinkNameValues = 
                        match verifyLinkAndLinkNameValues getLinkAndLinkNameValues with                
                        | Ok () -> serialize getLinkAndLinkNameValues "jsonLinkAndLinkNameValues.xml" 
                                   {
                                       V001 = getLinkAndLinkNameValues.V001; V002 = getLinkAndLinkNameValues.V002;
                                       V003 = getLinkAndLinkNameValues.V003; V004 = getLinkAndLinkNameValues.V004;
                                       V005 = getLinkAndLinkNameValues.V005; V006 = getLinkAndLinkNameValues.V006
                                       V001n = getLinkAndLinkNameValues.V001n; V002n = getLinkAndLinkNameValues.V002n;
                                       V003n = getLinkAndLinkNameValues.V003n; V004n = getLinkAndLinkNameValues.V004n;
                                       V005n = getLinkAndLinkNameValues.V005n; V006n = "Facebook"
                                   }
                        |_      ->
                                   {
                                       V001 = String.Empty; V002 = String.Empty;
                                       V003 = String.Empty; V004 = String.Empty;
                                       V005 = String.Empty; V006 = String.Empty
                                       V001n = String.Empty; V002n = String.Empty;
                                       V003n = String.Empty; V004n = String.Empty;
                                       V005n = String.Empty; V006n = "Facebook"
                                   }

                    return getNewLinkAndLinkNameValues
                  }
           
      sendOldLinkAndLinkNameValues =
          fun _ ->
              async
                  {
                    copyFiles 
                    <| "jsonLinkAndLinkNameValues.xml"
                    <| "jsonLinkAndLinkNameValuesBackUp.xml"

                    let sendOldLinkAndLinkNameValues = deserialize "jsonLinkAndLinkNameValuesBackUp.xml"
                    return sendOldLinkAndLinkNameValues
                  } 

      sendDeserialisedLinkAndLinkNameValues =
          fun _ ->
              async
                  {
                    let sendLinkAndLinkNameValues = deserialize "jsonLinkandLinkNameValues.xml"
                    return sendLinkAndLinkNameValues
                  }
    }

let webApp =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue IGetApi
    |> Remoting.buildHttpHandler

let app =
    application {
        use_router webApp
        memory_cache
        use_static "public"
        use_gzip
    }

[<EntryPoint>]
let main _ =    
    run app
    0