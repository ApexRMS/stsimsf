// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System.Data;
using SyncroSim.Core;
using SyncroSim.STSim;
using System.Diagnostics;
using SyncroSim.StochasticTime;
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

        /// <summary>
        /// Gets the output flow dictionary
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// We must lazy-load this dictionary because this transformer runs before ST-Sim's
        /// and so the cell data is not there yet.
        /// </remarks>
        private Dictionary<int, double[]> GetOutputFlowDictionary()
        {
            if (this.m_SpatialOutputFlowDict == null)
            {
                this.m_SpatialOutputFlowDict = new Dictionary<int, double[]>();

                foreach (FlowType ft in this.m_FlowTypes)
                {
                    double[] flowVals = null;
                    flowVals = new double[this.STSimTransformer.InputRasters.NumberCells];
                    this.m_SpatialOutputFlowDict.Add(ft.Id, flowVals);
                }
            }

            return this.m_SpatialOutputFlowDict;
        }

        /// <summary>
        /// Get Stock Values for the specified Stock Type ID, placing then into the DblCells() in the specified raster.
        /// </summary>
        /// <param name="stockTypeId">The Stock Type ID that we want values for</param>
        /// <param name="rastStockType">An object of type ApexRaster, where we will write the Stock Type values. The raster should be initialized with metadata and appropriate array sizing.</param>
        /// <remarks></remarks>
        private void GetStockValues(int stockTypeId, StochasticTimeRaster rastStockType)
        {
            double AmountPerCell = this.m_STSimTransformer.AmountPerCell;

            foreach (Cell c in this.STSimTransformer.Cells)
            {
                Dictionary<int, double> StockAmounts = GetStockAmountDictionary(c);

                if (StockAmounts.Count > 0)
                {
                    rastStockType.DblCells[c.CellId] = (StockAmounts[stockTypeId] / AmountPerCell);
                }
                else
                {
                    //I wouldnt expect to get here because of Stratum/StateClass test above
                    Debug.Assert(false);
                }
            }
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