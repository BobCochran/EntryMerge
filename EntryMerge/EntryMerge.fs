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
        logger.info (eventX "Working directory is {wd}"
                         >> setField "wd" wd)
        printfn "Getting puddle list from %s" signpuddleindexurl
        logger.info (eventX "Getting puddle list from {signpuddleindexurl}"
                        >> setField "signpuddleindexurl" signpuddleindexurl) 
        printfn "Base export page is %s" exportpage
        logger.info (eventX "Base export page is {exportpage}"
                        >> setField "exportpage" exportpage)
        let puddles = allsigntyppuddles signpuddleindexurl
        let filenames = 
                match puddles with
                    | Ok puddleslist ->
                        let puddlecount = List.length puddleslist
                        printfn "%i puddles found."  puddlecount
                        logger.info (eventX "{puddlecount} puddles found."
                                        >> setField "puddlecount" puddlecount)
                        List.map (fun (puddleid, puddlename ) -> (puddleid, puddlefilename wd puddleid puddlename))
                            (puddleslist |> List.take 5  ) //Todo remove take so that all are processed
                    | Result.Error errmsg -> 
                       printfn "%s" errmsg
                       logger.error(eventX "{errmsg}")
                       []


        if  not (Directory.Exists(wd)) then
            Directory.CreateDirectory(wd)|> ignore; () 
            printfn "%s " <| "Creating directory " + wd; 
            logger.info (eventX "Creating directory {wd}"
                            >> setField "wd" wd)
        printfn "%s " <| "Starting downloads" 
        logger.info (eventX "Starting downloads")
        let downloaded = downloadall exportpage filenames
        printfn "%i puddles downloaded.\r\n"  downloaded
        logger.info (eventX "{downloaded} puddles downloaded."
                        >> setField "downloaded" downloaded)
        pressanykey ()
        let merged = 
            let outputfilename =  
                Path.Combine (wd , "output.spml")
            mergeallfiles outputfilename filenames
        printfn "%i entries merged.\r\n" merged
        logger.info (eventX "{merged} entries merged."
                        >> setField "merged" merged)
        pressanykey ()
        0 // return an integer exit code
         