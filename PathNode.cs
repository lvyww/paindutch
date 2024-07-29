using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diff
{
    public abstract class PathNode
    {
        public int i;
        public int j;
        public PathNode prev;

        public PathNode(int i, int j, PathNode prev)
        {
            this.i = i;
            this.j = j;
            this.prev = prev;
        }

        public abstract bool IsSnake();
    }
}
