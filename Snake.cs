using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diff
{
    class Snake : PathNode
    {
        public Snake(int i, int j, PathNode prev) : base(i, j, prev) { }
        public override bool IsSnake()
        {
            return true;
        }
    }
}
