using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.PpsPro
{
    public class Main : MonoBehaviour
    {
        private GridMap gridMap;
        private GameObject gridRoot;
        private BaseScene curScene;
        void Start()
        {
            gridRoot = new GameObject("root");
            gridMap = new GridMap();
            gridMap.Load(30, 30, gridRoot);
            curScene = new BaseScene();
            curScene.Load();
        }
        bool isGo = true;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                gridMap?.UpdateData();
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                Vector3 targetPos = isGo ? new Vector3(27, 0, 27) : new Vector3(5,0, 5);
                isGo = !isGo;
                gridMap.MoveTo(curScene.Role._Transform, targetPos);
            }
            gridMap?.Update();
        }
        void OnDestroy()
        {
            gridMap?.Dispose();
        }
    }
}