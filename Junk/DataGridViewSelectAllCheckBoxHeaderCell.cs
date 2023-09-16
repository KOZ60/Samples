using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

public class DataGridViewSelectAllCheckBoxHeaderCell : DataGridViewColumnHeaderCell
{
    protected static class DataGridViewCheckBoxCellRenderer
    {
        private static readonly VisualStyleElement CheckBoxElement
            = VisualStyleElement.Button.CheckBox.UncheckedNormal;

        [ThreadStatic]
        static VisualStyleRenderer visualStyleRenderer;

        public static VisualStyleRenderer CheckBoxRenderer {
            get {
                if (visualStyleRenderer == null) {
                    visualStyleRenderer = new VisualStyleRenderer(CheckBoxElement);
                }
                return visualStyleRenderer;
            }
        }

        public static void DrawCheckBox(Graphics g, Rectangle bounds, CheckBoxState state) {
            CheckBoxRenderer.SetParameters(CheckBoxElement.ClassName, CheckBoxElement.Part, (int)state);
            CheckBoxRenderer.DrawBackground(g, bounds, Rectangle.Truncate(g.ClipBounds));
        }
    }

    private DataGridView _Owner;
    private CheckState? _AllCheckState = null;
    private bool _MouseIsDown;
    private bool _MouseIsOver;

    public DataGridViewSelectAllCheckBoxHeaderCell() {
        // For GetContentBounds
        Value = " ";
    }

    protected void Invalidate() {
        if (Owner != null && Owner.IsHandleCreated) {
            Owner.InvalidateCell(ColumnIndex, -1);
        }
    }

    protected override void OnDataGridViewChanged() {
        base.OnDataGridViewChanged();
        Owner = DataGridView;
    }

    protected DataGridView Owner {
        get {
            return _Owner;
        }
        set {
            if (_Owner != null) {
                _Owner.CurrentCellDirtyStateChanged -= Owner_CurrentCellDirtyStateChanged;
                _Owner.CellValueChanged -= Owner_CellValueChanged;
                _Owner.RowsAdded -= Owner_RowsAdded;
                _Owner.RowsRemoved -= Owner_RowsRemoved;
            }
            _Owner = value;
            if (_Owner != null) {
                _Owner.CurrentCellDirtyStateChanged += Owner_CurrentCellDirtyStateChanged;
                _Owner.CellValueChanged += Owner_CellValueChanged;
                _Owner.RowsAdded += Owner_RowsAdded;
                _Owner.RowsRemoved += Owner_RowsRemoved;
            }
        }
    }

    protected DataGridViewCheckBoxColumn OwnerColumn {
        get {
            if (Owner != null) {
                return Owner.Columns[ColumnIndex] as DataGridViewCheckBoxColumn;
            }
            return null;
        }
    }

    private void Owner_CurrentCellDirtyStateChanged(object sender, EventArgs e) {
        OnCurrentCellDirtyStateChanged(e);
    }

    private void Owner_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
        OnCellValueChanged(e);
    }

    private void Owner_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e) {
        OnRowsRemoved(e);
    }

    private void Owner_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e) {
        OnRowsAdded(e);
    }

    protected virtual void OnCurrentCellDirtyStateChanged(EventArgs e) {
        if (Owner.IsCurrentCellDirty &&
            Owner.CurrentCell.ColumnIndex == ColumnIndex) {
            Owner.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
    }

    protected virtual void OnCellValueChanged(DataGridViewCellEventArgs e) {
        if (e.ColumnIndex == ColumnIndex) {
            ResetState();
        }
    }

    protected virtual void OnRowsRemoved(DataGridViewRowsRemovedEventArgs e) {
        ResetState();
    }

    protected virtual void OnRowsAdded(DataGridViewRowsAddedEventArgs e) {
        ResetState();
    }

    protected void ResetState() {
        _AllCheckState = null;
        Invalidate();
    }

    protected int CommittedRowCount {
        get {
            int rowCount = Owner.RowCount;
            if (rowCount > 0) {
                if (Owner.Rows[rowCount - 1].IsNewRow) {
                    return rowCount - 1;
                } else {
                    return rowCount;
                }
            } else {
                return 0;
            }
        }
    }

    protected virtual CheckState AllCheckState {
        get {
            if (_AllCheckState.HasValue) {
                return _AllCheckState.Value;
            }
            int checkedCount = 0;
            int rowCount = CommittedRowCount;
            if (rowCount > 0) {
                for (int rowIndex = 0; rowIndex < rowCount; rowIndex++) {
                    object value = Owner[ColumnIndex, rowIndex].FormattedValue;
                    if (value != null) {
                        if (value is CheckState state) {
                            switch (state) {
                                case CheckState.Checked:
                                    checkedCount++;
                                    break;
                                case CheckState.Indeterminate:
                                    return CheckState.Indeterminate;
                            }
                        }
                        if (value is bool bChecked && bChecked) {
                            checkedCount++;
                        }
                    }
                }
                if (checkedCount == 0) {
                    _AllCheckState = CheckState.Unchecked;
                } else if (checkedCount == rowCount) {
                    _AllCheckState = CheckState.Checked;
                } else {
                    _AllCheckState = CheckState.Indeterminate;
                }
            } else {
                _AllCheckState = CheckState.Unchecked;
            }
            return _AllCheckState.Value;
        }
        set {
            if (OwnerColumn.ReadOnly) return;
            if (AllCheckState != value) {
                switch (value) {
                    case CheckState.Checked:
                    case CheckState.Unchecked:
                        object setValue;
                        if (OwnerColumn.ThreeState) {
                            if (value == CheckState.Checked) {
                                setValue = OwnerColumn.TrueValue ?? CheckState.Checked;
                            } else {
                                setValue = OwnerColumn.FalseValue ?? CheckState.Unchecked;
                            }
                        } else {
                            if (value == CheckState.Checked) {
                                setValue = OwnerColumn.TrueValue ?? true;
                            } else {
                                setValue = OwnerColumn.FalseValue ?? false;
                            }
                        }
                        int rowCount = CommittedRowCount;
                        if (Owner.IsCurrentCellInEditMode) {
                            Owner.EndEdit();
                        }
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++) {
                            Owner[ColumnIndex, rowIndex].Value = setValue;
                        }
                        break;
                }
                _AllCheckState = value;
                Invalidate();
            }
        }
    }

    protected virtual Rectangle ClickRectangle {
        get {
            using (var g = Owner.CreateGraphics()) {
                return GetContentBounds(g, OwnerColumn.InheritedStyle, -1);
            }
        }
    }

    protected bool MouseIsDown {
        get { return _MouseIsDown; }
        set {
            if (MouseIsDown != value) {
                _MouseIsDown = value;
                Invalidate();
            }
        }
    }

    protected bool MouseIsOver {
        get { return _MouseIsOver; }
        set {
            if (MouseIsOver != value) {
                _MouseIsOver = value;
                Invalidate();
            }
        }
    }

    protected override void OnMouseEnter(int rowIndex) {
        base.OnMouseEnter(rowIndex);
        var mouseLocation = Owner.PointToClient(Control.MousePosition);
        MouseIsOver = ClickRectangle.Contains(mouseLocation.X, mouseLocation.Y);
    }

    protected override void OnMouseLeave(int rowIndex) {
        base.OnMouseLeave(rowIndex);
        MouseIsOver = false;
        MouseIsDown = false;
    }

    protected override void OnMouseMove(DataGridViewCellMouseEventArgs e) {
        base.OnMouseMove(e);
        MouseIsOver = ClickRectangle.Contains(e.X, e.Y);
    }

    protected override void OnMouseDown(DataGridViewCellMouseEventArgs e) {
        base.OnMouseDown(e);
        if (e.Button == MouseButtons.Left && ClickRectangle.Contains(e.X, e.Y)) {
            MouseIsDown = true;
        }
    }

    protected override void OnMouseUp(DataGridViewCellMouseEventArgs e) {
        base.OnMouseUp(e);
        if (e.Button == MouseButtons.Left &&
            ClickRectangle.Contains(e.X, e.Y) &&
            MouseIsDown) {
            if (CommittedRowCount > 0) {
                if (AllCheckState == CheckState.Unchecked) {
                    AllCheckState = CheckState.Checked;
                } else {
                    AllCheckState = CheckState.Unchecked;
                }
            } else {
                AllCheckState = CheckState.Unchecked;
            }
        }
        MouseIsDown = false;
    }

    protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates dataGridViewElementState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts) {
        base.Paint(graphics, clipBounds, cellBounds, rowIndex, dataGridViewElementState, string.Empty, string.Empty, errorText, cellStyle, advancedBorderStyle, paintParts);
        if (!OwnerColumn.ReadOnly) {
            Rectangle checkBoxBounds = GetCheckBoxBounds(graphics, cellBounds,
                                                advancedBorderStyle, CheckBoxState.UncheckedNormal);
            CheckBoxState checkBoxState = GetCheckBoxState();
            DrawCheckBox(graphics, checkBoxBounds, checkBoxState);
        }
    }

    protected virtual void DrawCheckBox(Graphics g, Rectangle checkBoxBounds, CheckBoxState checkBoxState) {
        if (Application.RenderWithVisualStyles) {
            DataGridViewCheckBoxCellRenderer.DrawCheckBox(g, checkBoxBounds, checkBoxState);
        } else {
            ButtonState bs;
            bool mixDraw = false;
            switch (checkBoxState) {
                case CheckBoxState.UncheckedNormal:
                case CheckBoxState.UncheckedHot:
                case CheckBoxState.UncheckedPressed:
                case CheckBoxState.UncheckedDisabled:
                    bs = ButtonState.Normal;
                    break;
                case CheckBoxState.CheckedNormal:
                case CheckBoxState.CheckedHot:
                case CheckBoxState.CheckedPressed:
                case CheckBoxState.CheckedDisabled:
                    bs = ButtonState.Checked;
                    break;
                default:
                    bs = ButtonState.Inactive;
                    mixDraw = true;
                    break;
            }
            switch (checkBoxState) {
                case CheckBoxState.UncheckedPressed:
                case CheckBoxState.CheckedPressed:
                case CheckBoxState.MixedPressed:
                    bs |= ButtonState.Pushed;
                    break;
            }

            if (mixDraw) {
                ControlPaint.DrawMixedCheckBox(g, checkBoxBounds, bs);
            } else {
                ControlPaint.DrawCheckBox(g, checkBoxBounds, bs);
            }
        }
    }

    protected virtual CheckBoxState GetCheckBoxState() {
        if (OwnerColumn.ReadOnly) {
            if (MouseIsDown) {
                return CheckBoxState.UncheckedPressed;
            } else {
                switch (AllCheckState) {
                    case CheckState.Checked:
                        return CheckBoxState.CheckedDisabled;
                    case CheckState.Unchecked:
                        return CheckBoxState.UncheckedDisabled;
                    default:
                        return CheckBoxState.MixedDisabled;
                }
            }
        } else {
            if (MouseIsDown) {
                return CheckBoxState.UncheckedPressed;
            } else {
                switch (AllCheckState) {
                    case CheckState.Checked:
                        return CheckBoxState.CheckedNormal;
                    case CheckState.Unchecked:
                        return CheckBoxState.UncheckedNormal;
                    default:
                        return CheckBoxState.MixedNormal;
                }
            }
        }
    }

    protected virtual Rectangle GetCheckBoxBounds(Graphics g, Rectangle cellBounds,
                                        DataGridViewAdvancedBorderStyle advancedBorderStyle,
                                        CheckBoxState state) {
        Size checkBoxSize = GetCheckBoxSize(g, state);
        Rectangle valBounds = cellBounds;
        valBounds.Width -= 1;
        Rectangle borderWidths = BorderWidths(advancedBorderStyle);
        valBounds.Offset(borderWidths.X, borderWidths.Y);
        valBounds.Width -= borderWidths.Right;
        valBounds.Height -= borderWidths.Bottom;
        int x = valBounds.X + (valBounds.Width - checkBoxSize.Width) / 2;
        int y = valBounds.Y + (valBounds.Height - checkBoxSize.Height) / 2;
        return new Rectangle(new Point(x, y), checkBoxSize);
    }

    protected virtual Size GetCheckBoxSize(Graphics g, CheckBoxState state) {
        Size checkBoxSize;
        if (Application.RenderWithVisualStyles) {
            checkBoxSize = CheckBoxRenderer.GetGlyphSize(g, state);
            switch (OwnerColumn.FlatStyle) {
                case FlatStyle.Standard:
                case FlatStyle.System:
                    break;
                case FlatStyle.Flat:
                    checkBoxSize.Width -= 3;
                    checkBoxSize.Height -= 3;
                    break;
                case FlatStyle.Popup:
                    checkBoxSize.Width -= 2;
                    checkBoxSize.Height -= 2;
                    break;
            }
        } else {
            switch (OwnerColumn.FlatStyle) {
                case FlatStyle.Flat:
                    checkBoxSize = CheckBoxRenderer.GetGlyphSize(g, state);
                    checkBoxSize.Width -= 3;
                    checkBoxSize.Height -= 3;
                    break;
                case FlatStyle.Popup:
                    checkBoxSize = CheckBoxRenderer.GetGlyphSize(g, state);
                    checkBoxSize.Width -= 2;
                    checkBoxSize.Height -= 2;
                    break;
                default: // FlatStyle.Standard || FlatStyle.System
                    checkBoxSize = new Size(SystemInformation.Border3DSize.Width * 2 + 9,
                        SystemInformation.Border3DSize.Width * 2 + 9);
                    break;
            }
        }
        return checkBoxSize;
    }
}
