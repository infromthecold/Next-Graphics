using NextGraphics.Exporting.Common;
using NextGraphics.Exporting.Exporters;
using NextGraphics.Exporting.Exporters.Base;
using NextGraphics.Exporting.Exporters.ZXNext;
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

		public Exporter(MainModel model, ExportParameters parameters)
		{
			this.Data = new ExportData(model, parameters);
		}

		#endregion

		#region Exporting

		public void Export()
		{
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
				// Note: assembler exporter must be last because it needs data produced by binary exporters.
				exporters.Add(new ZXNextBinaryTilesDataExporter());
				exporters.Add(new ZXNextBinaryTilesMapExporter());
				exporters.Add(new ZXNextBinaryTilesExporter());
				exporters.Add(new ZXNextBinaryPaletteExporter());
				exporters.Add(new ZXNextAssemblerExporter());
			}
			else if (Data.Model.BinaryOutput)
			{
				// Note: assembler exporter must be last because it needs data produced by binary exporters.
				exporters.Add(new ZXNextBinaryTilesDataExporter());
				exporters.Add(new ZXNextBinaryTilesMapExporter());
				exporters.Add(new ZXNextBinaryPaletteExporter());
				exporters.Add(new ZXNextAssemblerExporter());
			}
			else
			{
				exporters.Add(new ZXNextAssemblerExporter());
			}

			if (Data.Model.BlocksAsImage)
			{
				exporters.Add(new ZXNextBlocksAsImageExporter());
			}

			if (Data.Model.TilesAsImage)
			{
				exporters.Add(new ZXNextTilesAsImageExporter());
			}
		}

		private void RegisterSpriteExporters(List<BaseExporter> exporters)
		{
			if (Data.Model.BinaryOutput && Data.Model.BinaryBlocksOutput)
			{
				// Note: assembler exporter must be last because it needs data produced by binary exporters.
				exporters.Add(new ZXNextBinaryTilesDataExporter());
				exporters.Add(new ZXNextBinaryTilesExporter());
				exporters.Add(new ZXNextBinaryPaletteExporter());
				exporters.Add(new ZXNextAssemblerExporter());
			}
			else if (Data.Model.BinaryOutput)
			{
				// Note: assembler exporter must be last because it needs data produced by binary exporters.
				exporters.Add(new ZXNextBinaryTilesDataExporter());
				exporters.Add(new ZXNextBinaryPaletteExporter());
				exporters.Add(new ZXNextAssemblerExporter());
			}
			else
			{
				exporters.Add(new ZXNextAssemblerExporter());
			}

			// Note: sprites mode exports each block as its own image, but it's controller by tiles flag (not sure if this is a bug or feature, leaving it as such, it's really simple to change!
			if (Data.Model.TilesAsImage)
			{
				exporters.Add(new ZXNextSpritesBlocksAsImageExporter());
				exporters.Add(new ZXNextSpritesTilesAsImageExporter());
			}
		}

		#endregion

		#region Preparing data

		/// <summary>
		/// Prepares all the data needed for export by cutting and remaping images.
		/// </summary>
		/// <remarks>
		/// Note: for the moment being this need to be called manually before <see cref="Export"/>, but ideally it should be called automatically as part of export - an idea for the future improvement.
		/// </remarks>
		public void Remap()
		{
			new Remapper(Data).Remap();
		}

		#endregion
	}
}
