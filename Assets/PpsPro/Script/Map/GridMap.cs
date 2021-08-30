using System;
using System.Collections.Generic;
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
            for (int i = 0; i < rowList.Count; i++)
            {
                List<BaseGrid> list = rowList[i];
                bool isBegin = false;

                for (int j = 0; j < list.Count; j++)
                {
                    BaseGrid grid = list[i];
                    AreaLineCell cell = null;
                    if (!isBegin && grid.GridState == EGridState.EActive)
                    {
                        isBegin = true;
                        cell = new AreaLineCell();
                        cell.Start(grid.ID);
                    }
                    else
                    {
                        if (isBegin)
                        {
                            cell.End(grid.ID);
                        }
                    }
                }
            }
        }
        public void Dispose()
        {
            UnRegisterEvent();
        }

        private void RegisterEvent()
        {
            GridMapFuncs.GetGridUnitById += GetGridUnitById;
            GridMapFuncs.TurnGrid += OnTurnGrid;
        }

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
            int t_y = Mathf.CeilToInt(targetPos.y);
            BaseGrid grid;
            BaseGrid targetGrid;
            if (gridDic.TryGetValue(new Vector2Int(pos_x, pos_y), out grid))
            {
                if (gridDic.TryGetValue(new Vector2Int(pos_x, pos_y), out targetGrid))
                {
                    List<BaseGrid> list = MatchPath(grid, targetGrid);
                }
            }
            return false;
        }

        public List<BaseGrid> MatchPath(BaseGrid begin, BaseGrid end)
        {
            List<BaseGrid> gridList = new List<BaseGrid>();
            List<Vector2Int> list = GridHelper.GetTouchedPosBetweenTwoPoints(begin.Center, end.Center);
            for (int i = 0; i < list.Count; i++)
            {
                BaseGrid grid;
                if (gridDic.TryGetValue(list[i], out grid))
                {
                    if (grid.GridState != EGridState.EActive) break;
                    gridList.Add(grid);
                    if (i == list.Count - 1) return gridList;
                }
            }
            gridList.Clear();
            return null;
        }
        public List<BaseGrid> MatchTurnPoint(BaseGrid begin, BaseGrid end)
        {
            List<BaseGrid> connectList = new List<BaseGrid>();
            for (int i = 0; i < turnList.Count; i++)
            {
                BaseGrid grid = turnList[i];
                List<Vector2Int> list = GridHelper.GetTouchedPosBetweenTwoPoints(begin.Center, end.Center);
                for (int j = 0; j < list.Count; j++)
                {
                    if (gridDic.TryGetValue(list[i], out grid))
                    {
                        if (grid.GridState != EGridState.EActive) break;
                        if (i == list.Count - 1) connectList.Add(grid);
                    }
                }

            }
            return connectList;
        }
    }
}