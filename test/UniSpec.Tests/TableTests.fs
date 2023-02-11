module Table.Tests

open Expecto
open UniSpec


[<Tests>]
let tests =
    testList
        "Table"
        [ testCase "提取参数"
          <| fun _ ->
              let header = [| "name"; "sex"; "age" |]
              let body = array2D [ [ "Sky"; "Male"; "18" ]; [ "Fly"; "Female"; "16" ] ]
              let table = { Header = header; Body = body }
              let args = Table.toArgs table
              Expect.hasLength args 2 "应该包括两组参数"
          testCase "转换数据表"
          <| fun _ ->
              let header = [| "name"; "sex"; "age" |]
              let body = array2D [ [ "Sky"; "Male"; "18" ]; [ "Fly"; "Female"; "16" ] ]
              let table = { Header = header; Body = body }
              let data = Table.toList<string * string * int> table
              let (name, sex, age) = data.Head
              Expect.hasLength data 2 "应该包括两行数据"
              Expect.equal (name.GetType()) typeof<string> "数据应为String类型"
              Expect.equal (sex.GetType()) typeof<string> "数据应为String类型"
              Expect.equal (age.GetType()) typeof<int> "数据应为Int类型"
          testCase "转换数据表，列表的数据类型不对"
          <| fun _ ->
              let header = [| "name"; "sex"; "age" |]
              let body = array2D [ [ "Sky"; "Male"; "18" ]; [ "Fly"; "Female"; "16" ] ]
              let table = { Header = header; Body = body }
              let f = fun _ -> Table.toList<string> table |> ignore
              Expect.throwsC f <| fun ex -> printfn "%s" ex.Message
          testCase "转换数据表，元组元素的数据类型不对"
          <| fun _ ->
              let header = [| "name"; "sex"; "age" |]
              let body = array2D [ [ "Sky"; "Male"; "18" ]; [ "Fly"; "Female"; "16" ] ]
              let table = { Header = header; Body = body }
              let f = fun _ -> Table.toList<string * int * int> table |> ignore
              Expect.throwsC f <| fun ex -> printfn "%s" ex.Message ]
    |> testLabel "UniSpec"
