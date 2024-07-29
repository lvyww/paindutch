using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diff
{
    public enum SplitedDiffType { Add, Delete, None, Modify }
    public class SplitedDiffRes
    {
        private int origIndex;
        private int revIndex;
        private SplitedDiffType type;

        public int OrigIndex { get => origIndex; set => origIndex = value; }
        public int RevIndex { get => revIndex; set => revIndex = value; }
        public SplitedDiffType Type { get => type; set => type = value; }

        public SplitedDiffRes(int origIndex, int revIndex, SplitedDiffType type)
        {
            OrigIndex = origIndex;
            RevIndex = revIndex;
            Type = type;
        }
    }
}
