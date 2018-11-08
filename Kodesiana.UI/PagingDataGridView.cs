using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kodesiana.UI
{
    public class PagingDataGridView : IDataGridViewManipulator
    {
        private int _pageCount;
        private int _currentPage;

        public DataGridView Control { get; set; }
        
        /// <summary>
        /// Invoked when current page is switched either by changing <see cref="CurrentPage"/> property or by calling <see cref="GotoPage"/>.
        /// </summary>
        public event EventHandler<PageSwitchedEventArgs> PageSwitched;

        #region Properties

        /// <summary>
        /// Gets or sets ConnectionString to connect to database.
        /// </summary>
        /// <remarks>Currently only supporting SQL Server databases.</remarks>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets table name to retrieve data.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets or sets columns to include in query.
        /// </summary>
        /// <remarks>Set this property to zero list or <c>null</c> to include all columns.</remarks>
        public List<string> Columns { get; set; }

        /// <summary>
        /// Gets or sets column used to order query.
        /// </summary>
        public string OrderColumn { get; set; }

        /// <summary>
        /// Gets or sets number of entries per page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets current page.
        /// </summary>
        /// <remarks>Changing value of this property will invoke <see cref="GotoPage"/> to advance current shown page.</remarks>
        public int CurrentPage
        {
            get => _currentPage;
            set => GotoPage(value);
        }

        #endregion

        /// <summary>
        /// Prepare data from database before using pagination.
        /// </summary>
        /// <remarks>This method counts rows inside the table and store it for pagination.</remarks>
        public void ReloadData()
        {
            using (var cn = OpenConnection())
            {
                var cmd = cn.CreateCommand();
                cmd.CommandText = string.Format("SELECT COUNT(*) FROM [{0}]", TableName);
                cmd.CommandType = CommandType.Text;

                var totalRows = Convert.ToInt32(cmd.ExecuteScalar());
                _pageCount = totalRows / PageSize;
            }
        }

        /// <summary>
        /// Advances current page to next page.
        /// </summary>
        public void NextPage()
        {
            GotoPage(++_currentPage);
        }

        /// <summary>
        /// Advances current page to previous page.
        /// </summary>
        public void PrevPage()
        {
            GotoPage(--_currentPage);
        }

        /// <summary>
        /// Advance current page to specified <paramref name="page"/>.
        /// </summary>
        /// <param name="page">Page to show. If the page is out of bound the current available page, it advances to first page.</param>
        public void GotoPage(int page)
        {
            if (page > _pageCount || page < 1) page = 1;
            var lastBatch = (page - 1) * PageSize;

            using (var cn = OpenConnection())
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = string.Format("SELECT ROW_NUMBER() OVER(ORDER BY [{0}]), {1} FROM [{2}] ORDER BY [{0}] OFFSET {3} ROWS FETCH NEXT {4} ROWS ONLY;", OrderColumn, GetColumns(), TableName, lastBatch, PageSize);
                cmd.CommandType = CommandType.Text;
                
                using (var tbl = new SqlDataAdapter(cmd))
                using (var ds = new DataSet())
                {
                    tbl.Fill(ds);
                    Control.DataSource = ds.Tables[0];
                }
            }

            _currentPage = page;
            OnPageSwitched(new PageSwitchedEventArgs { CurrentPage = page });
        }

        /// <summary>
        /// Gets current available pages to show.
        /// </summary>
        /// <returns></returns>
        public int GetPageCount()
        {
            return _pageCount;
        }

        #region Private Methods

        private string GetColumns()
        {
            if (Columns == null || Columns.Count == 0)
            {
                return "*";
            }
            else
            {
                return string.Join(", ", Columns.Select(x => string.Format("[{0}]", x)));
            }
        }

        private SqlConnection OpenConnection()
        {
            var cn = new SqlConnection(ConnectionString);
            cn.Open();
            return cn;
        }

        #endregion

        #region Event Invocator

        /// <summary>
        /// Invokes <see cref="PageSwitched"/> event.
        /// </summary>
        /// <param name="e"><see cref="PageSwitchedEventArgs"/> instance containing current event information.</param>
        protected virtual void OnPageSwitched(PageSwitchedEventArgs e)
        {
            PageSwitched?.Invoke(this, e);
        }

        #endregion
    }
}
