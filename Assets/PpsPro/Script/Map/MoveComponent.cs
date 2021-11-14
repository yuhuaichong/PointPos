using System;
using System.Collections.Generic;
using UnityEngine;

namespace PpsPro
{
    public class MoveComponent : BaseComponent
    {
        private List<BaseGrid> path;
        private EMoveState moveState;
        private bool isPause;

        public List<BaseGrid> Path { get { return path; } }

        protected override void OnLoad()
        {
            base.OnLoad();
            isPause = true;
            path = new List<BaseGrid>();
        }

        protected override void OnUpdate()
        {
            if (isPause) return;
            if (Path.Count == 0) return;
            owner.Position = Vector3.MoveTowards(owner.Position, Path[0].Position, Time.deltaTime * 5);
            if (Vector3.Distance(owner.Position, Path[0].Position) < .2f)
            {
                Path.RemoveAt(0);
            }
        }
        //寻路接口
        public bool MoveTo(Vector3 pos, bool isBegin = true)
        {
            Clear();
            path = GridMapFuncs.GetGridPath(owner.Position, pos);
            if (path == null || path.Count == 0) return false;
            if (isBegin) Start();
            return true;
        }
        public bool MoveToForce(Vector3 pos, bool isBegin = true)
        {
            Clear();
            path = GridMapFuncs.GetGridPath(owner.Position, pos);
            if (path == null || path.Count == 0) return false;
            if (isBegin) Start();
            return true;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            isPause = true;
            moveState = EMoveState.EStop;
            Clear();
            path = null;
        }

        public void Start()
        {
            isPause = false;
            moveState = EMoveState.EMoving;
        }
        //暂停
        public void Pause()
        {
            this.isPause = true;
            moveState = EMoveState.EPause;
        }

        private void Clear()
        {
            if (path?.Count > 0) path.Clear();
        }
    }
}
