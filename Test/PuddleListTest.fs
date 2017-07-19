namespace EntryMergeTest
module PuddleListTest =
    open EntryMerge
    open FSharp.Data
    open Fuchu

    let samplehtmldocument = (EntryMerge.PuddleList.SignTypeIndex.GetSample()).Html
    let puddleslength (l: Result<(string * string)list, string>) = 
                          match l with
                          | Ok list -> List.length list
                          | Error err -> 0
    [<Tests>] 
    let tests = 
        testList "Puddle List test group" [
            testCase "number of puddle names " <| 
                fun _ -> Assert.Equal("number of puddle names", 48, EntryMerge.PuddleList.puddlenames samplehtmldocument |> List.length   )
            testCase "number of puddle ids " <| 
                fun _ -> Assert.Equal("number of puddle ids", 48, EntryMerge.PuddleList.puddleids samplehtmldocument |> List.length  )
            testCase "number of puddles " <| 
                fun _ -> Assert.Equal("number of puddles", 48, EntryMerge.PuddleList.retrievepuddles samplehtmldocument 
                         |> puddleslength )
        ]