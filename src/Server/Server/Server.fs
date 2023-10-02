namespace Server

open System
open System.IO
//open System.Data.SqlClient

open Saturn
//open Dapper.FSharp
open Fable.Remoting.Server
open Fable.Remoting.Giraffe

open Shared
open SharedTypes

open Database.SqlRF      
//open DbAccess.Dapper
open DiscriminatedUnions.Server
open PatternBuilders.Server.PatternBuilders

open Auxiliaries.Errors.Errors
open Auxiliaries.Server.Security2
open Auxiliaries.Server.CopyingFiles
open Auxiliaries.Server.ROP_Functions
open Auxiliaries.Server.Serialisation
open Auxiliaries.Server.Deserialisation
open Auxiliaries.Connections.Connection

open TransLayerGet.Server.TransLayerGet
open TransLayerSend.Server.TransLayerSend

module Server =

    let private strContainsOnlySpace str = str |> Seq.forall (fun item -> item = (char)32)  //A string is a sequence of characters => use Seq.forall to test directly //(char)32 = space

    let private isValidLogin inputUsrString inputPswString = not (strContainsOnlySpace inputUsrString || strContainsOnlySpace inputPswString)

    let private isValidCenik param = ()   //TODO validation upon request from the user 
    let private isValidKontakt param = () //TODO validation upon request from the user 
    let private isValidLink param = ()    //TODO validation upon request from the user

    //************************************************************************
    //TODO create a separate solution and include a try with block
    let private pswHash() = //to be used only once before bundling             
       
        let usr = uberHash "" //delete username before bundling
        let psw = uberHash "" //delete password before bundling
        let mySeq = seq { usr; psw }
        
        use sw =
            new StreamWriter(Path.GetFullPath("uberHash.txt")) 
            |> Option.ofObj
            |> function
                | Some value -> value
                | None       -> //Check the "uberHash.txt" file manually 
                                new StreamWriter(Path.GetFullPath("")) 
        mySeq |> Seq.iter (fun item -> do sw.WriteLine(item)) 
    //************************************************************************

    let private verifyLogin (login: LoginInfo) =   // LoginInfo -> Async<LoginResult>>

        MyPatternBuilder    
            {
                let rc1 = { SharedApi.LoginProblems.line1 = "Závažná chyba na serveru !!!"; SharedApi.LoginProblems.line2 = "Chybí soubor pro ověření uživatelského jména a hesla" }
                let rc2 = { SharedApi.LoginProblems.line1 = "Závažná chyba na serveru !!!"; SharedApi.LoginProblems.line2 = "Problém s ověřením uživatelského jména a hesla" }
                let rc3 = { SharedApi.LoginProblems.line1 = "Buď uživatelské jméno anebo heslo je neplatné."; SharedApi.LoginProblems.line2 = "Prosím zadej údaje znovu." }  

                let usr = login.Username |> function SharedApi.Username value -> value //unwrapping SCDU
                let psw = login.Password |> function SharedApi.Password value -> value

                let uberHash x =
                    //File.Exists will not throw an exception, but GetFullPath will do so
                    match File.Exists(Path.GetFullPath("uberHash.txt")) with
                    | false -> Seq.empty //No need of any action, Seq.empty will do its job here                                
                    | true  ->                              
                               match File.ReadAllLines("uberHash.txt") |> Option.ofObj with //StreamReader refused to work here, that is why File.ReadAllLines was used 
                               | Some value -> value |> Seq.ofArray 
                               | None       -> Seq.empty //No need of any action, Seq.empty will do its job here
             
                let uberHash = (uberHash, (fun x -> ()), String.Empty) |||> tryWith |> deconstructor0 

                let! _ = (<>) uberHash Seq.empty, SharedApi.UsernameOrPasswordIncorrect rc1
                
                let! _ = isValidLogin usr psw, SharedApi.UsernameOrPasswordIncorrect rc3

                let verify1 x = verify (uberHash |> Seq.head) usr
                let verify1 = (verify1, (fun x -> ()), String.Empty) |||> tryWith |> deconstructor4

                let! _ = (<>) verify1 Exception, SharedApi.UsernameOrPasswordIncorrect rc2 

                let verify2 x = verify (uberHash |> Seq.last) psw
                let verify2 = (verify2, (fun x -> ()), String.Empty) |||> tryWith |> deconstructor4

                let! _ = (<>) verify2 Exception, SharedApi.UsernameOrPasswordIncorrect rc2
                let! _ = (&&) (verify1 = LegitimateTrue) (verify2 = LegitimateTrue), SharedApi.UsernameOrPasswordIncorrect rc3 
                                                                        
                return SharedApi.LoggedIn { Username = login.Username } //{ Username = login.Username; AccessToken = SharedApi.AccessToken accessToken }
            }
    
      //TODO validation upon request from the user 
    let private verifyCenikValues (cenikValues: CenikValuesDomain) =
        match isValidCenik () with
        | ()  -> Success ()        
        //| _ -> Failure ()

     //TODO validation upon request from the user 
    let private verifyKontaktValues (kontaktValues: KontaktValuesDomain) =
       match isValidKontakt () with
       | ()  -> Success ()        
       //| _ -> Failure ()

    //TODO validation upon request from the user 
    let private verifyLinkAndLinkNameValues (linkValues: LinkAndLinkNameValuesDomain) =
       match isValidLink () with
       | ()  -> Success ()        
       //| _ -> Failure ()

    let IGetApi errMsg =
        {            
            login =
                fun login -> async { return (verifyLogin login) }          
         (*
          getSecurityTokenFile = //TODO try with
              fun getSecurityTokenFile -> async { return File.Exists(Path.GetFullPath("securityToken.txt")) }             
     
          getSecurityToken =
              fun getSecurityToken ->  //TODO try with
                  async
                      {       
                          match File.Exists(Path.GetFullPath("securityToken.txt")) with
                          | false -> //TODO error + some action
                                     return Seq.empty  
                          | true  -> //StreamReader refused to work here, thar is why File.ReadAllLines was used                              
                                     match File.ReadAllLines("securityToken.txt") |> Option.ofObj with
                                     | Some value -> return (value |> Seq.ofArray) 
                                     | None       -> return Seq.empty  //TODO error + some action                        
                      }        
      
          deleteSecurityTokenFile =
              fun deleteSecurityTokenFile ->  //TODO try with
                  async
                      {
                          File.Delete(Path.GetFullPath("securityToken.txt"))
                          return ()
                      }   
          *)
            sendCenikValues =  
                fun sendCenikValues ->
                    async
                        {                   
                            let sendNewCenikValues: CenikValuesDomain =                        
                                match verifyCenikValues sendCenikValues with                
                                | Success () ->
                                              let dbNewCenikValues = { sendCenikValues with Id = 2; ValueState = "new" }

                                              //************* plain SQL or Dapper.FSharp ********************

                                              let cond = dbNewCenikValues.Msgs.Msg1 = "First run"
                                              let cenikValuesSend = cenikValuesTransferLayerSend dbNewCenikValues
                                              let du = errorMsgBox (insertOrUpdate getConnection closeConnection cenikValuesSend) cond
                                              let exnSql =
                                                  match du with
                                                  | FirstRunError       -> "Byly dosazeny defaultní nebo předchozí hodnoty, neb došlo k chybě při načítání hodnot z databáze."
                                                  | InsertOrUpdateError -> "Zadané hodnoty nebyly nebo nebudou uloženy, neb došlo k chybě při načítání hodnot z databáze." 
                                                  | NoInsertError       -> String.Empty
                                         
                                              { dbNewCenikValues with Msgs = { MessagesDomain.Default with Msg1 = exnSql } }
                                                                           
                                | Failure () ->
                                              CenikValuesDomain.Default

                          return sendNewCenikValues
                      }

            getOldCenikValues = 
                fun _ -> 
                    async
                        {                           
                            //************* plain SQL or Dapper.FSharp ********************                     
                            let IdNew = 2
                            let IdOld = 3

                            //********************************************************
                            let (dbGetNewCenikValues, exnSql2) =                              
                                match selectValues getConnection closeConnection IdNew with   //do rozhodnuti o podobe chybovych hlasek neprovadet refactoring!!!                                 
                                | value, InsertOrUpdateError1 -> value, "Byly dosazeny defaultní nebo předchozí hodnoty, neb došlo k chybě při ověřování existující databáze."
                                | value, InsertOrUpdateError2 -> value, "Došlo k chybě při načítání hodnot z databáze a dosazování defaultních hodnot. Zobrazované hodnoty mohou být chybné."
                                | value, ReadingDbError       -> value, "Chyba při načítání hodnot z databáze. Dosazeny defaultní hodnoty místo chybných hodnot."
                                | value, ConnectionError      -> value, "Chyba připojení k databázi. Dosazeny defaultní hodnoty místo chybných hodnot."
                                | value, NoSelectError        -> value, String.Empty

                            //********************************************************
                            let dbCenikValues = { dbGetNewCenikValues with Id = IdOld; ValueState = "old" }
                            let cond = dbCenikValues.Msgs.Msg1 = "First run"
                            let cenikValuesSend = cenikValuesTransferLayerSend dbCenikValues
                            let du = errorMsgBox (insertOrUpdate getConnection closeConnection cenikValuesSend) cond
                            let exnSql =
                                match du with
                                | FirstRunError       -> "Byly dosazeny defaultní nebo předchozí hodnoty, neb došlo k chybě při načítání hodnot z databáze."
                                | InsertOrUpdateError -> "Zadané hodnoty nebyly nebo nebudou uloženy, neb došlo k chybě při načítání hodnot z databáze." 
                                | NoInsertError       -> String.Empty

                            //********************************************************
                            let (dbSendOldCenikValues, exnSql3) =                              
                                match selectValues getConnection closeConnection IdOld with   //do rozhodnuti o podobe chybovych hlasek neprovadet refactoring!!! 
                                | value, InsertOrUpdateError1 -> value, "Byly dosazeny defaultní nebo předchozí hodnoty, neb došlo k chybě při ověřování existující databáze."
                                | value, InsertOrUpdateError2 -> value, "Došlo k chybě při načítání hodnot z databáze a dosazování defaultních hodnot. Zobrazované hodnoty mohou být chybné."
                                | value, ReadingDbError       -> value, "Chyba při načítání hodnot z databáze. Dosazeny defaultní hodnoty místo chybných hodnot."
                                | value, ConnectionError      -> value, "Chyba připojení k databázi. Dosazeny defaultní hodnoty místo chybných hodnot."
                                | value, NoSelectError        -> value, String.Empty

                            return { dbSendOldCenikValues with Msgs = { MessagesDomain.Default with Msg1 = exnSql; Msg2 = exnSql2; Msg3 = exnSql3 } }
                        }

            getDeserialisedCenikValues = 
               fun _ ->
                   async
                       {                          
                           //************* plain SQL or Dapper.FSharp ********************
                           let IdNew = 2
                    
                           let (dbSendCenikValues, exnSql1) =                              
                               match selectValues getConnection closeConnection IdNew with   //do rozhodnuti o podobe chybovych hlasek neprovadet refactoring!!! 
                               | value, InsertOrUpdateError1 -> value, "Byly dosazeny defaultní nebo předchozí hodnoty, neb došlo k chybě při ověřování existující databáze."
                               | value, InsertOrUpdateError2 -> value, "Došlo k chybě při načítání hodnot z databáze a dosazování defaultních hodnot. Zobrazované hodnoty mohou být chybné."
                               | value, ReadingDbError       -> value, "Chyba při načítání hodnot z databáze. Dosazeny defaultní hodnoty místo chybných hodnot."
                               | value, ConnectionError      -> value, "Chyba připojení k databázi. Dosazeny defaultní hodnoty místo chybných hodnot."
                               | value, NoSelectError        -> value, String.Empty      

                           return { dbSendCenikValues with Msgs = { MessagesDomain.Default with Msg1 = exnSql1; Msg2 = errMsg } } 
                       }

            //************* from here downwards Json/XML ********************   
            sendKontaktValues =
                fun sendKontaktValues ->
                    async
                        {
                            let sendNewKontaktValues: KontaktValuesDomain = 
                                match verifyKontaktValues sendKontaktValues with
                                | Success () ->                                                 
                                              let serializeNow x =
                                                 //failwith "Simulated exception10"
                                                 let sendKontaktValuesDtoSend = kontaktValuesTransferLayerSend sendKontaktValues
                                                 serialize sendKontaktValuesDtoSend "jsonKontaktValues.xml"                            
                                              let exnJson =
                                                  (serializeNow, (fun x -> ()), "Zadané hodnoty nebyly uloženy, neb došlo k této chybě: Error10")
                                                  |||> tryWith |> deconstructor1
                                              { sendKontaktValues with Msgs = { MessagesDomain.Default with Msg1 = exnJson } }                                   
                                | Failure () ->
                                              KontaktValuesDomain.Default

                            return sendNewKontaktValues
                        }

            getOldKontaktValues =
                fun _ ->
                    async
                        {
                            let getOldKontaktValuesNow x =
                                copyFiles 
                                <| "jsonKontaktValues.xml"
                                <| "jsonKontaktValuesBackUp.xml"
                                //failwith "Simulated exception12" 
                                kontaktValuesTransferLayerGet (deserialize "jsonKontaktValuesBackUp.xml")                             
                            let (getOldKontaktValues, exnJson) =
                                (getOldKontaktValuesNow, (fun x -> ()), "Pro zobrazování navrhovaných a předchozích hodnot kontaktů byly dosazeny defaultní hodnoty, neb došlo k této chybě: Error12")
                                |||> tryWith |> deconstructor2 KontaktValuesDomain.Default

                            return { getOldKontaktValues with Msgs = { MessagesDomain.Default with Msg1 = exnJson } }                 
                        } 

            getDeserialisedKontaktValues =
                fun _ ->
                    async
                        {
                            let getKontaktValuesNow x =
                                //failwith "Simulated exception13" 
                                kontaktValuesTransferLayerGet (deserialize "jsonKontaktValues.xml")                                 
                            let (getKontaktValues, exnJson1) =
                                (getKontaktValuesNow, (fun x -> ()), "Byly dosazeny defaultní hodnoty kontaktů, neb došlo k této chybě: Error13")
                                |||> tryWith |> deconstructor2 KontaktValuesDomain.Default

                            return { getKontaktValues with Msgs = { MessagesDomain.Default with Msg1 = exnJson1 } }                   
                        }

            sendLinkAndLinkNameValues =
               fun sendLinkAndLinkNameValues ->
                   async
                      {
                         let sendNewLinkAndLinkNameValues: LinkAndLinkNameValuesDomain = 
                             match verifyLinkAndLinkNameValues sendLinkAndLinkNameValues with
                             | Success () ->                                                                                   
                                           let serializeNow x =
                                               //failwith "Simulated exception14"   
                                               let sendLinkAndLinkNameValuesDtoSend = linkAndLinkNameValuesTransferLayerSend sendLinkAndLinkNameValues
                                               serialize sendLinkAndLinkNameValuesDtoSend "jsonLinkAndLinkNameValues.xml"
                                           let exnJson =
                                                (serializeNow, (fun x -> ()), "Zadané hodnoty nebyly uloženy, neb došlo k této chybě: Error14")
                                                |||> tryWith |> deconstructor1
                                           { sendLinkAndLinkNameValues with Msgs = { MessagesDomain.Default with Msg1 = exnJson } }     
                             | Failure () ->
                                           LinkAndLinkNameValuesDomain.Default                        

                         return sendNewLinkAndLinkNameValues
                      }
           
            getOldLinkAndLinkNameValues =
                fun _ ->
                   async
                       {
                           //let copyFilesNow x =
                           let getOldLinkAndLinkNameValuesNow x = 
                               copyFiles 
                               <| "jsonLinkAndLinkNameValues.xml"
                               <| "jsonLinkAndLinkNameValuesBackUp.xml"
                               //failwith "Simulated exception15"                           
                               linkAndLinkNameValuesTransferLayerGet (deserialize "jsonLinkAndLinkNameValuesBackUp.xml")
                               //failwith "Simulated exception16"
                           let (getOldLinkAndLinkNameValues, exnJson) =
                               (getOldLinkAndLinkNameValuesNow, (fun x -> ()), "Pro zobrazování navrhovaných a předchozích hodnot odkazů byly dosazeny defaultní hodnoty, neb došlo k této chybě: Error16")
                               |||> tryWith |> deconstructor2 LinkAndLinkNameValuesDomain.Default

                           return { getOldLinkAndLinkNameValues with Msgs = { MessagesDomain.Default with Msg1 = exnJson } }  
                       } 

            getDeserialisedLinkAndLinkNameValues =
               fun _ ->
                   async
                       {                              
                           let getLinkAndLinkNameValuesNow x =
                               //failwith "Simulated exception17" 
                               linkAndLinkNameValuesTransferLayerGet (deserialize "jsonLinkandLinkNameValues.xml")                                 
                           let (getLinkAndLinkNameValues, exnJson) =
                               (getLinkAndLinkNameValuesNow, (fun x -> ()), "Byly dosazeny defaultní hodnoty odkazů, neb došlo k této chybě: Error17")
                               |||> tryWith |> deconstructor2 LinkAndLinkNameValuesDomain.Default

                           return { getLinkAndLinkNameValues with Msgs = { MessagesDomain.Default with Msg1 = exnJson } }  
                       }
        }

    let webApp exnSql =
        Remoting.createApi ()
        |> Remoting.withRouteBuilder Route.builder
        |> Remoting.fromValue (IGetApi exnSql)
        |> Remoting.buildHttpHandler

    let app =
        //let exnSql = insertOrUpdate { GetCenikValues.Default with Msgs = { Messages.Default with Msg1 = "First run" } }
        let dbCenikValues = { CenikValuesDomain.Default with Msgs = { MessagesDomain.Default with Msg1 = "First run" } }
        let cenikValuesSend = cenikValuesTransferLayerSend dbCenikValues
        let du = errorMsgBox (insertOrUpdate getConnection closeConnection cenikValuesSend) true //true == first run
        let exnSql =
            match du with
            | FirstRunError       -> "Byly dosazeny defaultní nebo předchozí hodnoty, neb došlo k chybě při načítání hodnot z databáze."
            | InsertOrUpdateError -> "Zadané hodnoty nebyly nebo nebudou uloženy, neb došlo k chybě při načítání hodnot z databáze." 
            | NoInsertError       -> String.Empty

        application
            {
                use_router (webApp exnSql)
                memory_cache
                use_static "public"
                use_gzip
            }

    [<EntryPoint>]
    let main _ =   
        Dapper.FSharp.OptionTypes.register()
        //pswHash() //to be used only once && before bundling 
        run app    
        0