using System;
using System.Collections.Generic;
using App.Utilities;

namespace App.Presentation.Frontend
{
	/// Represents the connection between a model and a frontend object.
	public class ModelBinding<TModel, TFrontend> : IDisposable
		where TModel: IPropertyChanged
		where TFrontend: IFrontend<TModel>
	{
		private readonly TModel model;
		private readonly TFrontend frontend;
		private readonly Action<IEnumerable<string>> modelChangedAction;
		private readonly SetOnce<bool> disposed = new(false);

		internal ModelBinding(
			TModel model,
			TFrontend frontend,
			Action<IEnumerable<string>> modelChangedAction)
		{
			this.model = model;
			this.frontend = frontend;
			this.modelChangedAction = modelChangedAction;

			this.model.PropertyChanged.AddHandler(ModelChangedHandler);
			this.frontend.ModelUpdateRequired += ModelUpdateRequiredHandler;
		}

		/// <inheritdoc />
		public void Dispose()
		{
			if (disposed.Value)
				return;

			disposed.Set(true);

			model.PropertyChanged.RemoveHandler(ModelChangedHandler);
			frontend.ModelUpdateRequired -= ModelUpdateRequiredHandler;

			(model as IDisposable)?.Dispose();
			frontend.Dispose();
		}

		private void ModelUpdateRequiredHandler(Action<TModel> modelUpdate) => modelUpdate.Invoke(model);
		private void ModelChangedHandler(dynamic _, IReadOnlyList<string> args) => modelChangedAction.Invoke(args);
	}


	/// Static helpers for <see cref="ModelBinding{TModel,TFrontend}"/>
	public static class ModelBinding
	{
		/// Helper to create a new binding object.
		public static ModelBinding<TModel, TFrontend> New<TModel, TFrontend>(
			TModel model,
			TFrontend frontend,
			Action<IEnumerable<string>> modelChangedAction)
			where TModel: IPropertyChanged
			where TFrontend: IFrontend<TModel>
		{
			return new(model, frontend, modelChangedAction);
		}
	}
}
