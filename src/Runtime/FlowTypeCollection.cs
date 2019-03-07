﻿// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

using System.Collections.ObjectModel;

namespace SyncroSim.STSimStockFlow
{
    internal class FlowTypeCollection : KeyedCollection<int, FlowType>
    {
        protected override int GetKeyForItem(FlowType item)
        {
            return item.Id;
        }
    }
}