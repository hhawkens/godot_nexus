using System;
using System.Collections.Generic;
using System.IO;
using App.Core.Domain;
using App.Shell.State;
using App.TestHelpers;
using Microsoft.FSharp.Core;
using Moq;
using NUnit.Framework;

namespace App.Presentation.Frontend.Tests
{
	[TestFixture]
	public class PreferencesFullstackTests
	{
#pragma warning disable CS8618
		private readonly Preferences defaultPrefs = TestConfig.preferences;
		private List<Error> thrownErrors;
		private PreferencesFullstack sut;
#pragma warning restore CS8618

		[SetUp]
		public void SetUp()
		{
			thrownErrors = new();
			sut = new(defaultPrefs, thrownErrors.Add);
		}

		[Test]
		public void Preferences_Change_Is_Notified()
		{
			const string newPath = nameof(newPath);
			var newPrefs = TestConfig.withEnginePath(newPath, defaultPrefs);
			sut.NotifyPreferencesChanged(newPrefs);
			Assert.That(sut.GeneralConfig.EnginesPathConfig.Value.Name, Is.EqualTo(newPath));
		}

		[Test]
		public void Controller_Error_Is_Thrown()
		{
			sut.ModelUpdateRequired += delegate(Action<IPreferencesStateController> action)
			{
				var prefsController = Mock.Of<IPreferencesStateController>();
				Mock.Get(prefsController)
					.Setup(x => x.SetEnginesPathConfig(It.IsAny<string>()))
					.Returns(FSharpResult<Unit, string>.NewError("Something went very very wrong!"));
				action(prefsController);
			};

			sut.GeneralConfig.EnginesPathConfig.SetValue(new DirectoryInfo("path/to/exile"));

			Assert.That(thrownErrors.Count, Is.EqualTo(1));
			Assert.That(thrownErrors[0].Message, Is.EqualTo("Something went very very wrong!"));
		}
	}
}
