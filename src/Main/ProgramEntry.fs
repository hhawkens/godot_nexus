module public App.Main.ProgramEntry

open System
open Gtk

[<EntryPoint; STAThread>]
let entry _ =

    Application.Init();

    let app = new Application("org.godotengine.GodotNexus", GLib.ApplicationFlags.None)
    app.Register(GLib.Cancellable.Current) |> ignore

    let win = Installation.install()
    app.AddWindow(win)

    win.Show()
    Application.Run()

    0
