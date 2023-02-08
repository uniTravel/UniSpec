namespace UniSpec

open System.Reflection


/// <summary>执行测试的委托
/// </summary>
type internal Action = delegate of unit -> unit

/// <summary>任务模板Key
/// </summary>
/// <param name="Feature">业务功能名称。</param>
/// <param name="LineNumber">行号。</param>
/// <param name="Scenario">场景名称。</param>
type TemplateKey =
    { Feature: string
      LineNumber: int
      Scenario: string }

/// <summary>测试任务
/// <para>围绕场景开展行为测试。</para>
/// </summary>
/// <param name="Key">任务模板Key。</param>
/// <param name="Tags">标签列表。</param>
/// <param name="Action">执行测试的委托。</param>
type Todo =
    { Key: TemplateKey
      Tags: string list
      Action: Action }

/// <summary>UniSpec模块
/// </summary>
[<Sealed>]
type UniSpec =

    /// <summary>构造函数
    /// </summary>
    /// <param name="path">包含Feature文件的根目录名称。</param>
    /// <param name="assembly">包含Step定义的程序集。</param>
    new: path: string * assembly: Assembly -> UniSpec

    /// <summary>按Feature文件获取任务清单
    /// </summary>
    /// <param name="filename">Feature文件的文件名。</param>
    /// <returns>测试任务清单。</returns>
    member Get: filename: string -> string * Todo list

    /// <summary>按Feature文件与标签获取任务清单
    /// </summary>
    /// <param name="filename">Feature文件的文件名。</param>
    /// <param name="tags">标签列表。</param>
    /// <returns>测试任务清单。</returns>
    member GetTags: filename: string -> tags: string list -> string * Todo list
