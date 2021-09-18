using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PpsPro
{
    public class BaseScene
    {
        private GridMap gridMap;                           //地图数据
        private LevelData levelData;                       //关卡数据
        protected BaseActor role;                          //主角
        protected BaseActor monster;

        public BaseActor Role { get { return role; } }

        public void Load(LevelData data)
        {
            levelData = data;
            OnLoad();
        }
        protected virtual void OnLoad()
        {
            gridMap = new GridMap();
            gridMap.Init();
            gridMap.Load(levelData.MapName);
            role = new BaseActor();
            role.Load("Role");
            role.SetBirthPosition(levelData.birthPosList[0]);
            monster = new BaseActor();
            monster.Load("Target");
            monster.SetBirthPosition(new Vector3(18, 1, 18));
        }
        public void Dispose() { OnDispose(); }
        protected virtual void OnDispose()
        {

        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                //gridMap.MoveTo(Role._Transform, monster.Position);
                Role.MoveTo(monster.Position);
            }
            gridMap.Update();
            OnUpdate();
        }
        public void OnUpdate()
        {

        }
    }
}