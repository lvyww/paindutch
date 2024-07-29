using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diff
{
    class PartSplitedDiffRes
    {
        private int index;

        public int Index { get => index; set => index = value; }

        public PartSplitedDiffRes(int index)
        {
            Index = index;
        }
    }
}
