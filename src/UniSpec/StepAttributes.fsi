namespace UniSpec

open System


[<AbstractClass; AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)>]
type internal StepAttribute =
    inherit Attribute
    new : unit -> StepAttribute

[<AttributeUsage(AttributeTargets.Method, AllowMultiple = true)>]
type GivenAttribute =
    inherit StepAttribute
    new : unit -> GivenAttribute

[<AttributeUsage(AttributeTargets.Method, AllowMultiple = true)>]
type WhenAttribute =
    inherit StepAttribute
    new : unit -> WhenAttribute

[<AttributeUsage(AttributeTargets.Method, AllowMultiple = true)>]
type ThenAttribute =
    inherit StepAttribute
    new : unit -> ThenAttribute