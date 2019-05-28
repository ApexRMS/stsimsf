﻿// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using SyncroSim.STSim;
using System.Collections.Generic;

namespace SyncroSim.STSimStockFlow
{
    class LateralFlowAmountRecord
    {
        public List<Cell> Cells = new List<Cell>();
        public int StockTypeId;
        public double StockAmount;
        public double InverseMultiplier;
    }
}
