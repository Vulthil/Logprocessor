using System.Collections.Generic;
using System.Linq;

namespace LogProcessor.Models.Dto
{
    public class TableRequestResponse<T> where T : class
    {
        public T[] Rows { get; private set; }
        public int TotalRows { get; private set; }

        public TableRequestResponse(IEnumerable<T> rows, int totalRows)
        {
            Rows = rows.ToArray();
            TotalRows = totalRows;
        }
    }
}