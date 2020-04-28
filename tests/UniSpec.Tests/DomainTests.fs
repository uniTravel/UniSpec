module Domain.Tests

open Expecto


[<Tests>]
let tests =
    testList "General" [
        testCase "" <| fun _ ->
            printfn "Done!"
    ]
    |> testLabel "UniSpec"