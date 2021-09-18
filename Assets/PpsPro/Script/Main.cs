using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PpsPro
{
    public class Main : MonoBehaviour
    {
        public string mapName;
        private BaseScene curScene;
        void Start()
        {
            List<Vector3> list = new List<Vector3>()
            {
                new Vector3(1,1,1),
                new Vector3(2,1,2),
                new Vector3(3,1,3),
                new Vector3(4,1,4),
                new Vector3(5,1,5),
            };
            LevelData levelData = new LevelData();
            levelData.MapName = mapName;
            levelData.birthPosList = list;
            curScene = new BaseScene();
            curScene.Load(levelData);
        }

        void Update()
        {
            curScene.Update();
        }
    }
}