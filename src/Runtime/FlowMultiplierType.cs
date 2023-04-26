﻿// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2023 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System;
using SyncroSim.Core;
using SyncroSim.STSim;
using System.Diagnostics;

namespace SyncroSim.STSimStockFlow
{
    class FlowMultiplierType
    {
        private Scenario m_Scenario;
        private STSimDistributionProvider m_Provider;
        private int? m_FlowMultiplierTypeId;
        private FlowMultiplierCollection m_FlowMultipliers = new FlowMultiplierCollection();
        private FlowMultiplierMap m_FlowMultiplierMap;
        private FlowSpatialMultiplierCollection m_FlowSpatialMultipliers = new FlowSpatialMultiplierCollection();
        private FlowSpatialMultiplierMap m_FlowSpatialMultiplierMap;
        private FlowLateralMultiplierCollection m_FlowLateralMultipliers = new FlowLateralMultiplierCollection();
        private FlowLateralMultiplierMap m_FlowLateralMultiplierMap;
        private StockFlowMultiplierCollection m_StockFlowMultipliers = new StockFlowMultiplierCollection();
        private StockFlowMultiplierMap m_StockFlowMultiplierMap;

        public FlowMultiplierType(
            int? flowMultiplierTypeId, 
            Scenario scenario, 
            STSimDistributionProvider provider)
        {
            this.m_FlowMultiplierTypeId = flowMultiplierTypeId;
            this.m_Scenario = scenario;
            this.m_Provider = provider;
        }

        internal int? FlowMultiplierTypeId
        {
            get
            {
                return this.m_FlowMultiplierTypeId;
            }
        }

        internal FlowMultiplierMap FlowMultiplierMap
        {
            get
            {
                return this.m_FlowMultiplierMap;
            }
        }

        internal FlowSpatialMultiplierMap FlowSpatialMultiplierMap
        {
            get
            {
                return this.m_FlowSpatialMultiplierMap;
            }
        }

        internal FlowLateralMultiplierMap FlowLateralMultiplierMap
        {
            get
            {
                return this.m_FlowLateralMultiplierMap;
            }
        }

        internal StockFlowMultiplierMap StockFlowMultiplierMap
        {
            get
            {
                return this.m_StockFlowMultiplierMap;
            }
        }

        internal void AddFlowMultiplier(FlowMultiplier multiplier)
        {
            if (multiplier.FlowMultiplierTypeId != this.m_FlowMultiplierTypeId)
            {
                throw new ArgumentException("The flow multiplier type is not correct.");
            }

            this.m_FlowMultipliers.Add(multiplier);
        }

        internal void AddFlowSpatialMultiplier(FlowSpatialMultiplier multiplier)
        {
            if (multiplier.FlowMultiplierTypeId != this.m_FlowMultiplierTypeId)
            {
                throw new ArgumentException("The flow multiplier type is not correct.");
            }

            this.m_FlowSpatialMultipliers.Add(multiplier);
        }

        internal void AddFlowLateralMultiplier(FlowLateralMultiplier multiplier)
        {
            if (multiplier.FlowMultiplierTypeId != this.m_FlowMultiplierTypeId)
            {
                throw new ArgumentException("The flow multiplier type is not correct.");
            }

            this.m_FlowLateralMultipliers.Add(multiplier);
        }

        internal void AddStockFlowMultiplier(StockFlowMultiplier multiplier)
        {
            if (multiplier.FlowMultiplierTypeId != this.m_FlowMultiplierTypeId)
            {
                throw new ArgumentException("The flow multiplier type is not correct.");
            }

            this.m_StockFlowMultipliers.Add(multiplier);
        }

        internal void ClearFlowMultiplierMap()
        {
            this.m_FlowMultipliers.Clear();
            this.m_FlowMultiplierMap = null;
        }

        internal void ClearFlowSpatialMultiplierMap()
        {
            this.m_FlowSpatialMultipliers.Clear();
            this.m_FlowSpatialMultiplierMap = null;
        }

        internal void ClearFlowLateralMultiplierMap()
        {
            this.m_FlowLateralMultipliers.Clear();
            this.m_FlowLateralMultiplierMap = null;
        }

        internal void ClearStockFlowMultiplierMap()
        {
            this.m_StockFlowMultipliers.Clear();
            this.m_StockFlowMultiplierMap = null;
        }

        internal void CreateFlowMultiplierMap()
        {
            if (this.m_FlowMultipliers.Count > 0)
            {
                Debug.Assert(this.m_FlowMultiplierMap == null);

                this.m_FlowMultiplierMap = new FlowMultiplierMap(
                    this.m_Scenario, this.m_FlowMultipliers, this.m_Provider);
            }
        }

        internal void CreateSpatialFlowMultiplierMap()
        {
            if (this.m_FlowSpatialMultipliers.Count > 0)
            {
                Debug.Assert(this.m_FlowSpatialMultiplierMap == null);

                this.m_FlowSpatialMultiplierMap = new FlowSpatialMultiplierMap(
                    this.m_Scenario, this.m_FlowSpatialMultipliers);
            }
        }

        internal void CreateLateralFlowMultiplierMap()
        {
            if (this.m_FlowLateralMultipliers.Count > 0)
            {
                Debug.Assert(this.m_FlowLateralMultiplierMap == null);

                this.m_FlowLateralMultiplierMap = new FlowLateralMultiplierMap(
                    this.m_Scenario, this.m_FlowLateralMultipliers);
            }
        }

        internal void CreateStockFlowMultiplierMap()
        {
            if (this.m_StockFlowMultipliers.Count > 0)
            {
                Debug.Assert(this.m_StockFlowMultiplierMap == null);

                this.m_StockFlowMultiplierMap = new StockFlowMultiplierMap(
                    this.m_Scenario, this.m_StockFlowMultipliers, this.m_Provider);
            }
        }
    }
}
