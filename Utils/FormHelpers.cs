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

	public static class FormExtensions
	{
		/// <summary>
		/// Checks if a form of given type conforming the given conditions is already opened as MDI child of the given form. If so, it brings the form to front and returns the instance. Otherwise creates a new instance and returns it. If conditions closure is not provided, only type is checked
		/// </summary>
		/// <remarks>
		/// Note: only parameter-less constructors are supported!
		/// </remarks>
		public static T ShowOrCreateNewMdiChildInstance<T>(this Form mdiParent, Func<T, bool> conditions, Action<T> beforeShow = null) where T : Form
		{
			// If the form of given type is found in current children, and it conforms to given conditions (if any), activate it and return.
			foreach (Form form in mdiParent.MdiChildren)
			{
				if (form.GetType() == typeof(T))
				{
					if (conditions != null && !conditions(form as T))
					{
						continue;
					}

					form.BringToFront();

					return form as T;
				}
			}

			// Otherwise create a new instance as MDI child of the given parent.
			T result = Activator.CreateInstance(typeof(T)) as T;
			result.MdiParent = mdiParent;

			// If caller has supplied before show closure, call it.
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
