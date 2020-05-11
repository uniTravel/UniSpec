module Spec.Tests

open System.Reflection
open Expecto
open UniSpec


[<Tests>]
let tests =
    testSequenced <| testList "Spec" [
        testCase "创建UniSpec" <| fun _ ->
            let assembly = Assembly.GetExecutingAssembly()
            let spec = UniSpec("Features", assembly)
            let f = spec.Get "DataTable_simple.feature"
            Expect.hasLength f 1 "应该只有一个场景"
    ]
    |> testLabel "UniSpec"