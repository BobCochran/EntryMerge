namespace EntryMerge
open System
open System.Text.RegularExpressions
open System.IO

module MergePuddle =
    let readLines (filePath:string) = 
        if (File.Exists(filePath)) then
            seq {
                use sr = new StreamReader (filePath)
                while not sr.EndOfStream do
                    yield sr.ReadLine ()
                 } |> Ok
        else
            "File " + filePath + " does not exist. Skipping file." |> Error 
    let saveentries outputfile entries =
        use streamWriter = new StreamWriter(outputfile, true)
        let countsequence = 
            Seq.map (fun x -> streamWriter.WriteLine( x:string);1) entries
        Seq.sum countsequence

    let split (separators: string [] ) (x:string) = 
        x.Split(separators, StringSplitOptions.RemoveEmptyEntries)

    let splitintolines (str:string) = 
        split ([|"$newline$"|]) str

    let keepentries lines = 
        Seq.filter (fun (x:String) -> x.Contains "PUDDLEID ") lines

    let insertnewlinemarkerpuddleid puddleid str =
        (fun (x: String)  -> x.Replace("<entry","$newline$" + puddleid + "<entry")) str
        |> (fun x  -> x.Replace("</spml>","$newline$</spml>") )

    let jointags str = 
        Regex.Replace(str, "[>]\\s+[<]" , "><");

    let makeoneline lines =
        Seq.map (fun (x:String)  -> x.Replace("\r","").Replace("\n","").Trim()) lines
        |> String.concat ""

    let puddleIdText (puddleid: String) =
        "PUDDLEID XXX ENDPUDDLEID".Replace("XXX", puddleid.PadLeft(4, '0' ))

    let clean puddleid lines =
        lines
        |> makeoneline
        |> jointags
        |> insertnewlinemarkerpuddleid (puddleIdText puddleid)
        |> splitintolines 
        |> keepentries

    let mergefile outputfile filename puddleid =
        readLines (filename)
        |> Result.map (clean puddleid)
        |> Result.map (saveentries outputfile)

    let mergeallfiles outputfilename filenames =
        File.Delete outputfilename
        filenames 
        |> Seq.map (fun (puddleid, filename) ->
            match mergefile outputfilename filename puddleid with
                |Ok value -> value
                |Error err ->
                    printfn "%s " <| err
                    0)
        |> Seq.sum