module public App.Shell.Addons.Assembly

open System.Runtime.CompilerServices

[<assembly: InternalsVisibleTo("App.Shell.Addons.Tests")>]

do()
