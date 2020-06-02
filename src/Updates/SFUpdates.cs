// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System;
using System.IO;
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

			ASSERT_INDEX_EXISTS(store, "stsimsf_FlowPathway");
			ASSERT_INDEX_EXISTS(store, "stsimsf_OutputFlow");
			ASSERT_INDEX_EXISTS(store, "stsimsf_OutputStock");
			ASSERT_INDEX_EXISTS(store, "stsimsf_StockTypeGroupMembership");
			ASSERT_INDEX_EXISTS(store, "stsimsf_FlowTypeGroupMembership");
			ASSERT_INDEX_EXISTS(store, "stsimsf_StockTransitionMultiplier");
			ASSERT_INDEX_EXISTS(store, "stsimsf_FlowMultiplier");
#endif

		}

#if DEBUG

        private static void ASSERT_INDEX_EXISTS(DataStore store, string tableName)
        {
            if (store.TableExists(tableName))
            {
                string IndexName = tableName + "_Index";
                string Query = string.Format(CultureInfo.InvariantCulture, "SELECT COUNT(name) FROM sqlite_master WHERE type = 'index' AND name = '{0}'", IndexName);
                Debug.Assert((long)store.ExecuteScalar(Query) == 1);
            }
        }

#endif

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

			if (currentSchemaVersion < 19)
			{
				SF0000019(store);
			}

			if (currentSchemaVersion < 20)
			{
				SF0000020(store);
			}

            //This is the beginning of the v2.1.0 schema lineage.
            //We are setting the base version to 100 to leave room
            //for the legacy v2.0.N legacy branch.

            if (currentSchemaVersion < 100)
            {
                SF0000100(store);
            }

            if (currentSchemaVersion < 101)
            {
                SF0000101(store);
            }

            if (currentSchemaVersion < 102)
            {
                SF0000102(store);
            }

            if (currentSchemaVersion < 103)
            {
                SF0000103(store);
            }

            if (currentSchemaVersion < 104)
            {
                SF0000104(store);
            }

            if (currentSchemaVersion < 105)
            {
                SF0000105(store);
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

			if (!Convert.ToBoolean(store.ExecuteScalar("SELECT EXISTS(SELECT 1 FROM SF_FlowPathway)"), CultureInfo.InvariantCulture))
			{
				return;
			}

			//Get the existing scenarios there are no scenarios there is nothing to do

			DataTable Scenarios = store.CreateDataTable("core_Scenario");

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

				string s = Convert.ToString((char)((int)'A' + column), CultureInfo.InvariantCulture);
				s = s + (row + 1).ToString(CultureInfo.InvariantCulture);

				return s;
			};

			foreach (DataRow ScenarioRow in Scenarios.Rows)
			{
				int ScenarioId = Convert.ToInt32(ScenarioRow["ScenarioID"], CultureInfo.InvariantCulture);
				DataTable FlowPathways = store.CreateDataTable("SF_FlowPathway", "ScenarioID", ScenarioId);

				if (FlowPathways.Rows.Count == 0)
				{
					continue;
				}

				Dictionary<int, bool> StockTypeIds = new Dictionary<int, bool>();

				foreach (DataRow drflow in FlowPathways.Rows)
				{
					int FromStockId = Convert.ToInt32(drflow["FromStockTypeID"], CultureInfo.InvariantCulture);

					if (!StockTypeIds.ContainsKey(FromStockId))
					{
						StockTypeIds.Add(FromStockId, true);
					}

					int ToStockId = Convert.ToInt32(drflow["ToStockTypeId"], CultureInfo.InvariantCulture);

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
			DataTable Projects = store.CreateDataTable("core_Project");
			DataTable Scenarios = store.CreateDataTable("core_Scenario");
			Dictionary<int, DataTable> DistTables = new Dictionary<int, DataTable>();

			foreach (DataRow ProjectRow in Projects.Rows)
			{
				int ProjectId = Convert.ToInt32(ProjectRow["ProjectID"], CultureInfo.InvariantCulture);
				DataTable DistributionTypes = store.CreateDataTableFromQuery(string.Format(CultureInfo.InvariantCulture, "SELECT * FROM corestime_DistributionType WHERE ProjectID={0}", ProjectId), "DistributionTypes");
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
				int ScenarioId = Convert.ToInt32(ScenarioRow["ScenarioID"], CultureInfo.InvariantCulture);
				int ProjectId = Convert.ToInt32(ScenarioRow["ProjectID"], CultureInfo.InvariantCulture);
				DataTable DistributionTypes = distTables[ProjectId];

				foreach (DataRow DistTypeRow in DistributionTypes.Rows)
				{
					int DistTypeId = Convert.ToInt32(DistTypeRow["DistributionTypeID"], CultureInfo.InvariantCulture);
					string DistTypeName = Convert.ToString(DistTypeRow["Name"], CultureInfo.InvariantCulture);
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
				Debug.Assert(Convert.ToInt32(StochasticTime.DistributionFrequency.Always, CultureInfo.InvariantCulture) == 0);
				Debug.Assert(Convert.ToInt32(StochasticTime.DistributionFrequency.Iteration, CultureInfo.InvariantCulture) == 1);
				Debug.Assert(Convert.ToInt32(StochasticTime.DistributionFrequency.Timestep, CultureInfo.InvariantCulture) == 2);

				store.ExecuteNonQuery("ALTER TABLE SF_FlowMultiplier ADD COLUMN DistributionFrequencyID INTEGER");

				store.ExecuteNonQuery(string.Format(CultureInfo.InvariantCulture, 
                    "UPDATE SF_FlowMultiplier SET DistributionFrequencyID={0} WHERE DistributionType IS NOT NULL", 
                    Convert.ToInt32(StochasticTime.DistributionFrequency.Iteration, CultureInfo.InvariantCulture)));
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

		/// <summary>
		/// SF0000019
		/// </summary>
		/// <param name="store"></param>
		/// <remarks>
		/// This update adds a FlowMultiplierTypeID column to the SF_FlowMultiplier and SF_FlowSpatialMultiplier tables
		/// </remarks>
		private static void SF0000019(DataStore store)
		{
            if (store.TableExists("SF_FlowMultiplier"))
            {
                store.ExecuteNonQuery("ALTER TABLE SF_FlowMultiplier RENAME TO TEMP_TABLE");
                store.ExecuteNonQuery("CREATE TABLE SF_FlowMultiplier(FlowMultiplierID INTEGER PRIMARY KEY AUTOINCREMENT, ScenarioID INTEGER, Iteration INTEGER, Timestep INTEGER, StratumID INTEGER, SecondaryStratumID INTEGER, TertiaryStratumID INTEGER, StateClassID INTEGER, FlowMultiplierTypeID INTEGER, FlowGroupID INTEGER, Value DOUBLE, DistributionType INTEGER, DistributionFrequencyID INTEGER, DistributionSD DOUBLE, DistributionMin DOUBLE, DistributionMax DOUBLE)");
                store.ExecuteNonQuery("INSERT INTO SF_FlowMultiplier(ScenarioID, Iteration, Timestep, StratumID, SecondaryStratumID, TertiaryStratumID, StateClassID, FlowGroupID, Value, DistributionType, DistributionFrequencyID, DistributionSD, DistributionMin, DistributionMax) SELECT ScenarioID, Iteration, Timestep, StratumID, SecondaryStratumID, TertiaryStratumID, StateClassID, FlowGroupID, Value, DistributionType, DistributionFrequencyID, DistributionSD, DistributionMin, DistributionMax FROM TEMP_TABLE");
                store.ExecuteNonQuery("DROP TABLE TEMP_TABLE");

			    UpdateProvider.CreateIndex(store, "SF_FlowMultiplier", new[] {"ScenarioID"});
            }

            if (store.TableExists("SF_FlowSpatialMultiplier"))
            {
                store.ExecuteNonQuery("ALTER TABLE SF_FlowSpatialMultiplier RENAME TO TEMP_TABLE");
                store.ExecuteNonQuery("CREATE TABLE SF_FlowSpatialMultiplier(FlowSpatialMultiplierID INTEGER PRIMARY KEY AUTOINCREMENT, ScenarioID INTEGER, Iteration INTEGER, Timestep INTEGER, FlowGroupID INTEGER, FlowMultiplierTypeID INTEGER, MultiplierFileName TEXT)");           
                store.ExecuteNonQuery("INSERT INTO SF_FlowSpatialMultiplier(ScenarioID, Iteration, Timestep, FlowGroupID, MultiplierFileName) SELECT ScenarioID, Iteration, Timestep, FlowGroupID, MultiplierFileName FROM TEMP_TABLE");
                store.ExecuteNonQuery("DROP TABLE TEMP_TABLE");                
            }
        }

        /// <summary>
        /// SF0000020
        /// </summary>
        /// <param name="store"></param>
        /// <remarks>
        /// This update will:
        /// 
        /// (1.) Add an "IsAuto" column to the SF_StockGroup and SF_FlowGroup tables.
        /// (2.) Add auto groups for each existing type in SF_StockType and SF_StockGroup.
        /// (3.) Update SF_OutputStock and SF_OutputFlow to have a "Group" instead of a "Type"
        ///      and update the TypeID with its associated Auto-Group ID in both tables.
        /// (4.) Move the spatial type records and files as follows:
        ///      SF_OutputSpatialStockType -> SF_OutputSpatialStockGroup
        ///      SF_OutputSpatialFlowType  -> SF_OutputSpatialFlowGroup
        /// (5.) Delete the spatial type tables for the type tables in #4.
        /// </remarks>
        private static void SF0000020(DataStore store)
        {
            //(1.) Above

            //The type tables have existed since the beginning, but the group tables have not so 
            //if they are not there, create them.  If they are there, give them an IsAuto field.

            if (!store.TableExists("SF_StockGroup"))
            {
                store.ExecuteNonQuery("CREATE TABLE SF_StockGroup(StockGroupID INTEGER PRIMARY KEY, ProjectID INTEGER, Name TEXT, Description TEXT, Units TEXT, IsAuto INTEGER)");
            }
            else
            {
                store.ExecuteNonQuery("ALTER TABLE SF_StockGroup ADD COLUMN IsAuto INTEGER");
            }

            if (!store.TableExists("SF_FlowGroup"))
            {
                store.ExecuteNonQuery("CREATE TABLE SF_FlowGroup(FlowGroupID INTEGER PRIMARY KEY, ProjectID INTEGER, Name TEXT, Description TEXT, Units TEXT, IsAuto INTEGER)");
            }
            else
            {
                store.ExecuteNonQuery("ALTER TABLE SF_FlowGroup ADD COLUMN IsAuto INTEGER");
            }

            //(2.) Above

            DataTable Projects = store.CreateDataTable("core_Project");
            Dictionary<int, int> StockIDTranslator = new Dictionary<int, int>();
            Dictionary<int, int> FlowIDTranslator = new Dictionary<int, int>();

            foreach (DataRow ProjectRow in Projects.Rows)
            {
                int ProjectId = (int)(long)ProjectRow["ProjectID"];
                string StockTypeQuery = string.Format(CultureInfo.InvariantCulture, "SELECT * FROM SF_StockType WHERE ProjectID={0}", ProjectId);
                string FlowTypeQuery = string.Format(CultureInfo.InvariantCulture, "SELECT * FROM SF_FlowType WHERE ProjectID={0}", ProjectId);
                DataTable StockTypes = store.CreateDataTableFromQuery(StockTypeQuery, "StockTypes");
                DataTable FlowTypes = store.CreateDataTableFromQuery(FlowTypeQuery, "FlowTypes");

                foreach (DataRow StockTypeRow in StockTypes.Rows)
                {
                    string Name = (string)StockTypeRow["Name"];
                    int TypeId = (int)(long)StockTypeRow["StockTypeID"];
                    int GroupId = Library.GetNextSequenceId(store);
                    string AutoName = DataTableUtilities.GetAutoGeneratedGroupName(Name);
                    string InsertQuery = string.Format(CultureInfo.InvariantCulture, "INSERT INTO SF_StockGroup(StockGroupID, ProjectID, Name, IsAuto) VALUES({0}, {1}, '{2}', {3})", GroupId, ProjectId, AutoName, Booleans.BoolToInt(true));

                    store.ExecuteNonQuery(InsertQuery);
                    StockIDTranslator.Add(TypeId, GroupId);
                }

                foreach (DataRow FlowTypeRow in FlowTypes.Rows)
                {
                    string Name = (string)FlowTypeRow["Name"];
                    int TypeId = (int)(long)FlowTypeRow["FlowTypeID"]; int GroupId = Library.GetNextSequenceId(store);
                    string AutoName = DataTableUtilities.GetAutoGeneratedGroupName(Name);
                    string InsertQuery = string.Format(CultureInfo.InvariantCulture, "INSERT INTO SF_FlowGroup(FlowGroupID, ProjectID, Name, IsAuto) VALUES({0}, {1}, '{2}', {3})", GroupId, ProjectId, AutoName, Booleans.BoolToInt(true));

                    store.ExecuteNonQuery(InsertQuery);
                    FlowIDTranslator.Add(TypeId, GroupId);
                }
            }

            //(3.) Above

            //First, rename the ?TypeID column to ?GroupID
            store.ExecuteNonQuery("ALTER TABLE SF_OutputFlow RENAME TO TEMP_TABLE");
            store.ExecuteNonQuery("CREATE TABLE SF_OutputFlow(ScenarioID INTEGER, Iteration INTEGER, Timestep INTEGER, FromStratumID INTEGER, FromSecondaryStratumID INTEGER, FromTertiaryStratumID INTEGER, FromStateClassID INTEGER, FromStockTypeID INTEGER, TransitionTypeID INTEGER, ToStratumID INTEGER, ToStateClassID INTEGER, ToStockTypeID INTEGER, FlowGroupID INTEGER, Amount DOUBLE)");
            store.ExecuteNonQuery("INSERT INTO SF_OutputFlow(ScenarioID, Iteration, Timestep, FromStratumID, FromSecondaryStratumID, FromTertiaryStratumID, FromStateClassID, FromStockTypeID, TransitionTypeID, ToStratumID, ToStateClassID, ToStockTypeID, FlowGroupID, Amount) SELECT ScenarioID, Iteration, Timestep, FromStratumID, FromSecondaryStratumID, FromTertiaryStratumID, FromStateClassID, FromStockTypeID, TransitionTypeID, ToStratumID, ToStateClassID, ToStockTypeID, FlowTypeID, Amount FROM TEMP_TABLE");
            store.ExecuteNonQuery("DROP TABLE TEMP_TABLE");
         
            store.ExecuteNonQuery("ALTER TABLE SF_OutputStock RENAME TO TEMP_TABLE");
            store.ExecuteNonQuery("CREATE TABLE SF_OutputStock(ScenarioID INTEGER, Iteration INTEGER, Timestep INTEGER, StratumID INTEGER, SecondaryStratumID INTEGER, TertiaryStratumID INTEGER, StateClassID INTEGER, StockGroupID INTEGER, Amount DOUBLE)");
            store.ExecuteNonQuery("INSERT INTO SF_OutputStock(ScenarioID, Iteration, Timestep, StratumID, SecondaryStratumID, TertiaryStratumID, StateClassID, StockGroupID, Amount) SELECT ScenarioID, Iteration, Timestep, StratumID, SecondaryStratumID, TertiaryStratumID, StateClassID, StockTypeID, Amount FROM TEMP_TABLE");
            store.ExecuteNonQuery("DROP TABLE TEMP_TABLE");

            //Update ?GroupID column to have the value for its associated ?TypeID 

            foreach (KeyValuePair<int, int> keys in StockIDTranslator)
            {
                string q = string.Format(CultureInfo.InvariantCulture, "UPDATE SF_OutputStock SET StockGroupID={0} WHERE StockGroupID={1}", keys.Value, keys.Key);
                store.ExecuteNonQuery(q);
            }

            foreach (KeyValuePair<int, int> keys in FlowIDTranslator)
            {
                string q = string.Format(CultureInfo.InvariantCulture, "UPDATE SF_OutputFlow SET FlowGroupID={0} WHERE FlowGroupID={1}", keys.Value, keys.Key);
                store.ExecuteNonQuery(q);
            }

            //Update the indexes for these tables
                
            UpdateProvider.CreateIndex(store, "SF_OutputFlow", new[] { "ScenarioID", "Iteration", "Timestep", "FromStratumID", "FromSecondaryStratumID", "FromTertiaryStratumID", "FromStateClassID", "FromStockTypeID", "TransitionTypeID", "ToStratumID", "ToStateClassID", "ToStockTypeID", "FlowGroupID" });
            UpdateProvider.CreateIndex(store, "SF_OutputStock", new[] { "ScenarioID", "Iteration", "Timestep", "StratumID", "SecondaryStratumID", "TertiaryStratumID", "StateClassID", "StockGroupID" });

            //(4.) Above

            if (store.TableExists("SF_OutputSpatialStockType"))
            {
                MigrateStockTypeSpatialData(StockIDTranslator, store);
            }

            if (store.TableExists("SF_OutputSpatialFlowType"))
            {
                MigrateFlowTypeSpatialData(FlowIDTranslator, store);
            }

            //(5.) Above

            if (store.TableExists("SF_OutputSpatialStockType"))
            {
                store.ExecuteNonQuery("DROP TABLE SF_OutputSpatialStockType");
            }

            if (store.TableExists("SF_OutputSpatialFlowType"))
            {
                store.ExecuteNonQuery("DROP TABLE SF_OutputSpatialFlowType");
            }
        }

        private static void MigrateStockTypeSpatialData(Dictionary<int, int> stockIDTranslator, DataStore store)
        {
            string BaseOutputFolderName = GetOutputFolderNameCheckLegacy(store);

            if (BaseOutputFolderName == null || !Directory.Exists(BaseOutputFolderName))
            {
                return;
            }

            DataTable Scenarios = store.CreateDataTableFromQuery("SELECT DISTINCT ScenarioID FROM SF_OutputSpatialStockType", "Scenarios");

            foreach (DataRow ScenarioRow in Scenarios.Rows)
            {
                int sid = (int)(long)ScenarioRow["ScenarioID"];
                string BaseScenarioFolderName = string.Format(CultureInfo.InvariantCulture, "Scenario-{0}", sid);
                string FullScenarioFolderName = Path.Combine(BaseOutputFolderName, BaseScenarioFolderName);
                string StockTypeOutputFolderName = Path.Combine(FullScenarioFolderName, "SF_OutputSpatialStockType");
                string StockGroupOutputFolderName = Path.Combine(FullScenarioFolderName, "SF_OutputSpatialStockGroup");

                if (Directory.Exists(StockTypeOutputFolderName))
                {
                    if (!Directory.Exists(StockGroupOutputFolderName))
                    {
                        Directory.CreateDirectory(StockGroupOutputFolderName);
                    }

                    string StockTypeQuery = string.Format(CultureInfo.InvariantCulture, "SELECT * FROM SF_OutputSpatialStockType WHERE ScenarioID={0}", sid);
                    DataTable StockTypeRecords = store.CreateDataTableFromQuery(StockTypeQuery, "StockTypeRecords");

                    foreach (DataRow dr in StockTypeRecords.Rows)
                    {
                        int ScenarioId = (int)(long)dr["ScenarioID"];
                        int Iteration = (int)(long)dr["Iteration"];
                        int Timestep = (int)(long)dr["Timestep"];
                        int FlowTypeId = (int)(long)dr["StockTypeID"];
                        string Filename = (string)dr["Filename"];
                        object Band = dr["Band"];

                        if (Band == null || Band == DBNull.Value)
                        {
                            Band = "NULL";
                        }

                        string[] BaseSourceFileNameSplit = Filename.Split(new char[] { '.' }, 2);

                        string BaseTargetFileName = string.Format(CultureInfo.InvariantCulture, 
                            "stkg_{0}.{1}", stockIDTranslator[FlowTypeId], BaseSourceFileNameSplit[1]);

                        string SourceFileName = Path.Combine(StockTypeOutputFolderName, Filename);
                        string TargetFileName = Path.Combine(StockGroupOutputFolderName, BaseTargetFileName);
                        string SourceVRTFileName = Path.Combine(StockTypeOutputFolderName, Path.GetFileNameWithoutExtension(Filename) + ".vrt");
                        string TargetVRTFileName = Path.Combine(StockGroupOutputFolderName, Path.GetFileNameWithoutExtension(BaseTargetFileName) + ".vrt");

                        //We always want a new record

                        string StockGroupInsertQuery = string.Format(CultureInfo.InvariantCulture,
                            "INSERT INTO SF_OutputSpatialStockGroup(ScenarioID,Iteration,Timestep,StockGroupID,Filename,Band) VALUES({0},{1},{2},{3},'{4}',{5})",
                            ScenarioId, Iteration, Timestep, stockIDTranslator[FlowTypeId], BaseTargetFileName, Band);

                        store.ExecuteNonQuery(StockGroupInsertQuery);

                        //But we might have already moved the file(s) if there is multi-banding.

                        if (File.Exists(SourceFileName))
                        {
                            if (!File.Exists(TargetFileName))
                            {
                                File.Move(SourceFileName, TargetFileName);
                            }
                        }

                        if (File.Exists(SourceVRTFileName))
                        {
                            if (!File.Exists(TargetVRTFileName))
                            {
                                File.Move(SourceVRTFileName, TargetVRTFileName);
                            }
                        }                        
                    }

                    Directory.Delete(StockTypeOutputFolderName);
                }
            }
        }

        private static void MigrateFlowTypeSpatialData(Dictionary<int, int> flowIDTranslator, DataStore store)
        {
            string BaseOutputFolderName = GetOutputFolderNameCheckLegacy(store);

            if (BaseOutputFolderName == null || !Directory.Exists(BaseOutputFolderName))
            {
                return;
            }

            DataTable Scenarios = store.CreateDataTableFromQuery("SELECT DISTINCT ScenarioID FROM SF_OutputSpatialFlowType", "Scenarios");

            foreach (DataRow ScenarioRow in Scenarios.Rows)
            {
                int sid = (int)(long)ScenarioRow["ScenarioID"];
                string BaseScenarioFolderName = string.Format(CultureInfo.InvariantCulture, "Scenario-{0}", sid);
                string FullScenarioFolderName = Path.Combine(BaseOutputFolderName, BaseScenarioFolderName);
                string FlowTypeOutputFolderName = Path.Combine(FullScenarioFolderName, "SF_OutputSpatialFlowType");
                string FlowGroupOutputFolderName = Path.Combine(FullScenarioFolderName, "SF_OutputSpatialFlowGroup");

                if (Directory.Exists(FlowTypeOutputFolderName))
                {
                    if (!Directory.Exists(FlowGroupOutputFolderName))
                    {
                        Directory.CreateDirectory(FlowGroupOutputFolderName);
                    }

                    string FlowTypeQuery = string.Format(CultureInfo.InvariantCulture, "SELECT * FROM SF_OutputSpatialFlowType WHERE ScenarioID={0}", sid);
                    DataTable FlowTypeRecords = store.CreateDataTableFromQuery(FlowTypeQuery, "FlowTypeRecords");

                    foreach (DataRow dr in FlowTypeRecords.Rows)
                    {
                        int ScenarioId = (int)(long)dr["ScenarioID"];
                        int Iteration = (int)(long)dr["Iteration"];
                        int Timestep = (int)(long)dr["Timestep"];
                        int FlowTypeId = (int)(long)dr["FlowTypeID"];
                        string Filename = (string)dr["Filename"];
                        object Band = dr["Band"];

                        if (Band == null || Band == DBNull.Value)
                        {
                            Band = "NULL";
                        }

                        string[] BaseSourceFileNameSplit = Filename.Split(new char[] { '.' }, 2);

                        string BaseTargetFileName = string.Format(CultureInfo.InvariantCulture,
                            "stkg_{0}.{1}", flowIDTranslator[FlowTypeId], BaseSourceFileNameSplit[1]);

                        string SourceFileName = Path.Combine(FlowTypeOutputFolderName, Filename);
                        string TargetFileName = Path.Combine(FlowGroupOutputFolderName, BaseTargetFileName);
                        string SourceVRTFileName = Path.Combine(FlowTypeOutputFolderName, Path.GetFileNameWithoutExtension(Filename) + ".vrt");
                        string TargetVRTFileName = Path.Combine(FlowGroupOutputFolderName, Path.GetFileNameWithoutExtension(BaseTargetFileName) + ".vrt");

                        //We always want a new record

                        string FlowGroupInsertQuery = string.Format(CultureInfo.InvariantCulture,
                            "INSERT INTO SF_OutputSpatialFlowGroup(ScenarioID,Iteration,Timestep,FlowGroupID,Filename,Band) VALUES({0},{1},{2},{3},'{4}',{5})",
                            ScenarioId, Iteration, Timestep, flowIDTranslator[FlowTypeId], BaseTargetFileName, Band);

                        store.ExecuteNonQuery(FlowGroupInsertQuery);

                        //But we might have already moved the file(s) if there is multi-banding.

                        if (File.Exists(SourceFileName))
                        {
                            if (!File.Exists(TargetFileName))
                            {
                                File.Move(SourceFileName, TargetFileName);
                            }
                        }

                        if (File.Exists(SourceVRTFileName))
                        {
                            if (!File.Exists(TargetVRTFileName))
                            {
                                File.Move(SourceVRTFileName, TargetVRTFileName);
                            }
                        }
                    }

                    Directory.Delete(FlowTypeOutputFolderName);
                }
            }
        }

        private static string GetOutputFolderNameCheckLegacy(DataStore store)
        {
            DataRow SysFolderRow = null;
            string ColName = "OutputFolderName";
            string TableName = "SSim_SysFolder";

            if (!store.TableExists(TableName))
            {
                TableName = "core_SysFolder";
            }

            if (store.TableExists(TableName))
            {
                DataTable SysFolderTable = store.CreateDataTable(TableName);
                Debug.Assert(SysFolderTable.Rows.Count == 0 || SysFolderTable.Rows.Count == 1);

                if (SysFolderTable.Rows.Count == 1)
                {
                    SysFolderRow = SysFolderTable.Rows[0];
                }
            }

            string f = null;
                               
            if ((SysFolderRow != null) &&
                (SysFolderRow[ColName] != DBNull.Value) && 
                (!string.IsNullOrWhiteSpace((string)SysFolderRow[ColName])))
            {
                f = Convert.ToString(SysFolderRow[ColName], CultureInfo.InvariantCulture);
            }
            else
            {
                f = store.DataStoreConnection.ConnectionString + ".output";
            }

            if (f == null || !Directory.Exists(f))
            {
                return null;
            }

            return f;
        }

        /// <summary>
        /// SF0000100
        /// </summary>
        /// <param name="store"></param>
        /// <remarks>See comments in caller function for this dummy routine...</remarks>
        private static void SF0000100(DataStore store)
        {
            return;
        }

        /// <summary>
        /// SF0000101
        /// </summary>
        /// <param name="store"></param>
        /// <remarks>
        /// This update adds new fields to SF_FlowPathway and SF_OutputFlow to support the
        /// Lateral Flows feature.  It also adds new fields to SF_OutputOptions for this.
        /// </remarks>
        private static void SF0000101(DataStore store)
        {
            if (store.TableExists("SF_FlowPathway"))
            {
                store.ExecuteNonQuery("ALTER TABLE SF_FlowPathway RENAME TO TEMP_TABLE");

                store.ExecuteNonQuery("CREATE TABLE SF_FlowPathway( " +
                    "FlowPathwayID                INTEGER PRIMARY KEY AUTOINCREMENT, " +
                    "ScenarioID                   INTEGER, " +
                    "Iteration                    INTEGER, " +
                    "Timestep                     INTEGER, " +
                    "FromStratumID                INTEGER, " +
                    "FromSecondaryStratumID       INTEGER, " +
                    "FromTertiaryStratumID        INTEGER, " +
                    "FromStateClassID             INTEGER, " +
                    "FromAgeMin                   INTEGER, " +
                    "FromStockTypeID              INTEGER, " +
                    "ToStratumID                  INTEGER, " +
                    "ToStateClassID               INTEGER, " +
                    "ToAgeMin                     INTEGER, " +
                    "ToStockTypeID                INTEGER, " +
                    "TransitionGroupID            INTEGER, " +
                    "StateAttributeTypeID         INTEGER, " +
                    "FlowTypeID                   INTEGER, " +
                    "Multiplier                   DOUBLE, " +
                    "TransferToStratumID          INTEGER, " +
                    "TransferToSecondaryStratumID INTEGER, " +
                    "TransferToTertiaryStratumID  INTEGER, " +
                    "TransferToStateClassID       INTEGER, " +
                    "TransferToAgeMin             INTEGER)"
                );

                store.ExecuteNonQuery("INSERT INTO SF_FlowPathway( " + 
                    "ScenarioID, " + 
                    "Iteration, " + 
                    "Timestep,  " + 
                    "FromStratumID, " + 
                    "FromStateClassID,  " + 
                    "FromAgeMin, " + 
                    "FromStockTypeID,  " + 
                    "ToStratumID, " + 
                    "ToStateClassID,  " + 
                    "ToAgeMin, " + 
                    "ToStockTypeID,  " + 
                    "TransitionGroupID,  " + 
                    "StateAttributeTypeID,  " + 
                    "FlowTypeID,  " + 
                    "Multiplier) " + 
                    "SELECT " +
                    "ScenarioID,  " + 
                    "Iteration, " + 
                    "Timestep,  " + 
                    "FromStratumID, " + 
                    "FromStateClassID,  " + 
                    "FromAgeMin, " + 
                    "FromStockTypeID,  " + 
                    "ToStratumID, " + 
                    "ToStateClassID,  " + 
                    "ToAgeMin, " + 
                    "ToStockTypeID,  " + 
                    "TransitionGroupID,  " + 
                    "StateAttributeTypeID,  " + 
                    "FlowTypeID, " +
                    "Multiplier " +  
                    "FROM TEMP_TABLE");

                store.ExecuteNonQuery("DROP TABLE TEMP_TABLE");

                store.ExecuteNonQuery("DROP INDEX IF EXISTS SF_FlowPathway_Index");
                store.ExecuteNonQuery("CREATE INDEX SF_FlowPathway_Index ON SF_FlowPathway(ScenarioID)");
            }

            if (store.TableExists("SF_OutputFlow"))
            {
                store.ExecuteNonQuery("ALTER TABLE SF_OutputFlow ADD COLUMN EndStratumID INTEGER");
                store.ExecuteNonQuery("ALTER TABLE SF_OutputFlow ADD COLUMN EndSecondaryStratumID INTEGER");
                store.ExecuteNonQuery("ALTER TABLE SF_OutputFlow ADD COLUMN EndTertiaryStratumID INTEGER");
                store.ExecuteNonQuery("ALTER TABLE SF_OutputFlow ADD COLUMN EndStateClassID INTEGER");
                store.ExecuteNonQuery("ALTER TABLE SF_OutputFlow ADD COLUMN EndMinAge INTEGER");

                string[] cols = new string[] {
                    "ScenarioID",
                    "Iteration",
                    "Timestep",
                    "FromStratumID",
                    "FromSecondaryStratumID",
                    "FromTertiaryStratumID",
                    "FromStateClassID",
                    "FromStockTypeID",
                    "TransitionTypeID",
                    "ToStratumID",
                    "ToStateClassID",
                    "ToStockTypeID",
                    "FlowGroupID", 
                    "EndStratumID",
                    "EndSecondaryStratumID",
                    "EndTertiaryStratumID",
                    "EndStateClassID",
                    "EndMinAge"
                };

                UpdateProvider.CreateIndex(store, "SF_OutputFlow", cols);
            }

            if (store.TableExists("SF_OutputOptions"))
            {
                store.ExecuteNonQuery("ALTER TABLE SF_OutputOptions ADD COLUMN LateralOutputFL INTEGER");
                store.ExecuteNonQuery("ALTER TABLE SF_OutputOptions ADD COLUMN LateralOutputFLTimesteps INTEGER");
            }
        }

        /// <summary>
        /// SF0000102
        /// </summary>
        /// <param name="store"></param>
        /// <remarks></remarks>
        private static void SF0000102(DataStore store)
        {
            UpdateProvider.RenameTablesWithPrefix(store, "SF_", "stsimsf_");
            UpdateProvider.RenameInputFoldersWithPrefix(store, "SF_", "stsimsf_");
            UpdateProvider.RenameOutputFoldersWithPrefix(store, "SF_", "stsimsf_");

            store.ExecuteNonQuery("DROP INDEX IF EXISTS SF_FlowPathway_Index");
            store.ExecuteNonQuery("DROP INDEX IF EXISTS SF_FlowMultiplier_Index");
            store.ExecuteNonQuery("DROP INDEX IF EXISTS SF_StockTransitionMultiplier_Index");
            store.ExecuteNonQuery("DROP INDEX IF EXISTS SF_StockTypeGroupMembership_Index");
            store.ExecuteNonQuery("DROP INDEX IF EXISTS SF_FlowTypeGroupMembership_Index");
            store.ExecuteNonQuery("DROP INDEX IF EXISTS SF_OutputFlow_Index");
            store.ExecuteNonQuery("DROP INDEX IF EXISTS SF_OutputStock_Index");

            UpdateProvider.CreateIndex(store, "stsimsf_FlowPathway", new[] { "ScenarioID" });
            UpdateProvider.CreateIndex(store, "stsimsf_FlowMultiplier", new[] { "ScenarioID" });
            UpdateProvider.CreateIndex(store, "stsimsf_StockTransitionMultiplier", new[] { "ScenarioID" });
            UpdateProvider.CreateIndex(store, "stsimsf_StockTypeGroupMembership", new[] { "StockTypeID", "StockGroupID" });
            UpdateProvider.CreateIndex(store, "stsimsf_FlowTypeGroupMembership", new[] { "FlowTypeID", "FlowGroupID" });
            UpdateProvider.CreateIndex(store, "stsimsf_OutputFlow", new[] { "ScenarioID", "Iteration", "Timestep", "FromStratumID", "FromSecondaryStratumID", "FromTertiaryStratumID", "FromStateClassID", "FromStockTypeID", "TransitionTypeID", "ToStratumID", "ToStateClassID", "ToStockTypeID", "FlowGroupID", "EndStratumID", "EndSecondaryStratumID", "EndTertiaryStratumID", "EndStateClassID", "EndMinAge" });
            UpdateProvider.CreateIndex(store, "stsimsf_OutputStock", new[] { "ScenarioID", "Iteration", "Timestep", "StratumID", "SecondaryStratumID", "TertiaryStratumID", "StateClassID", "StockGroupID" });

            if (store.TableExists("corestime_Charts"))
            {
                store.ExecuteNonQuery("UPDATE corestime_Charts SET Criteria = REPLACE(Criteria, 'SF_', 'stsimsf_')");
            }

            if (store.TableExists("corestime_Maps"))
            {
                store.ExecuteNonQuery("UPDATE corestime_Maps SET Criteria = REPLACE(Criteria, 'SF_', 'stsimsf_')");
                store.ExecuteNonQuery("UPDATE corestime_Maps SET Criteria = REPLACE(Criteria, 'stkg', 'stsimsf_stkg')");
                store.ExecuteNonQuery("UPDATE corestime_Maps SET Criteria = REPLACE(Criteria, 'flog', 'stsimsf_flog')");
                store.ExecuteNonQuery("UPDATE corestime_Maps SET Criteria = REPLACE(Criteria, 'lflog', 'stsimsf_lflog')");
            }
        }

        /// <summary>
        /// SF0000103
        /// </summary>
        /// <param name="store"></param>
        /// <remarks>
        /// This update adds AgeMin and AgeMax fields to the Flow Multipliers table.
        /// </remarks>
        private static void SF0000103(DataStore store)
        {
            if (store.TableExists("stsimsf_FlowMultiplier"))
            {
                store.ExecuteNonQuery("ALTER TABLE stsimsf_FlowMultiplier RENAME TO TEMP_TABLE");
                store.ExecuteNonQuery("CREATE TABLE stsimsf_FlowMultiplier(FlowMultiplierID INTEGER PRIMARY KEY AUTOINCREMENT, ScenarioID INTEGER, Iteration INTEGER, Timestep INTEGER, StratumID INTEGER, SecondaryStratumID INTEGER, TertiaryStratumID INTEGER, StateClassID INTEGER, AgeMin INTEGER, AgeMax INTEGER, FlowGroupID INTEGER, FlowMultiplierTypeID INTEGER, Value DOUBLE, DistributionType INTEGER, DistributionFrequencyID INTEGER, DistributionSD DOUBLE, DistributionMin DOUBLE, DistributionMax DOUBLE)");
                store.ExecuteNonQuery("INSERT INTO stsimsf_FlowMultiplier(ScenarioID, Iteration, Timestep, StratumID, SecondaryStratumID, TertiaryStratumID, StateClassID, FlowGroupID, FlowMultiplierTypeID, Value, DistributionType, DistributionFrequencyID, DistributionSD, DistributionMin, DistributionMax) SELECT ScenarioID, Iteration, Timestep, StratumID, SecondaryStratumID, TertiaryStratumID, StateClassID, FlowGroupID, FlowMultiplierTypeID, Value, DistributionType, DistributionFrequencyID, DistributionSD, DistributionMin, DistributionMax FROM TEMP_TABLE");
                store.ExecuteNonQuery("DROP TABLE TEMP_TABLE");

                UpdateProvider.CreateIndex(store, "stsimsf_FlowMultiplier", new[] { "ScenarioID" });
            }
        }

        /// <summary>
        /// SF0000104
        /// </summary>
        /// <param name="store"></param>
        /// <remarks>
        /// This update will apply namespace prefixes to chart and map criteria.
        /// </remarks>
        private static void SF0000104(DataStore store)
        {
            if (store.TableExists("corestime_Charts"))
            {
                store.ExecuteNonQuery("UPDATE corestime_Charts SET Criteria = REPLACE(Criteria, 'StockGroups', 'stsimsf_StockVariablesGroup')");
                store.ExecuteNonQuery("UPDATE corestime_Charts SET Criteria = REPLACE(Criteria, 'FlowGroups', 'stsimsf_FlowVariablesGroup')");
            }

            UpdateProvider.RenameChartVariable(store, "stockgroupdensity", "stsimsf_StockGroupDensityVariable");
            UpdateProvider.RenameChartVariable(store, "stockgroup", "stsimsf_StockGroupVariable");
            UpdateProvider.RenameChartVariable(store, "flowgroupdensity", "stsimsf_FlowGroupDensityVariable");
            UpdateProvider.RenameChartVariable(store, "flowgroup", "stsimsf_FlowGroupVariable");

            UpdateProvider.RenameMapVariable(store, "stkg", "stsimsf_stkg");
            UpdateProvider.RenameMapVariable(store, "flog", "stsimsf_flog");
            UpdateProvider.RenameMapVariable(store, "lflog", "stsimsf_lflog");
        }

        /// <summary>
        /// SF0000105
        /// </summary>
        /// <param name="store"></param>
        private static void SF0000105(DataStore store)
        {
            if (store.TableExists("stsimsf_OutputOptions"))
            {
                store.ExecuteNonQuery("ALTER TABLE stsimsf_OutputOptions ADD COLUMN AvgSpatialOutputST INTEGER");
                store.ExecuteNonQuery("ALTER TABLE stsimsf_OutputOptions ADD COLUMN AvgSpatialOutputSTTimesteps INTEGER");
                store.ExecuteNonQuery("ALTER TABLE stsimsf_OutputOptions ADD COLUMN AvgSpatialOutputFL INTEGER");
                store.ExecuteNonQuery("ALTER TABLE stsimsf_OutputOptions ADD COLUMN AvgSpatialOutputFLTimesteps INTEGER");
            }
        }
    }
}
