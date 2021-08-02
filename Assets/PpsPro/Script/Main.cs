using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.PpsPro
{
    public class Main : MonoBehaviour
    {
        private GridMap gridMap;
        private GameObject gridRoot;
        void Start()
        {
            gridRoot = new GameObject("root");
            gridMap = new GridMap();
            gridMap.Load(3, 3, gridRoot);
        }
        private void Update()
        {
            gridMap?.Update();
        }
        void OnDestroy()
        {
            gridMap?.Dispose();
        }
    }
}