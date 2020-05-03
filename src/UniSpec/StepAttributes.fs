namespace UniSpec

open System


type SpecAttribute() =
    inherit Attribute()

[<AbstractClass>]
type StepAttribute() =
    inherit Attribute()

type GivenAttribute() =
    inherit StepAttribute()

type WhenAttribute() =
    inherit StepAttribute()

type ThenAttribute() =
    inherit StepAttribute()