using App.Core.Domain;
using App.Shell.State;
using App.Utilities;

namespace App.Presentation.Frontend
{
	/// View model for the UI of the preferences panel.
	/// This abstraction interacts with the GUI layer.
	public interface IPreferencesFrontend : IDestructible
	{
		/// "General" config section.
		GeneralConfigContainerFrontend GeneralConfig { get; }

		/// "UI" config section.
		UiConfigContainerFrontend UiConfig { get; }
	}


	/// View model for the UI of the preferences panel.
	/// This abstraction interacts with the business layer.
	public interface IPreferencesFullstack : IPreferencesFrontend, IFrontend<IPreferencesStateController>
	{
		/// Indicates a change in the preferences model, so this object can update itself.
		void NotifyPreferencesChanged(Preferences prefs);
	}
}
