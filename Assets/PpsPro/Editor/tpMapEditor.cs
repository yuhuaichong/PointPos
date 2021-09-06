using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.PpsPro
{
    public class tpMapEditor : Editor
    {
        [MenuItem("CONTEXT/MapTool/Save")]
        private static void SaveMapData(MenuCommand cmd)
        {
            MapTool tool = cmd.context as MapTool;
            tool?.SaveData();
        }
    }
}