namespace UniSpec

open System.Collections.Generic


/// <summary>数据表
/// </summary>
/// <param name="Header">表头。</param>
/// <param name="Body">数据体。</param>
type Table = { Header: string[]; Body: string[,] }


/// <summary>数据表模块
/// </summary>
[<RequireQualifiedAccess>]
module Table =

    /// <summary>从文本行生成数据表
    /// </summary>
    /// <param name="head">表头行。</param>
    /// <param name="rows">数据行列表。</param>
    val internal fromLines: string list -> string list list -> Table

    /// <summary>从数据表提取参数字典
    /// </summary>
    /// <param name="table">数据表。</param>
    /// <returns>参数字典列表。</returns>
    val internal toArgs: Table -> IDictionary<string, string> list

    /// <summary>转换数据表
    /// </summary>
    /// <param name="table">数据表。</param>
    /// <returns>数据表的元组列表形式。</returns>
    val toList<'t> : Table -> 't list
