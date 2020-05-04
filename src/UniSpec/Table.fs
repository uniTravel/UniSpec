namespace UniSpec

open System
open System.Reflection
open FSharp.Reflection


type Table = { Header: string[]; Body: string[,] }


module Table =
    let args table =
        [ 0 .. Array2D.length1 table.Body - 1 ]
        |> List.map (fun i -> dict <| Array.zip table.Header table.Body.[i,*])
    let parse table (typ: Type) =
        let fi = Seq.head <| typ.GetRuntimeFields()
        if fi.Name = "head" then
            let ts = FSharpType.GetTupleElements fi.FieldType
            [ 0 .. Array2D.length1 table.Body - 1 ]
            |> List.map (fun i ->
                let x = Array.map2 (fun v (t: Type) -> Convert.ChangeType(v, t)) table.Body.[i,*] ts
                FSharpValue.MakeTuple(x, fi.FieldType))
            |> box
        else failwithf "Target type should be F# list: %A" typ