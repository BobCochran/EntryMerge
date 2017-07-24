namespace EntryMerge
open System
open System.Text.RegularExpressions
open System.IO
open Logary
open Logary.Message 
module MergePuddle =
    let logger = Logging.getCurrentLogger ()
    let readLines (filePath:string) = 
        try 
            if (File.Exists(filePath)) then
                seq {
                    use sr = new StreamReader (filePath)
                    while not sr.EndOfStream do
                        yield sr.ReadLine ()
                     } |> Ok
            else
                logger.error(eventX "File {filepath} does not exist. Skipping file."
                      >> setField "filepath" filePath)    
                "File " + filePath + " does not exist. Skipping file." |> Result.Error 

        with
            | :? System.Exception as ex -> 
                 Message.eventError  "An error occured when reading file {filepath}. Skipping file." 
                    |> Message.setField "filepath" filePath 
                    |> Message.addExn ex
                    |> Logger.logSimple logger
                 "An error occured when reading file " + filePath + ". Skipping file." 
                 |> Result.Error   
                 
    let saveentries filename outputfile  entries =
        use streamWriter = new StreamWriter(outputfile, true)
        let countsequence = 
            Seq.map (fun x -> streamWriter.WriteLine( x:string);1) entries
        printfn "%s " <| "Merged " + filename
        logger.info (eventX "Merged {filename}."
                        >> setField "filename" filename)
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
        |> Result.map (saveentries filename outputfile)

    let mergeallfiles outputfilename filenames =
        File.Delete outputfilename
        filenames 
        |> Seq.map (fun (puddleid, filename) ->
            match mergefile outputfilename filename puddleid with
                |Ok value -> value
                |Result.Error err ->
                    logger.error(eventX err)
                    printfn "%s " <| err
                    0)
        |> Seq.sum

    let merge (logger: Logger) wd filenames =
        let mergecount = mergeallfiles (Path.Combine (wd , "output.spml")) filenames
        printfn "%i entries merged.\r\n" mergecount  
        logger.info (eventX "{merged} entries merged."
                        >> setField "merged" mergecount)
 