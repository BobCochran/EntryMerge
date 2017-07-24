namespace EntryMerge
open System
open System.Net
open Logary
open Logary.Message 
module Download =
    let logger = Logging.getCurrentLogger ()

    let fullurl puddleid  exportpage =
            exportpage + "?ui=1&ex_source=All&action=Download&sgn=" + puddleid

    let fetchAsync(filename, url:string) =
        async {
            try
                let uri = new System.Uri(url)
                let webClient = new WebClient()
                let! html = webClient.AsyncDownloadFile(uri, filename)
                printfn "%s " <| "Downloaded " + filename
                return 1
            with
                
                 | :?Exception as ex ->
                    Message.eventError  "Could not get download " 
                        |> Message.setField "filename" filename
                        |> Message.setField "url" url
                        |> Message.addExn ex
                        |> Logger.logSimple logger
                    printfn "%s" (url + " " + ex.Message); return 0
        }
    
    let filenamesurls exportpage filelist =
        let filenames = 
            List.map (fun (_ , filename ) -> filename) filelist
        let urls = 
            List.map (fun (puddleid, _ ) -> 
                fullurl puddleid exportpage) filelist
        List.zip filenames urls

    let downloadall exportpage filenames  = 
        filenamesurls exportpage filenames
                |> Seq.map fetchAsync
                |> Async.Parallel
                |> Async.RunSynchronously
                |> Seq.sum
          
    let download (logger: Logger) exportpage filenames =
        printfn "%s " "Starting downloads" 
        logger.info (eventX "Starting downloads")
        let downloaded = 
            downloadall exportpage filenames

        printfn "%i puddles downloaded.\r\n" downloaded |> ignore

        logger.info (eventX "{downloaded} puddles downloaded."
                        >> setField "downloaded" downloaded) |> ignore

 