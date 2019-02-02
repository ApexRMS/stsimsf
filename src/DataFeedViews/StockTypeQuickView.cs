// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.

using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.Globalization;
using System.Collections.Generic;
using SyncroSim.Core;
using SyncroSim.Core.Forms;

namespace SyncroSim.STSimStockFlow
{
	[ObfuscationAttribute(Exclude=true, ApplyToMembers=false)]
	internal partial class StockTypeQuickView
	{
		public StockTypeQuickView()
		{
			InitializeComponent();
		}

		private DataFeed m_DataFeed;
		private List<int> m_StockTypeIds;
		private DataSheet m_FlowDiagramSheet;
		private DataTable m_FlowDiagramData;
		private MultiRowDataFeedView m_FlowPathwayView;
		private BaseDataGridView m_FlowPathwayGrid;
		private DataSheet m_FlowPathwaySheet;
		private DataTable m_FlowPathwayData;
		private bool m_ShowFlowsFrom = true;
		private bool m_ShowFlowsTo;
		private bool m_FromStratumVisible;
		private bool m_FromStateClassVisible;
		private bool m_FromMinAgeVisible;
		private bool m_ToStratumVisible;
		private bool m_ToStateClassVisible;
		private bool m_ToMinAgeVisible;
		private bool m_TransitionGroupVisible;
		private bool m_StateAttributeTypeVisible;

		public void LoadStockTypes(DataFeed dataFeed, List<int> stockTypeIds)
		{
			this.m_DataFeed = dataFeed;
			this.m_StockTypeIds = stockTypeIds;

			WinFormSession sess = (WinFormSession)this.Project.Library.Session;

			this.m_FlowDiagramSheet = this.m_DataFeed.Scenario.GetDataSheet(Constants.DATASHEET_FLOW_PATHWAY_DIAGRAM_NAME);
			this.m_FlowDiagramData = this.m_FlowDiagramSheet.GetData();

			this.m_FlowPathwaySheet = this.m_DataFeed.GetDataSheet(Constants.DATASHEET_FLOW_PATHWAY_NAME);
			this.m_FlowPathwayData = this.m_FlowPathwaySheet.GetData();

			this.m_FlowPathwayView = (MultiRowDataFeedView)sess.CreateMultiRowDataFeedView(dataFeed.Scenario, dataFeed.Scenario);
			this.m_FlowPathwayView.LoadDataFeed(this.m_DataFeed, Constants.DATASHEET_FLOW_PATHWAY_NAME);

			this.m_FlowPathwayGrid = this.m_FlowPathwayView.GridControl;

			this.FilterFlowPathways();
			this.ConfigureContextMenus();
			this.InitializeColumnVisiblity();
			this.UpdateColumnVisibility();
			this.ConfigureColumnsReadOnly();

			this.PanelMain.Controls.Add(this.m_FlowPathwayView);
			this.m_FlowPathwayGrid.PaintGridBorders = false;
			this.m_FlowPathwayView.ManageOptionalColumns = false;

			this.m_FlowPathwayGrid.CellBeginEdit += OnGridCellBeginEdit;
			this.m_FlowPathwayGrid.CellEndEdit += OnGridCellEndEdit;
		}

		private void FilterFlowPathways()
		{
			string filter = this.CreateGridFilterString();
			((BindingSource)this.m_FlowPathwayGrid.DataSource).Filter = filter;
		}

		private string CreateGridFilterString()
		{
			string Filter = CreateIntegerFilterSpec(this.m_StockTypeIds);

			string FromFormatString = "FromStockTypeID IN ({0})";
			string ToFormatString = "ToStockTypeID IN ({0})";

			if (this.m_ShowFlowsFrom)
			{
				return string.Format(CultureInfo.InvariantCulture, FromFormatString, Filter);
			}
			else
			{
				Debug.Assert(this.m_ShowFlowsTo);
				return string.Format(CultureInfo.InvariantCulture, ToFormatString, Filter);
			}       
		}

		private void InitializeColumnVisiblity()
		{
			this.m_FromStratumVisible = DataTableUtilities.TableHasData(this.m_FlowPathwayData, Constants.FROM_STRATUM_ID_COLUMN_NAME);
			this.m_FromStateClassVisible = DataTableUtilities.TableHasData(this.m_FlowPathwayData, Constants.FROM_STATECLASS_ID_COLUMN_NAME);
			this.m_FromMinAgeVisible = DataTableUtilities.TableHasData(this.m_FlowPathwayData, Constants.FROM_MIN_AGE_COLUMN_NAME);
			this.m_ToStratumVisible = DataTableUtilities.TableHasData(this.m_FlowPathwayData, Constants.TO_STRATUM_ID_COLUMN_NAME);
			this.m_ToStateClassVisible = DataTableUtilities.TableHasData(this.m_FlowPathwayData, Constants.TO_STATECLASS_ID_COLUMN_NAME);
			this.m_ToMinAgeVisible = DataTableUtilities.TableHasData(this.m_FlowPathwayData, Constants.TO_MIN_AGE_COLUMN_NAME);
			this.m_TransitionGroupVisible = DataTableUtilities.TableHasData(this.m_FlowPathwayData, Constants.TRANSITION_GROUP_ID_COLUMN_NAME);
			this.m_StateAttributeTypeVisible = DataTableUtilities.TableHasData(this.m_FlowPathwayData, Constants.STATE_ATTRIBUTE_TYPE_ID_COLUMN_NAME);
		}

		private void UpdateColumnVisibility()
		{
			if (this.m_FlowPathwayGrid.CurrentCell != null)
			{
				bool ResetCurrentCell = false;
				int CurrentColumnIndex = this.m_FlowPathwayGrid.CurrentCell.ColumnIndex;
				int CurrentRowIndex = this.m_FlowPathwayGrid.CurrentCell.RowIndex;
				string CurrentColumnName = this.m_FlowPathwayGrid.Columns[CurrentColumnIndex].Name;

				if (CurrentColumnName == Constants.FROM_STRATUM_ID_COLUMN_NAME && (!this.m_FromStratumVisible))
				{
					ResetCurrentCell = true;
				}

				if (ResetCurrentCell)
				{
					this.m_FlowPathwayGrid.CurrentCell = this.m_FlowPathwayGrid.Rows[CurrentRowIndex].Cells[Constants.FROM_STOCK_TYPE_ID_COLUMN_NAME];
				}
			}

			this.m_FlowPathwayGrid.Columns[Constants.FROM_STRATUM_ID_COLUMN_NAME].Visible = this.m_FromStratumVisible;
			this.m_FlowPathwayGrid.Columns[Constants.FROM_STATECLASS_ID_COLUMN_NAME].Visible = this.m_FromStateClassVisible;
			this.m_FlowPathwayGrid.Columns[Constants.FROM_MIN_AGE_COLUMN_NAME].Visible = this.m_FromMinAgeVisible;
			this.m_FlowPathwayGrid.Columns[Constants.TO_STRATUM_ID_COLUMN_NAME].Visible = this.m_ToStratumVisible;
			this.m_FlowPathwayGrid.Columns[Constants.TO_STATECLASS_ID_COLUMN_NAME].Visible = this.m_ToStateClassVisible;
			this.m_FlowPathwayGrid.Columns[Constants.TO_MIN_AGE_COLUMN_NAME].Visible = this.m_ToMinAgeVisible;
			this.m_FlowPathwayGrid.Columns[Constants.TRANSITION_GROUP_ID_COLUMN_NAME].Visible = this.m_TransitionGroupVisible;
			this.m_FlowPathwayGrid.Columns[Constants.STATE_ATTRIBUTE_TYPE_ID_COLUMN_NAME].Visible = this.m_StateAttributeTypeVisible;
		}

		private void ConfigureContextMenus()
		{
			for (int i = this.m_FlowPathwayView.Commands.Count - 1; i >= 0; i--)
			{
				Command c = this.m_FlowPathwayView.Commands[i];

				if (c.Name == "ssim_delete_all" || c.Name == "ssim_import" || c.Name == "ssim_export" || c.Name == "ssim_export_all")
				{
					this.m_FlowPathwayView.Commands.RemoveAt(i);
				}
			}

			string StratumLabel = this.GetStratumLabelTerminology();
			string FromStratum = string.Format(CultureInfo.InvariantCulture, "From {0}", StratumLabel);
			string ToStratum = string.Format(CultureInfo.InvariantCulture, "To {0}", StratumLabel);

			this.m_FlowPathwayView.Commands.Add(new Command("Flows To", OnExecuteFlowsToCommand, OnUpdateFlowsToCommand));
			this.m_FlowPathwayView.Commands.Add(new Command("Flows From", OnExecuteFlowsFromCommand, OnUpdateFlowsFromCommand));
			this.m_FlowPathwayView.Commands.Add(Command.CreateSeparatorCommand());
			this.m_FlowPathwayView.Commands.Add(new Command(FromStratum, OnExecuteFromStratumCommand, OnUpdateFromStratumCommand));
			this.m_FlowPathwayView.Commands.Add(new Command("From State Class", OnExecuteFromStateClassCommand, OnUpdateFromStateClassCommand));
			this.m_FlowPathwayView.Commands.Add(new Command("From Min Age", OnExecuteFromMinAgeCommand, OnUpdateFromMinAgeCommand));
			this.m_FlowPathwayView.Commands.Add(new Command(ToStratum, OnExecuteToStratumCommand, OnUpdateToStratumCommand));
			this.m_FlowPathwayView.Commands.Add(new Command("To State Class", OnExecuteToStateClassCommand, OnUpdateToStateClassCommand));
			this.m_FlowPathwayView.Commands.Add(new Command("To Min Age", OnExecuteToMinAgeCommand, OnUpdateToMinAgeCommand));
			this.m_FlowPathwayView.Commands.Add(new Command("Transition Group", OnExecuteTransitionGroupCommand, OnUpdateTransitionGroupCommand));
			this.m_FlowPathwayView.Commands.Add(new Command("State Attribute Type", OnExecuteStateAttributeTypeCommand, OnUpdateStateAttributeTypeCommand));

			this.m_FlowPathwayView.RefreshContextMenuStrip();

			for (int i = this.m_FlowPathwayGrid.ContextMenuStrip.Items.Count - 1; i >= 0; i--)
			{
				ToolStripItem item = this.m_FlowPathwayGrid.ContextMenuStrip.Items[i];

				if (item.Name == "ssim_optional_column_separator" || item.Name == "ssim_optional_column_item")
				{
					this.m_FlowPathwayGrid.ContextMenuStrip.Items.RemoveAt(i);
				}
			}
		}

		private void OnExecuteFlowsToCommand(Command cmd)
		{
			if (!this.Validate())
			{
				return;
			}

			this.m_ShowFlowsFrom = false;
			this.m_ShowFlowsTo = true;

			this.FilterFlowPathways();
			this.ConfigureColumnsReadOnly();
			this.UpdateColumnVisibility();
		}

		private void OnUpdateFlowsToCommand(Command cmd)
		{
			cmd.IsEnabled = true;
			cmd.IsChecked = this.m_ShowFlowsTo;
		}

		private void OnExecuteFlowsFromCommand(Command cmd)
		{
			if (!this.Validate())
			{
				return;
			}

			this.m_ShowFlowsFrom = true;
			this.m_ShowFlowsTo = false;

			this.FilterFlowPathways();
			this.ConfigureColumnsReadOnly();
			this.UpdateColumnVisibility();
		}

		private void OnUpdateFlowsFromCommand(Command cmd)
		{
			cmd.IsEnabled = true;
			cmd.IsChecked = this.m_ShowFlowsFrom;
		}

		private void OnExecuteFromStratumCommand(Command cmd)
		{
			if (!this.Validate())
			{
				return;
			}

			this.m_FromStratumVisible = (!this.m_FromStratumVisible);
			this.UpdateColumnVisibility();
		}

		private void OnUpdateFromStratumCommand(Command cmd)
		{
			cmd.IsEnabled = true;
			cmd.IsChecked = this.m_FromStratumVisible;
		}

		private void OnExecuteFromStateClassCommand(Command cmd)
		{
			if (!this.Validate())
			{
				return;
			}

			this.m_FromStateClassVisible = (!this.m_FromStateClassVisible);
			this.UpdateColumnVisibility();
		}

		private void OnUpdateFromStateClassCommand(Command cmd)
		{
			cmd.IsEnabled = true;
			cmd.IsChecked = this.m_FromStateClassVisible;
		}

		private void OnExecuteFromMinAgeCommand(Command cmd)
		{
			if (!this.Validate())
			{
				return;
			}

			this.m_FromMinAgeVisible = (!this.m_FromMinAgeVisible);
			this.UpdateColumnVisibility();
		}

		private void OnUpdateFromMinAgeCommand(Command cmd)
		{
			cmd.IsEnabled = true;
			cmd.IsChecked = this.m_FromMinAgeVisible;
		}

		private void OnExecuteToStratumCommand(Command cmd)
		{
			if (!this.Validate())
			{
				return;
			}

			this.m_ToStratumVisible = (!this.m_ToStratumVisible);
			this.UpdateColumnVisibility();
		}

		private void OnUpdateToStratumCommand(Command cmd)
		{
			cmd.IsEnabled = true;
			cmd.IsChecked = this.m_ToStratumVisible;
		}

		private void OnExecuteToStateClassCommand(Command cmd)
		{
			if (!this.Validate())
			{
				return;
			}

			this.m_ToStateClassVisible = (!this.m_ToStateClassVisible);
			this.UpdateColumnVisibility();
		}

		private void OnUpdateToStateClassCommand(Command cmd)
		{
			cmd.IsEnabled = true;
			cmd.IsChecked = this.m_ToStateClassVisible;
		}

		private void OnExecuteToMinAgeCommand(Command cmd)
		{
			if (!this.Validate())
			{
				return;
			}

			this.m_ToMinAgeVisible = (!this.m_ToMinAgeVisible);
			this.UpdateColumnVisibility();
		}

		private void OnUpdateToMinAgeCommand(Command cmd)
		{
			cmd.IsEnabled = true;
			cmd.IsChecked = this.m_ToMinAgeVisible;
		}

		private void OnExecuteTransitionGroupCommand(Command cmd)
		{
			if (!this.Validate())
			{
				return;
			}

			this.m_TransitionGroupVisible = (!this.m_TransitionGroupVisible);
			this.UpdateColumnVisibility();
		}

		private void OnUpdateTransitionGroupCommand(Command cmd)
		{
			cmd.IsEnabled = true;
			cmd.IsChecked = this.m_TransitionGroupVisible;
		}

		private void OnExecuteStateAttributeTypeCommand(Command cmd)
		{
			if (!this.Validate())
			{
				return;
			}

			this.m_StateAttributeTypeVisible = (!this.m_StateAttributeTypeVisible);
			this.UpdateColumnVisibility();
		}

		private void OnUpdateStateAttributeTypeCommand(Command cmd)
		{
			cmd.IsEnabled = true;
			cmd.IsChecked = this.m_StateAttributeTypeVisible;
		}

		private void OnGridCellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
		{
			if (e.ColumnIndex == this.m_FlowPathwayGrid.Columns[Constants.FROM_STOCK_TYPE_ID_COLUMN_NAME].Index)
			{
				this.FilterStockTypeCombo(Constants.FROM_STOCK_TYPE_ID_COLUMN_NAME, e.RowIndex, true);
			}
			else if (e.ColumnIndex == this.m_FlowPathwayGrid.Columns[Constants.TO_STOCK_TYPE_ID_COLUMN_NAME].Index)
			{
				this.FilterStockTypeCombo(Constants.TO_STOCK_TYPE_ID_COLUMN_NAME, e.RowIndex, false);
		    }
		}

		private void OnGridCellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == this.m_FlowPathwayGrid.Columns[Constants.FROM_STOCK_TYPE_ID_COLUMN_NAME].Index)
			{
				this.ResetComboFilter(Constants.FROM_STOCK_TYPE_ID_COLUMN_NAME, e.RowIndex);
			}
			else if (e.ColumnIndex == this.m_FlowPathwayGrid.Columns[Constants.TO_STOCK_TYPE_ID_COLUMN_NAME].Index)
			{
				this.ResetComboFilter(Constants.TO_STOCK_TYPE_ID_COLUMN_NAME, e.RowIndex);
			}
		}

		private void ResetComboFilter(string columnName, int rowIndex)
		{
			DataGridViewRow dgv = this.m_FlowPathwayGrid.Rows[rowIndex];
			DataGridViewComboBoxCell DestStratumCell = (DataGridViewComboBoxCell)dgv.Cells[columnName];
			DataGridViewComboBoxColumn DestStratumColumn = (DataGridViewComboBoxColumn)this.m_FlowPathwayGrid.Columns[columnName];

			DestStratumCell.DataSource = DestStratumColumn.DataSource;
			DestStratumCell.ValueMember = DestStratumColumn.ValueMember;
			DestStratumCell.DisplayMember = DestStratumColumn.DisplayMember;
		}

		private void FilterStockTypeCombo(string columnName, int rowIndex, bool selectedTypesOnly)
		{
			DataSheet ds = this.m_DataFeed.Project.GetDataSheet(Constants.DATASHEET_STOCK_TYPE_NAME);
			DataGridViewRow dgr = this.m_FlowPathwayGrid.Rows[rowIndex];
			DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)dgr.Cells[columnName];
			string filter = null;

			if (selectedTypesOnly)
			{
				filter = this.CreateFromStockTypeFilter();
			}
			else
			{
				filter = this.CreateToStockTypeFilter();
			}

			DataView dv = new DataView(ds.GetData(), filter, ds.DisplayMember, DataViewRowState.CurrentRows);

			cell.DataSource = dv;
			cell.ValueMember = "StockTypeID";
			cell.DisplayMember = "Name";
		}

		private string CreateFromStockTypeFilter()
		{
			string spec = CreateIntegerFilterSpec(this.m_StockTypeIds);
			return string.Format(CultureInfo.InvariantCulture, "StockTypeID IN ({0})", spec);
		}

		private string CreateToStockTypeFilter()
		{
			List<int> lst = new List<int>();

			foreach (DataRow dr in this.m_FlowDiagramData.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					int Id = Convert.ToInt32(dr[Constants.STOCK_TYPE_ID_COLUMN_NAME], CultureInfo.InvariantCulture);

					if (!lst.Contains(Id))
					{
						lst.Add(Id);
					}
				}
			}

			if (lst.Count == 0)
			{
				return "StockTypeID=-1";
			}
			else
			{
				string filter = CreateIntegerFilterSpec(lst);
				return string.Format(CultureInfo.InvariantCulture, "StockTypeID IN ({0})", filter);
			}
		}

		private static string CreateIntegerFilterSpec(List<int> ids)
		{
			StringBuilder sb = new StringBuilder();

			foreach (int i in ids)
			{
				sb.Append(i.ToString(CultureInfo.InvariantCulture));
				sb.Append(",");
			}

			return sb.ToString().TrimEnd(',');
		}

		private string GetStratumLabelTerminology()
		{
			string l = "Stratum";
			DataRow dr = this.m_DataFeed.Project.GetDataSheet("STSim_Terminology").GetDataRow();

			if (dr != null)
			{
				if (dr["PrimaryStratumLabel"] != DBNull.Value)
				{
					l = Convert.ToString(dr["PrimaryStratumLabel"], CultureInfo.InvariantCulture);
				}
			}

			return l;
		}

		private void SetColumnReadOnly(string columnName)
		{
			DataGridViewColumn col = this.m_FlowPathwayGrid.Columns[columnName];
			col.DefaultCellStyle.BackColor = Color.LightGray;
			col.ReadOnly = true;     
		}

		private void ConfigureColumnsReadOnly()
		{
			Debug.Assert(!(this.m_ShowFlowsTo && this.m_ShowFlowsFrom));

			foreach (DataGridViewColumn c in this.m_FlowPathwayGrid.Columns)
			{
				c.DefaultCellStyle.BackColor = Color.White;
				c.ReadOnly = false;
			}

			if (this.m_ShowFlowsTo)
			{
				SetColumnReadOnly(Constants.TO_STOCK_TYPE_ID_COLUMN_NAME);
				SetColumnReadOnly(Constants.FROM_STOCK_TYPE_ID_COLUMN_NAME);
			}
		}
	}
}