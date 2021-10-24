using App.Shell.State;

namespace App.Presentation.Frontend
{
	/// View model for the UI of the preferences panel.
	/// This abstraction interacts with the GUI layer.
	public interface IPreferencesFrontend : IFrontend
	{
		/// "General" config section.
		GeneralConfigContainerFrontend GeneralConfig { get; }

		/// "UI" config section.
		UiConfigContainerFrontend UiConfig { get; }
	}


	/// View model for the UI of the preferences panel.
	/// This abstraction can also interact with the business layer.
	public interface IPreferencesFullstack : IPreferencesFrontend, IBackend<IPreferencesStateController>
	{
	}
}
