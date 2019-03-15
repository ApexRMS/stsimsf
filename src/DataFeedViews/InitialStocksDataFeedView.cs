// stsim-stockflow: SyncroSim Add-On Package (to stsim) for integrating stocks and flows into state-and-transition simulation models in ST-Sim.
// Copyright © 2007-2019 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System.IO;
using System.Reflection;
using System.Windows.Forms;
using SyncroSim.Core;
using SyncroSim.Core.Forms;
using SyncroSim.Common.Forms;

namespace SyncroSim.STSimStockFlow
{
	[ObfuscationAttribute(Exclude=true, ApplyToMembers=false)]
	internal partial class InitialStocksDataFeedView
	{
		public InitialStocksDataFeedView()
		{
			InitializeComponent();
		}

		private DataFeedView m_RasterFilesView;
		private DataGridView m_RasterFilesDataGrid;
		private delegate void DelegateNoArgs();
		private bool m_ColumnsInitialized;
		private bool m_IsEnabled = true;

		private const string BROWSE_BUTTON_TEXT = "...";
		private const int FILE_NAME_COLUMN_INDEX = 2;
		private const int BROWSE_COLUMN_INDEX = 3;

		protected override void InitializeView()
		{
			base.InitializeView();

			DataFeedView v1 = this.Session.CreateMultiRowDataFeedView(this.Scenario, this.ControllingScenario);
			this.PanelNonSpatial.Controls.Add(v1);

			this.m_RasterFilesView = this.Session.CreateMultiRowDataFeedView(this.Scenario, this.ControllingScenario);
			this.PanelSpatial.Controls.Add(this.m_RasterFilesView);
			this.m_RasterFilesDataGrid = ((MultiRowDataFeedView)this.m_RasterFilesView).GridControl;

			this.ConfigureContextMenu();
		}

		public override void LoadDataFeed(Core.DataFeed dataFeed)
		{
			base.LoadDataFeed(dataFeed);

			DataFeedView v1 = (DataFeedView)this.PanelNonSpatial.Controls[0];
			v1.LoadDataFeed(dataFeed, Constants.DATASHEET_INITIAL_STOCK_NON_SPATIAL);

			DataFeedView v2 = (DataFeedView)this.PanelSpatial.Controls[0];
			v2.LoadDataFeed(dataFeed, Constants.DATASHEET_INITIAL_STOCK_SPATIAL);

			if (!this.m_ColumnsInitialized)
			{
				//Add handlers
				this.m_RasterFilesDataGrid.CellEnter += this.OnGridCellEnter;
				this.m_RasterFilesDataGrid.CellMouseDown += this.OnGridCellMouseDown;
				this.m_RasterFilesDataGrid.DataBindingComplete += this.OnGridBindingComplete;
				this.m_RasterFilesDataGrid.KeyDown += this.OnGridKeyDown;

                //Configure columns
                this.m_RasterFilesDataGrid.Columns[FILE_NAME_COLUMN_INDEX].DefaultCellStyle.BackColor = Constants.READONLY_COLUMN_COLOR;

				//Add the browse button column
				DataGridViewButtonColumn BrowseColumn = new DataGridViewButtonColumn();

				BrowseColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
				BrowseColumn.Width = 40;
				BrowseColumn.MinimumWidth = 40;

				this.m_RasterFilesDataGrid.Columns.Add(BrowseColumn);
				this.m_ColumnsInitialized = true;
			}
		}

		/// <summary>
		/// Overrides EnableView
		/// </summary>
		/// <param name="enable"></param>
		/// <remarks>
		/// We override this so that we can manually enable the nested data feed view.  If we don't do this
		/// then the user will not be abled to interact with it at all if it is disabled and this is not really
		/// what we want here.  Also, we want to have control over the enabled state of the buttons.
		/// </remarks>
		public override void EnableView(bool enable)
		{
			if (this.PanelNonSpatial.Controls.Count > 0)
			{
				DataFeedView v = (DataFeedView)this.PanelNonSpatial.Controls[0];
				v.EnableView(enable);
			}

			if (this.PanelSpatial.Controls.Count > 0)
			{
				DataFeedView v = (DataFeedView)this.PanelSpatial.Controls[0];
				v.EnableView(enable);
			}

			this.LabelNonSpatial.Enabled = enable;
			this.LabelSpatial.Enabled = enable;
			this.m_IsEnabled = enable;
		}

		/// <summary>
		/// Handles the cell enter event for the grid
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		private void OnGridCellEnter(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == FILE_NAME_COLUMN_INDEX)
			{
				this.Session.MainForm.BeginInvoke(new DelegateNoArgs(this.OnNewCellEnterAsync), null);
			}
		}

		/// <summary>
		/// Handles the CellMouseDown event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		private void OnGridCellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (e.RowIndex >= 0)
			{
				if (e.ColumnIndex == BROWSE_COLUMN_INDEX)
				{
					this.GetMultiplierFile(e.RowIndex);
				}
			}
		}

		/// <summary>
		/// Handles the grid's KeyDown event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		private void OnGridKeyDown(object sender, KeyEventArgs e)
		{
			if (this.m_RasterFilesDataGrid.CurrentCell != null)
			{
				if (this.m_RasterFilesDataGrid.CurrentCell.ColumnIndex == BROWSE_COLUMN_INDEX)
				{
					if (e.KeyValue == (System.Int32)Keys.Enter)
					{
						this.GetMultiplierFile(this.m_RasterFilesDataGrid.CurrentCell.RowIndex);
						e.Handled = true;
					}
				}
			}       
		}

		/// <summary>
		/// Handles the grid's DataBindingComplete event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		private void OnGridBindingComplete(object sender, System.Windows.Forms.DataGridViewBindingCompleteEventArgs e)
		{
			foreach (DataGridViewRow dgr in this.m_RasterFilesDataGrid.Rows)
			{
				dgr.Cells[BROWSE_COLUMN_INDEX].Value = BROWSE_BUTTON_TEXT;
			}
		}

		/// <summary>
		/// Handles the CellEnter event
		/// </summary>
		/// <param name="argument"></param>
		/// <remarks></remarks>
		private void OnNewCellEnterAsync()
		{
			int Row = this.m_RasterFilesDataGrid.CurrentCell.RowIndex;
			int Col = this.m_RasterFilesDataGrid.CurrentCell.ColumnIndex;

			if (Col == FILE_NAME_COLUMN_INDEX)
			{
				if (ModifierKeys == Keys.Shift)
				{
					Col -= 1;
				}
				else
				{
					Col += 1;
				}

				this.m_RasterFilesDataGrid.CurrentCell = this.m_RasterFilesDataGrid.Rows[Row].Cells[Col];
				this.m_RasterFilesDataGrid.BeginEdit(true);
			}
		}

		/// <summary>
		/// Gets a multiplier file from the user
		/// </summary>
		/// <param name="rowIndex"></param>
		/// <remarks></remarks>
		private void GetMultiplierFile(int rowIndex)
		{
			if (!this.m_IsEnabled)
			{
				return;
			}

			DataSheet ds = this.Scenario.GetDataSheet(Constants.DATASHEET_INITIAL_STOCK_SPATIAL);
			string RasterFile = RasterUtilities.ChooseRasterFileName("Raster File");

			if (RasterFile == null)
			{
				return;
			}

            using (HourGlass h = new HourGlass())
            {
                DataGridViewEditMode OldMode = this.m_RasterFilesDataGrid.EditMode;

			    this.m_RasterFilesDataGrid.EditMode = DataGridViewEditMode.EditProgrammatically;
			    this.m_RasterFilesDataGrid.CurrentCell = this.m_RasterFilesDataGrid.Rows[rowIndex].Cells[FILE_NAME_COLUMN_INDEX];
			    this.m_RasterFilesDataGrid.Rows[rowIndex].Cells[Constants.RASTER_FILE_COLUMN_NAME].Value = Path.GetFileName(RasterFile);
			    this.m_RasterFilesDataGrid.NotifyCurrentCellDirty(true);
			    this.m_RasterFilesDataGrid.BeginEdit(false);
			    this.m_RasterFilesDataGrid.EndEdit();
			    this.m_RasterFilesDataGrid.CurrentCell = this.m_RasterFilesDataGrid.Rows[rowIndex].Cells[BROWSE_COLUMN_INDEX];

			    ds.AddExternalInputFile(RasterFile);
			    this.m_RasterFilesDataGrid.EditMode = OldMode;
            }
		}

		/// <summary>
		/// Configures the context menu for this view
		/// </summary>
		/// <remarks>We only want a subset of the default commands, so remove the others</remarks>
		private void ConfigureContextMenu()
		{
			for (int i = this.m_RasterFilesView.Commands.Count - 1; i >= 0; i--)
			{
				Command c = this.m_RasterFilesView.Commands[i];

				if (c.Name != "ssim_delete" && c.Name != "ssim_delete_all" && c.Name != "ssim_import" && c.Name != "ssim_export_all")
				{
					if (!c.IsSeparator)
					{
						this.m_RasterFilesView.Commands.RemoveAt(i);
					}
				}

				if (c.Name == "ssim_export_all")
				{
					c.DisplayName = "Export...";
				}
			}

			this.m_RasterFilesView.RefreshContextMenuStrip();
		}
	}
}