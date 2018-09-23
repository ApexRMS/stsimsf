// STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
// Copyright © 2007-2018 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

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