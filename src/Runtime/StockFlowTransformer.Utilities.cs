// STSimStockFlow: A SyncroSim Package for the ST-Sim Stocks and Flows Add-In.
// Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

using System.Data;
using SyncroSim.Core;
using SyncroSim.STSim;
using System.Collections.Generic;

namespace SyncroSim.STSimStockFlow
{
	internal partial class StockFlowTransformer
	{  
		private const string STOCK_AMOUNT_KEY = "stockamountkey";

		/// <summary>
		/// Gets the ST-Sim transformer
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		private STSimTransformer GetSTSimTransformer()
		{
			foreach (Transformer t in this.Transformers)
			{
				if (t.Name == "stsim:runtime")
				{
					return (STSim.STSimTransformer)t;
				}
			}

			ExceptionUtils.ThrowInvalidOperationException("ST-Sim Transformer not found.  Fatal error!");
			return null;
		}

		/// <summary>
		/// Gets the stock amount dictionary for the specified cell
		/// </summary>
		/// <param name="cell"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		private static Dictionary<int, double> GetStockAmountDictionary(Cell cell)
		{
			Dictionary<int, double> StockAmounts = (Dictionary<int, double>)cell.GetAssociatedObject(STOCK_AMOUNT_KEY);

			if (StockAmounts == null)
			{
				StockAmounts = new Dictionary<int, double>();
				cell.SetAssociatedObject(STOCK_AMOUNT_KEY, StockAmounts);
			}

			return StockAmounts;
		}

		private double GetAttributeValue(
            int stateAttributeTypeId, int stratumId, int? secondaryStratumId, int? tertiaryStratumId, 
            int stateClassId, int iteration, int timestep, int age)
		{
			double val = 0.0;

			double? v = this.STSimTransformer.GetAttributeValueByAge(
                stateAttributeTypeId, stratumId, secondaryStratumId, tertiaryStratumId, 
                stateClassId, iteration, timestep, age);

			if (!v.HasValue)
			{
				v = this.STSimTransformer.GetAttributeValueNoAge(
                    stateAttributeTypeId, stratumId, secondaryStratumId, tertiaryStratumId, 
                    stateClassId, iteration, timestep);
			}

			if (v.HasValue)
			{
				val = v.Value;
			}

			return val;
		}

		protected bool AnyOutputOptionsSelected()
		{
			DataRow dr = this.ResultScenario.GetDataSheet(Constants.DATASHEET_OO_NAME).GetDataRow();

			if (dr == null)
			{
				return false;
			}

			if (DataTableUtilities.GetDataBool(
                dr, Constants.DATASHEET_OO_SUMMARY_OUTPUT_ST_COLUMN_NAME) || 
                DataTableUtilities.GetDataBool(dr, Constants.DATASHEET_OO_SUMMARY_OUTPUT_FL_COLUMN_NAME) || 
                DataTableUtilities.GetDataBool(dr, Constants.DATASHEET_OO_SPATIAL_OUTPUT_ST_COLUMN_NAME) ||
                DataTableUtilities.GetDataBool(dr, Constants.DATASHEET_OO_SPATIAL_OUTPUT_FL_COLUMN_NAME))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Determines if it is possible to compute stocks and flows
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// (1.) If none of the stock-flow settings are provided then the user is not using the stock-flow feature.
		/// (2.) If the flow pathways are missing then we can't compute stocks and flows.  However, log a message if any of the other settings are specified.
		/// (3.) If flow pathways exist but no initial stocks are specified, log a warning that all stocks will be initialized to zero.
		/// </remarks>
		protected bool CanComputeStocksAndFlows()
		{
			bool OutputOptionsExist = this.AnyOutputOptionsSelected();
			bool ICSpatialRecordsExist = (this.ResultScenario.GetDataSheet(Constants.DATASHEET_INITIAL_STOCK_SPATIAL).GetData().Rows.Count > 0);
			bool ICNonSpatialRecordsExist = (this.ResultScenario.GetDataSheet(Constants.DATASHEET_INITIAL_STOCK_NON_SPATIAL).GetData().Rows.Count > 0);

			if (!OutputOptionsExist && !ICSpatialRecordsExist && !ICNonSpatialRecordsExist)
			{
				return false;
			}

			if (!ICSpatialRecordsExist && !ICNonSpatialRecordsExist)
			{
				this.RecordStatus(StatusType.Information, "No initial stocks have been specified.  All stocks will be initialized to zero.");
			}

			return true;
		}
	}
}