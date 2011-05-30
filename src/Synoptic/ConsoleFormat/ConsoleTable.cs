using System.Collections.Generic;
using System.Linq;

namespace Synoptic.ConsoleFormat
{
    public class ConsoleTable
    {
        private readonly List<ConsoleRow> _rows = new List<ConsoleRow>();
        public int? Width { get; set; }

        public ConsoleTable() { }

        public ConsoleTable(params string[] text)
        {
            _rows.AddRange(text.Select(t => new ConsoleRow(t)));
        }

        public ConsoleTable(params ConsoleRow[] rows)
        {
            _rows.AddRange(rows);
        }

        public ConsoleTable AddRow(params ConsoleCell[] cells)
        {
            _rows.Add(new ConsoleRow(cells));
            return this;
        }

        public IEnumerable<ConsoleRow> Rows
        {
            get { return _rows; }
        }
    }
}