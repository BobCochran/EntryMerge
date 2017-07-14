namespace EntryMerge
open EntryMerge.PuddleList
open FSharp.Data

module Main =
        
    let  [<Literal>] wd = 
        "C:\Users\Jonathan\Documents\Merge"
    let exportpage = 
        "http://signtyp.uconn.edu/signpuddle/export.php"
    let signpuddleindexurl = 
        "http://signtyp.uconn.edu/signpuddle/index.php"

    [<EntryPoint>]
    let main argv =
        let programName  =
           "EntryMerge"
        printfn "%A" <| allsigntyppuddles  "http://signtyp.uconn.edu/signpuddle/index.php"
        0 // return an integer exit code
         