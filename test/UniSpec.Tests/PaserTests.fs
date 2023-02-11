module Parser.Tests

open System.IO
open Expecto
open UniSpec


[<Tests>]
let tests =
    testList
        "Parser"
        [ testCase "简单数据表"
          <| fun _ ->
              let lines = File.ReadAllLines "Features/DataTable_simple.feature"
              let feature = BlockParser.parse lines
              Expect.equal feature.Name "DataTables" "Feature名称提取错误"
          testCase "数据表内含注释、有空行"
          <| fun _ ->
              let lines = File.ReadAllLines "Features/DataTable_comments.feature"
              let feature = BlockParser.parse lines

              match feature.Scenarios.[0].Steps.[0].Argument with
              | Table { Header = header; Body = body } ->
                  Expect.hasLength header 3 "表头应该有三列"
                  Expect.hasLength body.[*, 0] 2 "数据应该有两行"
                  Expect.hasLength body.[0, *] 3 "数据应该有三列"
              | _ -> failtestNoStack "应该返回Table类型的参数"
          testCase "数据表只有表头"
          <| fun _ ->
              let lines = File.ReadAllLines "Bad/DataTable_oneline.feature"
              let f = fun () -> BlockParser.parse lines |> ignore
              Expect.throwsC f (fun ex -> printfn "%s" ex.Message)
          testCase "数据行有不同的列数"
          <| fun _ ->
              let lines = File.ReadAllLines "Bad/DataTable_different_columns.feature"
              let f = fun () -> BlockParser.parse lines |> ignore
              Expect.throwsC f (fun ex -> printfn "%s" ex.Message) ]
    |> testLabel "UniSpec"
