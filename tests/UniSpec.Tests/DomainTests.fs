module Domain.Tests

open System
open System.Reflection
open Expecto
open UniSpec


let [<Given>] ``First <pm> Test`` x y =
    x + y + 1

[<Tests>]
let tests =
    testList "General" [
        ptestCase "1" <| fun _ ->
            let a = Assembly.GetExecutingAssembly()
            let ts = a.GetExportedTypes()
            let mis = ts |> Array.collect (fun t -> t.GetMethods())
            let mf = mis |> Array.filter (fun m -> m.GetCustomAttribute(typeof<GivenAttribute>) |> isNull |> not)
            let modu = a.GetType("Domain.Tests")
            let mi = modu.GetMethod("First <pm> Test")
            let pis = mi.GetParameters()
            let s = Convert.ChangeType("12", typeof<int>)
            let r2 = mi.Invoke(null, [| s; 2 |])
            printfn "Done!"
        testCase "" <| fun _ ->
            let a = array2D [ [ 1; 2 ]; [ 3; 4 ]; [ 5; 6 ] ]
            let l1 = Array2D.length1 a
            let l2 = Array2D.length2 a
            printfn "Done!"
    ]
    |> testLabel "UniSpec"