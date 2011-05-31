using System.Collections.Generic;
using System.Linq;

namespace Synoptic.ConsoleFormat
{
    public class ConsoleRow
    {
        private readonly List<ConsoleCell> _cells = new List<ConsoleCell>();

        public List<ConsoleCell> Cells
        {
            get { return _cells; }
        }

        public ConsoleRow() { }

        public ConsoleRow(params string[] text)
        {
            _cells.AddRange(text.Select(t => new ConsoleCell(t)));
        }

        public ConsoleRow(params ConsoleCell[] cells)
        {
            _cells.AddRange(cells);
        }

        public void AddCell(ConsoleCell cell)
        {
            _cells.Add(cell);
        }

        internal IEnumerable<int> CalculateCellTextWidths(int tableWidth)
        {
            var flexibleWidthCells = _cells.Where(c => !c.Width.HasValue).ToList();
            var paddingTotal = _cells.Sum(c => c.Padding);

            // Padding is not available for text.
            var availableWidth = tableWidth - _cells.Sum(c => (c.Width ?? 0)) - paddingTotal;

            if (availableWidth <= 0)
                availableWidth = tableWidth;
            
            foreach(var cell in _cells)
            {
                if (cell.Width.HasValue)
                    yield return cell.Width.Value;
                else
                    yield return (availableWidth / flexibleWidthCells.Count());
            }
        }

        internal bool HasMoreLines()
        {
            return _cells.Any(c => c.HasMoreWords);
        }
    }
}