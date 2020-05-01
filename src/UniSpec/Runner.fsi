namespace UniSpec

open System.Reflection
open System.Collections.Generic


/// <summary>测试状态
/// </summary>
/// <param name="Init">初始状态，尚未执行测试。</param>
/// <param name="Success">成功。</param>
/// <param name="Failed">失败。</param>
type internal State =
    | Init
    | Success
    | Failed

/// <summary>测试输出
/// <para>围绕场景开展行为测试，跟踪测试结果。</para>
/// </summary>
/// <param name="Feature">业务功能名称。</param>
/// <param name="LineNumber">行号。</param>
/// <param name="Scenario">场景名称。</param>
/// <param name="Tags">标签列表。</param>
/// <param name="Background">背景。</param>
/// <param name="Steps">步骤清单。</param>
/// <param name="Parameters">参数Map，场景大纲拆分产物。</param>
/// <param name="State">测试状态。</param>
type internal Output =
    { Feature: string
      LineNumber: int
      Scenario: string
      Tags: string list
      Background: Step list
      Steps: Step list
      Parameters: IDictionary<string, string> voption
      State: State }

/// <summary>Runner模块
/// <para>集中调度行为测试任务。</para>
/// </summary>
[<RequireQualifiedAccess>]
module internal Runner =

    /// <summary>Runner
    /// </summary>
    type T

    /// <summary>生成Runner
    /// </summary>
    /// <param name="features">待测试的Feature序列。</param>
    /// <param name="assembly">包含Step定义的程序集。</param>
    val generate : Feature list -> Assembly -> T

    /// <summary>执行测试
    /// </summary>
    /// <param name="runner">Runner。</param>
    /// <returns>测试执行结果。</returns>
    val run : T -> Output list

    /// <summary>按标签执行测试
    /// </summary>
    /// <param name="runner">Runner。</param>
    /// <param name="tags">标签列表。</param>
    /// <returns>测试执行结果。</returns>
    val runTags : T -> string list -> Output list