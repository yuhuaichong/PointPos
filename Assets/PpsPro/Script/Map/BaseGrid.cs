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
        private int id;                                 //id
        private int groupId;                            //组ID
        private Vector2Int center;                      //中心点   
        private float radius;                           //半径
        private bool isTurn;                            //是转角位
        private EGridState gridState;                   //格子状态
        public GroudUnit gridCube;                      //格子组件
        private List<BaseGrid> intersList;
        private List<BaseGrid> rectList;
        //private List<bool> rectState;
        private Vector3 position;


        #region 显示相关

        private string stateImgeName;
        public GameObject model;
        public GameObject objCube;                      //格子模型
        private string turnImgName;
        private TextMesh textMesh;
        #endregion


        public int ID { get { return id; } }
        public bool IsTurn { get { return isTurn; } }
        public Vector2Int Center { get { return center; } }
        public EGridState GridState { get { return gridState; } }
        public Vector3 Position { get { return position; } }

        public void Load(int id, int x, int y, GameObject parent)
        {
            this.x = x;
            this.y = y;
            this.id = id;
            center = new Vector2Int(x, y);
            position = new Vector3(x, 1, y);
            turnImgName = "55";
            stateImgeName = "11";

            model = GameObject.Instantiate(Resources.Load("GridCube")) as GameObject;
            if (model == null)
            {
                Debug.LogError("GameObject 未加载到");
                return;
            }
            model.transform.parent = parent.transform;
            model.transform.position = new Vector3(center.x, 0, center.y);
            objCube = model.transform.GetChild(0).gameObject;
            gridCube = objCube.transform.GetComponent<GroudUnit>();
            gridCube.ClickHandle += OnClickHandle;
            SetGridState(EGridState.EActive);
            InitExtendList();
        }

        private void InitExtendList()
        {
            rectList = new List<BaseGrid>(4) { null, null, null, null };
            intersList = new List<BaseGrid>(4) { null, null, null, null };
            //rectState = new List<bool> { false, false, false, false };
            for (int i = x - 1; i <= x; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i == x && j == y) continue;
                    BaseGrid unit = GridMapFuncs.GetGridUnitById(new Vector2Int(i, j));
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
        //设置组ID
        public void SetGroupId(int groupId)
        {
            this.groupId = groupId;
            textMesh.text = groupId.ToString();
        }

        //检测是否是拐点
        public void CheckIsTurn()
        {
            bool hasTurn = false;
            if (gridState == EGridState.EActive)
            {
                int count = intersList.Count;
                for (int i = 0; i < count; i++)
                {
                    BaseGrid grid = intersList[i];
                    if (grid == null) continue;
                    if (grid.gridState == EGridState.EActive) continue;
                    BaseGrid prev = rectList[i];
                    if (prev == null) continue;
                    BaseGrid back = i == count - 1 ? rectList[0] : rectList[i + 1];
                    if (prev.gridState == EGridState.EActive && back.gridState == EGridState.EActive)
                    {
                        hasTurn = true;
                    }
                }
            }
            TrunGrid(hasTurn);
        }

        private void TrunGrid(bool isTurn)
        {
            if (this.isTurn == isTurn) return;
            this.isTurn = isTurn;
            LoadTexture(isTurn ? turnImgName : stateImgeName);
            GridMapFuncs.TurnGrid(this, isTurn);
        }

        //加入扩展列表
        public void AddExtendUnit(BaseGrid unit, bool recursive = true)
        {
            if (unit.Center.x == x || unit.Center.y == y)
                AddRectPoint(unit);
            else
                AddIntersPoint(unit);
            if (recursive) unit.AddExtendUnit(this, false);
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

        private void AddRectPoint(BaseGrid unit)
        {
            if (unit == null) return;
            //坐标顺序预定：左下角 - 西 北 东 南
            int index = 0;
            if (unit.Center.x < x) index = 0;
            if (unit.Center.x > x) index = 2;
            if (unit.y > y) index = 1;
            if (unit.y < y) index = 3;
            rectList[index] = unit;
            //rectState[index] = unit.gridState == EGridState.EActive;
        }

        private void AddIntersPoint(BaseGrid unit)
        {
            if (unit == null) return;
            int index = 0;
            if (unit.Center.x < x && unit.center.y > y) index = 0;
            if (unit.Center.x > x && unit.center.y > y) index = 1;
            if (unit.Center.x > x && unit.y < y) index = 2;
            if (unit.Center.x < x && unit.y < y) index = 3;
            intersList[index] = unit;
        }
    }
}