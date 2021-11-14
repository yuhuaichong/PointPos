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
        protected List<BaseActor> monsterList;

        public BaseActor Role { get { return role; } }

        public void Load(LevelData data)
        {
            levelData = data;
            monsterList = new List<BaseActor>();
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
            for (int i = 0; i < 100; i++)
            {
                BaseActor actor = new BaseActor();
                actor.Load("Target");
                actor.SetBirthPosition(new Vector3(Random.Range(0,20), 1, Random.Range(0, 20)));
                monsterList.Add(actor);
            }

            monster = new BaseActor();
            monster.Load("Target");
            monsterList.Add(monster);
            monster.SetBirthPosition(new Vector3(Random.Range(0,20), 1, Random.Range(0, 20)));
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
                //Role.MoveTo(monster.Position);
                for (int i = 0; i < monsterList.Count; i++)
                {
                    monsterList[i].MoveTo(role.Position);
                }
            }
            gridMap.Update();
            OnUpdate();
            role.Update();
            //monster.Update();
            for (int i = 0; i < monsterList.Count; i++)
            {
                monsterList[i].Update();
            }
        }
        public void OnUpdate()
        {

        }
    }
}