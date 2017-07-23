﻿namespace EntryMerge
open FSharp.Data
module PuddleList =
    open System
    open Logary

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