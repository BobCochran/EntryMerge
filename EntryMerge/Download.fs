namespace EntryMerge


module Download =
    let getfullurl puddleid  exportpage =
            exportpage + "?ui=1&ex_source=All&action=Download&sgn=" + puddleid

    let getpuddlefilename wd puddleid puddlename =
           wd + "\\" +  "sgn" + puddleid + "" + puddlename+ ".spml"

    let downloadfile (url:string) filename =
        use wc = 
            new System.Net.WebClient()
        wc.DownloadFile ( url, filename)

    let download wd exportpage puddleid puddlename =
        let filename =
            getpuddlefilename wd puddleid puddlename 
        filename
            |> printf "%s\r\n"
        let url = 
            getfullurl puddleid  exportpage
        downloadfile url filename
        
    let downloadall wd exportpage puddles =
         printf "%s"  "Downloading puddles\r\n"
         match puddles with
            | Ok puddleslist -> 
                puddleslist|> 
                Seq.map (fun (puddleid, puddlename) ->
                    download wd exportpage puddleid puddlename
                    1
                    )
                 |> Seq.sum
            | Error _ -> 0
        