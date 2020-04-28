namespace UniSpec


type Table = { Header: string[]; Body: string[,] }

type Category =
    | Given of string
    | When of string
    | Then of string

type Argument =
    | Table of Table
    | Doc of string
    | Empty

type Step = { LineNumber: int; Category: Category; Argument: Argument }

type Examples = { Tags: string list; Table: Table }

type State =
    | Init
    | Success
    | Failed

type Scenario =
    { Name: string
      LineNumber: int
      Tags: string list
      Steps: Step list
      State: State }

type Outline =
    { Name: string
      LineNumber: int
      Tags: string list
      Steps: Step list
      Examples: Examples list
      Scenarios: Scenario list }

type Feature =
    { Name: string
      Tags: string list
      Background: Step list
      Scenarios: Scenario list
      Outlines: Outline list }