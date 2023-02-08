namespace UniSpec

open System


[<AbstractClass; AttributeUsage(AttributeTargets.Method, Inherited = false)>]
type internal StepAttribute =
    inherit Attribute
    new: unit -> StepAttribute

[<AttributeUsage(AttributeTargets.Method, Inherited = false)>]
type GivenAttribute =
    inherit StepAttribute
    new: unit -> GivenAttribute

[<AttributeUsage(AttributeTargets.Method, Inherited = false)>]
type WhenAttribute =
    inherit StepAttribute
    new: unit -> WhenAttribute

[<AttributeUsage(AttributeTargets.Method, Inherited = false)>]
type ThenAttribute =
    inherit StepAttribute
    new: unit -> ThenAttribute
