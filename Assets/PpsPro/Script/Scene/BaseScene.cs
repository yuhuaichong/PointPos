using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PpsPro
{
    public class BaseScene : IObject
    {
        protected BaseActor role;
        private GridMap gridMap;
        private bool isGo = true;

        public BaseActor Role { get { return role; } }

        public void Load() { OnLoad(); }
        protected virtual void OnLoad()
        {
            gridMap = new GridMap();
            gridMap.Init();
            gridMap.Load("Map");
            role = new BaseActor();
            role.Load();
        }
        public void Dispose() { OnDispose(); }
        protected virtual void OnDispose()
        {

        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                Vector3 targetPos = isGo ? new Vector3(27, 0, 27) : new Vector3(5, 0, 5);
                isGo = !isGo;
                gridMap.MoveTo(Role._Transform, targetPos);
            }
            gridMap.Update();
            OnUpdate();
        }
        public void OnUpdate()
        {

        }
    }
}