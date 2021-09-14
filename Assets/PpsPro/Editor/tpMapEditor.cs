using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PpsPro;
public class tpMapEditor : EditorWindow
{


    private int len = 110;
    private int wid = 110;

    [@MenuItem("GameTools/MapBlockEditor", false, 1)]
    private static void Init()
    {
        tpMapEditor window = (tpMapEditor)EditorWindow.GetWindow(typeof(tpMapEditor), true, "MapBlockEditor");
        window.Show();

    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        {
            GUILayout.Label("地图编辑器-tpMapEditor");

            wid = EditorGUILayout.IntField("网格宽度", wid, GUILayout.Width(200));
            len = EditorGUILayout.IntField("网格高度", len, GUILayout.Width(200));

            if (GUILayout.Button("保 存"))
            {

            }

            if (GUILayout.Button("保 存"))
            {
                AssetDatabase.Refresh();
            }

            GUILayout.Label(" ");
            GUILayout.Label("默认层：Default； 格子类型层（暂未开启，草：Grass）");
            GUILayout.Label(" ");

        }
        EditorGUILayout.EndVertical();
    }
}

//组件编辑
public class tpMapComponentEditor : Editor
{
    [MenuItem("CONTEXT/MapTool/Save")]
    private static void SaveMapData(MenuCommand cmd)
    {
        MapTool tool = cmd.context as MapTool;
        tool?.SaveData();
        AssetDatabase.Refresh();
    }
}