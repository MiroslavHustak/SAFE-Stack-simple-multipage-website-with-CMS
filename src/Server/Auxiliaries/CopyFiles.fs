namespace Auxiliaries.Server

open System.IO
open CEBuilders
open FsToolkit.ErrorHandling

module CopyingFiles =  //trywith transferred to Server.fs
        
    let internal copyFiles source destination =
                                                                    
        let perform x =

            pyramidOfDoom
                {
                    let sourceFilepath = Path.GetFullPath(source) |> Option.ofNull 
                    let! sourceFilepath = sourceFilepath, Error (sprintf "%s%s" "Chyba při čtení cesty k souboru " source)

                    let destinFilepath = Path.GetFullPath(destination) |> Option.ofNull  
                    let! destinFilepath = destinFilepath, Error (sprintf "%s%s" "Chyba při čtení cesty k souboru " source)

                    let fInfodat: FileInfo = new FileInfo(sourceFilepath)
                    let! _ =  fInfodat.Exists |> Option.ofBool, Error (sprintf "Soubor %s nenalezen" source)                    

                    return Ok <| File.Copy(sourceFilepath, destinFilepath, true)
                }           

        perform ()

