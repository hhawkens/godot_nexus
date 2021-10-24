using System;
using App.Utilities;

namespace App.Presentation.Frontend
{
	/// Represents the connection between a model and a frontend object.
	public class ModelBinding<TModel, TFrontend> : IDisposable
		where TModel: IMutable
		where TFrontend: IFrontend<TModel>
	{
		private readonly TModel model;
		private readonly TFrontend frontend;
		private readonly SetOnce<bool> disposed = new(false);

		internal ModelBinding(TModel model, TFrontend frontend)
		{
			this.model = model;
			this.frontend = frontend;

			this.model.StateChanged.AddHandler(ModelChangedHandler);
			this.frontend.ModelUpdateRequired += ModelUpdateRequiredHandler;

			ModelChangedHandler();
		}

		/// <inheritdoc />
		public void Dispose()
		{
			if (disposed.Value)
				return;

			disposed.Set(true);

			model.StateChanged.RemoveHandler(ModelChangedHandler);
			frontend.ModelUpdateRequired -= ModelUpdateRequiredHandler;

			(model as IDisposable)?.Dispose();
			frontend.Dispose();
		}

		private void ModelUpdateRequiredHandler(Action<TModel> modelUpdate) => modelUpdate.Invoke(model);
		private void ModelChangedHandler(dynamic? a = null, dynamic? b = null) => frontend.NotifyModelUpdated(model);
	}


	/// Static helpers for <see cref="ModelBinding{TModel,TFrontend}"/>
	public static class ModelBinding
	{
		/// Helper to create a new binding object.
		public static ModelBinding<TModel, TFrontend> New<TModel, TFrontend>(TModel model, TFrontend frontend)
			where TModel: IMutable
			where TFrontend: IFrontend<TModel>
		{
			return new(model, frontend);
		}
	}
}
