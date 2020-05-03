namespace UniSpec

open System.Collections.Generic


/// <summary>数据表
/// </summary>
/// <param name="Header">表头。</param>
/// <param name="Body">数据体。</param>
type internal Table = { Header: string[]; Body: string[,] }


/// <summary>数据表模块
/// </summary>
[<RequireQualifiedAccess>]
module Table =

    /// <summary>从数据表提取参数字典
    /// </summary>
    /// <param name="table">数据表。</param>
    /// <returns>参数字典列表。</returns>
    val internal paras : Table -> IDictionary<string, string> list