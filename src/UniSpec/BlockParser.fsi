namespace UniSpec


/// <summary>文本块转换器模块
/// <para>转换Ferture文档中的文本块。</para>
/// </summary>
[<RequireQualifiedAccess>]
module internal BlockParser =

    /// <summary>转换
    /// </summary>
    val parse: string seq -> Feature
