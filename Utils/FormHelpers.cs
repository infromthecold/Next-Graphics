using System;
using System.Windows.Forms;

namespace NextGraphics.Utils
{
	public class FormHelpers
	{
		/// <summary>
		/// Checks if a form of given type is already opened. If so, activates it and returns the instance. Otherwise creates a new instance and returns it.
		/// </summary>
		/// <remarks>
		/// Note: only parameter-less constructors are supported!
		/// </remarks>
		public static T ShowOrCreateNewModelessInstance<T>(Action<T> beforeShow = null) where T : Form
		{
			// If the form of given type is found in currently open forms, activate it and return.
			foreach (Form form in Application.OpenForms)
			{
				if (form.GetType () == typeof(T))
				{
					form.Activate();

					return form as T;
				}
			}

			// Otherwise create a new instance. If caller has supplied before show closure, call it.
			T result = Activator.CreateInstance(typeof(T)) as T;
			if (beforeShow != null)
			{
				beforeShow(result);
			}

			// Show the form and return it as result.
			result.Show();
			return result;
		}
	}
}
