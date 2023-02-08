namespace UniSpec


type Category =
    | Given
    | When
    | Then

type Argument =
    | Table of Table
    | Doc of string
    | Empty

type Step =
    { Name: string
      Category: Category
      Argument: Argument }

type Examples = { Tags: string list; Table: Table }

type Scenario =
    { Name: string
      LineNumber: int
      Tags: string list
      Steps: Step list }

type Outline =
    { Name: string
      LineNumber: int
      Tags: string list
      Steps: Step list
      Examples: Examples list }

type Feature =
    { Name: string
      Tags: string list
      Background: Step list
      Scenarios: Scenario list
      Outlines: Outline list }
