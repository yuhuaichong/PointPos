using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.PpsPro
{
    public class BaseGrid
    {
        private int x;                                  //x点
        private int y;                                  //y点
        private Vector2 id;                             //id
        private Vector2 center;                         //中心点   
        private float radius;                           //半径
        private bool isTurn;                            //是转角位
        private EGridState gridState;                   //格子状态
        public GameObject objCube;                      //格子模型
        public GridCube gridCube;                       //格子组件
        private string stateImgeName;
        private string turnImgName;
        private List<BaseGrid> intersList;
        private List<BaseGrid> rectList;
        private List<bool> rectState;

        public bool IsTurn { get { return isTurn; } }
        public Vector2 ID { get { return id; } }

        public void Load(int x, int y, GameObject parent)
        {
            this.x = x;
            this.y = y;
            id = new Vector2(x, y);
            center = new Vector2(x, y);
            turnImgName = "55";
            stateImgeName = "11";

            objCube = GameObject.Instantiate(Resources.Load("ACube")) as GameObject;
            if (objCube == null)
            {
                Debug.LogError("GameObject 未加载到");
                return;
            }
            objCube.transform.parent = parent.transform;
            objCube.transform.position = new Vector3(center.x, 0, center.y);
            gridCube = objCube.GetComponent<GridCube>();
            gridCube.ClickHandle += OnClickHandle;
            SetGridState(EGridState.EActive);
            InitExtendList();
        }

        private void InitExtendList()
        {
            rectList = new List<BaseGrid>();
            intersList = new List<BaseGrid>();
            //坐标顺序预定：东 北 西 南
            rectState = new List<bool> { false, false, false, false };
            for (int i = x - 1; i <= x; i++)
            {
                for (int j = y - 1; j <= y; j++)
                {
                    if (i == x && j == y) continue;
                    BaseGrid unit = GridMapFuncs.GetGridUnitById (new Vector2(i, j));
                    if (unit != null)
                    {
                        AddExtendUnit(unit);
                        if (x < 0 && y < 0)
                        {
                            Debug.Log($"[InitRadiant]: {id.ToString()} 添加辐射点 {i} , {j}");
                        }
                    }
                }
            }
        }

        //检测是否是拐点
        public void CheckIsTurn()
        {
            if (gridState == EGridState.EActive)
            {
                if (intersList.Count > 0)
                {
                    bool last = rectState[3];
                    for (int i = 0; i < rectState.Count; i++)
                    {
                        if (rectState[i] && last)
                        {
                            isTurn = true;
                            LoadTexture(turnImgName);
                            return;
                        }
                        last = rectState[i];
                    }
                }
            }
            isTurn = false;
            LoadTexture(stateImgeName);
        }

        //加入扩展列表
        private void AddExtendUnit(BaseGrid unit)
        {
            if (unit.ID.x == x || unit.ID.y == y)
            {
                rectList.Add(unit);
                int index = 0;
                if (unit.ID.x < x) index = 0;
                if (unit.ID.x > x) index = 2;
                if (unit.y > y) index = 1;
                if (unit.y < y) index = 3;
                rectState[index] = unit.gridState == EGridState.EActive;
            }
            else intersList.Add(unit);
        }

        private void OnClickHandle()
        {
            if (gridState == EGridState.EActive) SetGridState(EGridState.EDisactive);
            else SetGridState(EGridState.EActive);
        }

        public void Dispose()
        {
            if (gridCube != null) gridCube.ClickHandle -= OnClickHandle;
        }
        //设置格子状态
        public void SetGridState(EGridState state)
        {
            if (gridState == state) return;
            this.gridState = state;
            switch (state)
            {
                case EGridState.EActive:
                    stateImgeName = "11";
                    break;
                case EGridState.EDisactive:
                    stateImgeName = "44";
                    break;
                case EGridState.EBreak:
                    stateImgeName = "22";
                    break;
            }
            if (!string.IsNullOrEmpty(stateImgeName)) LoadTexture(stateImgeName);
        }

        public void LoadTexture(string imgName)
        {
            objCube.GetComponent<MeshRenderer>().materials[0].mainTexture = (Texture)(Resources.Load($"ImgCube/{imgName}"));
        }
    }
}