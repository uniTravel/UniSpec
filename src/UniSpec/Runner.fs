namespace UniSpec

open System
open System.Reflection
open System.Collections.Generic


type State =
    | Init
    | Success
    | Failed

type Output =
    { Feature: string
      LineNumber: int
      Scenario: string
      Tags: string list
      Background: Step list
      Steps: Step list
      Parameters: IDictionary<string, string> voption
      State: State }

module Runner =

    type T = { Tasks: Output list; Steps: Dictionary<string, MethodInfo * (string * Type)[]> }

    let getExample feature (outline: Outline) examples =
        [ 0 .. Array2D.length1 examples.Table.Body - 1 ]
        |> List.map (fun i ->
            let paras = dict <| Array.zip examples.Table.Header examples.Table.Body.[i, *]
            { Feature = feature.Name
              LineNumber = outline.LineNumber
              Scenario = outline.Name
              Tags = feature.Tags @ outline.Tags @ examples.Tags
              Background = feature.Background
              Steps = outline.Steps
              Parameters = ValueSome paras
              State = Init })

    let generate features (assembly: Assembly) =
        let rec generateFeature features = [
            match features with
            | feature :: tail ->
                yield! feature.Scenarios
                |> List.map (fun scenario ->
                    { Feature = feature.Name
                      LineNumber = scenario.LineNumber
                      Scenario = scenario.Name
                      Tags = feature.Tags @ scenario.Tags
                      Background = feature.Background
                      Steps = scenario.Steps
                      Parameters = ValueNone
                      State = Init })
                yield! feature.Outlines
                |> List.collect (fun outline ->
                    outline.Examples |> List.collect (fun examples -> getExample feature outline examples))
                yield! generateFeature tail
            | [] -> ()
        ]
        let tasks = generateFeature features
        let steps = Dictionary<string, MethodInfo * (string * Type)[]>()
        assembly.GetExportedTypes()
        |> Array.collect (fun t -> t.GetMethods())
        |> Array.filter (fun m -> m.GetCustomAttribute(typeof<StepAttribute>) |> isNull |> not)
        |> Array.iter (fun m ->
            let paras = m.GetParameters() |> Array.map (fun p -> p.Name, p.ParameterType)
            steps.Add(m.Name, (m, paras)))
        { Tasks = tasks; Steps = steps }

    let runScenario (steps: Dictionary<string, MethodInfo * (string * Type)[]>) task : Output =
        // let mi = steps.[task.Scenario]
        failwith ""

    let run { Tasks = tasks; Steps = steps } =
        List.map (fun task -> runScenario steps task) tasks

    let runTags { Tasks = tasks; Steps = steps } tags =
        tasks
        |> List.filter (fun task -> List.exists (fun tr -> List.exists (fun tag -> tr = tag) task.Tags) tags)
        |> List.map (fun task -> runScenario steps task)