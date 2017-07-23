namespace EntryMerge
open EntryMerge.MergePuddle
open EntryMerge.PuddleList
open EntryMerge.Download
open System
open System.IO

open Hopac 
open Logary  
open Logary.Message 
open Logary.Configuration  
open Logary.Targets 


module Main =
    let directoryNameOfMyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
    let wd = Path.Combine (directoryNameOfMyDocuments, "Merge") 

    let exportpage = 
        "http://signtyp.uconn.edu/signpuddle/export.php"
    let signpuddleindexurl = 
        "http://signtyp.uconn.edu/signpuddle/index.php"
    let pressanykey () =
        printfn "%s" "Press any key to continue"
        System.Console.ReadKey() |> ignore

    [<EntryPoint>]
    let main argv =
        let programName  =
           "EntryMerge"

        use logary =
            withLogaryManager programName (
                withTargets [ File.create (File.FileConf.create wd (Logary.Targets.File.Naming ("EntryMerge-{datetime}", "log"))) "file" ] >>
                withRules [ Rule.createForTarget "file" ])
         |> run  
                
        let logger =
            logary.getLogger (PointName [| "EntryMerge"; "EntryMerge"; "main" |])

        printfn "Working directory is %s" wd
        logger.info (eventX "Working directory is {wd}")
           
        printfn "Getting puddle list from %s" signpuddleindexurl
        logger.info (eventX "Getting puddle list from {signpuddleindexurl}")
        printfn "Base export page is %s" exportpage
        logger.info (eventX "Base export page is{exportpage}")
        let puddles = allsigntyppuddles signpuddleindexurl
                      
        match puddles with
            | Ok puddleslist ->  printfn "%i puddles found."  <| List.length puddleslist
            | Result.Error errmsg -> printfn "%s" errmsg
        let filenames = 
                match puddles with
                    | Ok puddleslist ->   List.map (fun (puddleid, puddlename ) -> (puddleid, puddlefilename wd puddleid puddlename)) (puddleslist  |> List.take 5  ) //Todo remove take so that all are processed
                    | Result.Error errmsg -> []
        if  not (Directory.Exists(wd)) then
            Directory.CreateDirectory(wd)|> ignore; () 
            printfn "%s " <| "Creating directory " + wd; 
        printfn "%s " <| "Starting downloads" 
        let downloaded = downloadall exportpage filenames
        printfn "%i puddles downloaded.\r\n"  downloaded
        let merged = 
            let outputfilename =  
                Path.Combine (wd , "output.spml")
            mergeallfiles outputfilename filenames
        printfn "%i entries merged.\r\n" merged
        pressanykey ()
        0 // return an integer exit code
         