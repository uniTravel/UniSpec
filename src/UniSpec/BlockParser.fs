namespace UniSpec

open System.Text


module BlockParser =
    let raiseParseException message lines =
        match lines with
        | (number, _, _) :: _ -> failwith $"Parsing failed on row %d{number}: %s{message}"
        | _ -> failwith $"Parsing failed on the end of file: %s{message}"

    let parseTags lines =
        let rec parseTagsInternal tags lines =
            match lines with
            | (_, _, Tag tagList) :: xs -> parseTagsInternal (tags @ tagList) xs
            | _ -> tags, lines

        parseTagsInternal [] lines

    let parseTable lines =
        let rec readTableRows rows lines =
            match lines with
            | (_, _, Item(_, TableRow row)) :: xs -> readTableRows (row :: rows) xs
            | _ -> List.rev rows, lines

        let allRows, lines = readTableRows [] lines

        match allRows with
        | head :: rows ->
            try
                Table.fromLines head rows, lines
            with ex ->
                raiseParseException ex.Message lines
        | _ -> lines |> raiseParseException "Table expected"

    let parseMultiLine lines =
        let lines =
            match lines with
            | (_, _, Item(_, MultiLineStart)) :: xs -> xs
            | _ -> lines |> raiseParseException "DocString start expected"

        let rec readLines lines (sb: StringBuilder) =
            match lines with
            | (_, _, Item(_, MultiLine s)) :: xs ->
                if sb.Length > 0 then
                    sb.AppendLine "" |> ignore

                sb.Append s |> ignore
                readLines xs sb
            | _ -> sb.ToString(), lines

        let text, lines = readLines lines <| StringBuilder()

        match lines with
        | (_, _, Item(_, MultiLineEnd)) :: xs -> text, xs
        | _ -> lines |> raiseParseException "DocString end expected"

    let parseExamples lines =
        let rec parseExamplesInternal examples lines =
            let tags, lines = parseTags lines

            match lines with
            | (_, _, Examples) :: xs ->
                let table, lines = parseTable xs
                parseExamplesInternal (examples @ [ { Tags = tags; Table = table } ]) lines
            | _ -> examples, lines

        parseExamplesInternal [] lines

    let parseItem lines =
        match lines with
        | (_, _, Item(_, MultiLineStart _)) :: _ ->
            let text, lines = parseMultiLine lines
            Doc text, lines
        | (_, _, Item(_, TableRow _)) :: _ ->
            let table, lines = parseTable lines
            Table table, lines
        | _ -> Empty, lines

    let parseSteps lines =
        let parseStep lines =
            match lines with
            | (_, _, Step(c, s)) :: xs
            | (_, _, OutlineStep(c, s)) :: xs ->
                let item, newLines = parseItem xs

                Some
                    { Name = s
                      Category = c
                      Argument = item },
                newLines
            | _ -> None, lines

        let rec parseStepsInternal steps lines =
            let parsedStep, lines = parseStep lines

            match parsedStep with
            | Some step -> parseStepsInternal (step :: steps) lines
            | None -> steps, lines

        let steps, lines = parseStepsInternal [] lines

        if steps.IsEmpty then
            lines |> raiseParseException "At least one step is expected"

        List.rev steps, lines

    let parseBackground lines =
        match lines with
        | (_, _, Background) :: xs -> parseSteps xs
        | _ -> [], lines

    let parseScenarios lines =
        let rec parseScenariosInternal scenarios outlines lines =
            let tags, lines = parseTags lines

            match lines with
            | (number, _, Scenario name) :: xs ->
                let steps, lines = parseSteps xs

                let parsedScenario =
                    { Name = name
                      LineNumber = number
                      Tags = tags
                      Steps = steps }

                parseScenariosInternal (parsedScenario :: scenarios) outlines lines
            | (number, _, Outline name) :: xs ->
                let steps, lines = parseSteps xs
                let examples, lines = parseExamples lines

                let parsedOutline =
                    { Name = name
                      LineNumber = number
                      Tags = tags
                      Steps = steps
                      Examples = examples }

                parseScenariosInternal scenarios (parsedOutline :: outlines) lines
            | _ -> scenarios, outlines, lines

        let scenarios, outlines, lines = parseScenariosInternal [] [] lines

        if scenarios.IsEmpty && outlines.IsEmpty then
            lines |> raiseParseException "At least one scenario/outline is expected"

        List.rev scenarios, List.rev outlines, lines

    let parseFeature lines =
        let tags, lines = parseTags lines

        let featureName, lines =
            match lines with
            | (_, _, Feature name) :: xs -> name, xs
            | _ -> lines |> raiseParseException "Expected feature in the beginning of file"

        let background, lines = parseBackground lines
        let scenarios, outlines, lines = parseScenarios lines

        if not lines.IsEmpty then
            lines |> raiseParseException "File continues unexpectedly"

        { Name = featureName
          Tags = tags
          Background = background
          Scenarios = scenarios
          Outlines = outlines }

    let parseFeatureFile parsedLines =
        match parsedLines with
        | (_, _, FileStart) :: xs -> parseFeature xs
        | _ -> parsedLines |> raiseParseException "Unexpected call of parser"

    let parse (lines: string seq) =
        lines
        |> Seq.mapi (fun number content -> (number + 1, content))
        |> Seq.map (fun (number, content) ->
            let i = content.IndexOf("#")

            if i = -1 then
                number, content
            else
                number, content.Substring(0, i))
        |> Seq.filter (fun (_, content) -> content.Trim().Length > 0)
        |> Seq.scan
            (fun (_, _, lastParsedLine) (number, content) ->
                let parsed = LineParser.parse (lastParsedLine, content)

                match parsed with
                | SyntaxError ->
                    failwithf "Syntax error on line %d %s\r\n%s" number content
                    <| LineParser.expecting lastParsedLine
                | Tag _ -> number, content, lastParsedLine
                | _ -> number, content, parsed)
            (0, "", FileStart)
        |> Seq.filter (fun (_, _, line) ->
            match line with
            | FeatureDescription _
            | Description _
            | Rule _ -> false
            | _ -> true)
        |> Seq.toList
        |> parseFeatureFile
