//*********************************************************************************************
// STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
//
// Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
//
//*********************************************************************************************

using System;
using System.Data;
using System.Reflection;
using SyncroSim.Core;
using System.Globalization;
using System.Collections.Generic;

namespace SyncroSim.STSimStockFlow
{
	[ObfuscationAttribute(Exclude=true, ApplyToMembers=false)]
	internal class FlowPathwayDiagramDataSheet : DataSheet
	{
		public const string ERROR_INVALID_CELL_ADDRESS = "The value must be a valid cell address (a valid cell address is a letter from 'A' to 'Z' followed by a number from 1 to 255.  Example: 'A25')";

		public override void Validate(object proposedValue, string columnName)
		{
			base.Validate(proposedValue, columnName);

			if (columnName == Constants.LOCATION_COLUMN_NAME)
			{
				if (!IsValidLocation(proposedValue))
				{
					throw new DataException(ERROR_INVALID_CELL_ADDRESS);
				}
			}
		}

		public override string GetDeleteRowsConfirmation()
		{
			string m = base.GetDeleteRowsConfirmation();

			if (string.IsNullOrWhiteSpace(m))
			{
				m = "Associated flows will also be deleted.  Continue?";
			}

			return m;
		}

		protected override void OnRowsDeleted(object sender, SyncroSim.Core.DataSheetRowEventArgs e)
		{
			bool DeletedRows = false;
			Dictionary<int, bool> RemainingStockTypes = LookupKeyUtilities.CreateRecordLookup(this, Constants.STOCK_TYPE_ID_COLUMN_NAME);
			DataSheet FlowPathwaySheet = this.GetDataSheet(Constants.DATASHEET_FLOW_PATHWAY_NAME);
			DataTable FlowPathwayData = FlowPathwaySheet.GetData();

			for (int i = FlowPathwayData.Rows.Count - 1; i >= 0; i--)
			{
				DataRow dr = FlowPathwayData.Rows[i];

				if (dr.RowState == DataRowState.Deleted)
				{
					continue;
				}

				int FromStockTypeId = Convert.ToInt32(dr[Constants.FROM_STOCK_TYPE_ID_COLUMN_NAME]);
				int ToStockTypeId = Convert.ToInt32(dr[Constants.TO_STOCK_TYPE_ID_COLUMN_NAME]);

				if ((!RemainingStockTypes.ContainsKey(FromStockTypeId)) | (!RemainingStockTypes.ContainsKey(ToStockTypeId)))
				{
					DataTableUtilities.DeleteTableRow(FlowPathwayData, dr);
					DeletedRows = true;
				}
			}

			if (DeletedRows)
			{
				FlowPathwaySheet.Changes.Add(new ChangeRecord(this, "Diagram data deleted rows"));
			}

			base.OnRowsDeleted(sender, e);
		}

		private static bool IsValidLocation(object proposedLocation)
		{
			if (proposedLocation == null)
			{
				return false;
			}

			string Location = Convert.ToString(proposedLocation);

			if (string.IsNullOrEmpty(Location))
			{
				return false;
			}

			string LocUpper = Location.ToUpper(CultureInfo.InvariantCulture).Trim();

			if (string.IsNullOrEmpty(LocUpper))
			{
				return false;
			}

			if (LocUpper.Length < 2)
			{
				return false;
			}

			string CharPart = LocUpper.Substring(0, 1);
			string NumPart = LocUpper.Substring(1, LocUpper.Length - 1);

			if (string.IsNullOrEmpty(CharPart) | string.IsNullOrEmpty(NumPart))
			{
				return false;
			}

			if (CharPart[0] < 'A' || CharPart[0] > 'Z')
			{
				return false;
			}

			int n = 0;
			if (!int.TryParse(NumPart, out n))
			{
				return false;
			}

			if (n <= 0 || n > 256)
			{
				return false;
			}

			return true;
		}
	}
}