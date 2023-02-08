namespace UniSpec

open System


[<AbstractClass>]
type HookAttribute() =
    inherit Attribute()

type BeforeAttribute() =
    inherit HookAttribute()

type AfterAttribute() =
    inherit HookAttribute()
