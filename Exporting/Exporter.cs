using NextGraphics.Exporting.Common;
using NextGraphics.Exporting.Remapping;
using NextGraphics.Models;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NextGraphics.Exporting
{
	/// <summary>
	/// Exporter is responsible for generating all export related data and managing export itself.
	/// 
	/// Though this is the entry point for generating export data (including intermediate data required for export), it's more of a coordinator for various underlying export classes where export actually happens.
	/// </summary>
	public class Exporter
	{
		public ExportData Data { get; private set; }

		#region Initialization & Disposal

		public Exporter(MainModel model)
		{
			this.Data = new ExportData(model);
		}

		#endregion

		#region Exporting

		public void Export(ExportParameters parameters)
		{
			Data.Parameters = parameters;

			Data.LabelName = Regex.Replace(Data.Model.Name, @"\s+", ""); // Strip all whitespace from model name
			Data.BlockLabelName = DetermineBlockLabelName();
		}

		#endregion

		#region Preparing data

		/// <summary>
		/// Prepares all the data needed for export by cutting and remaping images.
		/// </summary>
		/// <remarks>
		/// Note: for the moment being this need to be called manually before <see cref="Export"/>, but ideally it should be called automatically as part of export - an idea for the future improvement.
		/// </remarks>
		public void Remap(RemapCallbacks callbacks = null)
		{
			new Remapper(Data, callbacks).Remap();
		}

		#endregion

		#region Helpers

		private string DetermineBlockLabelName()
		{
			switch (Data.Model.OutputType)
			{
				case OutputType.Tiles: return "Tile";
				default: return "Sprite";
			}
		}

		#endregion
	}
}
