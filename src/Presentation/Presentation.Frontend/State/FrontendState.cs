using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace App.Presentation.Frontend
{
	internal class FrontendState : IFrontendState
	{
#pragma warning disable 0067
		public event PropertyChangedEventHandler? PropertyChanged;
		public event Action<FrontEndError>? ErrorOccurred;
#pragma warning restore 0067

#pragma warning disable 8603
		public ITopLevel TopLevel => null; // TODO restore warnings, implement properly
#pragma warning restore 8603

		public IReadOnlyList<Job<EngineInstalling>> EngineInstallingJobs => new List<Job<EngineInstalling>>();
	}
}
