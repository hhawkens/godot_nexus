using System;
using App.Presentation.Frontend;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace App.Presentation.Gui
{
    public class MainWindow : Window
    {
#pragma warning disable 8625, 0414
        [UI("main_window")]
        private readonly Window windowRoot = null;
        [UI("header_content")]
        private readonly Stack headerContent = null;
        [UI("sidebar")]
        private readonly Stack sidebar = null;
        [UI("content")]
        private readonly Stack content = null;
        [UI("footer")]
        private readonly Box footer = null;
#pragma warning restore 8625, 0414

        // ReSharper disable once NotAccessedField.Local TODO remove
        private readonly IAppStateFrontend frontendState;
        private ThemeTone currentThemeTone;

        public MainWindow(IAppStateFrontend frontendState)
            : this(new Builder("MainWindow.glade"), frontendState)
        {
        }

        private MainWindow(Builder builder, IAppStateFrontend frontendState)
            : base(builder.GetObject("main_window").Handle)
        {
            this.frontendState = frontendState;

            GetDefaultSize(out var defaultWidth, out var defaultHeight);
            SetSizeRequest(defaultWidth, defaultHeight); // Sets minimum window size to default size
            builder.Autoconnect(this);

            LoadCustomCss();
            AddSidebarWidgets();
            AddHeaderWidgets();

            UpdatePresetTheme();

            DeleteEvent += delegate { Application.Quit(); };
        }

        private void AddSidebarWidgets()
        {
            var sidebarEntries = new (SidebarEntry, EventHandler)[]
            {
                (new SidebarEntry(
                    "Projects",
                    new IconInfo(IconType.Projects, currentThemeTone)),
                    delegate { Console.WriteLine("projects"); }),
                (new SidebarEntry(
                    "Engines",
                    new IconInfo(IconType.Engines, currentThemeTone)),
                    delegate { Console.WriteLine("engines"); }),
                (new SidebarEntry(
                    "Preferences",
                    new IconInfo(IconType.Preferences, currentThemeTone)),
                    delegate { Console.WriteLine("prefs"); }),
            };
            var sidebarContent = new SidebarContent("TestSidebar", sidebarEntries);
            sidebar.Add(sidebarContent);
            sidebar.ShowAll();
        }

        private void AddHeaderWidgets()
        {
            headerContent.Add(new HeaderContent("Godot Nexus"));
            headerContent.ShowAll();
        }

        private void LoadCustomCss()
        {
            var cssProvider = new CssProvider ();
            cssProvider.LoadFromPath("Resources/Styles/styles.css");
            StyleContext.AddProviderForScreen(Gdk.Screen.Default, cssProvider, StyleProviderPriority.User);
            currentThemeTone = sidebar.GetCurrentThemeTone();
        }

        private void UpdatePresetTheme() => ThemeTones.SetPresetThemeTone(content.GetCurrentThemeTone());
    }
}
