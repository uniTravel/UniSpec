namespace UniSpec

open System
open System.Collections.Generic


/// <summary>数据表
/// </summary>
/// <param name="Header">表头。</param>
/// <param name="Body">数据体。</param>
type internal Table = { Header: string[]; Body: string[,] }


/// <summary>数据表模块
/// </summary>
[<RequireQualifiedAccess>]
module internal Table =

    /// <summary>从数据表提取参数字典
    /// </summary>
    /// <param name="table">数据表。</param>
    /// <returns>参数字典列表。</returns>
    val args : Table -> IDictionary<string, string> list

    /// <summary>转换数据表
    /// </summary>
    /// <param name="table">数据表。</param>
    /// <param name="typ">目标类型，应为数据表相应元组的链表。</param>
    /// <returns>数据表的元组链表形式的对象。</returns>
    val parse : Table -> Type -> obj