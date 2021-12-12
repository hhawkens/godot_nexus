using System;
using System.Collections.Generic;
using Gtk;

namespace App.Presentation.Gui;

public class SidebarContent : ListBox
{
	public SidebarContent(
		string name,
		IEnumerable<(SidebarEntry, EventHandler selectioCallback)> entries)
	{
		Name = name;
		foreach (var (entry, callback) in entries)
		{
			AddSidebarEntry(entry, callback);
		}
	}

	public void AddSidebarEntry(SidebarEntry entry, EventHandler selectionCallback)
	{
		RowSelected += (_, args) =>
		{
			if (args.Row == entry)
			{
				selectionCallback(this, EventArgs.Empty);
			}
		};
		Add(entry);
	}

	public override string ToString() => $"{nameof(SidebarContent)}-{Name}";
}
