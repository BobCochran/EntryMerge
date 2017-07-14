// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
 
open Fuchu
let tests = 
    testList "A test group" [
        testCase "one test" <|
            fun _ -> Assert.Equal("2+2", 4, 2+2)
        testCase "another test" <|
            fun _ -> Assert.Equal("3+3", 3, 3+3)
    ]
[<EntryPoint>]
let main args = defaultMainThisAssembly args
