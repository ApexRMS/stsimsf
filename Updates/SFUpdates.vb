'*********************************************************************************************
' STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
'
' Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
'
'*********************************************************************************************

Imports SyncroSim.Core
Imports System.Globalization
Imports System.Reflection

<ObfuscationAttribute(Exclude:=True, ApplyToMembers:=False)>
Class SFUpdates
    Inherits UpdateProvider

    ''' <summary>
    ''' Performs the database updates for Stocks and Flows
    ''' </summary>
    ''' <param name="store"></param>
    ''' <param name="currentSchemaVersion"></param>
    ''' <remarks>
    ''' NOTE: We are starting at version 5 because the legacy version of Stocks and Flows leaves
    ''' things at version 4.  (Actually, I think there may have been a mistake here and it was left at 
    ''' version 3.  However, I am not going to touch this now, and it should not matter anyway because
    ''' the upgrade will eventually be performed and then things will behave normally for the rest of
    ''' the lifetime of this module.)
    ''' </remarks>
    Public Overrides Sub PerformUpdate(store As DataStore, currentSchemaVersion As Integer)

        If (currentSchemaVersion < 5) Then
            SF0000005(store)
        End If

        If (currentSchemaVersion < 6) Then
            SF0000006(store)
        End If

        If (currentSchemaVersion < 7) Then
            SF0000007(store)
        End If

        If (currentSchemaVersion < 8) Then
            SF0000008(store)
        End If

        If (currentSchemaVersion < 9) Then
            SF0000009(store)
        End If

        If (currentSchemaVersion < 10) Then
            SF0000010(store)
        End If

        If (currentSchemaVersion < 11) Then
            SF0000011(store)
        End If

        If (currentSchemaVersion < 12) Then
            SF0000012(store)
        End If

        If (currentSchemaVersion < 13) Then
            SF0000013(store)
        End If

        If (currentSchemaVersion < 14) Then
            SF0000014(store)
        End If

        If (currentSchemaVersion < 15) Then
            SF0000015(store)
        End If

        If (currentSchemaVersion < 16) Then
            SF0000016(store)
        End If

        If (currentSchemaVersion < 17) Then
            SF0000017(store)
        End If

    End Sub

    ''' <summary>
    ''' SF0000005
    ''' </summary>
    ''' <param name="store"></param>
    ''' <remarks>
    ''' Adds (or enhances) indexes on the following tables:
    ''' 
    ''' SF_OutputStock
    ''' SF_StockTypeGroupMembership
    ''' SF_OutputFlow
    ''' SF_FlowTypeGroupMembership
    ''' 
    ''' NOTE: It's possible that the indexes on the output tables exist as a result of testing...
    ''' </remarks>
    Private Shared Sub SF0000005(ByVal store As DataStore)

        store.ExecuteNonQuery("DROP INDEX IF EXISTS SF_OutputStock_Index")
        store.ExecuteNonQuery("CREATE INDEX SF_OutputStock_Index ON SF_OutputStock(ScenarioID, Iteration, Timestep, StratumID, SecondaryStratumID, StateClassID, StockTypeID)")

        If (store.TableExists("SF_StockTypeGroupMembership")) Then
            store.ExecuteNonQuery("CREATE INDEX SF_StockTypeGroupMembership_Index ON SF_StockTypeGroupMembership(ScenarioID, StockTypeID, StockGroupID)")
        End If

        store.ExecuteNonQuery("DROP INDEX IF EXISTS SF_OutputFlow_Index")
        store.ExecuteNonQuery("CREATE INDEX SF_OutputFlow_Index ON SF_OutputFlow(ScenarioID, Iteration, Timestep, FromStratumID, FromSecondaryStratumID, FromStateClassID, FromStockTypeID, TransitionTypeID, ToStratumID, ToSecondaryStratumID, ToStateClassID, ToStockTypeID, FlowTypeID)")

        If (store.TableExists("SF_FlowTypeGroupMembership")) Then
            store.ExecuteNonQuery("CREATE INDEX SF_FlowTypeGroupMembership_Index ON SF_FlowTypeGroupMembership(ScenarioID, FlowTypeID, FlowGroupID)")
        End If

    End Sub

    ''' <summary>
    ''' SF0000006
    ''' </summary>
    ''' <param name="store"></param>
    ''' <remarks>
    ''' This update will remove the "group-group" tables which are obsolete
    ''' </remarks>
    Private Shared Sub SF0000006(ByVal store As DataStore)

        If (store.TableExists("SF_StockGroupGroupMembership")) Then
            store.DeleteSchema("SF_StockGroupGroupMembership")
        End If

        If (store.TableExists("SF_FlowGroupGroupMembership")) Then
            store.DeleteSchema("SF_FlowGroupGroupMembership")
        End If

    End Sub

    ''' <summary>
    ''' SF0000007
    ''' </summary>
    ''' <param name="store"></param>
    ''' <remarks>
    ''' This update removes Iteration, Timestep, and SecondaryStratumID from the FlowPathways table
    ''' </remarks>
    Private Shared Sub SF0000007(ByVal store As DataStore)

        If (store.TableExists("SF_FlowPathway")) Then

            store.ExecuteNonQuery("ALTER TABLE SF_FlowPathway RENAME TO TEMP_TABLE")
            store.ExecuteNonQuery("CREATE TABLE SF_FlowPathway(FlowPathwayID INTEGER PRIMARY KEY AUTOINCREMENT, ScenarioID INTEGER, FromStratumID INTEGER, FromStateClassID INTEGER, FromAgeMin INTEGER, FromStockTypeID INTEGER, ToStratumID INTEGER, ToStateClassID INTEGER, ToAgeMin INTEGER, ToStockTypeID INTEGER, TransitionGroupID INTEGER, StateAttributeTypeID INTEGER, FlowTypeID INTEGER, Multiplier DOUBLE)")
            store.ExecuteNonQuery("INSERT INTO SF_FlowPathway(ScenarioID, FromStratumID, FromStateClassID, FromAgeMin, FromStockTypeID, ToStratumID, ToStateClassID, ToAgeMin, ToStockTypeID, TransitionGroupID, StateAttributeTypeID, FlowTypeID, Multiplier) SELECT ScenarioID, FromStratumID, FromStateClassID, FromAgeMin, FromStockTypeID, ToStratumID, ToStateClassID, ToAgeMin, ToStockTypeID, TransitionGroupID, StateAttributeTypeID, FlowTypeID, Multiplier FROM TEMP_TABLE")
            store.ExecuteNonQuery("DROP TABLE TEMP_TABLE")

        End If

    End Sub

    ''' <summary>
    ''' SF0000008
    ''' </summary>
    ''' <param name="store"></param>
    ''' <remarks>
    ''' This update will drop the 'output ? group multiplier' tables that are obsolete.
    ''' </remarks>
    Private Shared Sub SF0000008(ByVal store As DataStore)

        If (store.TableExists("SF_OutputStockGroupMultiplier")) Then
            store.ExecuteNonQuery("DROP TABLE SF_OutputStockGroupMultiplier")
        End If

        If (store.TableExists("SF_OutputFlowGroupMultiplier")) Then
            store.ExecuteNonQuery("DROP TABLE SF_OutputFlowGroupMultiplier")
        End If

    End Sub

    ''' <summary>
    ''' SF0000009
    ''' </summary>
    ''' <param name="store"></param>
    ''' <remarks>
    ''' This update will remove the unused ToSecondaryStratumID field from SF_OutputFlow
    ''' </remarks>
    Private Shared Sub SF0000009(ByVal store As DataStore)

        If (store.TableExists("SF_OutputFlow")) Then

            store.ExecuteNonQuery("DROP INDEX IF EXISTS SF_OutputFlow_Index")

            store.ExecuteNonQuery("ALTER TABLE SF_OutputFlow RENAME TO TEMP_TABLE")
            store.ExecuteNonQuery("CREATE TABLE SF_OutputFlow(ScenarioID INTEGER, Iteration INTEGER, Timestep INTEGER, FromStratumID INTEGER, FromSecondaryStratumID INTEGER, FromStateClassID INTEGER, FromStockTypeID INTEGER, TransitionTypeID INTEGER, ToStratumID INTEGER, ToStateClassID INTEGER, ToStockTypeID INTEGER, FlowTypeID INTEGER, Amount DOUBLE)")
            store.ExecuteNonQuery("INSERT INTO SF_OutputFlow(ScenarioID, Iteration, Timestep, FromStratumID, FromSecondaryStratumID, FromStateClassID, FromStockTypeID, TransitionTypeID, ToStratumID, ToStateClassID, ToStockTypeID, FlowTypeID, Amount) SELECT ScenarioID, Iteration, Timestep, FromStratumID, FromSecondaryStratumID, FromStateClassID, FromStockTypeID, TransitionTypeID, ToStratumID, ToStateClassID, ToStockTypeID, FlowTypeID, Amount FROM TEMP_TABLE")
            store.ExecuteNonQuery("DROP TABLE TEMP_TABLE")

            store.ExecuteNonQuery("CREATE INDEX SF_OutputFlow_Index ON SF_OutputFlow(ScenarioID, Iteration, Timestep, FromStratumID, FromSecondaryStratumID, FromStateClassID, FromStockTypeID, TransitionTypeID, ToStratumID, ToStateClassID, ToStockTypeID, FlowTypeID)")

        End If

    End Sub

    ''' <summary>
    ''' SF0000010
    ''' </summary>
    ''' <param name="store"></param>
    ''' <remarks>
    ''' Flow diagrams introduce a new table called SF_FlowPathwayDiagram which contains each (unique) Stock Type
    ''' found SF_FlowPathway along with a location for the diagram.  The items in this table must match the ones
    ''' found in SF_FlowPathway exactly or the application will crash.  This means that we need to populate the 
    ''' SF_FlowPathwayDiagram table if flow data exists as follows:
    ''' 
    ''' (1.) First, delete any diagram data that an internal (Apex) user might have created.  Sorry, but your
    '''      diagrams will have to be reformatted after this update.
    ''' 
    ''' (2.) If flow data exists in ANY scenario we are going to apply this update.
    ''' 
    ''' (3.) If there are more stock types than spaces in the diagram we are going to fail the update.  I can't imagine
    '''      that this would be possible, however, since there are 26x256 spaces per diagram...
    ''' </remarks>
    Private Shared Sub SF0000010(ByVal store As DataStore)

        'Delete any existing diagram data no matter what

        If (store.TableExists("SF_FlowPathwayDiagram")) Then
            store.ExecuteNonQuery("DELETE FROM SF_FlowPathwayDiagram")
        End If

        'If there is no flow pathway data at all then there is nothing to do

        If (Not store.TableExists("SF_FlowPathway")) Then
            Return
        End If

        If (Not CBool(store.ExecuteScalar("SELECT EXISTS(SELECT 1 FROM SF_FlowPathway)"))) Then
            Return
        End If

        'Get the existing scenarios there are no scenarios there is nothing to do

        Dim Scenarios As DataTable = store.CreateDataTable("SSim_Scenario")

        If (Scenarios.Rows.Count = 0) Then
            Return
        End If

        'Create the flow diagram data table
        If (Not store.TableExists("SF_FlowPathwayDiagram")) Then
            store.ExecuteNonQuery("CREATE TABLE SF_FlowPathwayDiagram(FlowPathwayDiagramID INTEGER PRIMARY KEY AUTOINCREMENT, ScenarioID INTEGER, StockTypeID INTEGER, Location TEXT)")
        End If

        'For each scenario, populate the SF_FlowPathwayDiagram if there is flow data

        Dim RowColToLocation As Func(Of Integer, Integer, String) =
            Function(row As Integer, column As Integer) As String

                Debug.Assert(column < 26)

                Dim s As String = CStr(ChrW(Asc("A") + column))
                s = s & (row + 1).ToString(CultureInfo.InvariantCulture)

                Return s

            End Function

        For Each ScenarioRow As DataRow In Scenarios.Rows

            Dim ScenarioId As Integer = CInt(ScenarioRow("ScenarioID"))
            Dim FlowPathways As DataTable = store.CreateDataTable("SF_FlowPathway", "ScenarioID", ScenarioId)

            If (FlowPathways.Rows.Count = 0) Then
                Continue For
            End If

            Dim StockTypeIds As New Dictionary(Of Integer, Boolean)

            For Each drflow As DataRow In FlowPathways.Rows

                Dim FromStockId As Integer = CInt(drflow("FromStockTypeID"))

                If (Not StockTypeIds.ContainsKey(FromStockId)) Then
                    StockTypeIds.Add(FromStockId, True)
                End If

                Dim ToStockId As Integer = CInt(drflow("ToStockTypeId"))

                If (Not StockTypeIds.ContainsKey(ToStockId)) Then
                    StockTypeIds.Add(ToStockId, True)
                End If

            Next

            If (StockTypeIds.Count > (26 * 256)) Then

                ExceptionUtils.ThrowInvalidOperationException(
                    "There are too many stock types.  Please clear your flow pathways and try this update again.")

            End If

            Dim CurrentRow As Integer = 0
            Dim CurrentCol As Integer = 0

            For Each StockTypeId As Integer In StockTypeIds.Keys

                Dim Location As String = RowColToLocation(CurrentRow, CurrentCol)

                Dim Query As String = String.Format(CultureInfo.InvariantCulture,
                    "INSERT INTO SF_FlowPathwayDiagram(ScenarioID, StockTypeID, Location) VALUES({0}, {1}, '{2}')",
                    ScenarioId, StockTypeId, Location)

                store.ExecuteNonQuery(Query)
                CurrentCol += 1

                If (CurrentCol = 26) Then

                    CurrentCol = 0
                    CurrentRow += 1

                End If

                Debug.Assert(CurrentRow <= 255)

            Next

        Next

    End Sub

    ''' <summary>
    ''' SF0000011
    ''' </summary>
    ''' <param name="store"></param>
    ''' <remarks>
    ''' Data feeds with distributions now get their distribution types from Stats_DistributionType instead
    ''' of a hard coded list.  This means that we must update every table so that it uses the new project
    ''' specific primary key IDs.
    ''' </remarks>
    Private Shared Sub SF0000011(ByVal store As DataStore)

        Dim Projects As DataTable = store.CreateDataTable("SSim_Project")
        Dim Scenarios As DataTable = store.CreateDataTable("SSim_Scenario")
        Dim DistTables As New Dictionary(Of Integer, DataTable)

        For Each ProjectRow As DataRow In Projects.Rows

            Dim ProjectId As Integer = CInt(ProjectRow("ProjectID"))

            Dim DistributionTypes As DataTable =
                 store.CreateDataTableFromQuery(String.Format(CultureInfo.InvariantCulture,
                     "SELECT * FROM STime_DistributionType WHERE ProjectID={0}",
                     ProjectId), "DistributionTypes")

            Debug.Assert(DistributionTypes.Rows.Count = 4)
            DistTables.Add(ProjectId, DistributionTypes)

        Next

        UpdateDistributions("SF_FlowMultiplier", store, Scenarios, DistTables)

    End Sub

    Private Shared Sub UpdateDistributions(
        ByVal tableName As String,
        ByVal store As DataStore,
        ByVal scenarios As DataTable,
        ByVal distTables As Dictionary(Of Integer, DataTable))

        If (Not store.TableExists(tableName)) Then
            Return
        End If

        store.ExecuteNonQuery(String.Format(CultureInfo.InvariantCulture, "UPDATE {0} SET DistributionType=-1 WHERE DistributionType=0", tableName)) 'Normal
        store.ExecuteNonQuery(String.Format(CultureInfo.InvariantCulture, "UPDATE {0} SET DistributionType=-2 WHERE DistributionType=1", tableName)) 'Beta
        store.ExecuteNonQuery(String.Format(CultureInfo.InvariantCulture, "UPDATE {0} SET DistributionType=-3 WHERE DistributionType=2", tableName)) 'Uniform

        For Each ScenarioRow As DataRow In scenarios.Rows

            Dim ScenarioId As Integer = CInt(ScenarioRow("ScenarioID"))
            Dim ProjectId As Integer = CInt(ScenarioRow("ProjectID"))
            Dim DistributionTypes As DataTable = distTables(ProjectId)

            For Each DistTypeRow As DataRow In DistributionTypes.Rows

                Dim DistTypeId As Integer = CInt(DistTypeRow("DistributionTypeID"))
                Dim DistTypeName As String = CStr(DistTypeRow("Name"))
                Dim TempDistId As Integer
                Dim DoUpdate As Boolean = True

                If (DistTypeName = "Normal") Then
                    TempDistId = -1
                ElseIf (DistTypeName = "Beta") Then
                    TempDistId = -2
                ElseIf (DistTypeName = "Uniform") Then
                    TempDistId = -3
                Else
                    DoUpdate = False
                    Debug.Assert(DistTypeName = "Uniform Integer")
                End If

                If (DoUpdate) Then

                    store.ExecuteNonQuery(String.Format(CultureInfo.InvariantCulture,
                        "UPDATE {0} SET DistributionType={1} WHERE DistributionType={2} AND ScenarioID={3}",
                        tableName, DistTypeId, TempDistId, ScenarioId))

                End If

            Next

        Next

    End Sub

    ''' <summary>
    ''' SF0000012
    ''' </summary>
    ''' <param name="store"></param>
    ''' <remarks>
    ''' This update adds a DistributionFrequency column to the relevant Stock and Flow tables
    ''' </remarks>
    Private Shared Sub SF0000012(ByVal store As DataStore)

        If (store.TableExists("SF_FlowMultiplier")) Then

            Debug.Assert(CInt(StochasticTime.DistributionFrequency.Always) = 0)
            Debug.Assert(CInt(StochasticTime.DistributionFrequency.Iteration) = 1)
            Debug.Assert(CInt(StochasticTime.DistributionFrequency.Timestep) = 2)

            store.ExecuteNonQuery("ALTER TABLE SF_FlowMultiplier ADD COLUMN DistributionFrequencyID INTEGER")
            store.ExecuteNonQuery(String.Format(CultureInfo.InvariantCulture, "UPDATE SF_FlowMultiplier SET DistributionFrequencyID={0} WHERE DistributionType IS NOT NULL", CInt(StochasticTime.DistributionFrequency.Iteration)))

        End If

    End Sub

    ''' <summary>
    ''' SF0000013
    ''' </summary>
    ''' <param name="store"></param>
    ''' <remarks>
    ''' This update adds indexes to various tables to improve performance with large data sets
    ''' </remarks>
    Private Shared Sub SF0000013(ByVal store As DataStore)

        If (store.TableExists("SF_FlowMultiplier")) Then
            store.ExecuteNonQuery("CREATE INDEX SF_FlowMultiplier_Index ON SF_FlowMultiplier(ScenarioID)")
        End If

        If (store.TableExists("SF_FlowPathway")) Then
            store.ExecuteNonQuery("CREATE INDEX SF_FlowPathway_Index ON SF_FlowPathway(ScenarioID)")
        End If

    End Sub

    ''' <summary>
    ''' SF0000014
    ''' </summary>
    ''' <param name="store"></param>
    ''' <remarks>
    ''' This update adds an Iteration field to SF_InitialStockSpatial
    ''' </remarks>
    Private Shared Sub SF0000014(ByVal store As DataStore)

        If (store.TableExists("SF_InitialStockSpatial")) Then

            store.ExecuteNonQuery("ALTER TABLE SF_InitialStockSpatial RENAME TO TEMP_TABLE")
            store.ExecuteNonQuery("CREATE TABLE SF_InitialStockSpatial(InitialStockSpatialID INTEGER PRIMARY KEY AUTOINCREMENT,ScenarioID INTEGER,Iteration INTEGER,StockTypeID INTEGER,RasterFileName TEXT)")
            store.ExecuteNonQuery("INSERT INTO SF_InitialStockSpatial(ScenarioID,StockTypeID,RasterFileName) SELECT ScenarioID,StockTypeID,RasterFileName FROM TEMP_TABLE")
            store.ExecuteNonQuery("DROP TABLE TEMP_TABLE")

        End If

    End Sub

    ''' <summary>
    ''' SF0000015
    ''' </summary>
    ''' <param name="store"></param>
    ''' <remarks>New columns for SF_FlowPathway if it exists</remarks>
    Private Shared Sub SF0000015(ByVal store As DataStore)

        If (store.TableExists("SF_FlowPathway")) Then

            store.ExecuteNonQuery("ALTER TABLE SF_FlowPathway RENAME TO TEMP_TABLE")
            store.ExecuteNonQuery("CREATE TABLE SF_FlowPathway(FlowPathwayID INTEGER PRIMARY KEY AUTOINCREMENT, ScenarioID INTEGER, Iteration INTEGER, Timestep INTEGER, FromStratumID INTEGER, FromStateClassID INTEGER, FromAgeMin INTEGER, FromStockTypeID INTEGER, ToStratumID INTEGER, ToStateClassID INTEGER, ToAgeMin INTEGER, ToStockTypeID INTEGER, TransitionGroupID INTEGER, StateAttributeTypeID INTEGER, FlowTypeID INTEGER, Multiplier DOUBLE)")
            store.ExecuteNonQuery("INSERT INTO SF_FlowPathway(ScenarioID, FromStratumID, FromStateClassID, FromAgeMin, FromStockTypeID, ToStratumID, ToStateClassID, ToAgeMin, ToStockTypeID, TransitionGroupID, StateAttributeTypeID, FlowTypeID, Multiplier) SELECT ScenarioID, FromStratumID, FromStateClassID, FromAgeMin, FromStockTypeID, ToStratumID, ToStateClassID, ToAgeMin, ToStockTypeID, TransitionGroupID, StateAttributeTypeID, FlowTypeID, Multiplier FROM TEMP_TABLE")
            store.ExecuteNonQuery("DROP TABLE TEMP_TABLE")

        End If

    End Sub

    ''' <summary>
    ''' SF0000016
    ''' </summary>
    ''' <param name="store"></param>
    ''' <remarks>
    ''' Add missing index on SF_FlowPathway if missing drop</remarks>
    Private Shared Sub SF0000016(ByVal store As DataStore)

        If (store.TableExists("SF_FlowPathway")) Then

            store.ExecuteNonQuery("DROP INDEX IF EXISTS SF_FlowPathway_Index")
            store.ExecuteNonQuery("CREATE INDEX SF_FlowPathway_Index ON SF_FlowPathway(ScenarioID)")

        End If

    End Sub

    ''' <summary>
    ''' SF0000017
    ''' </summary>
    ''' <param name="store"></param>
    ''' <remarks>This update adds a tertiary stratum to all applicable tables</remarks>
    Private Shared Sub SF0000017(ByVal store As DataStore)

        If (store.TableExists("SF_FlowMultiplier")) Then

            store.ExecuteNonQuery("ALTER TABLE SF_FlowMultiplier RENAME TO TEMP_TABLE")
            store.ExecuteNonQuery("CREATE TABLE SF_FlowMultiplier(FlowMultiplierID INTEGER PRIMARY KEY AUTOINCREMENT, ScenarioID INTEGER, Iteration INTEGER, Timestep INTEGER, StratumID INTEGER, SecondaryStratumID INTEGER, TertiaryStratumID INTEGER, StateClassID INTEGER, FlowGroupID INTEGER, Value DOUBLE, DistributionType INTEGER, DistributionFrequencyID INTEGER, DistributionSD DOUBLE, DistributionMin DOUBLE, DistributionMax DOUBLE)")
            store.ExecuteNonQuery("INSERT INTO SF_FlowMultiplier(ScenarioID, Iteration, Timestep, StratumID, SecondaryStratumID, StateClassID, FlowGroupID, Value, DistributionType, DistributionFrequencyID, DistributionSD, DistributionMin, DistributionMax) SELECT ScenarioID, Iteration, Timestep, StratumID, SecondaryStratumID, StateClassID, FlowGroupID, Value, DistributionType, DistributionFrequencyID, DistributionSD, DistributionMin, DistributionMax FROM TEMP_TABLE")
            store.ExecuteNonQuery("DROP TABLE TEMP_TABLE")

        End If

        If (store.TableExists("SF_StockLimit")) Then

            store.ExecuteNonQuery("ALTER TABLE SF_StockLimit RENAME TO TEMP_TABLE")
            store.ExecuteNonQuery("CREATE TABLE SF_StockLimit(StockLimitID INTEGER PRIMARY KEY AUTOINCREMENT, ScenarioID INTEGER, Iteration INTEGER, Timestep INTEGER, StockTypeID INTEGER, StratumID INTEGER, SecondaryStratumID INTEGER, TertiaryStratumID INTEGER, StateClassID INTEGER, StockMinimum DOUBLE, StockMaximum DOUBLE)")
            store.ExecuteNonQuery("INSERT INTO SF_StockLimit(ScenarioID, Iteration, Timestep, StockTypeID, StratumID, SecondaryStratumID, StateClassID, StockMinimum, StockMaximum) SELECT ScenarioID, Iteration, Timestep, StockTypeID, StratumID, SecondaryStratumID, StateClassID, StockMinimum, StockMaximum FROM TEMP_TABLE")
            store.ExecuteNonQuery("DROP TABLE TEMP_TABLE")

        End If

        If (store.TableExists("SF_OutputFlow")) Then

            store.ExecuteNonQuery("ALTER TABLE SF_OutputFlow RENAME TO TEMP_TABLE")
            store.ExecuteNonQuery("CREATE TABLE SF_OutputFlow(ScenarioID INTEGER, Iteration INTEGER, Timestep INTEGER, FromStratumID INTEGER, FromSecondaryStratumID INTEGER, FromTertiaryStratumID INTEGER, FromStateClassID INTEGER, FromStockTypeID INTEGER, TransitionTypeID INTEGER, ToStratumID INTEGER, ToStateClassID INTEGER, ToStockTypeID INTEGER, FlowTypeID INTEGER, Amount DOUBLE)")
            store.ExecuteNonQuery("INSERT INTO SF_OutputFlow(ScenarioID, Iteration, Timestep, FromStratumID, FromSecondaryStratumID, FromStateClassID, FromStockTypeID, TransitionTypeID, ToStratumID, ToStateClassID, ToStockTypeID, FlowTypeID, Amount) SELECT ScenarioID, Iteration, Timestep, FromStratumID, FromSecondaryStratumID, FromStateClassID, FromStockTypeID, TransitionTypeID, ToStratumID, ToStateClassID, ToStockTypeID, FlowTypeID, Amount FROM TEMP_TABLE")
            store.ExecuteNonQuery("DROP TABLE TEMP_TABLE")

        End If

        If (store.TableExists("SF_OutputStock")) Then

            store.ExecuteNonQuery("ALTER TABLE SF_OutputStock RENAME TO TEMP_TABLE")
            store.ExecuteNonQuery("CREATE TABLE SF_OutputStock(ScenarioID INTEGER, Iteration INTEGER, Timestep INTEGER, StratumID INTEGER, SecondaryStratumID INTEGER, TertiaryStratumID INTEGER, StateClassID INTEGER, StockTypeID INTEGER, Amount DOUBLE)")
            store.ExecuteNonQuery("INSERT INTO SF_OutputStock(ScenarioID, Iteration, Timestep, StratumID, SecondaryStratumID, StateClassID, StockTypeID, Amount) SELECT ScenarioID, Iteration, Timestep, StratumID, SecondaryStratumID, StateClassID, StockTypeID, Amount FROM TEMP_TABLE")
            store.ExecuteNonQuery("DROP TABLE TEMP_TABLE")

        End If

    End Sub

End Class
