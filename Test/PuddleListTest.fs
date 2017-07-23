namespace EntryMergeTest
module PuddleListTest =
    open EntryMerge
    open EntryMerge.PuddleList
    open FSharp.Data
    open System.IO
    open Fuchu

    let samplehtmldocument = (EntryMerge.PuddleList.SignTypeIndex.GetSample()).Html
    let puddleslength (l: Result<(string * string)list, string>) = 
                          match l with
                          | Ok list -> List.length list
                          | Error err -> 0
    let wd = Path.Combine ("C:\Users\User\Documents", "Merge") 
    [<Tests>] 
    let tests = 
        testList "Puddle List test group" [
            testCase "number of puddle names " <| 
                fun _ -> Assert.Equal("number of puddle names", 48, puddlenames samplehtmldocument |> List.length   )
            testCase "number of puddle ids " <| 
                fun _ -> Assert.Equal("number of puddle ids", 48,puddleids samplehtmldocument |> List.length  )
            testCase "number of puddles " <| 
                fun _ -> Assert.Equal("number of puddles", 48, retrievepuddles samplehtmldocument 
                         |> puddleslength )
            testCase "construct filename " <| 
                fun _ -> Assert.Equal("filename for puddle", 
                            "C:\Users\User\Documents\Merge\sgn106_PuddleName.spml", 
                            puddlefilename wd "106" "PuddleName"  )
        ]