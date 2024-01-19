namespace Helpers.Server

open System.IO
open CEBuilders
open FsToolkit.ErrorHandling

module CopyingFiles =  //trywith transferred to Server.fs
        
    let internal copyFiles source destination test =
                                                                    
        let perform x =

            pyramidOfDoom
                {
                    //Strings handled with extra care due to potential type-related concerns (you can call it "paranoia" :-)).
                    let! sourceFilepath = Path.GetFullPath(source) |> Option.ofNull, Error (sprintf "%s%s" "Kontaktuj programátora, chyba při čtení cesty k souboru " source)
                    let! destinFilepath = Path.GetFullPath(destination) |> Option.ofNull, Error (sprintf "%s%s" "Kontaktuj programátora, chyba při čtení cesty k souboru " source)
                    let! _ = (new FileInfo(sourceFilepath)).Exists |> Option.ofBool, Error (sprintf "Kontaktuj programátora, soubor %s nenalezen" source)

                    match test with
                    | true  -> return Ok ()
                    | false -> return Ok <| File.Copy(sourceFilepath, destinFilepath, true)
                }           

        perform ()

