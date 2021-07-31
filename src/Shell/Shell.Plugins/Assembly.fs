module public App.Shell.Plugins.Assembly

open System.Runtime.CompilerServices

[<assembly: InternalsVisibleTo("App.Shell.Plugins.Tests")>]

do()
