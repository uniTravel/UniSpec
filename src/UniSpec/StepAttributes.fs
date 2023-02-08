namespace UniSpec

open System


[<AbstractClass>]
type StepAttribute() =
    inherit Attribute()

type GivenAttribute() =
    inherit StepAttribute()

type WhenAttribute() =
    inherit StepAttribute()

type ThenAttribute() =
    inherit StepAttribute()
