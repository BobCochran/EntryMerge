namespace EntryMerge
open FSharp.Data
module PuddleList =
    open System
    open System.IO
    open Logary
    open Logary.Message 
 

    let logger = Logging.getCurrentLogger ()
    type SignTypeIndex =   
        FSharp.Data.HtmlProvider<"./sampleindex.html"> 

    
    let puddlenames (htmldocument: HtmlDocument) =   
        htmldocument.CssSelect("form > button > table tr > td > font")
        |> List.map (fun n -> n.DirectInnerText())

    let puddleids (htmldocument: HtmlDocument) =   
        htmldocument.CssSelect("form > input[name*='sgn']") 
        |> List.map (fun n -> n.AttributeValue( "value"))

    let retrievepuddles (htmldocument: HtmlDocument)  =
        let ids = puddleids htmldocument
        let names = puddlenames htmldocument
        if (ids.Length = names.Length) then
             List.zip  ids names |> Ok
        else
            "Could not get matching puddle ids and puddle names" |> Result.Error

    let document url =
        try
            SignTypeIndex.Load(url:string).Html |> Ok
        with
            | :?Exception as ex ->
            Message.eventError  "Could not get file " 
                |> Message.addExn ex
                |> Logger.logSimple logger
            "Could not get file " + url + " " + ex.Message   |> Result.Error

    let allsigntyppuddles url = 
        document url
        |> Result.bind retrievepuddles  

    let puddlefilename wd puddleid puddlename =
          Path.Combine (wd, "sgn" + puddleid + "_" + puddlename+ ".spml")

    let filenamesfromindexpage (logger: Logger) wd signpuddleindexurl = 
        allsigntyppuddles signpuddleindexurl
        |> Result.map (fun puddleslist ->
                let puddlecount = List.length puddleslist
                printfn "%i puddles found."  puddlecount
                logger.info (eventX "{puddlecount} puddles found."
                                >> setField "puddlecount" puddlecount)
                List.map (fun (puddleid, puddlename ) -> (puddleid, puddlefilename wd puddleid puddlename)) puddleslist) 
