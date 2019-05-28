// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System;
using System.Data;
using System.Linq;
using SyncroSim.Core;
using SyncroSim.STSim;
using System.Globalization;
using System.Collections.Generic;

namespace SyncroSim.STSimStockFlow
{
	internal partial class StockFlowTransformer
	{
		/// <summary>
		/// Normalizes the output options data feed
		/// </summary>
		/// <remarks></remarks>
		private void NormalizeOutputOptions()
		{
			DataRow drrc = this.ResultScenario.GetDataSheet("STSim_RunControl").GetDataRow();
			int MaxTimestep = Convert.ToInt32(drrc["MaximumTimestep"], CultureInfo.InvariantCulture);
			DataSheet dsoo = this.ResultScenario.GetDataSheet(Constants.DATASHEET_OO_NAME);
			DataRow droo = dsoo.GetDataRow();

			if (droo == null)
			{
				droo = dsoo.GetData().NewRow();
				dsoo.GetData().Rows.Add(droo);
			}

			if (!AnyOutputOptionsSelected())
			{
				DataTableUtilities.SetRowValue(droo, Constants.DATASHEET_OO_SUMMARY_OUTPUT_ST_COLUMN_NAME, Booleans.BoolToInt(true));
				DataTableUtilities.SetRowValue(droo, Constants.DATASHEET_OO_SUMMARY_OUTPUT_FL_COLUMN_NAME, Booleans.BoolToInt(true));
				DataTableUtilities.SetRowValue(droo, Constants.DATASHEET_OO_SUMMARY_OUTPUT_ST_TIMESTEPS_COLUMN_NAME, 1);
				DataTableUtilities.SetRowValue(droo, Constants.DATASHEET_OO_SUMMARY_OUTPUT_FL_TIMESTEPS_COLUMN_NAME, 1);

				this.RecordStatus(StatusType.Information, Constants.NO_SUMMARY_OUTPUT_OPTIONS_INFORMATION);
			}

			this.ValidateTimesteps(droo, Constants.DATASHEET_OO_SUMMARY_OUTPUT_ST_COLUMN_NAME, Constants.DATASHEET_OO_SUMMARY_OUTPUT_ST_TIMESTEPS_COLUMN_NAME, "Summary stocks", MaxTimestep);
			this.ValidateTimesteps(droo, Constants.DATASHEET_OO_SUMMARY_OUTPUT_FL_COLUMN_NAME, Constants.DATASHEET_OO_SUMMARY_OUTPUT_FL_TIMESTEPS_COLUMN_NAME, "Summary flows", MaxTimestep);
			this.ValidateTimesteps(droo, Constants.DATASHEET_OO_SPATIAL_OUTPUT_ST_COLUMN_NAME, Constants.DATASHEET_OO_SPATIAL_OUTPUT_ST_TIMESTEPS_COLUMN_NAME, "Spatial stocks", MaxTimestep);
			this.ValidateTimesteps(droo, Constants.DATASHEET_OO_SPATIAL_OUTPUT_FL_COLUMN_NAME, Constants.DATASHEET_OO_SPATIAL_OUTPUT_FL_TIMESTEPS_COLUMN_NAME, "Spatial flows", MaxTimestep);
			this.ValidateTimesteps(droo, Constants.DATASHEET_OO_LATERAL_OUTPUT_FL_COLUMN_NAME, Constants.DATASHEET_OO_LATERAL_OUTPUT_FL_TIMESTEPS_COLUMN_NAME, "Lateral flows", MaxTimestep);
        }

		/// <summary>
		/// Validates the timesteps for the specified column name and maximum timestep
		/// </summary>
		/// <param name="dr"></param>
		/// <param name="optionColumnName"></param>
		/// <param name="timestepsColumnName"></param>
		/// <param name="timestepsColumnHeaderText"></param>
		/// <param name="maxTimestep"></param>
		/// <remarks></remarks>
		private void ValidateTimesteps(DataRow dr, string optionColumnName, string timestepsColumnName, string timestepsColumnHeaderText, int maxTimestep)
		{
			if (dr[optionColumnName] == DBNull.Value)
			{
				return;
			}

			if (!Convert.ToBoolean(dr[optionColumnName], CultureInfo.InvariantCulture))
			{
				return;
			}

			if (dr[timestepsColumnName] == DBNull.Value)
			{
				string message = string.Format(CultureInfo.InvariantCulture, 
                    "Stocks and Flows Timestep value for '{0}' is invalid.  Using default.", 
                    timestepsColumnHeaderText);

				this.RecordStatus(StatusType.Warning, message);
				dr[timestepsColumnName] = 1;

				return;
			}

			int val = Convert.ToInt32(dr[timestepsColumnName], CultureInfo.InvariantCulture);

			if (val > maxTimestep)
			{
				string message = string.Format(CultureInfo.InvariantCulture, 
                    "Stocks and Flows Timestep value for '{0}' out of range.  Using default.", 
                    timestepsColumnHeaderText);

				this.RecordStatus(StatusType.Warning, message);
				dr[timestepsColumnName] = maxTimestep;

				return;
			}
		}

		private void NormalizeForUserDistributions()
		{
			if (this.m_STSimTransformer.DistributionProvider.Values.Count > 0)
			{
				STSimDistributionBaseExpander Expander = new STSimDistributionBaseExpander(this.m_STSimTransformer.DistributionProvider);
				this.ExpandFlowMultipliers(Expander);
			}
		}

		private void ExpandFlowMultipliers(STSimDistributionBaseExpander expander)
		{
			if (this.m_FlowMultipliers.Count > 0)
			{
				IEnumerable<STSimDistributionBase> NewItems = expander.Expand(this.m_FlowMultipliers);
				this.m_FlowMultipliers.Clear();

				foreach (FlowMultiplier t in NewItems)
				{
					this.m_FlowMultipliers.Add(t);
				}
			}
		}
	}
}