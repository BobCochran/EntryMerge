namespace EntryMerge
open System.Net
open System.IO
module Download =
    let getfullurl puddleid  exportpage =
            exportpage + "?ui=1&ex_source=All&action=Download&sgn=" + puddleid

    let getpuddlefilename wd puddleid puddlename =
          Path.Combine (wd, "sgn" + puddleid + "" + puddlename+ ".spml")

    let fetchAsync(filename, url:string) =
        async {
            try
                let uri = new System.Uri(url)
                let webClient = new WebClient()
                let! html = webClient.AsyncDownloadFile(uri, filename)
                printfn "Downloaded %s " filename
                return 1
            with
                | ex -> printfn "%s" (url + " " + ex.Message); return 0
        }
    
    let filenamesurls wd exportpage puddlelist =
        let filenames = List.map (fun (puddleid, puddlename ) -> 
                                    getpuddlefilename wd puddleid puddlename) puddlelist
        let urls = List.map (fun (puddleid, _ ) -> 
                                    getfullurl puddleid exportpage) puddlelist
        List.zip filenames urls
        

    let puddlesasync wd exportpage puddlelist =
                filenamesurls wd exportpage puddlelist
                |> Seq.map fetchAsync
                |> Async.Parallel
                |> Async.RunSynchronously
                |> Seq.sum

    let downloadall wd exportpage puddles  = 
         match puddles with
            | Ok puddlelist  -> 
                puddlesasync wd exportpage puddlelist
            | Error _ -> 0
         
       

 