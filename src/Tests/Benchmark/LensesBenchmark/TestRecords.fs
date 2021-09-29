namespace App.Benchmark

[<Struct>]
type public TestTheme =
    | System = 0
    | Light = 1
    | Dark = 2

type public TestPreferences = {
    General: TestGeneralPreferences
    UI: TestUiPreferences
}

and public TestGeneralPreferences = {
    EnginesPath: string TestConfigData
    ProjectsPath: (string * string) TestConfigData
}

and public TestUiPreferences = {
    Theme: TestTheme TestConfigData
}

and public TestConfigData<'a when 'a: equality> = {
    Description: string
    DefaultValue: 'a
    CurrentValue: 'a
}
