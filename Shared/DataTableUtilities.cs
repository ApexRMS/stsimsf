// STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
// Copyright © 2007-2018 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

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
	}
}