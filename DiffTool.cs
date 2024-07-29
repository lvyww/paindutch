using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diff
{
    static public class DiffTool
    {

        /// <summary>
        /// 寻找最优路径
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="rev"></param>
        /// <returns></returns>
        static public List<DiffRes> Diff(List<string> orig, List<string> rev)
        {
            if (orig == null)
                throw new Exception("original sequence is null");
            if (rev == null)
                throw new Exception("revised sequence is null");
            int n = orig.Count;
            int m = rev.Count;
            //最大步数（先全减后全加）
            int max = n + m + 1;
            int size = 1 + 2 * max;
            int middle = size / 2;
            //构建纵坐标数组（用于存储每一步的最优路径位置）
            PathNode[] diagonal = new PathNode[size];
            //用于获取初试位置的辅助节点
            diagonal[middle + 1] = new Snake(0, -1, null);
            //外层循环步数
            for (int d = 0; d < max; d++)
            {
                //内层循环所处偏移量，以2为步长，因为从所在位置走一步，偏移量只会相差2
                for (int k = -d; k <= d; k += 2)
                {
                    //找出对应偏移量所在的位置，以及它上一步的位置（高位与低位）
                    int kmiddle = middle + k;
                    int kplus = kmiddle + 1;
                    int kminus = kmiddle - 1;
                    //若k为-d，则一定是从上往下走，即i相同
                    //若diagonal[kminus].i < diagonal[kplus].i，则最优路径一定是从上往下走，即i相同
                    int i;
                    PathNode prev;
                    if ((k == -d) || (k != d && diagonal[kminus].i < diagonal[kplus].i))
                    {
                        i = diagonal[kplus].i;
                        prev = diagonal[kplus];
                    }
                    else
                    {
                        //若k为d，则一定是从左往右走，即i+1
                        //若diagonal[kminus].i = diagonal[kplus].i，则最优路径一定是从左往右走，即i+1
                        i = diagonal[kminus].i + 1;
                        prev = diagonal[kminus];
                    }
                    //根据i与k，计算出j
                    int j = i - k;
                    //上一步的低位数据不再存储在数组中（每个k只清空低位即可全部清空）
                    diagonal[kminus] = null;
                    //当前是diff节点
                    PathNode node = new DiffNode(i, j, prev);
                    //判断被比较的两个数组中，当前位置的数据是否相同，相同，则去到对角线位置
                    while (i < n && j < m && orig[i].Equals(rev[j]))
                    {
                        i++;
                        j++;
                    }
                    //判断是否去到对角线位置，若是，则生成snack节点，前节点为diff节点
                    if (i > node.i)
                        node = new Snake(i, j, node);
                    //设置当前位置到数组中
                    diagonal[kmiddle] = node;
                    //达到目标位置，返回当前node
                    if (i >= n && j >= m)
                    {
                        return GetDiff(diagonal[kmiddle], orig, rev);
                    }
                }
            }
            throw new Exception("could not find a diff path");
        }

        /// <summary>
        /// 寻找最优路径
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="rev"></param>
        /// <returns></returns>
        static public List<DiffRes> Diff(string orig, string rev)
        {
            if (orig == null)
                throw new Exception("original sequence is null");
            if (rev == null)
                throw new Exception("revised sequence is null");
            int n = orig.Length;
            int m = rev.Length;
            //最大步数（先全减后全加）
            int max = n + m + 1;
            int size = 1 + 2 * max;
            int middle = size / 2;
            //构建纵坐标数组（用于存储每一步的最优路径位置）
            PathNode[] diagonal = new PathNode[size];
            //用于获取初试位置的辅助节点
            diagonal[middle + 1] = new Snake(0, -1, null);
            //外层循环步数
            for (int d = 0; d < max; d++)
            {
                //内层循环所处偏移量，以2为步长，因为从所在位置走一步，偏移量只会相差2
                for (int k = -d; k <= d; k += 2)
                {
                    //找出对应偏移量所在的位置，以及它上一步的位置（高位与低位）
                    int kmiddle = middle + k;
                    int kplus = kmiddle + 1;
                    int kminus = kmiddle - 1;
                    //若k为-d，则一定是从上往下走，即i相同
                    //若diagonal[kminus].i < diagonal[kplus].i，则最优路径一定是从上往下走，即i相同
                    int i;
                    PathNode prev;
                    if ((k == -d) || (k != d && diagonal[kminus].i < diagonal[kplus].i))
                    {
                        i = diagonal[kplus].i;
                        prev = diagonal[kplus];
                    }
                    else
                    {
                        //若k为d，则一定是从左往右走，即i+1
                        //若diagonal[kminus].i = diagonal[kplus].i，则最优路径一定是从左往右走，即i+1
                        i = diagonal[kminus].i + 1;
                        prev = diagonal[kminus];
                    }
                    //根据i与k，计算出j
                    int j = i - k;
                    //上一步的低位数据不再存储在数组中（每个k只清空低位即可全部清空）
                    diagonal[kminus] = null;
                    //当前是diff节点
                    PathNode node = new DiffNode(i, j, prev);
                    //判断被比较的两个数组中，当前位置的数据是否相同，相同，则去到对角线位置
                    while (i < n && j < m && orig[i].Equals(rev[j]))
                    {
                        i++;
                        j++;
                    }
                    //判断是否去到对角线位置，若是，则生成snack节点，前节点为diff节点
                    if (i > node.i)
                        node = new Snake(i, j, node);
                    //设置当前位置到数组中
                    diagonal[kmiddle] = node;
                    //达到目标位置，返回当前node
                    if (i >= n && j >= m)
                    {
                        return GetDiff(diagonal[kmiddle], orig, rev);
                    }
                }
            }
            throw new Exception("could not find a diff path");
        }

        static public List<GroupedDiffRes> GetGroupedResult(List<DiffRes> diffResList)
        {
            List<GroupedDiffRes> rangeList = new List<GroupedDiffRes>();
            if (diffResList == null || diffResList.Count == 0)
            {
                return rangeList;
            }
            DiffType typeNow = diffResList[0].Type;
            int rangeStart = 0;
            int rangeEnd = 0;
            int diffResListCount = diffResList.Count;
            for (int i = 0; i < diffResListCount; ++i)
            {
                DiffRes diffRes = diffResList[i];
                if (diffRes.Type != typeNow)
                {
                    GroupedDiffRes groupedDiffRes = new GroupedDiffRes();
                    groupedDiffRes.RangeStart = rangeStart;
                    groupedDiffRes.RangeEnd = rangeEnd;
                    groupedDiffRes.Type = typeNow;
                    rangeList.Add(groupedDiffRes);
                    typeNow = diffRes.Type;
                    rangeStart = i;
                }
                rangeEnd = i;
            }


            GroupedDiffRes groupedDiffResLast = new GroupedDiffRes();
            groupedDiffResLast.RangeStart = rangeStart;
            groupedDiffResLast.RangeEnd = rangeEnd;
            groupedDiffResLast.Type = typeNow;
            rangeList.Add(groupedDiffResLast);

            return rangeList;
        }

        static public List<SplitedDiffRes> GetSplitedResult(List<DiffRes> diffResList)
        {
            return GetSplitedResult(GetGroupedResult(diffResList));
        }

        static public List<SplitedDiffRes> GetSplitedResult(List<GroupedDiffRes> rangeList)
        {
            if (rangeList == null || rangeList.Count == 0)
            {
                return new List<SplitedDiffRes>();
            }

            DiffType typeNow = rangeList[0].Type;

            int rangeListCount = rangeList.Count;
            List<PartSplitedDiffRes> origList = new List<PartSplitedDiffRes>();
            List<PartSplitedDiffRes> revList = new List<PartSplitedDiffRes>();
            List<SplitedDiffType> splitedDiffTypeeList = new List<SplitedDiffType>();

            int origIndex = 0;
            int revIndex = 0;
            
            for (int i = 0; i < rangeListCount; ++i)
            {
                GroupedDiffRes groupedDiffRes = rangeList[i];
                typeNow = groupedDiffRes.Type;
                if (typeNow == DiffType.Delete)
                {
                    if (i + 1 < rangeListCount)
                    {
                        GroupedDiffRes groupedDiffResNext = rangeList[i + 1];
                        if (groupedDiffResNext.Type == DiffType.Add)
                        {
                            for (int j = groupedDiffRes.RangeStart; j <= groupedDiffRes.RangeEnd; ++j)
                            {
                                origList.Add(new PartSplitedDiffRes(origIndex++));
                            }
                            for (int j = groupedDiffResNext.RangeStart; j <= groupedDiffResNext.RangeEnd; ++j)
                            {
                                revList.Add(new PartSplitedDiffRes(revIndex++));
                            }
                            int subGroupedDiffRes = groupedDiffRes.RangeEnd - groupedDiffRes.RangeStart;
                            int subGroupedDiffResNext = groupedDiffResNext.RangeEnd - groupedDiffResNext.RangeStart;
                            if (subGroupedDiffRes <= subGroupedDiffResNext)
                            {
                                for (int j = 0; j < subGroupedDiffResNext - subGroupedDiffRes; ++j)
                                {
                                    origList.Add(new PartSplitedDiffRes(-1));
                                }
                                for (int j = 0; j < subGroupedDiffResNext + 1; ++j)
                                {
                                    splitedDiffTypeeList.Add(SplitedDiffType.Modify);
                                }
                            }
                            else
                            {
                                for (int j = 0; j < subGroupedDiffRes - subGroupedDiffResNext; ++j)
                                {
                                    revList.Add(new PartSplitedDiffRes(-1));
                                }
                                for (int j = 0; j < subGroupedDiffRes + 1; ++j)
                                {
                                    splitedDiffTypeeList.Add(SplitedDiffType.Modify);
                                }
                            }

                            ++i;
                            continue;
                        }
                    }

                    for (int j = groupedDiffRes.RangeStart; j <= groupedDiffRes.RangeEnd; ++j)
                    {
                        origList.Add(new PartSplitedDiffRes(origIndex++));
                        revList.Add(new PartSplitedDiffRes(-1));
                        splitedDiffTypeeList.Add(SplitedDiffType.Delete);
                    }
                }
                else if (typeNow == DiffType.Add)
                {
                    for (int j = groupedDiffRes.RangeStart; j <= groupedDiffRes.RangeEnd; ++j)
                    {
                        origList.Add(new PartSplitedDiffRes(-1));
                        revList.Add(new PartSplitedDiffRes(revIndex++));
                        splitedDiffTypeeList.Add(SplitedDiffType.Add);
                    }
                }
                else if (typeNow == DiffType.None)
                {
                    for (int j = groupedDiffRes.RangeStart; j <= groupedDiffRes.RangeEnd; ++j)
                    {
                        origList.Add(new PartSplitedDiffRes(origIndex++));
                        revList.Add(new PartSplitedDiffRes(revIndex++));
                        splitedDiffTypeeList.Add(SplitedDiffType.None);
                    }
                }
            }

            List<SplitedDiffRes> splitedDiffResList = new List<SplitedDiffRes>();
            if (origList.Count == revList.Count && revList.Count == splitedDiffTypeeList.Count)
            {
                int count = splitedDiffTypeeList.Count;

                for (int i = 0; i < count; ++i)
                {
                    splitedDiffResList.Add(new SplitedDiffRes(origList[i].Index, revList[i].Index, splitedDiffTypeeList[i]));
                }
            }

            return splitedDiffResList;
        }

        /// <summary>
        /// 反推路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="orig"></param>
        /// <param name="rev"></param>
        /// <returns></returns>
        static private List<DiffRes> GetDiff(PathNode path, List<string> orig, List<string> rev)
        {
            List<DiffRes> result = new List<DiffRes>();
            if (path == null)
                throw new Exception("path is null");
            if (orig == null)
                throw new Exception("original sequence is null");
            if (rev == null)
                throw new Exception("revised sequence is null");
            while (path != null && path.prev != null && path.prev.j >= 0)
            {
                if (path.IsSnake())
                {
                    int endi = path.i;
                    int begini = path.prev.i;
                    int beginj = path.j;
                    for (int i = endi - 1; i >= begini; i--)
                    {
                        result.Add(new DiffRes(DiffType.None, i, --beginj));
                    }
                }
                else
                {
                    int i = path.i;
                    int j = path.j;
                    int prei = path.prev.i;
                    if (prei < i)
                    {
                        result.Add(new DiffRes(DiffType.Delete, i - 1, -1));
                    }
                    else
                    {
                        result.Add(new DiffRes(DiffType.Add, -1, j - 1));
                    }
                }
                path = path.prev;
            }

            result.Reverse();
            return result;
        }

        /// <summary>
        /// 反推路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="orig"></param>
        /// <param name="rev"></param>
        /// <returns></returns>
        static private List<DiffRes> GetDiff(PathNode path, string orig, string rev)
        {
            List<DiffRes> result = new List<DiffRes>();
            if (path == null)
                throw new Exception("path is null");
            if (orig == null)
                throw new Exception("original sequence is null");
            if (rev == null)
                throw new Exception("revised sequence is null");
            while (path != null && path.prev != null && path.prev.j >= 0)
            {
                if (path.IsSnake())
                {
                    int endi = path.i;
                    int begini = path.prev.i;
                    int beginj = path.j;
                    for (int i = endi - 1; i >= begini; i--)
                    {
                        result.Add(new DiffRes(DiffType.None, i, --beginj));
                    }
                }
                else
                {
                    int i = path.i;
                    int j = path.j;
                    int prei = path.prev.i;
                    if (prei < i)
                    {
                        result.Add(new DiffRes(DiffType.Delete, i, -1));
                    }
                    else
                    {
                        result.Add(new DiffRes(DiffType.Add, -1, i));
                    }
                }
                path = path.prev;
            }

            result.Reverse();
            return result;
        }
    }
}
