using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PpsPro
{
    public class tpMapEditor : Editor
    {
        [MenuItem("CONTEXT/MapTool/Save")]
        private static void SaveMapData(MenuCommand cmd)
        {
            MapTool tool = cmd.context as MapTool;
            tool?.SaveData();
            AssetDatabase.Refresh();
        }

        private static bool isShow = false;
        [@MenuItem("GameTools/ShowTurnPoint")]
        private static void ShowTurnPoint()
        {
            isShow = !isShow;
            GridMapFuncs.ShowTurnGrid?.Invoke(isShow);
        }
    }
}