namespace UniSpec


type Action = delegate of unit -> unit

type Scenario =
    { Name: string
      Description: string
      Action: Action
      Parameters: (string * string)[]
      Tags: string[] }

type Feature =
    { Name : string
      Source : string
      Assembly : System.Reflection.Assembly
      Scenarios : Scenario seq }