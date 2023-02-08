namespace UniSpec

open System
open System.IO
open System.Reflection
open System.Collections.Generic
open FSharp.Reflection


type Action = delegate of unit -> unit

type TemplateKey =
    { Feature: string
      LineNumber: int
      Scenario: string }

type Todo =
    { Key: TemplateKey
      Tags: string list
      Action: Action }

type Template =
    { Before: MethodInfo voption
      Steps: (MethodInfo * (string * Type)[]) list
      After: MethodInfo voption }

type Task =
    { Key: TemplateKey
      Tags: string list
      Steps: Step list
      ArgDict: IDictionary<string, string> voption }


[<Sealed>]
type UniSpec(path: string, assembly: Assembly) =
    let stepDict = Dictionary<string, MethodInfo>()
    let beforeDict = Dictionary<Type, MethodInfo>()
    let afterDict = Dictionary<Type, MethodInfo>()
    let templateDict = Dictionary<TemplateKey, Template>()
    let tasksDict = Dictionary<string, string * Todo list>()

    let generateTemplate key (steps: Step list) =
        let mis =
            steps
            |> List.map (fun step ->
                if stepDict.ContainsKey step.Name then
                    let mi = stepDict[step.Name]

                    let paras =
                        mi.GetParameters() |> Array.map (fun para -> para.Name, para.ParameterType)

                    mi, paras
                else
                    failwith $"Could not find step definition of %s{step.Name}")

        let (mi, _) = mis.Head
        let t = mi.DeclaringType

        if mis.Tail |> List.exists (fun (mi, _) -> mi.DeclaringType <> t) then
            failwith
                $"Steps should in same module: Feature[%s{key.Feature}]; LineNumber[%d{key.LineNumber}]; Scenario[%s{key.Scenario}]"

        let before =
            if beforeDict.ContainsKey t then
                ValueSome beforeDict[t]
            else
                ValueNone

        let after =
            if afterDict.ContainsKey t then
                ValueSome afterDict[t]
            else
                ValueNone

        { Before = before
          Steps = mis
          After = after }

    let getExample feature (outline: Outline) examples =
        Table.toArgs examples.Table
        |> List.map (fun argDict ->
            let key =
                { Feature = feature.Name
                  LineNumber = outline.LineNumber
                  Scenario = outline.Name }

            { Key = key
              Tags = feature.Tags @ outline.Tags @ examples.Tags
              Steps = feature.Background @ outline.Steps
              ArgDict = ValueSome argDict })

    let generate feature =
        feature.Scenarios
        |> List.iter (fun scenario ->
            let key =
                { Feature = feature.Name
                  LineNumber = scenario.LineNumber
                  Scenario = scenario.Name }

            let steps = feature.Background @ scenario.Steps
            templateDict.Add(key, generateTemplate key steps))

        feature.Outlines
        |> List.iter (fun outline ->
            let key =
                { Feature = feature.Name
                  LineNumber = outline.LineNumber
                  Scenario = outline.Name }

            let steps = feature.Background @ outline.Steps
            templateDict.Add(key, generateTemplate key steps))

        let todo key steps argDict =
            let template = templateDict[key]

            fun _ ->
                match template.Before with
                | ValueSome mi -> mi.Invoke(null, null) |> ignore
                | ValueNone -> ()

                List.iter2
                    (fun (step: Step) (mi: MethodInfo, paras) ->
                        let outlineStep paras (argDict: IDictionary<string, string>) =
                            paras
                            |> Array.map (fun (n, t: Type) ->
                                if argDict.ContainsKey n then
                                    Convert.ChangeType(argDict[n], t)
                                else
                                    failwith $"Parameter '%s{n}' should defined by examples table: Step[%s{step.Name}]")

                        let args =
                            match step.Argument, argDict with
                            | Empty, ValueNone -> null
                            | Empty, ValueSome argDict -> outlineStep paras argDict
                            | Table table, ValueNone -> [| table |]
                            | Table table, ValueSome argDict ->
                                [| yield! outlineStep paras[0 .. paras.Length - 2] argDict; table |]
                            | Doc doc, ValueNone -> [| doc |]
                            | Doc doc, ValueSome argDict ->
                                [| yield! outlineStep paras.[0 .. paras.Length - 2] argDict; doc |]

                        mi.Invoke(null, args) |> ignore)
                    steps
                    template.Steps

                match template.After with
                | ValueSome mi -> mi.Invoke(null, null) |> ignore
                | ValueNone -> ()

        let tasks =
            [ yield!
                  feature.Scenarios
                  |> List.map (fun scenario ->
                      let key =
                          { Feature = feature.Name
                            LineNumber = scenario.LineNumber
                            Scenario = scenario.Name }

                      { Key = key
                        Tags = feature.Tags @ scenario.Tags
                        Steps = feature.Background @ scenario.Steps
                        ArgDict = ValueNone })
              yield!
                  feature.Outlines
                  |> List.collect (fun outline ->
                      outline.Examples
                      |> List.collect (fun examples -> getExample feature outline examples)) ]

        tasks
        |> List.map (fun task ->
            { Key = task.Key
              Tags = task.Tags
              Action = Action(todo task.Key task.Steps task.ArgDict) })

    do
        assembly.GetTypes()
        |> Array.filter FSharpType.IsModule
        |> Array.iter (fun t ->
            t.GetMethods()
            |> Array.iter (fun m ->
                if m.IsDefined(typeof<StepAttribute>, false) then
                    stepDict.Add(m.Name, m)

                if m.IsDefined(typeof<BeforeAttribute>, false) then
                    beforeDict.Add(t, m)

                if m.IsDefined(typeof<AfterAttribute>, false) then
                    afterDict.Add(t, m)))

        Directory.EnumerateFiles(path, "*.feature", SearchOption.AllDirectories)
        |> Seq.map (fun path -> Path.GetFileName path, BlockParser.parse <| File.ReadAllLines path)
        |> Seq.iter (fun (fileName, feature) -> tasksDict.Add(fileName, (feature.Name, generate feature)))

    member _.Get filename =
        if tasksDict.ContainsKey filename then
            tasksDict[filename]
        else
            failwith $"Could not get feature test tasks of %s{filename}"

    member _.GetTags filename tags =
        if tasksDict.ContainsKey filename then
            let name, todoList = tasksDict[filename]

            let todoList =
                todoList
                |> List.filter (fun todo -> tags |> List.exists (fun tag -> todo.Tags |> List.contains tag))

            name, todoList
        else
            failwith $"Could not get feature test tasks of %s{filename}"
