﻿// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System;
using System.Data;
using System.Globalization;

namespace SyncroSim.STSimStockFlow
{
	internal static class DataTableUtilities
	{
		public static void SetRowValue(DataRow dr, string columnName, object value)
		{
			if (object.ReferenceEquals(value, DBNull.Value) || object.ReferenceEquals(value, null))
			{
				dr[columnName] = DBNull.Value;
			}
			else
			{
				dr[columnName] = value;
			}
		}

		public static bool GetDataBool(object value)
		{
			if (object.ReferenceEquals(value, DBNull.Value))
			{
				return false;
			}
			else
			{
				return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
			}
		}

		public static bool GetDataBool(DataRow dr, string columnName)
		{
			return GetDataBool(dr[columnName]);
		}

		public static object GetNullableDatabaseValue(int? value)
		{
			if (value.HasValue)
			{
				return value.Value;
			}
			else
			{
				return DBNull.Value;
			}
		}

		public static void DeleteTableRow(DataTable dt, DataRow dr)
		{
			if (dr.RowState == DataRowState.Added)
			{
				dt.Rows.Remove(dr);
			}
			else
			{
				dr.Delete();
			}
		}

		public static object GetTableValue(DataTable table, string idColumnName, int idColumnValue, string valueColumnName)
		{
			foreach (DataRow dr in table.Rows)
			{
				if (dr.RowState == DataRowState.Deleted)
				{
					continue;
				}

				if (Convert.ToInt32(dr[idColumnName], CultureInfo.InvariantCulture) == idColumnValue)
				{
					return (dr[valueColumnName]);
				}
			}

			return DBNull.Value;
		}

		public static bool TableHasData(DataTable table, string columnName)
		{

			foreach (DataRow dr in table.Rows)
			{
				if (dr.RowState == DataRowState.Deleted)
				{
					continue;
				}

				object value = dr[columnName];

				if (!object.ReferenceEquals(value, DBNull.Value))
				{
					return true;
				}
			}

			return false;
		}

        public static string GetAutoGeneratedGroupName(string typeName)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} {1}", typeName, Constants.AUTO_COLUMN_SUFFIX);
        }

        public static FlowPathway CreateFlowPathway(DataRow dr)
        {
            int? Iteration = null;
            int? Timestep = null;
            int? FromStratumId = null;
            int? FromSecondaryStratumId = null;
            int? FromTertiaryStratumId = null;
            int? FromStateClassId = null;
            int? FromMinimumAge = null;
            int FromStockTypeId = 0;
            int? ToStratumId = null;
            int? ToStateClassId = null;
            int? ToMinimumAge = null;
            int ToStockTypeId = 0;
            int TransitionGroupId = 0;
            int? StateAttributeTypeId = null;
            int FlowTypeId = 0;
            double Multiplier = 0;
            int? TransferToStratumId = null;
            int? TransferToSecondaryStratumId = null;
            int? TransferToTertiaryStratumId = null;
            int? TransferToStateClassId = null;
            int? TransferToMinimumAge = null;

            if (dr[Constants.ITERATION_COLUMN_NAME] != DBNull.Value)
            {
                Iteration = Convert.ToInt32(dr[Constants.ITERATION_COLUMN_NAME], CultureInfo.InvariantCulture);
            }

            if (dr[Constants.TIMESTEP_COLUMN_NAME] != DBNull.Value)
            {
                Timestep = Convert.ToInt32(dr[Constants.TIMESTEP_COLUMN_NAME], CultureInfo.InvariantCulture);
            }

            if (dr[Constants.FROM_STRATUM_ID_COLUMN_NAME] != DBNull.Value)
            {
                FromStratumId = Convert.ToInt32(dr[Constants.FROM_STRATUM_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
            }

            if (dr[Constants.FROM_SECONDARY_STRATUM_ID_COLUMN_NAME] != DBNull.Value)
            {
                FromSecondaryStratumId = Convert.ToInt32(dr[Constants.FROM_SECONDARY_STRATUM_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
            }

            if (dr[Constants.FROM_TERTIARY_STRATUM_ID_COLUMN_NAME] != DBNull.Value)
            {
                FromTertiaryStratumId = Convert.ToInt32(dr[Constants.FROM_TERTIARY_STRATUM_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
            }

            if (dr[Constants.FROM_STATECLASS_ID_COLUMN_NAME] != DBNull.Value)
            {
                FromStateClassId = Convert.ToInt32(dr[Constants.FROM_STATECLASS_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
            }

            if (dr[Constants.FROM_MIN_AGE_COLUMN_NAME] != DBNull.Value)
            {
                FromMinimumAge = Convert.ToInt32(dr[Constants.FROM_MIN_AGE_COLUMN_NAME], CultureInfo.InvariantCulture);
            }

            FromStockTypeId = Convert.ToInt32(dr[Constants.FROM_STOCK_TYPE_ID_COLUMN_NAME], CultureInfo.InvariantCulture);

            if (dr[Constants.TO_STRATUM_ID_COLUMN_NAME] != DBNull.Value)
            {
                ToStratumId = Convert.ToInt32(dr[Constants.TO_STRATUM_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
            }

            if (dr[Constants.TO_STATECLASS_ID_COLUMN_NAME] != DBNull.Value)
            {
                ToStateClassId = Convert.ToInt32(dr[Constants.TO_STATECLASS_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
            }

            if (dr[Constants.TO_MIN_AGE_COLUMN_NAME] != DBNull.Value)
            {
                ToMinimumAge = Convert.ToInt32(dr[Constants.TO_MIN_AGE_COLUMN_NAME], CultureInfo.InvariantCulture);
            }

            ToStockTypeId = Convert.ToInt32(dr[Constants.TO_STOCK_TYPE_ID_COLUMN_NAME], CultureInfo.InvariantCulture);

            if (dr[Constants.TRANSITION_GROUP_ID_COLUMN_NAME] != DBNull.Value)
            {
                TransitionGroupId = Convert.ToInt32(dr[Constants.TRANSITION_GROUP_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
            }
            else
            {
                TransitionGroupId = 0;
            }

            if (dr[Constants.STATE_ATTRIBUTE_TYPE_ID_COLUMN_NAME] != DBNull.Value)
            {
                StateAttributeTypeId = Convert.ToInt32(dr[Constants.STATE_ATTRIBUTE_TYPE_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
            }

            FlowTypeId = Convert.ToInt32(dr[Constants.FLOW_TYPE_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
            Multiplier = Convert.ToDouble(dr[Constants.MULTIPLIER_COLUMN_NAME], CultureInfo.InvariantCulture);

            if (dr[Constants.TRANSFER_TO_STRATUM_ID_COLUMN_NAME] != DBNull.Value)
            {
                TransferToStratumId = Convert.ToInt32(dr[Constants.TRANSFER_TO_STRATUM_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
            }

            if (dr[Constants.TRANSFER_TO_SECONDARY_STRATUM_ID_COLUMN_NAME] != DBNull.Value)
            {
                TransferToSecondaryStratumId = Convert.ToInt32(dr[Constants.TRANSFER_TO_SECONDARY_STRATUM_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
            }

            if (dr[Constants.TRANSFER_TO_TERTIARY_STRATUM_ID_COLUMN_NAME] != DBNull.Value)
            {
                TransferToTertiaryStratumId = Convert.ToInt32(dr[Constants.TRANSFER_TO_TERTIARY_STRATUM_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
            }

            if (dr[Constants.TRANSFER_TO_STATECLASS_ID_COLUMN_NAME] != DBNull.Value)
            {
                TransferToStateClassId = Convert.ToInt32(dr[Constants.TRANSFER_TO_STATECLASS_ID_COLUMN_NAME], CultureInfo.InvariantCulture);
            }

            if (dr[Constants.TRANSFER_TO_MIN_AGE_COLUMN_NAME] != DBNull.Value)
            {
                TransferToMinimumAge = Convert.ToInt32(dr[Constants.TRANSFER_TO_MIN_AGE_COLUMN_NAME], CultureInfo.InvariantCulture);
            }

            FlowPathway p = new FlowPathway(
                Iteration, Timestep,
                FromStratumId, FromSecondaryStratumId, FromTertiaryStratumId, FromStateClassId, FromMinimumAge, FromStockTypeId,
                ToStratumId, ToStateClassId, ToMinimumAge, ToStockTypeId,
                TransitionGroupId, StateAttributeTypeId, FlowTypeId, Multiplier,
                TransferToStratumId, TransferToSecondaryStratumId, TransferToTertiaryStratumId, TransferToStateClassId, TransferToMinimumAge);

            return p;
        }
    }
}