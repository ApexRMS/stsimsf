// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System.Windows.Forms;

namespace SyncroSim.STSimStockFlow
{
	internal static class RasterUtilities
	{
		public static string ChooseRasterFileName(string dialogTitle)
		{
			OpenFileDialog dlg = new OpenFileDialog();

			dlg.Title = dialogTitle;
			dlg.Filter = "GeoTIFF File (*.tif)|*.tif";

			if (dlg.ShowDialog() != DialogResult.OK)
			{
				return null;
			}

			return dlg.FileName;
		}
	}
}