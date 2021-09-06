﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Assets.PpsPro
{
    //格子地图
    public class GridMap
    {
        private Dictionary<Vector2Int, BaseGrid> gridDic;                               //所有格子字典
        private List<BaseGrid> gridList;                                                //所有格子列表 
        private Dictionary<int, Dictionary<Vector2Int, BaseGrid>> gridGroupDic;         //格子组字典 
        private Dictionary<Vector2Int, BaseGrid> turnDic;                               //拐点字典
        private List<BaseGrid> turnList;
        private List<List<BaseGrid>> rowList;
        private List<MoveItem> moveList;
        public GameObject root;
        private int len;
        private int width;

        public void Load(int length, int width, GameObject root)
        {
            turnList = new List<BaseGrid>();
            moveList = new List<MoveItem>();
            turnDic = new Dictionary<Vector2Int, BaseGrid>();
            this.len = length;
            this.width = width;
            this.root = root;
            RegisterEvent();
            gridList = new List<BaseGrid>();
            gridDic = new Dictionary<Vector2Int, BaseGrid>();
            int oriId = 0;
            for (int i = 0; i < len; i++)
            {
                rowList = new List<List<BaseGrid>>();
                List<BaseGrid> tempList = new List<BaseGrid>();
                for (int j = 0; j < width; j++)
                {
                    BaseGrid grid = new BaseGrid();
                    grid.Load(oriId, i, j, root);
                    gridList.Add(grid);
                    gridDic.Add(grid.Center, grid);
                    oriId++;
                    tempList.Add(grid);
                }
                rowList.Add(tempList);
            }

        }
        public void Update()
        {
            if (moveList?.Count > 0)
            {
                for (int i = 0; i < moveList.Count; i++)
                {
                    if (!moveList[i].MoveTo())
                    {
                        moveList.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
        public void UpdateData()
        {

            if (gridList?.Count > 0)
            {
                for (int i = 0; i < gridList.Count; i++)
                {
                    gridList[i].CheckIsTurn();
                }
            }
            //for (int i = 0; i < rowList.Count; i++)
            //{
            //    List<BaseGrid> list = rowList[i];
            //    bool isBegin = false;

            //    for (int j = 0; j < list.Count; j++)
            //    {
            //        BaseGrid grid = list[i];
            //        AreaLineCell cell = null;
            //        if (!isBegin && grid.GridState == EGridState.EActive)
            //        {
            //            isBegin = true;
            //            cell = new AreaLineCell();
            //            cell.Start(grid.ID);
            //        }
            //        else
            //        {
            //            if (isBegin)
            //            {
            //                cell.End(grid.ID);
            //            }
            //        }
            //    }
            //}
        }
        public void Dispose()
        {
            UnRegisterEvent();
        }

        //注册事件
        private void RegisterEvent()
        {
            GridMapFuncs.GetGridUnitById += GetGridUnitById;
            GridMapFuncs.TurnGrid += OnTurnGrid;
        }
        //移除事件
        private void UnRegisterEvent()
        {
            GridMapFuncs.GetGridUnitById -= GetGridUnitById;
            GridMapFuncs.TurnGrid -= OnTurnGrid;
        }

        private BaseGrid GetGridUnitById(Vector2Int gridId)
        {
            BaseGrid unit = null;
            gridDic.TryGetValue(gridId, out unit);
            return unit;
        }
        //当拐点变化时
        private void OnTurnGrid(BaseGrid grid, bool state)
        {
            if (state) AddTurnGrid(grid);
            else RemoveTrunGrid(grid);
        }
        //添加拐点
        private void AddTurnGrid(BaseGrid grid)
        {
            BaseGrid old;
            if (turnDic.TryGetValue(grid.Center, out old))
            {
                Debug.Log("[error] 添加重复的拐点");
                return;
            }
            turnDic.Add(grid.Center, grid);
            turnList.Add(grid);
        }
        //移除拐点
        private void RemoveTrunGrid(BaseGrid grid)
        {
            BaseGrid old;
            if (!turnDic.TryGetValue(grid.Center, out old))
            {
                Debug.Log("[error] 移除不存在的拐点");
                return;
            }
            turnDic.Remove(grid.Center);
            turnList.Remove(grid);
        }

        public bool MoveTo(Transform t, Vector3 targetPos)
        {
            int pos_x = Mathf.CeilToInt(t.position.x);
            int pos_y = Mathf.CeilToInt(t.position.z);

            int t_x = Mathf.CeilToInt(targetPos.x);
            int t_y = Mathf.CeilToInt(targetPos.z);
            BaseGrid grid;
            BaseGrid targetGrid;
            if (gridDic.TryGetValue(new Vector2Int(pos_x, pos_y), out grid))
            {
                if (gridDic.TryGetValue(new Vector2Int(t_x, t_y), out targetGrid))
                {
                    List<BaseGrid> list = MatchPath(grid, targetGrid);
                    MoveItem item = new MoveItem();
                    item.Path = list;
                    item.Item = t;
                    moveList.Add(item);
                    return true;
                }
            }
            return false;
        }

        public List<BaseGrid> MatchPath(BaseGrid begin, BaseGrid end)
        {
            List<BaseGrid> passed = new List<BaseGrid>();
            if (CanLinkEnd(begin, end))
            {
                passed.Add(end);
                return passed;
            }
            if (begin.IsTurn) passed.Add(begin);
            //if (end.IsTurn) pathed.Add(end);
            List<BaseGrid> pathed = new List<BaseGrid>();
            //List<List<BaseGrid>> pathList = MatchTurnPoint(begin, end, pathed, ref passed);
            List<List<BaseGrid>> pathList = new List<List<BaseGrid>>();
            //// 最左侧寻路 -- 只找寻到了一条路
            //MatchTurnPoint1(begin, end, pathed, ref passed, ref pathList);


            MatchTurnPoint1(begin, end, passed, pathed, ref pathList);
            if (pathList?.Count > 0)
            {
                List<BaseGrid> list = pathList[0];
                for (int i = 1; i < pathList.Count; i++)
                {
                    list = GetNearestPath(list, pathList[i]);
                }
                return list;
            }
            return null;
        }

        private void MatchTurnPoint1(BaseGrid begin, BaseGrid end, List<BaseGrid> turns, List<BaseGrid> invailds, ref List<List<BaseGrid>> root)
        {
            Debug.Log($" _________________________________________________________________________________");
            Debug.Log($" _________________________________________________________________________________");
            Debug.Log($" _________________________________________________________________________________");

            Debug.Log($" begin: {begin.Center}");
            if (CanLinkEnd(end, begin))
            {
                turns.Add(end);
                Debug.Log($" begin: {begin.Center} 可以到达B点");
                string tEnd = "";
                for (int i = 0; i < turns.Count; i++)
                {
                    tEnd += turns[i].Center.ToString() + "  ";
                }
                Debug.Log($" begin: {begin.Center} 连接到的拐点： { tEnd} ");
                root.Add(turns);
            }
            else
            {
                List<BaseGrid> sameLvs = GetTurnsByGrid(begin);
                string tLvs = "";
                for (int i = 0; i < sameLvs.Count; i++)
                {
                    tLvs += sameLvs[i].Center.ToString() + "  ";
                }
                Debug.Log($" begin: {begin.Center} 连接到的拐点： { tLvs} ");

                string tTurns = "";
                for (int i = 0; i < turns.Count; i++)
                {
                    tTurns += turns[i].Center.ToString() + "  ";
                }
                Debug.Log($" begin: {begin.Center} 不可挑选都有： { tTurns} ");
                for (int i = 0; i < turnList.Count; i++)
                {
                    BaseGrid grid = turnList[i];
                    if (turns.Contains(grid)) continue;
                    if (invailds.Contains(grid)) continue;
                    if (CanLinkEnd(begin, grid))
                    {
                        Debug.Log($" begin: {begin.Center} 向下检测到： { grid.Center.ToString()} ");
                        //children.Add(grid);
                        //turns.Add(grid);
                        List<BaseGrid> recodeTurn = new List<BaseGrid>();
                        recodeTurn.AddRange(turns);
                        //recodeTurn.AddRange(sameLvs);
                        recodeTurn.Add(grid);
                        MatchTurnPoint1(grid, end, recodeTurn, sameLvs, ref root);
                        //invailds.Add(grid);
                    }
                }
            }
        }
        //// 最左侧寻路 -- 只找寻到了一条路
        //private void MatchTurnPoint1(BaseGrid begin, BaseGrid end, List<BaseGrid> turns, ref List<BaseGrid> pathed, ref List<List<BaseGrid>> root)
        //{
        //    turns.Add(begin);
        //    if (CanLinkEnd(end, begin))
        //    {
        //        turns.Add(end);
        //        root.Add(turns);
        //        return;
        //    }
        //    List<BaseGrid> children = new List<BaseGrid>();

        //    for (int i = 0; i < turnList.Count; i++)
        //    {
        //        BaseGrid grid = turnList[i];
        //        if (pathed.Contains(grid)) continue;
        //        if (CanLinkEnd(begin, grid))
        //        {
        //            children.Add(grid);
        //            pathed.Add(grid);
        //        }
        //    }

        //    for (int i = 0; i < children.Count; i++)
        //    {
        //        List<BaseGrid> recodeTurn = new List<BaseGrid>();
        //        recodeTurn.AddRange(turns);
        //        MatchTurnPoint1(children[i], end, recodeTurn, ref pathed, ref root);
        //    }
        //}

        private List<List<BaseGrid>> MatchTurnPoint(BaseGrid begin, BaseGrid end, List<BaseGrid> turns, ref List<BaseGrid> pathed)
        {
            turns.Add(begin);
            pathed.Add(begin);

            List<List<BaseGrid>> pathList = new List<List<BaseGrid>>();
            if (CanLinkEnd(end, begin))
            {
                turns.Add(end);
                pathList.Add(turns);
                return pathList;
            }
            List<BaseGrid> children = new List<BaseGrid>();

            for (int i = 0; i < turnList.Count; i++)
            {
                if (pathed.Contains(turnList[i])) continue;
                if (CanLinkEnd(begin, turnList[i]))
                {
                    children.Add(turnList[i]);
                    pathed.Add(turnList[i]);
                }
            }
            for (int i = 0; i < children.Count; i++)
            {
                List<List<BaseGrid>> list = MatchTurnPoint(children[i], end, turns, ref pathed);
                if (list == null) return null;
                else pathList.AddRange(list);
            }
            return pathList;
        }

        public bool CanLinkEnd(BaseGrid begin, BaseGrid end)
        {
            List<Vector2Int> list = GridHelper.GetTouchedPosBetweenTwoPoints(begin.Center, end.Center);
            if (list?.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    BaseGrid grid;
                    if (gridDic.TryGetValue(list[i], out grid))
                    {
                        if (grid.GridState != EGridState.EActive) return false;
                    }
                }
            }
            return true;
        }

        private List<BaseGrid> GetTurnsByGrid(BaseGrid ori)
        {
            List<BaseGrid> list = new List<BaseGrid>();
            for (int i = 0; i < turnList.Count; i++)
            {
                BaseGrid grid = turnList[i];
                if (CanLinkEnd(ori, grid))
                {
                    list.Add(grid);
                }
            }
            return list;
        }

        public List<BaseGrid> GetNearestPath(List<BaseGrid> list1, List<BaseGrid> list2)
        {
            float distance1 = GetPathLength(list1);
            float distance2 = GetPathLength(list2);
            return distance1 < distance2 ? list1 : list2;
        }
        private float GetPathLength(List<BaseGrid> path)
        {
            float distance = 0;
            if (path?.Count > 1)
            {
                for (int i = 0; i < path.Count; i++)
                {
                    if (i <= path.Count - 2)
                    {
                        distance += Vector3.Distance(path[i].Position, path[i + 1].Position);
                    }
                }
            }
            return distance;
        }
        private void ControlTurnPoints()
        {

        }

        private void ControlEditorState()
        {

        }
        public void SaveData(string mapName)
        {

            string filePath = Application.dataPath + "/PpsPro/Resources/Data/" + mapName + ".txt";
            if (File.Exists(filePath))
                File.Delete(filePath);

            FileStream fs = new FileStream(filePath, FileMode.Create);
            StringBuilder content = new StringBuilder();
            //获得字节数组
            for (int i = 0; i < gridList.Count; i++)
            {
                if (i == 0) content.Append("[");
                BaseGrid grid = gridList[i];
                content.Append($"[id:{grid.ID},center:{grid.Center},state:{(int)grid.GridState},isTurn:{grid.IsTurn}]\n");
                if (i == gridList.Count - 1) content.Append("]");
            }

            byte[] data = System.Text.Encoding.Default.GetBytes(content.ToString());
            //开始写入
            fs.Write(data, 0, data.Length);
            //清空缓冲区、关闭流
            fs.Flush();
            fs.Close();

            Debug.Log(" save... ");
        }
    }
}