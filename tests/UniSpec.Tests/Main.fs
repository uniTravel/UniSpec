module UniSpec.Test

open Expecto

[<EntryPoint>]
let main args =
    let test =
        Impl.testFromThisAssembly()
        |> Option.defaultValue (TestList ([], Normal))
    runTestsWithCLIArgs [] args test