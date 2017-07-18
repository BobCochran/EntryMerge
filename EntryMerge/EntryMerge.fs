namespace EntryMerge
open EntryMerge.PuddleList
open FSharp.Data
open System
open System.IO
module Main =
        
    let directoryNameOfMyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
    let wd = Path.Combine (directoryNameOfMyDocuments, "Merge") 
        //"C:\Users\Jonathan\Documents\Merge"
    let exportpage = 
        "http://signtyp.uconn.edu/signpuddle/export.php"
    let signpuddleindexurl = 
        "http://signtyp.uconn.edu/signpuddle/index.php"

    [<EntryPoint>]
    let main argv =
        let programName  =
           "EntryMerge"
        printfn "Working directory is %s" wd
        printfn "Getting puddle list from %s" signpuddleindexurl
        printfn "Base export page is %s" exportpage
        let puddles = allsigntyppuddles signpuddleindexurl
        match puddles with
            | Ok puddleslist ->  printfn "%i puddles founds."  <| List.length puddleslist
            | Error errmsg -> printfn "%s" errmsg
        
        0 // return an integer exit code
         