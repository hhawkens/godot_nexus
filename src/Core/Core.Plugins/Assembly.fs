module public App.Core.Plugins.Assembly

open System.Runtime.CompilerServices

[<assembly: InternalsVisibleTo("App.Core.Plugins.Tests")>]

do()
