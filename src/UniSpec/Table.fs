namespace UniSpec


type Table = { Header: string[]; Body: string[,] }


module Table =
    let paras table =
        [ 0 .. Array2D.length1 table.Body - 1 ]
        |> List.map (fun i ->
            dict <| Array.zip table.Header table.Body.[i, *] )