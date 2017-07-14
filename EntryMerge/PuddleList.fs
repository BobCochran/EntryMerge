namespace EntryMerge
open FSharp.Data
module PuddleList =
    type SignTypeIndex =   
        FSharp.Data.HtmlProvider<"./sampleindex.html"> 

    let retrievepuddles (webpage: HtmlDocument)  =
        let puddlenames =   
            webpage.CssSelect("form > button > table > tr > td > font")
            |> List.map (fun n -> n.DirectInnerText())

        let puddleids =   
            webpage.CssSelect("form > input[name*='sgn']") 
            |> List.map (fun n -> n.AttributeValue( "value"))
        in
        List.zip puddleids puddlenames    
         
    let allsigntyppuddles url = 
        retrievepuddles <| SignTypeIndex.Load(url:string).Html