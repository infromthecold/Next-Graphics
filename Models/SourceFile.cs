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

		/// <summary>
		/// Determines whether data should be auto-loaded or not. This affects loading when assigning <see cref="Filename"/> and <see cref="Reload"/>, if the flag is false, then nothing will happen in there.
		/// </summary>
		public bool AutoLoad { get; set; } = true;

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
				if (AutoLoad)
				{
					Reload();
				}
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
			if (AutoLoad)
			{
				Dispose();

				if (Filename != null)
				{
					Data = OnLoadDataFromFile(Filename);
				}
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
			// When manually assigning data, we set auto load to false by default - we don't want subsequent reloads destroying the data. This is mostly used for unit testing where we use mock data which cannot be reloaded.
			AutoLoad = false;

			Filename = filename;
			Data = data;
		}

		public SourceFile(string filename, bool autoLoad = true)
		{
			// Assigning filename to property will trigger data loading as well in the setter.
			AutoLoad = autoLoad;
			Filename = filename;
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
			return default;
		}

		/// <summary>
		/// Called during disposal. Default implementation disposes <see cref="Data"/>, if it's <see cref="IDisposable"/>. This should be sufficient for most use cases, but subclass that needs to dispose additional resources can override and handle it manually. It's important to call super implementation in such case to ensure disposal of underlying data.
		/// </summary>
		protected virtual void OnDispose()
		{
			// Dispose data if possible.
			if (Data is IDisposable disposable)
			{
				disposable.Dispose();
			}

			// Reset data to default value.
			Data = default;
		}

		#endregion
	}
}
