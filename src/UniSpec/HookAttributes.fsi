namespace UniSpec

open System


[<AbstractClass; AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)>]
type internal HookAttribute =
    inherit Attribute
    new : unit -> HookAttribute

[<AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)>]
type BeforeAttribute =
    inherit HookAttribute
    new : unit -> BeforeAttribute

[<AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)>]
type AfterAttribute =
    inherit HookAttribute
    new : unit -> AfterAttribute