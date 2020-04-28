namespace UniSpec


/// <summary>数据表
/// </summary>
/// <param name="Header">表头。</param>
/// <param name="Body">数据体，二维数组。</param>
type internal Table = { Header: string[]; Body: string[,] }

/// <summary>步骤类别
/// <para>Gherkin的步骤最终映射到Given、When、Then三种类别。</para>
/// </summary>
type internal Category =
    | Given of string
    | When of string
    | Then of string

/// <summary>步骤参数
/// </summary>
/// <param name="Table">数据表作为参数。</param>
/// <param name="Doc">多行文档作为参数。</param>
/// <param name="Empty">参数为空。</param>
type internal Argument =
    | Table of Table
    | Doc of string
    | Empty

/// <summary>步骤
/// </summary>
/// <param name="LineNumber">行号。</param>
/// <param name="Category">步骤类别。</param>
/// <param name="Argument">步骤参数。</param>
type internal Step = { LineNumber: int; Category: Category; Argument: Argument }

/// <summary>场景大纲例子
/// </summary>
/// <param name="Tags">标签列表。</param>
/// <param name="Table">数据表。</param>
type internal Examples = { Tags: string list; Table: Table }

/// <summary>测试状态
/// </summary>
/// <param name="Init">初始状态，尚未执行测试。</param>
/// <param name="Success">成功。</param>
/// <param name="Failed">失败。</param>
type internal State =
    | Init
    | Success
    | Failed

/// <summary>场景
/// <para>围绕场景开展行为测试，跟踪测试结果。</para>
/// </summary>
/// <param name="Name">场景名称。</param>
/// <param name="LineNumber">行号。</param>
/// <param name="Tags">标签列表。</param>
/// <param name="Steps">步骤清单。</param>
/// <param name="State">测试状态。</param>
type internal Scenario =
    { Name: string
      LineNumber: int
      Tags: string list
      Steps: Step list
      State: State }

/// <summary>场景大纲
/// <para>参数结构相同的场景聚合，要拆分成场景清单，并将大纲步骤具体化为场景步骤。</para>
/// </summary>
/// <param name="Name">场景大纲名称。</param>
/// <param name="LineNumber">行号。</param>
/// <param name="Tags">标签列表。</param>
/// <param name="Steps">大纲步骤清单。</param>
/// <param name="Examples">场景大纲例子。</param>
/// <param name="Scenarios">场景清单。</param>
type internal Outline =
    { Name: string
      LineNumber: int
      Tags: string list
      Steps: Step list
      Examples: Examples list
      Scenarios: Scenario list }

/// <summary>业务功能
/// </summary>
/// <param name="Name">业务功能名称。</param>
/// <param name="Tags">标签列表。</param>
/// <param name="Background">背景。</param>
/// <param name="Scenarios">场景清单。</param>
/// <param name="Outlines">场景大纲清单。</param>
type internal Feature =
    { Name: string
      Tags: string list
      Background: Step list
      Scenarios: Scenario list
      Outlines: Outline list }