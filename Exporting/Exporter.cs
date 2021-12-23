using NextGraphics.Exporting.Common;
using NextGraphics.Exporting.Exporters;
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

			var exporters = new List<BaseExporter>();

			switch (Data.Model.OutputType)
			{
				case OutputType.Tiles:
					RegisterTilesExporters(exporters);
					break;

				case OutputType.Sprites:
					RegisterSpriteExporters(exporters);
					break;
			}

			// Run all exporters.
			foreach (var exporter in exporters)
			{
				exporter.Export(Data);
			}
		}

		private void RegisterTilesExporters(List<BaseExporter> exporters)
		{
			if (Data.Model.BinaryOutput && Data.Model.BinaryBlocksOutput)
			{
				// Note: Z80 exporter must be last because it needs data produced by binary exporters.
				exporters.Add(new BinaryTilesDataExporter());
				exporters.Add(new BinaryTilesMapExporter());
				exporters.Add(new BinaryTilesExporter());
				exporters.Add(new BinaryPaletteExporter());
				exporters.Add(new Z80NAssemblerExporter());
			}
			else if (Data.Model.BinaryOutput)
			{
				// Note: Z80 exporter must be last because it needs data produced by binary exporters.
				exporters.Add(new BinaryTilesDataExporter());
				exporters.Add(new BinaryTilesMapExporter());
				exporters.Add(new BinaryPaletteExporter());
				exporters.Add(new Z80NAssemblerExporter());
			}
			else
			{
				exporters.Add(new Z80NAssemblerExporter());
			}

			if (Data.Model.BlocksAsImage)
			{
				exporters.Add(new BlocksAsImageExporter());
			}

			if (Data.Model.TilesAsImage)
			{
				exporters.Add(new TilesAsImageExporter());
			}
		}

		private void RegisterSpriteExporters(List<BaseExporter> exporters)
		{

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
	}
}
