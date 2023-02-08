namespace UniSpec

open System


[<AbstractClass; AttributeUsage(AttributeTargets.Method, Inherited = false)>]
type internal HookAttribute =
    inherit Attribute
    new: unit -> HookAttribute

[<AttributeUsage(AttributeTargets.Method, Inherited = false)>]
type BeforeAttribute =
    inherit HookAttribute
    new: unit -> BeforeAttribute

[<AttributeUsage(AttributeTargets.Method, Inherited = false)>]
type AfterAttribute =
    inherit HookAttribute
    new: unit -> AfterAttribute
