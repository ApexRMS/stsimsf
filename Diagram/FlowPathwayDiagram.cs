//*********************************************************************************************
// STSimStockFlow: A SyncroSim Module for the ST-Sim Stocks and Flows Add-In.
//
// Copyright © 2007-2017 Apex Resource Management Solution Ltd. (ApexRMS). All rights reserved.
//
//*********************************************************************************************

using System;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using SyncroSim.Core;
using SyncroSim.Common.Forms;

namespace SyncroSim.STSimStockFlow
{
	internal class FlowPathwayDiagram : BoxArrowDiagram
	{
		private DataFeed m_DataFeed;
		private DataSheet m_FlowDiagramSheet;
		private DataTable m_FlowDiagramData;
		private DataSheet m_FlowPathwaySheet;
		private DataTable m_FlowPathwayData;
		private Dictionary<int, StockTypeShape> m_ShapeLookup = new Dictionary<int, StockTypeShape>();
		private bool m_IsFilterApplied;

		public FlowPathwayDiagram()
		{
			this.AutoScroll = false;

			this.Rows = Constants.DIAGRAM_MAX_ROWS;
			this.Columns = Constants.DIAGRAM_MAX_COLUMNS;
			this.CellPadding = Constants.DIAGRAM_SHAPE_PADDING;
			this.BoxSize = Constants.DIAGRAM_SHAPE_SIZE;
			this.LanesBetweenShapes = Constants.DIAGRAM_LANES_BETWEEN_SHAPES;
		}

		public void Load(DataFeed dataFeed)
		{
			Debug.Assert(this.m_DataFeed == null);

			this.m_DataFeed = dataFeed;

			this.m_FlowDiagramSheet = dataFeed.Scenario.GetDataSheet(Constants.DATASHEET_FLOW_PATHWAY_DIAGRAM_NAME);
			this.m_FlowDiagramData = this.m_FlowDiagramSheet.GetData();

			this.m_FlowPathwaySheet = dataFeed.Scenario.GetDataSheet(Constants.DATASHEET_FLOW_PATHWAY_NAME);
			this.m_FlowPathwayData = this.m_FlowPathwaySheet.GetData();

			this.RefreshDiagram();
		}

		public void RefreshDiagram()
		{
			List<int> Selected = new List<int>();

			foreach (StockTypeShape s in this.SelectedShapes)
			{
				Selected.Add(s.StockTypeId);
			}

			this.RefreshStockTypeShapes();
			this.RefreshLocationLookups();
			this.RefreshFlowPathwayLines();
			this.ConfigureShapeReadOnlySetting();

			foreach (int id in Selected)
			{
				if (this.m_ShapeLookup.ContainsKey(id))
				{
					this.SelectStockTypeShape(id);
				}
			}

			this.Invalidate();
		}

		public void FilterFlowPathways(DiagramFilterCriteria criteria)
		{
			this.m_IsFilterApplied = IsFilterApplied(criteria);

			foreach (FlowPathwayLine l in this.Lines)
			{
				l.IsVisible = criteria.FlowTypes[l.Pathway.FlowTypeId];
			}

			this.Invalidate();
		}

		public bool CanOpenStockTypes()
		{
			return (this.SelectedShapes.Count() > 0 && !this.IsReadOnly);
		}

		public bool CanCutStockTypes()
		{
			return (this.SelectedShapes.Count() > 0 && !this.IsReadOnly);
		}

		public void AddStockTypeShape(int stockTypeId)
		{
			Debug.Assert(this.GetStockTypeShape(stockTypeId) == null);

			string Location = this.GetNextLocation();

			if (Location == null)
			{
				FormsUtilities.ErrorMessageBox("There are no more available locations on the diagram.");
				return;
			}

			this.m_FlowDiagramSheet.BeginAddRows();
			DataRow NewRow = this.m_FlowDiagramData.NewRow();

			NewRow[Constants.STOCK_TYPE_ID_COLUMN_NAME] = stockTypeId;
			NewRow[Constants.LOCATION_COLUMN_NAME] = Location;

			this.m_FlowDiagramData.Rows.Add(NewRow);
			this.m_FlowDiagramSheet.EndAddRows();

			this.RefreshDiagram();
			this.SelectStockTypeShape(stockTypeId);
		}

		public void SelectStockTypeShape(int stockTypeId)
		{
			if (this.m_ShapeLookup.ContainsKey(stockTypeId))
			{
				StockTypeShape s = this.m_ShapeLookup[stockTypeId];
				this.SelectShape(s);
			}
		}

		public void DeleteSelectedStockTypeShapes()
		{
			this.m_FlowDiagramSheet.BeginDeleteRows();
			this.m_FlowPathwaySheet.BeginDeleteRows();

			foreach (StockTypeShape Shape in this.SelectedShapes)
			{
				this.DeleteStockTypeShape(Shape);
			}

			this.m_FlowDiagramSheet.EndDeleteRows();
			this.m_FlowPathwaySheet.EndDeleteRows();

			this.RefreshDiagram();
		}

		public StockTypeShape GetStockTypeShape(int stockTypeId)
		{

#if DEBUG
			string q = string.Format(CultureInfo.InvariantCulture, "StockTypeID={0}", stockTypeId);
			DataRow[] r = this.m_FlowDiagramData.Select(q);
			Debug.Assert(r.Count() == 0 || r.Count() == 1);
#endif

			if (this.m_ShapeLookup.ContainsKey(stockTypeId))
			{
				return this.m_ShapeLookup[stockTypeId];
			}
			else
			{
				return null;
			}
		}

		public void ConfigureShapeReadOnlySetting()
		{
			foreach (StockTypeShape Shape in this.Shapes)
			{
				Shape.IsReadOnly = this.IsReadOnly;
			}

			this.Invalidate();
		}

		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaint(e);

			if (this.m_IsFilterApplied)
			{
				e.Graphics.DrawImage(Properties.Resources.Filter16x16, new Point(4, 4));
			}
		}

		protected override void OnDropSelectedShapes(DiagramDragEventArgs e)
		{
			base.OnDropSelectedShapes(e);

			foreach (StockTypeShape Shape in this.SelectedShapes)
			{
				Debug.Assert(this.WorkspaceRectangle.Contains(Shape.Bounds));

				DataRow row = this.GetStockTypeRecord(Shape.StockTypeId);
				row[Constants.LOCATION_COLUMN_NAME] = RowColToLocation(Shape.Row, Shape.Column);
			}

			this.RefreshLocationLookups();
			this.RefreshFlowPathwayLines();

			//DEVTODO: It would be better not to do this, but there is no obvious way to tell
			//if the user is about to drop the shapes they are dragging.  Is there?

			this.m_FlowDiagramSheet.BeginModifyRows();
			this.m_FlowDiagramSheet.EndModifyRows();

			this.Focus();
		}

		private void RefreshStockTypeShapes()
		{
			this.RemoveAllShapes();
			List<StockTypeShape> l = this.CreateStockTypeShapes();

			foreach (StockTypeShape s in l)
			{
				Rectangle rc = this.GetShapeRectangleFromRowCol(s.Row, s.Column);

				s.SetDimensions(rc.Width, rc.Height);
				s.SetLocation(rc.X, rc.Y);
				s.CreateConnectorPoints();

				this.FillOutgoingPathways(s);
				this.AddShape(s);
			}
		}

		private List<StockTypeShape> CreateStockTypeShapes()
		{
			List<StockTypeShape> l = new List<StockTypeShape>();
			DataSheet ds = this.m_DataFeed.Project.GetDataSheet(Constants.DATASHEET_STOCK_TYPE_NAME);

			foreach (DataRow dr in this.m_FlowDiagramData.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					int StockTypeId = Convert.ToInt32(dr[Constants.STOCK_TYPE_ID_COLUMN_NAME]);
					string DisplayName = ds.ValidationTable.GetDisplayName(StockTypeId);
					int ShapeRow = -1;
					int ShapeColumn = -1;
					string Location = Convert.ToString(dr[Constants.LOCATION_COLUMN_NAME]);

					LocationToRowCol(Location, ref ShapeRow, ref ShapeColumn);

					if (ShapeRow > Constants.DIAGRAM_MAX_ROWS || ShapeColumn > Constants.DIAGRAM_MAX_COLUMNS)
					{
						Debug.Assert(false);
						continue;
					}

					StockTypeShape NewShape = new StockTypeShape(StockTypeId, DisplayName);

					NewShape.Row = ShapeRow;
					NewShape.Column = ShapeColumn;

					l.Add(NewShape);
				}
			}

			return l;
		}

		private void DeleteStockTypeShape(StockTypeShape shape)
		{
			DataRow dr = this.GetStockTypeRecord(shape.StockTypeId);

			if (dr.RowState == DataRowState.Added)
			{
				this.m_FlowDiagramData.Rows.Remove(dr);
			}
			else
			{
				dr.Delete();
			}
		}

		private void FillOutgoingPathways(StockTypeShape shape)
		{
			DataRow[] OutgoingRows = this.GetOutgoingPathways(shape.StockTypeId);

			foreach (DataRow dr in OutgoingRows)
			{
				Debug.Assert(Convert.ToInt32(dr[Constants.FROM_STOCK_TYPE_ID_COLUMN_NAME]) == shape.StockTypeId);

				FlowPathway p = CreateFlowPathway(dr);
				shape.OutgoingFlowPathways.Add(p);
			}
		}

		private DataRow[] GetOutgoingPathways(int stockTypeId)
		{
			string Query = string.Format(CultureInfo.InvariantCulture, "{0}={1}", Constants.FROM_STOCK_TYPE_ID_COLUMN_NAME, stockTypeId);
			return this.m_FlowPathwayData.Select(Query, null);
		}

		private static FlowPathway CreateFlowPathway(DataRow dr)
		{
			int? Iteration = null;
			int? Timestep = null;
			int? FromStratumId = null;
			int? FromStateClassId = null;
			int? FromMinimumAge = null;
			int FromStockTypeId = 0;
			int? ToStratumId = null;
			int? ToStateClassId = null;
			int? ToMinimumAge = null;
			int ToStockTypeId = 0;
			int TransitionGroupId = 0;
			int? StateAttributeTypeId = null;
			int FlowTypeId = 0;
			double Multiplier = 0;

			if (dr[Constants.ITERATION_COLUMN_NAME] != DBNull.Value)
			{
				Iteration = Convert.ToInt32(dr[Constants.ITERATION_COLUMN_NAME]);
			}

			if (dr[Constants.TIMESTEP_COLUMN_NAME] != DBNull.Value)
			{
				Timestep = Convert.ToInt32(dr[Constants.TIMESTEP_COLUMN_NAME]);
			}

			if (dr[Constants.FROM_STRATUM_ID_COLUMN_NAME] != DBNull.Value)
			{
				FromStratumId = Convert.ToInt32(dr[Constants.FROM_STRATUM_ID_COLUMN_NAME]);
			}

			if (dr[Constants.FROM_STATECLASS_ID_COLUMN_NAME] != DBNull.Value)
			{
				FromStateClassId = Convert.ToInt32(dr[Constants.FROM_STATECLASS_ID_COLUMN_NAME]);
			}

			if (dr[Constants.FROM_MIN_AGE_COLUMN_NAME] != DBNull.Value)
			{
				FromMinimumAge = Convert.ToInt32(dr[Constants.FROM_MIN_AGE_COLUMN_NAME]);
			}

			FromStockTypeId = Convert.ToInt32(dr[Constants.FROM_STOCK_TYPE_ID_COLUMN_NAME]);

			if (dr[Constants.TO_STRATUM_ID_COLUMN_NAME] != DBNull.Value)
			{
				ToStratumId = Convert.ToInt32(dr[Constants.TO_STRATUM_ID_COLUMN_NAME]);
			}

			if (dr[Constants.TO_STATECLASS_ID_COLUMN_NAME] != DBNull.Value)
			{
				ToStateClassId = Convert.ToInt32(dr[Constants.TO_STATECLASS_ID_COLUMN_NAME]);
			}

			if (dr[Constants.TO_MIN_AGE_COLUMN_NAME] != DBNull.Value)
			{
				ToMinimumAge = Convert.ToInt32(dr[Constants.TO_MIN_AGE_COLUMN_NAME]);
			}

			ToStockTypeId = Convert.ToInt32(dr[Constants.TO_STOCK_TYPE_ID_COLUMN_NAME]);

			if (dr[Constants.TRANSITION_GROUP_ID_COLUMN_NAME] != DBNull.Value)
			{
				TransitionGroupId = Convert.ToInt32(dr[Constants.TRANSITION_GROUP_ID_COLUMN_NAME]);
			}
			else
			{
				TransitionGroupId = 0;
			}

			if (dr[Constants.STATE_ATTRIBUTE_TYPE_ID_COLUMN_NAME] != DBNull.Value)
			{
				StateAttributeTypeId = Convert.ToInt32(dr[Constants.STATE_ATTRIBUTE_TYPE_ID_COLUMN_NAME]);
			}

			FlowTypeId = Convert.ToInt32(dr[Constants.FLOW_TYPE_ID_COLUMN_NAME]);
			Multiplier = Convert.ToDouble(dr[Constants.MULTIPLIER_COLUMN_NAME]);

			FlowPathway p = new FlowPathway(
                Iteration, Timestep, FromStratumId, FromStateClassId, FromMinimumAge, FromStockTypeId, 
                ToStratumId, ToStateClassId, ToMinimumAge, ToStockTypeId, TransitionGroupId, 
                StateAttributeTypeId, FlowTypeId, Multiplier);

			return p;
		}

		private void RefreshLocationLookups()
		{
			this.m_ShapeLookup.Clear();

			foreach (StockTypeShape s in this.Shapes)
			{
				this.m_ShapeLookup.Add(s.StockTypeId, s);
			}

			Debug.Assert(this.m_ShapeLookup.Count() == this.Shapes.Count());
		}

		private static bool IsFilterApplied(DiagramFilterCriteria criteria)
		{
			foreach (bool b in criteria.FlowTypes.Values)
			{
				if (!b)
				{
					return true;
				}
			}

			return false;
		}

		private DataRow GetStockTypeRecord(int stockTypeId)
		{
			foreach (DataRow dr in this.m_FlowDiagramData.Rows)
			{
				if (dr.RowState != DataRowState.Deleted)
				{
					int id = Convert.ToInt32(dr[Constants.STOCK_TYPE_ID_COLUMN_NAME]);

					if (id == stockTypeId)
					{
						Debug.Assert(this.GetStockTypeShape(stockTypeId) != null);
						return dr;
					}
				}           
			}

			Debug.Assert(this.GetStockTypeShape(stockTypeId) != null);
			return null;
		}

		private void RefreshFlowPathwayLines()
		{
			this.ClearLines();

			if (this.Shapes.Count() == 0)
			{
				return;
			}

			foreach (StockTypeShape Shape in this.Shapes)
			{
				Shape.ResetConnectorPoints();
			}

			Dictionary<string, FlowPathwayLine> AlreadyAdded = new Dictionary<string, FlowPathwayLine>();

			foreach (StockTypeShape FromShape in this.Shapes)
			{
				foreach (FlowPathway Pathway in FromShape.OutgoingFlowPathways)
				{
					FlowPathwayLine l = null;
					StockTypeShape ToShape = this.m_ShapeLookup[Pathway.ToStockTypeId];
					string Lookup = string.Format(CultureInfo.InvariantCulture, "k1{0}-k2{1}", FromShape.StockTypeId, Pathway.ToStockTypeId);

					if (AlreadyAdded.ContainsKey(Lookup))
					{
						FlowPathwayLine ExistingLine = AlreadyAdded[Lookup];

						l = new FlowPathwayLine(Constants.DIAGRAM_FLOW_PATHWAY_LINE_COLOR, Pathway);
						l.RenderPath = ExistingLine.CloneRenderPath();
					}
					else
					{
						if (FromShape == ToShape)
						{
							l = CreatePathwayLineToSelf(FromShape, Pathway);
						}
						else
						{
							l = new FlowPathwayLine(Constants.DIAGRAM_FLOW_PATHWAY_LINE_COLOR, Pathway);
							this.FillLineSegments(FromShape, ToShape, l, BoxArrowDiagramConnectorMode.Vertical);
						}

						AlreadyAdded.Add(Lookup, l);
					}

					this.AddLine(l);
				}
			}
		}

		private static FlowPathwayLine CreatePathwayLineToSelf(StockTypeShape shape, FlowPathway pathway)
		{
			FlowPathwayLine l = new FlowPathwayLine(Constants.DIAGRAM_FLOW_PATHWAY_LINE_COLOR, pathway);
			const int PT_CIRCLE_RADIUS = 10;
			int lrx = shape.Bounds.X + shape.Bounds.Width - PT_CIRCLE_RADIUS;
			int lry = shape.Bounds.Y + shape.Bounds.Height - PT_CIRCLE_RADIUS;
			Rectangle rc = new Rectangle(lrx, lry, 2 * PT_CIRCLE_RADIUS, 2 * PT_CIRCLE_RADIUS);

			l.AddEllipse(rc);
			return l;
		}

		private string GetNextLocation()
		{
			if (this.GetShapeAt(this.CurrentMouseRow, this.CurrentMouseColumn) == null)
			{
				string ColLetter = Convert.ToString((char)((int)'A' + this.CurrentMouseColumn));
				string RowLetter = Convert.ToString(this.CurrentMouseRow + 1);

				return ColLetter + RowLetter;
			}

			for (int col = 0; col < Constants.DIAGRAM_MAX_COLUMNS; col++)
			{
				for (int row = 0; row < Constants.DIAGRAM_MAX_ROWS; row++)
				{
					if (this.GetShapeAt(row, col) == null)
					{
						string ColLetter = Convert.ToString((char)((int)'A' + col));
						string RowLetter = (row + 1).ToString();

						return (ColLetter + RowLetter);
					}
				}
			}

			return null;
		}

		private static void LocationToRowCol(string location, ref int row, ref int column)
		{
			string LocUpper = location.ToUpper(CultureInfo.InvariantCulture);

			string CharPart = LocUpper.Substring(0, 1);
			string NumPart = LocUpper.Substring(1, LocUpper.Length - 1);

			char[] chars = CharPart.ToCharArray();
			char c = chars[0];
            int CharVal = ((int)c - (int)'A');

			column = CharVal;
			row = int.Parse(NumPart) - 1;

			Debug.Assert(column >= 0 && row >= 0);
		}

		private static string RowColToLocation(int row, int column)
		{
			Debug.Assert(column < 26);

			string s = Convert.ToString((char)((int)'A' + column));
			s = s + (row + 1).ToString(CultureInfo.InvariantCulture);

			return s;
		}
	}
}