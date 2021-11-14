using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using LitJson;

namespace PpsPro
{
    //格子地图
    public class GridMap
    {
        private Dictionary<Vector2Int, BaseGrid> gridDic;                               //所有格子字典 
        private List<BaseGrid> gridList;                                                //所有格子列表 
        private Dictionary<int, Dictionary<Vector2Int, BaseGrid>> gridGroupDic;         //格子组字典 
        private Dictionary<Vector2Int, BaseGrid> turnDic;                               //拐点字典
        private List<BaseGrid> turnList;
        private List<MoveComponent> moveList;
        public GameObject root;
        private int len;
        private int width;

        public GameObject Root { get { return root; } }

        public void Init()
        {
            root = new GameObject("root");
            InitCacheList();
            RegisterEvent();
        }

        public void Load(string name)
        {

            //[id:0,center:(0, 0),state:0,isTurn:False]
            TextAsset text = Resources.Load<TextAsset>($"Data/{name}");
            JsonData data = JsonMapper.ToObject(text.text);

            for (int i = 0; i < data.Count; i++)
            {
                JsonData gridData = data[i];
                BaseGrid grid = new BaseGrid();
                int id = 0;
                int.TryParse(gridData["id"].ToString(), out id);

                int x = 0;
                int.TryParse(gridData["posx"].ToString(), out x);
                int y = 0;
                int.TryParse(gridData["posy"].ToString(), out y);

                int state;
                int.TryParse(gridData["state"].ToString(), out state);
                EGridState gridState = (EGridState)state;
                bool isTurn = false;
                bool.TryParse(gridData["isTurn"].ToString(), out isTurn);
                grid.Load(id, x, y, isTurn);
                if (isTurn) OnTurnGrid(grid, isTurn);
                grid.SetGridState(gridState);
                grid.SetParent(root.transform);
                AddGrid(grid);
            }
        }

        private void InitCacheList()
        {
            turnList = new List<BaseGrid>();
            moveList = new List<MoveComponent>();
            turnDic = new Dictionary<Vector2Int, BaseGrid>();
            gridList = new List<BaseGrid>();
            gridDic = new Dictionary<Vector2Int, BaseGrid>();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
                UpdateData();
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
            GridMapFuncs.GetGridPath += GetGridPath;
            GridMapFuncs.TurnGrid += OnTurnGrid;
            GridMapFuncs.ShowTurnGrid += OnShowTurnGrid;
        }
        //移除事件
        private void UnRegisterEvent()
        {
            GridMapFuncs.GetGridUnitById -= GetGridUnitById;
            GridMapFuncs.GetGridPath -= GetGridPath;
            GridMapFuncs.TurnGrid -= OnTurnGrid;
            GridMapFuncs.ShowTurnGrid -= OnShowTurnGrid;
        }

        public void AddGrid(BaseGrid grid)
        {
            gridList.Add(grid);
            gridDic.Add(grid.Center, grid);
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

        private List<BaseGrid> GetGridPath(Vector3 originPos, Vector3 targetPos)
        {
            BaseGrid grid = GetGridByPos(originPos);
            BaseGrid targetGrid = GetGridByPos(targetPos);
            if (grid != null && targetGrid != null)
            {
                Debug.Log($"originPos: {originPos} grid : {grid.Center}");
                List<BaseGrid> list = MatchPath(grid, targetGrid);
                return list;
            }
            return null;
        }

        private List<BaseGrid> GetNearestGridPath(Vector3 originPos, Vector3 targetPos)
        {
            BaseGrid grid = GetGridByPos(originPos);
            BaseGrid targetGrid = GetGridByPos(targetPos);
            if (grid != null && targetGrid != null)
            {
                Debug.Log($"originPos: {originPos} grid : {grid.Center}");
                List<BaseGrid> list = MatchPath(grid, targetGrid);
                return list;
            }
            return null;
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
            List<BaseGrid> pathed = new List<BaseGrid>();
            List<List<BaseGrid>> pathList = new List<List<BaseGrid>>();


            MatchTurnPoint(begin, end, passed, pathed, 0, ref pathList);
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

        private void MatchTurnPoint(BaseGrid begin, BaseGrid end, List<BaseGrid> turns, List<BaseGrid> invailds, int index, ref List<List<BaseGrid>> root)
        {
            //Debug.Log($" _________________________________________________________________________________");
            //Debug.Log($" _________________________________________________________________________________");
            //Debug.Log($" _________________________________________________________________________________");
            if (index >= 10) return;
            Debug.Log($" begin: {begin.Center}");
            if (CanLinkEnd(end, begin))
            {
                turns.Add(end);
                Debug.Log($" begin: {begin.Center} 可以到达B点");
                //string tEnd = "";
                //for (int i = 0; i < turns.Count; i++)
                //{
                //    tEnd += turns[i].Center.ToString() + "  ";
                //}
                //Debug.Log($" begin: {begin.Center} 连接到的拐点： { tEnd} ");
                root.Add(turns);
            }
            else
            {
                List<BaseGrid> sameLvs = GetTurnsByGrid(begin);
                //string tLvs = "";
                //for (int i = 0; i < sameLvs.Count; i++)
                //{
                //    tLvs += sameLvs[i].Center.ToString() + "  ";
                //}
                //Debug.Log($" begin: {begin.Center} 连接到的拐点： { tLvs} ");

                //string tTurns = "";
                //for (int i = 0; i < turns.Count; i++)
                //{
                //    tTurns += turns[i].Center.ToString() + "  ";
                //}
                //Debug.Log($" begin: {begin.Center} 不可挑选都有： { tTurns} ");
                for (int i = 0; i < sameLvs.Count; i++)
                {
                    BaseGrid grid = sameLvs[i];
                    if (turns.Contains(grid)) continue;
                    if (invailds.Contains(grid)) continue;
                    //if (CanLinkEnd(begin, grid))
                    {
                        Debug.Log($" begin: {begin.Center} 向下检测到： { grid.Center.ToString()} ");
                        List<BaseGrid> recodeTurn = new List<BaseGrid>();
                        recodeTurn.AddRange(turns);
                        recodeTurn.Add(grid);
                        index++;
                        MatchTurnPoint(grid, end, recodeTurn, sameLvs, index, ref root);
                        //invailds.Add(grid);
                    }
                }
            }
        }

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

        private BaseGrid GetGridByPos(Vector3 pos)
        {
            BaseGrid grid;
            int pos_x = Mathf.FloorToInt(pos.x);
            int pos_y = Mathf.FloorToInt(pos.z);
            gridDic.TryGetValue(new Vector2Int(pos_x, pos_y), out grid);
            return grid;
        }

        //显示拐点格子
        private void OnShowTurnGrid(bool isShow)
        {
            for (int i = 0; i < gridList.Count; i++)
            {
                gridList[i].ShowTurnPoint(isShow);
            }

        }
        public void SaveData(string mapName)
        {
            UpdateData();
            string filePath = Application.dataPath + "/PpsPro/Resources/Data/" + mapName + ".txt";
            if (File.Exists(filePath))
                File.Delete(filePath);

            FileStream fs = new FileStream(filePath, FileMode.Create);


            JsonWriter jw = new JsonWriter();
            jw.WriteArrayStart();//json里面的中括号

            //StringBuilder content = new StringBuilder();
            //content.Append("[");
            //获得字节数组
            for (int i = 0; i < gridList.Count; i++)
            {
                BaseGrid grid = gridList[i];
                //content.Append("{");
                //content.Append($"'id':{grid.ID},'center':{grid.Center},'state':{(int)grid.GridState},'isTurn':{grid.IsTurn}");
                //content.Append("}");

                jw.WriteObjectStart();//json里面的花括号
                jw.WritePropertyName("id");
                jw.Write(grid.ID);

                jw.WritePropertyName("posx");
                jw.Write(grid.Center.x);

                jw.WritePropertyName("posy");
                jw.Write(grid.Center.y);

                jw.WritePropertyName("state");
                jw.Write((int)grid.GridState);

                jw.WritePropertyName("isTurn");
                jw.Write(grid.IsTurn);
                jw.WriteObjectEnd();//花括号括回去
            }
            jw.WriteArrayEnd(); //中括号括回去

            string content = jw.ToString();
            //content.Append("]");           
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