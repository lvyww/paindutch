using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diff
{
    public enum DiffType {Add, Delete, None};
    public class DiffRes
    {
        private DiffType type;
        public DiffType Type
        {
            get { return type; }
            set { type = value; }
        }
        private int origIndex;
        private int revIndex;
        public int OrigIndex
        {
            get { return origIndex; }
            set { origIndex = value; }
        }

        public int RevIndex
        {
            get { return revIndex; }
            set { revIndex = value; }
        }

        public DiffRes(DiffType type, int origIndex, int revIndex)
        {
            Type = type;
            OrigIndex = origIndex;
            RevIndex = revIndex;
        }
    }
}
