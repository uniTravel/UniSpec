namespace UniSpec


type internal ItemType =
    | TableRow of string list
    | MultiLineStart
    | MultiLine of string
    | MultiLineEnd

type internal LineType =
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


/// <summary>行转换器模块
/// <para>转换Ferture文档中的文本行。</para>
/// </summary>
[<RequireQualifiedAccess>]
module internal LineParser =

    /// <summary>转换
    /// </summary>
    val parse: (LineType * string) -> LineType

    /// <summary>期望
    /// </summary>
    val expecting: LineType -> string
