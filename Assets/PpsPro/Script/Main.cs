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
            gridMap.Load(15, 15, gridRoot);
            curScene = new BaseScene();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                gridMap?.UpdateData();
            }
        }
        void OnDestroy()
        {
            gridMap?.Dispose();
        }
    }
}