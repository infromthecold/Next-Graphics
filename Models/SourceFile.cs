using System;

namespace NextGraphics.Models
{
	/// <summary>
	/// The purpose of interface is to allow using it in lists and other places where mixed types can be used within single container such as `List`.
	/// </summary>
	public interface ISourceFile : IDisposable
	{
		string Filename { get; }

		bool IsDataValid { get; }

		void Reload();
	}

	public abstract class SourceFile<T> : ISourceFile
	{
		/// <summary>
		/// Parsed data from <see cref="Filename"/>, ready for consumption with the app. Null if not loaded yet.
		/// </summary>
		public T Data { get; private set; }

		#region ISourceFile
		
		/// <summary>
		/// File name and full path.
		/// </summary>
		public string Filename
		{
			get => _filename;
			set
			{
				if (value == _filename) return;
				_filename = value;
				Reload();
			}
		}
		private string _filename;

		/// <summary>
		/// A helper for checking if <see cref="Data"/> is valid (instead of writting Image != null...
		/// </summary>
		public bool IsDataValid { get => Data != null; }

		/// <summary>
		/// Disposes current data and then reloads it from assigned <see cref="Filename"/>. If filename is not assigned, only disposal occurs.
		/// </summary>
		public void Reload()
		{
			Dispose();

			if (Filename != null)
			{
				Data = OnLoadDataFromFile(Filename);
			}
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			OnDispose();
		}

		#endregion

		#region Initialization & Disposal

		public SourceFile(string filename, T data)
		{
			// In this case we should not assign to property since caller is also providing the data. Assigning to property triggers data loading.
			_filename = filename;

			// Data property doesn't use side-effects, so we can assign directly (we don't have explicit underlying field anyway).
			Data = data;
		}

		public SourceFile(string filename)
		{
			// Assigning filename to property will trigger data loading as well in the setter.
			Filename = filename;
		}

		public SourceFile()
		{
			// Default constructor for cases where we don't have filename ready from get go.
		}

		~SourceFile()
		{
			Dispose();
		}

		#endregion

		#region Subclass

		/// <summary>
		/// If subclass needs to parse the data, this is where it can implement it.
		/// </summary>
		protected virtual T OnLoadDataFromFile(string filename)
		{
			// We don't deal with any data by default, so we simply return default value.
			return default(T);
		}

		/// <summary>
		/// Called during disposal. Default implementation disposes <see cref="Data"/>, if it implements <see cref="IDisposable"/>. Subclass that needs to release additional resources can do it here.
		/// </summary>
		protected virtual void OnDispose()
		{
			// Dispose data if possible.
			if (Data is IDisposable disposable)
			{
				disposable.Dispose();
			}

			// Reset data to default value.
			Data = default(T);
		}

		#endregion
	}
}
