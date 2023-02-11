module Spec.Tests

open System.Reflection
open Expecto
open UniSpec


let assembly = Assembly.GetExecutingAssembly()
let spec = UniSpec("Features", assembly)

let featureTest feature =
    let name, todoList = spec.Get feature

    todoList
    |> List.map (fun todo -> testCase todo.Key.Scenario todo.Action.Invoke)
    |> testList name

[<Tests>]
let tests = featureTest "DataTable_simple.feature" |> testLabel "Spec"
