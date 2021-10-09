using System;
using System.Collections.Generic;
using System.IO;
using App.Core.Domain;
using App.Shell.State;
using App.TestHelpers;
using Microsoft.FSharp.Control;
using Microsoft.FSharp.Core;
using NUnit.Framework;

namespace App.Presentation.Frontend.Tests
{
	[TestFixture]
	public class PreferencesFrontendTests
	{
#pragma warning disable CS8618
		private PreferencesFrontend sut;
		private PreferencesStateControllerMock controller;
		private List<Error> thrownErrors;
#pragma warning restore CS8618

		[SetUp]
		public void SetUp()
		{
			controller = new PreferencesStateControllerMock { Preferences = TestConfig.preferences };
			thrownErrors = new();
			sut = new(controller, thrownErrors.Add);
		}

		[Test]
		public void Controller_Event_Is_Propagated_To_Configs()
		{
			const string newPath = nameof(newPath);
			controller.Preferences = TestConfig.withEnginePath(newPath, controller.Preferences);
			controller.InvokePrefsChanged();
			Assert.That(sut.GeneralConfig.EnginesPathConfig.Value.Name, Is.EqualTo(newPath));
		}

		[Test]
		public void Is_Not_Bound_To_Event_After_Dispose()
		{
			var oldPath = sut.GeneralConfig.EnginesPathConfig.Value.Name;
			const string newPath = nameof(newPath);
			Assert.That(oldPath, Is.Not.EqualTo(newPath));

			sut.Dispose();
			controller.Preferences = TestConfig.withEnginePath(newPath, controller.Preferences);
			controller.InvokePrefsChanged();
			Assert.That(sut.GeneralConfig.EnginesPathConfig.Value.Name, Is.EqualTo(oldPath));
		}

		[Test]
		public void Controller_Error_Is_Thrown()
		{
			// The state controller mock has been prepared to throw an error when setting engines path
			sut.GeneralConfig.EnginesPathConfig.SetValue(new DirectoryInfo("some/path"));

			Assert.That(thrownErrors.Count, Is.EqualTo(1));
			Assert.That(thrownErrors[0].Message, Is.EqualTo("Engines Path Error!"));
		}
	}


	public class PreferencesStateControllerMock : IPreferencesStateController
	{
		public Preferences Preferences { get; set; } = new(default, default);
		public IEvent<FSharpHandler<Preferences>, Preferences> PreferencesChanged => prefsChanged.Publish;

		private readonly FSharpEvent<Preferences> prefsChanged = new();

		public FSharpResult<Unit, string> SetEnginesPathConfig(string _) =>
			FSharpResult<Unit, string>.NewError("Engines Path Error!");

		public FSharpResult<Unit, string> SetProjectsPathConfig(string _) => throw new NotImplementedException();
		public FSharpResult<Unit, string> SetThemeConfig(Theme _) => throw new NotImplementedException();

		internal void InvokePrefsChanged() => prefsChanged.Trigger(Preferences);
	}
}
