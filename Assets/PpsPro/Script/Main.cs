using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PpsPro
{
    public class Main : MonoBehaviour
    {
        private BaseScene curScene;
        void Start()
        {
            curScene = new BaseScene();
            curScene.Load();
        }

        void Update()
        {
            curScene.Update();
        }
    }
}