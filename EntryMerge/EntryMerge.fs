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

    let logsettings (logger: Logger) =
        printfn "Working directory is %s" wd
        logger.info (eventX "Working directory is {wd}"
                         >> setField "wd" wd)
        printfn "Getting puddle list from %s" signpuddleindexurl
        logger.info (eventX "Getting puddle list from {signpuddleindexurl}"
                        >> setField "signpuddleindexurl" signpuddleindexurl) 
        printfn "Base export page is %s" exportpage
        logger.info (eventX "Base export page is {exportpage}"
                        >> setField "exportpage" exportpage)

    let createdirectory (logger: Logger) wd =
        if  not (Directory.Exists(wd)) then
            Directory.CreateDirectory(wd)|> ignore; () 
            printfn "%s " <| "Creating directory " + wd; 
            logger.info (eventX "Creating directory {wd}"
                            >> setField "wd" wd)

    [<EntryPoint>]
    let main argv =
        try
            let programName  =
               "EntryMerge"

            use logary =
                withLogaryManager programName (
                    withTargets 
                        [ File.create 
                            (File.FileConf.create wd 
                                (Logary.Targets.File.Naming 
                                    ("EntryMerge-{datetime}", "log"))) "file" ] >>  
                    withRules [ Rule.createForTarget "file" ])
             |> run  
                
            let logger =
                logary.getLogger (PointName [| "EntryMerge"; "EntryMerge"; "main" |])

            logsettings logger

            createdirectory logger wd

            let filenames = 
                filenamesfromindexpage logger wd signpuddleindexurl
                //|> List.take 5   //Todo remove take so that all are processed

            download logger exportpage filenames
        
            merge logger wd filenames
                
            pressanykey ()
            logger.info (eventX "EntryMerge program exiting.")
        with
            | :?Exception as ex ->
            Message.eventError  "Exception occured" 
                |> Message.addExn ex
                |> Logger.logSimple logger
        0 // return an integer exit code
         