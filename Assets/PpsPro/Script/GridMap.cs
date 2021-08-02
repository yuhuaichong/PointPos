using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.PpsPro
{
    //格子地图
    public class GridMap
    {
        private Dictionary<Vector2, BaseGrid> gridDic;
        private List<BaseGrid> gridList;
        public GameObject root;
        private int len;
        private int width;
        public GridMap()
        {

        }
        public void Load(int length, int width, GameObject root)
        {
            this.len = length;
            this.width = width;
            this.root = root;
            RegisterEvent();
            gridList = new List<BaseGrid>();
            gridDic = new Dictionary<Vector2, BaseGrid>();
            for (int i = -len; i < len; i++)
            {
                for (int j = -width; j < width; j++)
                {
                    BaseGrid grid = new BaseGrid();
                    grid.Load(i, j, root);
                    gridList.Add(grid);
                    gridDic.Add(grid.ID, grid);
                }
            }

        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                if (gridList?.Count > 0)
                {
                    for (int i = 0; i < gridList.Count; i++)
                    {
                        gridList[i].CheckIsTurn();
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
        }
        private void UnRegisterEvent()
        {
            GridMapFuncs.GetGridUnitById -= GetGridUnitById;
        }
        private BaseGrid GetGridUnitById(Vector2 gridId)
        {
            BaseGrid unit = null;
            gridDic.TryGetValue(gridId, out unit);
            return unit;
        }
    }
}