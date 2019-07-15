// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using SyncroSim.Core;
using System.Reflection;
using System.Globalization;

namespace SyncroSim.STSimStockFlow
{
	[ObfuscationAttribute(Exclude=true, ApplyToMembers=false)]
	internal class OutputFlowDataSheet : DataSheet
	{
		protected override void OnDataFeedsRefreshed()
		{
			base.OnDataFeedsRefreshed();

			string s = null;
			string ss = null;
			string ts = null;

			TerminologyUtilities.GetStratumLabelStrings(this.Project.GetDataSheet(Constants.DATASHEET_STSIM_TERMINOLOGY), ref s, ref ss, ref ts);

			this.Columns[Constants.FROM_STRATUM_ID_COLUMN_NAME].DisplayName = string.Format(CultureInfo.InvariantCulture, "From {0}", s);
			this.Columns[Constants.FROM_SECONDARY_STRATUM_ID_COLUMN_NAME].DisplayName = string.Format(CultureInfo.InvariantCulture, "From {0}", ss);
			this.Columns[Constants.FROM_TERTIARY_STRATUM_ID_COLUMN_NAME].DisplayName = string.Format(CultureInfo.InvariantCulture, "From {0}", ts);

			this.Columns[Constants.TO_STRATUM_ID_COLUMN_NAME].DisplayName = string.Format(CultureInfo.InvariantCulture, "To {0}", s);

			this.Columns[Constants.END_STRATUM_ID_COLUMN_NAME].DisplayName = string.Format(CultureInfo.InvariantCulture, "End {0}", s);
			this.Columns[Constants.END_SECONDARY_STRATUM_ID_COLUMN_NAME].DisplayName = string.Format(CultureInfo.InvariantCulture, "End {0}", ss);
			this.Columns[Constants.END_TERTIARY_STRATUM_ID_COLUMN_NAME].DisplayName = string.Format(CultureInfo.InvariantCulture, "End {0}", ts);
		}
	}
}
