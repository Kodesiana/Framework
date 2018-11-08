using System.Drawing;
using System.Windows.Forms;

namespace Kodesiana.UI.Styles
{
    public class MarsPallete : DataGridViewPallete
    {
        public MarsPallete()
        {
            var font = new Font("Tahoma", 9.87F, FontStyle.Regular, GraphicsUnit.Point, 0);

            // global style
            Font = font;
            BackColor = Color.Black;
            ForeColor = Color.Maroon;
            RowHeight = 25;
            HeaderHeight = 60;

            // column header
            HeaderStyle = new DataGridViewCellStyle
            {
                Font = font,
                ForeColor = Color.White,
                BackColor = Color.Black,
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            // cell style
            CellStyle = new DataGridViewCellStyle
            {
                SelectionBackColor = Color.Red,
                SelectionForeColor = Color.Yellow
            };

            // alternating cell style
            AlternatingRowCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.LemonChiffon
            };
        }
    }
}
