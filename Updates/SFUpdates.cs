//*********************************************************************************************
// STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
//
// Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
//
//*********************************************************************************************

using System;
using System.Data;
using System.Diagnostics;
using SyncroSim.Core;
using System.Globalization;
using System.Reflection;
using System.Collections.Generic;

namespace SyncroSim.STSimStockFlow
{
	[ObfuscationAttribute(Exclude=true, ApplyToMembers=false)]
	internal class SFUpdates : UpdateProvider
	{
		public override void PerformUpdate(DataStore store, int currentSchemaVersion)
		{
			PerformUpdateInternal(store, currentSchemaVersion);

#if DEBUG
			//Verify that all expected indexes exist after the update because it is easy to forget to recreate them after 
			//adding a column to an existing table (which requires the table to be recreated if you want to preserve column order.)

			ASSERT_INDEX_EXISTS(store, "SF_FlowPathway");
			ASSERT_INDEX_EXISTS(store, "SF_OutputFlow");
			ASSERT_INDEX_EXISTS(store, "SF_OutputStock");
			ASSERT_INDEX_EXISTS(store, "STSim_DistributionValue");
			ASSERT_INDEX_EXISTS(store, "SF_StockTypeGroupMembership");
			ASSERT_INDEX_EXISTS(store, "SF_FlowTypeGroupMembership");
			ASSERT_INDEX_EXISTS(store, "SF_StockTransitionMultiplier");
			ASSERT_INDEX_EXISTS(store, "SF_FlowMultiplier");
#endif

		}

		/// <summary>
		/// Performs the database updates for Stocks and Flows
		/// </summary>
		/// <param name="store"></param>
		/// <param name="currentSchemaVersion"></param>
		/// <remarks>
		/// NOTE: We are starting at version 5 because the legacy version of Stocks and Flows leaves
		/// things at version 4.  (Actually, I think there may have been a mistake here and it was left at 
		/// version 3.  However, I am not going to touch this now, and it should not matter anyway because
		/// the upgrade will eventually be performed and then things will behave normally for the rest of
		/// the lifetime of this module.)
		/// </remarks>
		private static void PerformUpdateInternal(DataStore store, int currentSchemaVersion)
		{
			if (currentSchemaVersion < 5)
			{
				SF0000005(store);
			}

			if (currentSchemaVersion < 6)
			{
				SF0000006(store);
			}

			if (currentSchemaVersion < 7)
			{
				SF0000007(store);
			}

			if (currentSchemaVersion < 8)
			{
				SF0000008(store);
			}

			if (currentSchemaVersion < 9)
			{
				SF0000009(store);
			}

			if (currentSchemaVersion < 10)
			{
				SF0000010(store);
			}

			if (currentSchemaVersion < 11)
			{
				SF0000011(store);
			}

			if (currentSchemaVersion < 12)
			{
				SF0000012(store);
			}

			if (currentSchemaVersion < 13)
			{
				SF0000013(store);
			}

			if (currentSchemaVersion < 14)
			{
				SF0000014(store);
			}

			if (currentSchemaVersion < 15)
			{
				SF0000015(store);
			}

			if (currentSchemaVersion < 16)
			{
				SF0000016(store);
			}

			if (currentSchemaVersion < 17)
			{
				SF0000017(store);
			}

			if (currentSchemaVersion < 18)
			{
				SF0000018(store);
			}
		}

		/// <summary>
		/// SF0000005
		/// </summary>
		/// <param name="store"></param>
		/// <remarks>
		/// Adds (or enhances) indexes on the following tables:
		/// 
		/// SF_OutputStock
		/// SF_StockTypeGroupMembership
		/// SF_OutputFlow
		/// SF_FlowTypeGroupMembership
		/// 
		/// NOTE: It's possible that the indexes on the output tables exist as a result of testing...
		/// </remarks>
		private static void SF0000005(DataStore store)
		{
			store.ExecuteNonQuery("DROP INDEX IF EXISTS SF_OutputStock_Index");
			store.ExecuteNonQuery("CREATE INDEX SF_OutputStock_Index ON SF_OutputStock(ScenarioID, Iteration, Timestep, StratumID, SecondaryStratumID, StateClassID, StockTypeID)");

			if (store.TableExists("SF_StockTypeGroupMembership"))
			{
				store.ExecuteNonQuery("CREATE INDEX SF_StockTypeGroupMembership_Index ON SF_StockTypeGroupMembership(ScenarioID, StockTypeID, StockGroupID)");
			}

			store.ExecuteNonQuery("DROP INDEX IF EXISTS SF_OutputFlow_Index");
			store.ExecuteNonQuery("CREATE INDEX SF_OutputFlow_Index ON SF_OutputFlow(ScenarioID, Iteration, Timestep, FromStratumID, FromSecondaryStratumID, FromStateClassID, FromStockTypeID, TransitionTypeID, ToStratumID, ToSecondaryStratumID, ToStateClassID, ToStockTypeID, FlowTypeID)");

			if (store.TableExists("SF_FlowTypeGroupMembership"))
			{
				store.ExecuteNonQuery("CREATE INDEX SF_FlowTypeGroupMembership_Index ON SF_FlowTypeGroupMembership(ScenarioID, FlowTypeID, FlowGroupID)");
			}
		}

		/// <summary>
		/// SF0000006
		/// </summary>
		/// <param name="store"></param>
		/// <remarks>
		/// This update will remove the "group-group" tables which are obsolete
		/// </remarks>
		private static void SF0000006(DataStore store)
		{
			if (store.TableExists("SF_StockGroupGroupMembership"))
			{
				store.DeleteSchema("SF_StockGroupGroupMembership");
			}

			if (store.TableExists("SF_FlowGroupGroupMembership"))
			{
				store.DeleteSchema("SF_FlowGroupGroupMembership");
			}
		}

		/// <summary>
		/// SF0000007
		/// </summary>
		/// <param name="store"></param>
		/// <remarks>
		/// This update removes Iteration, Timestep, and SecondaryStratumID from the FlowPathways table
		/// </remarks>
		private static void SF0000007(DataStore store)
		{
			if (store.TableExists("SF_FlowPathway"))
			{
				store.ExecuteNonQuery("ALTER TABLE SF_FlowPathway RENAME TO TEMP_TABLE");
				store.ExecuteNonQuery("CREATE TABLE SF_FlowPathway(FlowPathwayID INTEGER PRIMARY KEY AUTOINCREMENT, ScenarioID INTEGER, FromStratumID INTEGER, FromStateClassID INTEGER, FromAgeMin INTEGER, FromStockTypeID INTEGER, ToStratumID INTEGER, ToStateClassID INTEGER, ToAgeMin INTEGER, ToStockTypeID INTEGER, TransitionGroupID INTEGER, StateAttributeTypeID INTEGER, FlowTypeID INTEGER, Multiplier DOUBLE)");
				store.ExecuteNonQuery("INSERT INTO SF_FlowPathway(ScenarioID, FromStratumID, FromStateClassID, FromAgeMin, FromStockTypeID, ToStratumID, ToStateClassID, ToAgeMin, ToStockTypeID, TransitionGroupID, StateAttributeTypeID, FlowTypeID, Multiplier) SELECT ScenarioID, FromStratumID, FromStateClassID, FromAgeMin, FromStockTypeID, ToStratumID, ToStateClassID, ToAgeMin, ToStockTypeID, TransitionGroupID, StateAttributeTypeID, FlowTypeID, Multiplier FROM TEMP_TABLE");
				store.ExecuteNonQuery("DROP TABLE TEMP_TABLE");
			}
		}

		/// <summary>
		/// SF0000008
		/// </summary>
		/// <param name="store"></param>
		/// <remarks>
		/// This update will drop the 'output ? group multiplier' tables that are obsolete.
		/// </remarks>
		private static void SF0000008(DataStore store)
		{        
			if (store.TableExists("SF_OutputStockGroupMultiplier"))
			{
				store.ExecuteNonQuery("DROP TABLE SF_OutputStockGroupMultiplier");
			}

			if (store.TableExists("SF_OutputFlowGroupMultiplier"))
			{
				store.ExecuteNonQuery("DROP TABLE SF_OutputFlowGroupMultiplier");
			}
		}

		/// <summary>
		/// SF0000009
		/// </summary>
		/// <param name="store"></param>
		/// <remarks>
		/// This update will remove the unused ToSecondaryStratumID field from SF_OutputFlow
		/// </remarks>
		private static void SF0000009(DataStore store)
		{
			if (store.TableExists("SF_OutputFlow"))
			{
				store.ExecuteNonQuery("DROP INDEX IF EXISTS SF_OutputFlow_Index");

				store.ExecuteNonQuery("ALTER TABLE SF_OutputFlow RENAME TO TEMP_TABLE");
				store.ExecuteNonQuery("CREATE TABLE SF_OutputFlow(ScenarioID INTEGER, Iteration INTEGER, Timestep INTEGER, FromStratumID INTEGER, FromSecondaryStratumID INTEGER, FromStateClassID INTEGER, FromStockTypeID INTEGER, TransitionTypeID INTEGER, ToStratumID INTEGER, ToStateClassID INTEGER, ToStockTypeID INTEGER, FlowTypeID INTEGER, Amount DOUBLE)");
				store.ExecuteNonQuery("INSERT INTO SF_OutputFlow(ScenarioID, Iteration, Timestep, FromStratumID, FromSecondaryStratumID, FromStateClassID, FromStockTypeID, TransitionTypeID, ToStratumID, ToStateClassID, ToStockTypeID, FlowTypeID, Amount) SELECT ScenarioID, Iteration, Timestep, FromStratumID, FromSecondaryStratumID, FromStateClassID, FromStockTypeID, TransitionTypeID, ToStratumID, ToStateClassID, ToStockTypeID, FlowTypeID, Amount FROM TEMP_TABLE");
				store.ExecuteNonQuery("DROP TABLE TEMP_TABLE");

				store.ExecuteNonQuery("CREATE INDEX SF_OutputFlow_Index ON SF_OutputFlow(ScenarioID, Iteration, Timestep, FromStratumID, FromSecondaryStratumID, FromStateClassID, FromStockTypeID, TransitionTypeID, ToStratumID, ToStateClassID, ToStockTypeID, FlowTypeID)");
			}
		}

		/// <summary>
		/// SF0000010
		/// </summary>
		/// <param name="store"></param>
		/// <remarks>
		/// Flow diagrams introduce a new table called SF_FlowPathwayDiagram which contains each (unique) Stock Type
		/// found SF_FlowPathway along with a location for the diagram.  The items in this table must match the ones
		/// found in SF_FlowPathway exactly or the application will crash.  This means that we need to populate the 
		/// SF_FlowPathwayDiagram table if flow data exists as follows:
		/// 
		/// (1.) First, delete any diagram data that an internal (Apex) user might have created.  Sorry, but your
		///      diagrams will have to be reformatted after this update.
		/// 
		/// (2.) If flow data exists in ANY scenario we are going to apply this update.
		/// 
		/// (3.) If there are more stock types than spaces in the diagram we are going to fail the update.  I can't imagine
		///      that this would be possible, however, since there are 26x256 spaces per diagram...
		/// </remarks>
		private static void SF0000010(DataStore store)
		{
			//Delete any existing diagram data no matter what

			if (store.TableExists("SF_FlowPathwayDiagram"))
			{
				store.ExecuteNonQuery("DELETE FROM SF_FlowPathwayDiagram");
			}

			//If there is no flow pathway data at all then there is nothing to do

			if (!store.TableExists("SF_FlowPathway"))
			{
				return;
			}

			if (!Convert.ToBoolean(store.ExecuteScalar("SELECT EXISTS(SELECT 1 FROM SF_FlowPathway)")))
			{
				return;
			}

			//Get the existing scenarios there are no scenarios there is nothing to do

			DataTable Scenarios = store.CreateDataTable("SSim_Scenario");

			if (Scenarios.Rows.Count == 0)
			{
				return;
			}

			//Create the flow diagram data table
			if (!store.TableExists("SF_FlowPathwayDiagram"))
			{
				store.ExecuteNonQuery("CREATE TABLE SF_FlowPathwayDiagram(FlowPathwayDiagramID INTEGER PRIMARY KEY AUTOINCREMENT, ScenarioID INTEGER, StockTypeID INTEGER, Location TEXT)");
			}

			//For each scenario, populate the SF_FlowPathwayDiagram if there is flow data

			Func<int, int, string> RowColToLocation = (int row, int column) =>
			{
				Debug.Assert(column < 26);

				string s = Convert.ToString((char)((int)'A' + column));
				s = s + (row + 1).ToString(CultureInfo.InvariantCulture);

				return s;
			};

			foreach (DataRow ScenarioRow in Scenarios.Rows)
			{
				int ScenarioId = Convert.ToInt32(ScenarioRow["ScenarioID"]);
				DataTable FlowPathways = store.CreateDataTable("SF_FlowPathway", "ScenarioID", ScenarioId);

				if (FlowPathways.Rows.Count == 0)
				{
					continue;
				}

				Dictionary<int, bool> StockTypeIds = new Dictionary<int, bool>();

				foreach (DataRow drflow in FlowPathways.Rows)
				{
					int FromStockId = Convert.ToInt32(drflow["FromStockTypeID"]);

					if (!StockTypeIds.ContainsKey(FromStockId))
					{
						StockTypeIds.Add(FromStockId, true);
					}

					int ToStockId = Convert.ToInt32(drflow["ToStockTypeId"]);

					if (!StockTypeIds.ContainsKey(ToStockId))
					{
						StockTypeIds.Add(ToStockId, true);
					}
				}

				if (StockTypeIds.Count > (26 * 256))
				{
					ExceptionUtils.ThrowInvalidOperationException("There are too many stock types.  Please clear your flow pathways and try this update again.");
				}

				int CurrentRow = 0;
				int CurrentCol = 0;

				foreach (int StockTypeId in StockTypeIds.Keys)
				{              
					string Location = RowColToLocation(CurrentRow, CurrentCol);
					string Query = string.Format(CultureInfo.InvariantCulture, "INSERT INTO SF_FlowPathwayDiagram(ScenarioID, StockTypeID, Location) VALUES({0}, {1}, '{2}')", ScenarioId, StockTypeId, Location);

					store.ExecuteNonQuery(Query);
					CurrentCol += 1;

					if (CurrentCol == 26)
					{
						CurrentCol = 0;
						CurrentRow += 1;
					}

					Debug.Assert(CurrentRow <= 255);
				}
			}
		}

		/// <summary>
		/// SF0000011
		/// </summary>
		/// <param name="store"></param>
		/// <remarks>
		/// Data feeds with distributions now get their distribution types from Stats_DistributionType instead
		/// of a hard coded list.  This means that we must update every table so that it uses the new project
		/// specific primary key IDs.
		/// </remarks>
		private static void SF0000011(DataStore store)
		{
			DataTable Projects = store.CreateDataTable("SSim_Project");
			DataTable Scenarios = store.CreateDataTable("SSim_Scenario");
			Dictionary<int, DataTable> DistTables = new Dictionary<int, DataTable>();

			foreach (DataRow ProjectRow in Projects.Rows)
			{
				int ProjectId = Convert.ToInt32(ProjectRow["ProjectID"]);
				DataTable DistributionTypes = store.CreateDataTableFromQuery(string.Format(CultureInfo.InvariantCulture, "SELECT * FROM STime_DistributionType WHERE ProjectID={0}", ProjectId), "DistributionTypes");

				Debug.Assert(DistributionTypes.Rows.Count == 4);
				DistTables.Add(ProjectId, DistributionTypes);
			}

			UpdateDistributions("SF_FlowMultiplier", store, Scenarios, DistTables);
		}

		private static void UpdateDistributions(string tableName, DataStore store, DataTable scenarios, Dictionary<int, DataTable> distTables)
		{
			if (!store.TableExists(tableName))
			{
				return;
			}

			store.ExecuteNonQuery(string.Format(CultureInfo.InvariantCulture, "UPDATE {0} SET DistributionType=-1 WHERE DistributionType=0", tableName)); //Normal
			store.ExecuteNonQuery(string.Format(CultureInfo.InvariantCulture, "UPDATE {0} SET DistributionType=-2 WHERE DistributionType=1", tableName)); //Beta
			store.ExecuteNonQuery(string.Format(CultureInfo.InvariantCulture, "UPDATE {0} SET DistributionType=-3 WHERE DistributionType=2", tableName)); //Uniform

			foreach (DataRow ScenarioRow in scenarios.Rows)
			{
				int ScenarioId = Convert.ToInt32(ScenarioRow["ScenarioID"]);
				int ProjectId = Convert.ToInt32(ScenarioRow["ProjectID"]);
				DataTable DistributionTypes = distTables[ProjectId];

				foreach (DataRow DistTypeRow in DistributionTypes.Rows)
				{
					int DistTypeId = Convert.ToInt32(DistTypeRow["DistributionTypeID"]);
					string DistTypeName = Convert.ToString(DistTypeRow["Name"]);
					int TempDistId = 0;
					bool DoUpdate = true;

					if (DistTypeName == "Normal")
					{
						TempDistId = -1;
					}
					else if (DistTypeName == "Beta")
					{
						TempDistId = -2;
					}
					else if (DistTypeName == "Uniform")
					{
						TempDistId = -3;
					}
					else
					{
						DoUpdate = false;
						Debug.Assert(DistTypeName == "Uniform Integer");
					}

					if (DoUpdate)
					{
						store.ExecuteNonQuery(string.Format(CultureInfo.InvariantCulture, 
                            "UPDATE {0} SET DistributionType={1} WHERE DistributionType={2} AND ScenarioID={3}", 
                            tableName, DistTypeId, TempDistId, ScenarioId));
					}
				}
			}
		}

		/// <summary>
		/// SF0000012
		/// </summary>
		/// <param name="store"></param>
		/// <remarks>
		/// This update adds a DistributionFrequency column to the relevant Stock and Flow tables
		/// </remarks>
		private static void SF0000012(DataStore store)
		{
			if (store.TableExists("SF_FlowMultiplier"))
			{
				Debug.Assert(Convert.ToInt32(StochasticTime.DistributionFrequency.Always) == 0);
				Debug.Assert(Convert.ToInt32(StochasticTime.DistributionFrequency.Iteration) == 1);
				Debug.Assert(Convert.ToInt32(StochasticTime.DistributionFrequency.Timestep) == 2);

				store.ExecuteNonQuery("ALTER TABLE SF_FlowMultiplier ADD COLUMN DistributionFrequencyID INTEGER");

				store.ExecuteNonQuery(string.Format(CultureInfo.InvariantCulture, 
                    "UPDATE SF_FlowMultiplier SET DistributionFrequencyID={0} WHERE DistributionType IS NOT NULL", 
                    Convert.ToInt32(StochasticTime.DistributionFrequency.Iteration)));
			}
		}

		/// <summary>
		/// SF0000013
		/// </summary>
		/// <param name="store"></param>
		/// <remarks>
		/// This update adds indexes to various tables to improve performance with large data sets
		/// </remarks>
		private static void SF0000013(DataStore store)
		{
			if (store.TableExists("SF_FlowMultiplier"))
			{
				store.ExecuteNonQuery("CREATE INDEX SF_FlowMultiplier_Index ON SF_FlowMultiplier(ScenarioID)");
			}

			if (store.TableExists("SF_FlowPathway"))
			{
				store.ExecuteNonQuery("CREATE INDEX SF_FlowPathway_Index ON SF_FlowPathway(ScenarioID)");
			}
		}

		/// <summary>
		/// SF0000014
		/// </summary>
		/// <param name="store"></param>
		/// <remarks>
		/// This update adds an Iteration field to SF_InitialStockSpatial
		/// </remarks>
		private static void SF0000014(DataStore store)
		{
			if (store.TableExists("SF_InitialStockSpatial"))
			{
				store.ExecuteNonQuery("ALTER TABLE SF_InitialStockSpatial RENAME TO TEMP_TABLE");
				store.ExecuteNonQuery("CREATE TABLE SF_InitialStockSpatial(InitialStockSpatialID INTEGER PRIMARY KEY AUTOINCREMENT,ScenarioID INTEGER,Iteration INTEGER,StockTypeID INTEGER,RasterFileName TEXT)");
				store.ExecuteNonQuery("INSERT INTO SF_InitialStockSpatial(ScenarioID,StockTypeID,RasterFileName) SELECT ScenarioID,StockTypeID,RasterFileName FROM TEMP_TABLE");
				store.ExecuteNonQuery("DROP TABLE TEMP_TABLE");
			}
		}

		/// <summary>
		/// SF0000015
		/// </summary>
		/// <param name="store"></param>
		/// <remarks>New columns for SF_FlowPathway if it exists</remarks>
		private static void SF0000015(DataStore store)
		{
			if (store.TableExists("SF_FlowPathway"))
			{
				store.ExecuteNonQuery("ALTER TABLE SF_FlowPathway RENAME TO TEMP_TABLE");
				store.ExecuteNonQuery("CREATE TABLE SF_FlowPathway(FlowPathwayID INTEGER PRIMARY KEY AUTOINCREMENT, ScenarioID INTEGER, Iteration INTEGER, Timestep INTEGER, FromStratumID INTEGER, FromStateClassID INTEGER, FromAgeMin INTEGER, FromStockTypeID INTEGER, ToStratumID INTEGER, ToStateClassID INTEGER, ToAgeMin INTEGER, ToStockTypeID INTEGER, TransitionGroupID INTEGER, StateAttributeTypeID INTEGER, FlowTypeID INTEGER, Multiplier DOUBLE)");
				store.ExecuteNonQuery("INSERT INTO SF_FlowPathway(ScenarioID, FromStratumID, FromStateClassID, FromAgeMin, FromStockTypeID, ToStratumID, ToStateClassID, ToAgeMin, ToStockTypeID, TransitionGroupID, StateAttributeTypeID, FlowTypeID, Multiplier) SELECT ScenarioID, FromStratumID, FromStateClassID, FromAgeMin, FromStockTypeID, ToStratumID, ToStateClassID, ToAgeMin, ToStockTypeID, TransitionGroupID, StateAttributeTypeID, FlowTypeID, Multiplier FROM TEMP_TABLE");
				store.ExecuteNonQuery("DROP TABLE TEMP_TABLE");
			}
		}

		/// <summary>
		/// SF0000016
		/// </summary>
		/// <param name="store"></param>
		/// <remarks>
		/// Add missing index on SF_FlowPathway if missing drop</remarks>
		private static void SF0000016(DataStore store)
		{
			if (store.TableExists("SF_FlowPathway"))
			{
				store.ExecuteNonQuery("DROP INDEX IF EXISTS SF_FlowPathway_Index");
				store.ExecuteNonQuery("CREATE INDEX SF_FlowPathway_Index ON SF_FlowPathway(ScenarioID)");
			}
		}

		/// <summary>
		/// SF0000017
		/// </summary>
		/// <param name="store"></param>
		/// <remarks>This update adds a tertiary stratum to all applicable tables</remarks>
		private static void SF0000017(DataStore store)
		{
			if (store.TableExists("SF_FlowMultiplier"))
			{
				store.ExecuteNonQuery("ALTER TABLE SF_FlowMultiplier RENAME TO TEMP_TABLE");
				store.ExecuteNonQuery("CREATE TABLE SF_FlowMultiplier(FlowMultiplierID INTEGER PRIMARY KEY AUTOINCREMENT, ScenarioID INTEGER, Iteration INTEGER, Timestep INTEGER, StratumID INTEGER, SecondaryStratumID INTEGER, TertiaryStratumID INTEGER, StateClassID INTEGER, FlowGroupID INTEGER, Value DOUBLE, DistributionType INTEGER, DistributionFrequencyID INTEGER, DistributionSD DOUBLE, DistributionMin DOUBLE, DistributionMax DOUBLE)");
				store.ExecuteNonQuery("INSERT INTO SF_FlowMultiplier(ScenarioID, Iteration, Timestep, StratumID, SecondaryStratumID, StateClassID, FlowGroupID, Value, DistributionType, DistributionFrequencyID, DistributionSD, DistributionMin, DistributionMax) SELECT ScenarioID, Iteration, Timestep, StratumID, SecondaryStratumID, StateClassID, FlowGroupID, Value, DistributionType, DistributionFrequencyID, DistributionSD, DistributionMin, DistributionMax FROM TEMP_TABLE");
				store.ExecuteNonQuery("DROP TABLE TEMP_TABLE");
			}

			if (store.TableExists("SF_StockLimit"))
			{
				store.ExecuteNonQuery("ALTER TABLE SF_StockLimit RENAME TO TEMP_TABLE");
				store.ExecuteNonQuery("CREATE TABLE SF_StockLimit(StockLimitID INTEGER PRIMARY KEY AUTOINCREMENT, ScenarioID INTEGER, Iteration INTEGER, Timestep INTEGER, StockTypeID INTEGER, StratumID INTEGER, SecondaryStratumID INTEGER, TertiaryStratumID INTEGER, StateClassID INTEGER, StockMinimum DOUBLE, StockMaximum DOUBLE)");
				store.ExecuteNonQuery("INSERT INTO SF_StockLimit(ScenarioID, Iteration, Timestep, StockTypeID, StratumID, SecondaryStratumID, StateClassID, StockMinimum, StockMaximum) SELECT ScenarioID, Iteration, Timestep, StockTypeID, StratumID, SecondaryStratumID, StateClassID, StockMinimum, StockMaximum FROM TEMP_TABLE");
				store.ExecuteNonQuery("DROP TABLE TEMP_TABLE");
			}

			if (store.TableExists("SF_OutputFlow"))
			{
				store.ExecuteNonQuery("ALTER TABLE SF_OutputFlow RENAME TO TEMP_TABLE");
				store.ExecuteNonQuery("CREATE TABLE SF_OutputFlow(ScenarioID INTEGER, Iteration INTEGER, Timestep INTEGER, FromStratumID INTEGER, FromSecondaryStratumID INTEGER, FromTertiaryStratumID INTEGER, FromStateClassID INTEGER, FromStockTypeID INTEGER, TransitionTypeID INTEGER, ToStratumID INTEGER, ToStateClassID INTEGER, ToStockTypeID INTEGER, FlowTypeID INTEGER, Amount DOUBLE)");
				store.ExecuteNonQuery("INSERT INTO SF_OutputFlow(ScenarioID, Iteration, Timestep, FromStratumID, FromSecondaryStratumID, FromStateClassID, FromStockTypeID, TransitionTypeID, ToStratumID, ToStateClassID, ToStockTypeID, FlowTypeID, Amount) SELECT ScenarioID, Iteration, Timestep, FromStratumID, FromSecondaryStratumID, FromStateClassID, FromStockTypeID, TransitionTypeID, ToStratumID, ToStateClassID, ToStockTypeID, FlowTypeID, Amount FROM TEMP_TABLE");
				store.ExecuteNonQuery("DROP TABLE TEMP_TABLE");
			}

			if (store.TableExists("SF_OutputStock"))
			{
				store.ExecuteNonQuery("ALTER TABLE SF_OutputStock RENAME TO TEMP_TABLE");
				store.ExecuteNonQuery("CREATE TABLE SF_OutputStock(ScenarioID INTEGER, Iteration INTEGER, Timestep INTEGER, StratumID INTEGER, SecondaryStratumID INTEGER, TertiaryStratumID INTEGER, StateClassID INTEGER, StockTypeID INTEGER, Amount DOUBLE)");
				store.ExecuteNonQuery("INSERT INTO SF_OutputStock(ScenarioID, Iteration, Timestep, StratumID, SecondaryStratumID, StateClassID, StockTypeID, Amount) SELECT ScenarioID, Iteration, Timestep, StratumID, SecondaryStratumID, StateClassID, StockTypeID, Amount FROM TEMP_TABLE");
				store.ExecuteNonQuery("DROP TABLE TEMP_TABLE");
			}
		}

		/// <summary>
		/// SF0000018
		/// </summary>
		/// <param name="store"></param>
		/// <remarks>
		/// This update restores indexes that were dropped as a result of a previous alteration to a table
		/// </remarks>
		private static void SF0000018(DataStore store)
		{
			UpdateProvider.CreateIndex(store, "SF_FlowMultiplier", new[] {"ScenarioID"});
			UpdateProvider.CreateIndex(store, "SF_StockTransitionMultiplier", new[] {"ScenarioID"});
			UpdateProvider.CreateIndex(store, "SF_OutputFlow", new[] {"ScenarioID", "Iteration", "Timestep", "FromStratumID", "FromSecondaryStratumID", "FromTertiaryStratumID", "FromStateClassID", "FromStockTypeID", "TransitionTypeID", "ToStratumID", "ToStateClassID", "ToStockTypeID", "FlowTypeID"});
			UpdateProvider.CreateIndex(store, "SF_OutputStock", new[] {"ScenarioID", "Iteration", "Timestep", "StratumID", "SecondaryStratumID", "TertiaryStratumID", "StateClassID", "StockTypeID"});
		}
	}
}