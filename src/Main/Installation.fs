module internal App.Main.Installation

open App.Presentation.Frontend
open App.Presentation.Gui

let internal install () =
    let frontend = {new IAppStateFrontend}
    let win = new MainWindow(frontend)
    win
