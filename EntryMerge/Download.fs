namespace EntryMerge
open System.Net
open System.IO
module Download =
    let fullurl puddleid  exportpage =
            exportpage + "?ui=1&ex_source=All&action=Download&sgn=" + puddleid

    let puddlefilename wd puddleid puddlename =
          Path.Combine (wd, "sgn" + puddleid + "_" + puddlename+ ".spml")

    let fetchAsync(filename, url:string) =
        async {
            try
                let uri = new System.Uri(url)
                let webClient = new WebClient()
                let! html = webClient.AsyncDownloadFile(uri, filename)
                printfn "%s " <| "Downloaded " + filename
                return 1
            with
                | ex -> printfn "%s" (url + " " + ex.Message); return 0
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

         
       

 