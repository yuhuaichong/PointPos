using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.PpsPro
{
    public class MapTool : MonoBehaviour
    {
        private GridMap gridMap;
        private GameObject gridRoot;
        private BaseScene curScene;
        private bool isEditor;
        private bool isShowTP;
        private bool showGroup;
        private float group_x,initGroup_x,targetGroup_x;

        public GridMap GridMap { get { return gridMap; } }

        void Start()
        {
            gridRoot = new GameObject("root");
            gridMap = new GridMap();
            gridMap.Load(30, 30, gridRoot);
            curScene = new BaseScene();
            curScene.Load();
            group_x = initGroup_x = -310;
            targetGroup_x = 10;
        }
        bool isGo = true;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E)) showGroup = !showGroup;
            group_x = Mathf.MoveTowards(group_x, showGroup ? targetGroup_x : initGroup_x, Time.deltaTime * 400);
            if (Input.GetKeyDown(KeyCode.K))
            {
                gridMap?.UpdateData();
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                Vector3 targetPos = isGo ? new Vector3(27, 0, 27) : new Vector3(5, 0, 5);
                isGo = !isGo;
                gridMap.MoveTo(curScene.Role._Transform, targetPos);
            }
            gridMap?.Update();
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

            GUI.Label(new Rect(20, 35, 80, 35), "编辑模式：");
            GUI.Label(new Rect(110, 35, 80, 35), isEditor ? "开启中" : "关闭中");
            if (GUI.Button(new Rect(200, 35, 80, 30), ">>> 切换 >>>")) isEditor = !isEditor;

            GUI.Label(new Rect(20, 75, 80, 35), "显示拐点：");
            GUI.Label(new Rect(110, 75, 80, 35), isShowTP ? "开启中" : "关闭中");
            if (GUI.Button(new Rect(200, 75, 80, 30), ">>> 切换 >>>")) isShowTP = !isShowTP;

            GUI.EndGroup();
            
        }
    }
}