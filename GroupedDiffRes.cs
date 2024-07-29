using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diff
{
    public class GroupedDiffRes
    {
        private int rangeStart;
        private int rangeEnd;
        private DiffType type;

        public int RangeStart { get => rangeStart; set => rangeStart = value; }
        public int RangeEnd { get => rangeEnd; set => rangeEnd = value; }
        public DiffType Type { get => type; set => type = value; }
    }
}
