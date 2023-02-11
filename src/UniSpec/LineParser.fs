namespace UniSpec

open System.Text.RegularExpressions


type ItemType =
    | TableRow of string list
    | MultiLineStart
    | MultiLine of string
    | MultiLineEnd

type LineType =
    | FileStart
    | Tag of string list
    | Feature of string
    | FeatureDescription of string
    | Background
    | Rule of string
    | Scenario of string
    | Outline of string
    | Description of string
    | Examples
    | Step of Category * string
    | OutlineStep of Category * string
    | Bullet of Category
    | Item of LineType * ItemType
    | SyntaxError


module LineParser =
    let tryRegex input pattern (idx: int) =
        let m = Regex.Match(input, pattern, RegexOptions.IgnoreCase)
        if m.Success then m.Groups[idx].Value |> Some else None

    let startsWith pattern (s: string) =
        s.StartsWith(pattern, System.StringComparison.InvariantCultureIgnoreCase)

    let (|Trim|) (s: string) = s.Trim()

    let (|AfterStep|_|) =
        function
        | Step(c, s) -> AfterStep(c, s) |> Some
        | Item(Step(c, s), TableRow _) -> AfterStep(c, s) |> Some
        | Item(Step(c, s), MultiLineEnd) -> AfterStep(c, s) |> Some
        | _ -> None

    let (|AfterOutlineStep|_|) =
        function
        | OutlineStep(c, s) -> AfterOutlineStep(c, s) |> Some
        | Item(OutlineStep(c, s), TableRow _) -> AfterOutlineStep(c, s) |> Some
        | Item(OutlineStep(c, s), MultiLineEnd) -> AfterOutlineStep(c, s) |> Some
        | _ -> None

    let (|GivenLine|_|) s =
        tryRegex s "^\s*Given\s(.*)" 1
        |> Option.map (function
            | Trim t -> GivenLine t)

    let (|WhenLine|_|) s =
        tryRegex s "^\s*When\s(.*)" 1
        |> Option.map (function
            | Trim t -> WhenLine t)

    let (|ThenLine|_|) s =
        tryRegex s "^\s*Then\s(.*)" 1
        |> Option.map (function
            | Trim t -> ThenLine t)

    let (|AndLine|_|) s =
        tryRegex s "^\s*And\s(.*)" 1
        |> Option.map (function
            | Trim t -> AndLine t)

    let (|ButLine|_|) s =
        tryRegex s "^\s*But\s(.*)" 1
        |> Option.map (function
            | Trim t -> ButLine t)

    let (|BulletLine|_|) s =
        tryRegex s "^\s*\*\s(.*)" 1
        |> Option.map (function
            | Trim t -> BulletLine t)

    let (|ScenarioLine|_|) (s: string) =
        tryRegex s "^\s*(Scenario:|Example:)\s(.*)" 2
        |> Option.map (function
            | Trim t -> ScenarioLine t)

    let (|OutlineLine|_|) (s: string) =
        tryRegex s "^\s*(Scenario Outline:|Scenario Template:)\s(.*)" 2
        |> Option.map (function
            | Trim t -> OutlineLine t)

    let (|ExamplesLine|_|) (s: string) =
        if s.Trim() |> startsWith "Examples" then
            Some ExamplesLine
        else
            None

    let (|RuleLine|_|) (s: string) =
        let trimmed = s.Trim()

        if trimmed |> startsWith "Rule" then
            RuleLine trimmed |> Some
        else
            None

    let (|Attributes|_|) (s: string) =
        if s.Trim() |> startsWith "@" then
            let tags = [ for tag in Regex.Matches(s, @"@(\w+)") -> tag.Value.Substring(1) ]
            Attributes tags |> Some
        else
            None

    let (|FeatureLine|_|) (s: string) =
        tryRegex s "^\s*Feature:\s(.*)" 1
        |> Option.map (function
            | Trim t -> FeatureLine t)

    let (|BackgroundLine|_|) (s: string) =
        if s.Trim() |> startsWith "Background" then
            Some BackgroundLine
        else
            None

    let (|TableRowLine|_|) (s: string) =
        if s.Trim().StartsWith("|") then
            let cs = s.Trim().Split([| '|' |], System.StringSplitOptions.RemoveEmptyEntries)
            let columns = [ for (Trim s) in cs -> s ]
            TableRowLine columns |> Some
        else
            None

    let (|DocMarker|_|) (s: string) =
        let trimmed = s.Trim()
        if trimmed = "\"\"\"" then Some trimmed else None

    let parse =
        function
        | Background, GivenLine text
        | Scenario _, GivenLine text
        | AfterStep _, GivenLine text
        | AfterStep(Given, _), AndLine text
        | AfterStep(Given, _), ButLine text
        | AfterStep(Given, _), BulletLine text -> Step(Given, text)
        | Scenario _, WhenLine text
        | Outline _, WhenLine text
        | AfterStep _, WhenLine text
        | AfterStep(When, _), AndLine text
        | AfterStep(When, _), ButLine text
        | AfterStep(When, _), BulletLine text -> Step(When, text)
        | AfterStep _, ThenLine text
        | AfterStep(Then, _), AndLine text
        | AfterStep(Then, _), ButLine text
        | AfterStep(Then, _), BulletLine text -> Step(Then, text)
        | Outline _, GivenLine text
        | AfterOutlineStep _, GivenLine text
        | AfterOutlineStep(Given, _), AndLine text
        | AfterOutlineStep(Given, _), ButLine text
        | AfterOutlineStep(Given, _), BulletLine text -> OutlineStep(Given, text)
        | Outline _, WhenLine text
        | AfterOutlineStep _, WhenLine text
        | AfterOutlineStep(When, _), AndLine text
        | AfterOutlineStep(When, _), ButLine text
        | AfterOutlineStep(When, _), BulletLine text -> OutlineStep(When, text)
        | AfterOutlineStep _, ThenLine text
        | AfterOutlineStep(Then, _), AndLine text
        | AfterOutlineStep(Then, _), ButLine text
        | AfterOutlineStep(Then, _), BulletLine text -> OutlineStep(Then, text)
        | Feature _, ScenarioLine text
        | Rule _, ScenarioLine text
        | FeatureDescription _, ScenarioLine text
        | Step _, ScenarioLine text
        | Item(Step _, TableRow _), ScenarioLine text
        | Item(Step _, MultiLineEnd), ScenarioLine text
        | Item(Examples, TableRow _), ScenarioLine text -> Scenario text
        | Feature _, OutlineLine text
        | Rule _, OutlineLine text
        | FeatureDescription _, OutlineLine text
        | Step _, OutlineLine text
        | Item(Step _, TableRow _), OutlineLine text
        | Item(Step _, MultiLineEnd), OutlineLine text
        | Item(Examples, TableRow _), OutlineLine text -> Outline text
        | OutlineStep _, ExamplesLine
        | Item(OutlineStep _, TableRow _), ExamplesLine
        | Item(OutlineStep _, MultiLineEnd), ExamplesLine
        | Item(Examples, TableRow _), ExamplesLine -> Examples
        | Feature _, RuleLine text
        | FeatureDescription _, RuleLine text
        | Step _, RuleLine text
        | Item(Step _, TableRow _), RuleLine text
        | Item(Step _, MultiLineEnd), RuleLine text
        | Item(Examples, TableRow _), RuleLine text -> Rule text
        | _, Attributes tags -> Tag tags
        | FileStart, FeatureLine text -> Feature text
        | Feature _, BackgroundLine
        | FeatureDescription _, BackgroundLine -> Background
        | (Step _ as l), TableRowLine row
        | (OutlineStep _ as l), TableRowLine row
        | (Examples as l), TableRowLine row
        | Item(l, TableRow _), TableRowLine row -> Item(l, TableRow row)
        | (Step _ as l), DocMarker _
        | (OutlineStep _ as l), DocMarker _ -> Item(l, MultiLineStart)
        | Item(l, MultiLineStart _), Trim line
        | Item(l, MultiLine _), Trim line -> Item(l, MultiLine line)
        | Item(l, MultiLine _), DocMarker _ -> Item(l, MultiLineEnd)
        | Feature _, line
        | FeatureDescription _, line -> FeatureDescription line
        | Background, line
        | Rule _, line
        | Scenario _, line
        | Outline _, line
        | Description _, line -> Description line
        | _, _ -> SyntaxError

    let expecting =
        function
        | FileStart -> "Expecting feature definition in the beginning of file"
        | Scenario _
        | Background -> "Expecting steps"
        | Examples -> "Expecting table row"
        | Step _ -> "Expecting another step, table row, bullet, examples or end of scenario"
        | Item(Step _, TableRow _) -> "Expecting another table row, next step, examples or end of scenario"
        | Item(Examples, _) -> "Expecting a table row, another examples or end of scenario"
        | _ -> "Unexpected line"
