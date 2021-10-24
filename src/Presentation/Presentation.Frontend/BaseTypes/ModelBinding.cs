using System;
using App.Utilities;

namespace App.Presentation.Frontend
{
	/// Represents the connection between a model and a backend object.
	public class ModelBinding<TModel, TBackend> : IDisposable
		where TModel: IMutable
		where TBackend: IBackend<TModel>
	{
		private readonly TModel model;
		private readonly TBackend backend;
		private readonly SetOnce<bool> disposed = new(false);

		internal ModelBinding(TModel model, TBackend backend)
		{
			this.model = model;
			this.backend = backend;

			this.model.StateChanged.AddHandler(ModelChangedHandler);
			this.backend.ModelUpdateRequired += ModelUpdateRequiredHandler;

			ModelChangedHandler();
		}

		/// <inheritdoc />
		public void Dispose()
		{
			if (disposed.Value)
				return;

			disposed.Set(true);

			model.StateChanged.RemoveHandler(ModelChangedHandler);
			backend.ModelUpdateRequired -= ModelUpdateRequiredHandler;

			(model as IDisposable)?.Dispose();
			backend.Dispose();
		}

		private void ModelUpdateRequiredHandler(Action<TModel> modelUpdate) => modelUpdate.Invoke(model);
		private void ModelChangedHandler(dynamic? a = null, dynamic? b = null) => backend.NotifyModelUpdated(model);
	}


	/// Static helpers for <see cref="ModelBinding{TModel,TBackend}"/>
	public static class ModelBinding
	{
		/// Helper to create a new binding object.
		public static ModelBinding<TModel, TBackend> New<TModel, TBackend>(TModel model, TBackend backend)
			where TModel: IMutable
			where TBackend: IBackend<TModel>
		{
			return new(model, backend);
		}
	}
}
