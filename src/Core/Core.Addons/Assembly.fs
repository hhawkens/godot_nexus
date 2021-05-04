module public App.Core.Addons.Assembly

open System.Runtime.CompilerServices

[<assembly: InternalsVisibleTo("App.Core.Addons.Tests")>]

do()
