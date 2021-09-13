using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PpsPro
{
    public class MapTool : MonoBehaviour
    {
        private GridMap gridMap;
        private GameObject root;

        private string mapLen;
        private string mapWid;

        private bool isEditor;
        private bool isShowTP;
        private bool showGroup;
        private float group_x,initGroup_x,targetGroup_x;


        void Start()
        {
            root = new GameObject("root");
            group_x = initGroup_x = -310;
            targetGroup_x = 10;
        }

        private void LoadMap(int length, int width)
        {
            int oriId = 0;
            for (int i = 0; i < length; i++)
            {
                List<BaseGrid> tempList = new List<BaseGrid>();
                for (int j = 0; j < width; j++)
                {
                    BaseGrid grid = new BaseGrid();
                    grid.Load(oriId, i, j);
                    grid.SetParent(root.transform);
                    gridMap.AddGrid(grid);
                    oriId++;
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E)) showGroup = !showGroup;
            group_x = Mathf.MoveTowards(group_x, showGroup ? targetGroup_x : initGroup_x, Time.deltaTime * 400);
        }
        public void SaveData()
        {
            gridMap.SaveData("Map");
        }
        void OnDestroy()
        {
            gridMap?.Dispose();
        }

        private void OnGUI()
        {

            if(!showGroup)GUI.Label(new Rect(10, 10, 130, 30), "按E打开地图编辑面板");

            GUI.BeginGroup(new Rect(group_x, 10, 300, 120));
            GUI.Box(new Rect(0, 0, 300, 120), "地图编辑区域");

            GUI.Label(new Rect(20, 35, 80, 25), "地图的长度：");
            mapLen = GUI.TextField(new Rect(110, 35, 60, 25), mapLen);

            GUI.Label(new Rect(20, 75, 80, 25), "地图的宽度：");
            mapWid = GUI.TextField(new Rect(110, 75, 60, 25), mapWid);

            //GUI.Label(new Rect(20, 75, 80, 35), "显示拐点：");
            //GUI.Label(new Rect(110, 75, 80, 35), isShowTP ? "开启中" : "关闭中");
            //if (GUI.Button(new Rect(200, 75, 80, 30), ">>> 切换 >>>")) isShowTP = !isShowTP;
            if (GUI.Button(new Rect(200, 75, 80, 25), ">>> 加载 >>>"))
            {
                int len, wid;
                if (string.IsNullOrEmpty(mapLen) || string.IsNullOrEmpty(mapWid))
                {
                    Debug.LogError("[error]: 地图尺寸未设置");
                    return;
                }
                if (int.TryParse (mapLen,out len) && int.TryParse(mapWid,out wid))
                {
                    LoadMap(len, wid);
                    return;
                }
                    Debug.LogError("[error]: 地图尺寸输入不规范");
            }

            GUI.EndGroup();
            
        }
    }
}