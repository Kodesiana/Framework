using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using Kodesiana.UI.Styles;

namespace Kodesiana.UI
{
    public class StylableDataGridView : IDataGridViewManipulator
    {
        public DataGridView Control { get; set; }

        /// <summary>
        /// Sets current <see cref="DataGridView"/> instance double buffer capability.
        /// </summary>
        /// <param name="enabled">Set to <c>True</c> to enable double buffering, otherwise <c>False</c>.</param>
        public void DoubleBufferMode(bool enabled)
        {
            var type = typeof(Control);
            var prop = type.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            Debug.Assert(prop != null, "prop != null");
            prop.SetValue(Control, enabled, null);
        }

        /// <summary>
        /// Applies style pallete to current <see cref="DataGridView"/> instance.
        /// </summary>
        /// <param name="pallete"><see cref="StylePallete"/> instance containing styling information.</param>
        public void ApplyStyle(DataGridViewPallete pallete)
        {
            // globals
            Control.Font = pallete.Font;
            Control.BackColor = pallete.BackColor;
            Control.ForeColor = pallete.ForeColor;
            Control.BorderStyle = BorderStyle.None;
            Control.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // column header
            Control.EnableHeadersVisualStyles = false;
            Control.ColumnHeadersHeight = pallete.HeaderHeight;
            Control.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
            Control.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // rows
            Control.RowHeadersVisible = false;
            Control.RowTemplate.Height = pallete.RowHeight;
            Control.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            Control.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            Control.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;

            // cell style
            Control.ColumnHeadersDefaultCellStyle = pallete.HeaderStyle;
            Control.DefaultCellStyle = pallete.CellStyle;
            Control.AlternatingRowsDefaultCellStyle = pallete.AlternatingRowCellStyle;
        }
    }
}
