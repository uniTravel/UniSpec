namespace UniSpec

open System
open FSharp.Reflection


type Table = { Header: string[]; Body: string[,] }


module Table =
    let fromLines head (rows: string list list) =
        if rows.IsEmpty then
            failwith "Table need a header and at least one data line"

        let body = array2D rows

        { Header = Array.ofList head
          Body = body }

    let toArgs table =
        [ 0 .. Array2D.length1 table.Body - 1 ]
        |> List.map (fun i -> dict <| Array.zip table.Header table.Body.[i, *])

    let toList<'t> table =
        let typ = typeof<'t>
        let ts = FSharpType.GetTupleElements typ

        [ 0 .. Array2D.length1 table.Body - 1 ]
        |> List.map (fun i ->
            let x =
                Array.map2 (fun v (t: Type) -> Convert.ChangeType(v, t)) table.Body.[i, *] ts

            FSharpValue.MakeTuple(x, typ) :?> 't)
