using System;
using System.Collections.Generic;
using UnityEngine;

namespace PpsPro
{
    public class MoveComponent : BaseComponent
    {
        private List<BaseGrid> path;

        public List<BaseGrid> Path { get { return path; } }

        protected override void OnLoad()
        {
            base.OnLoad();
            path = new List<BaseGrid>();
        }
        public bool MoveTo(Vector3 pos)
        {
            Clear();
            if (Path.Count == 0) return false;
            owner.Position = Vector3.MoveTowards(owner.Position, Path[0].Position, Time.deltaTime * 5);
            if (Vector3.Distance(owner.Position, Path[0].Position) < .2f)
            {
                Path.RemoveAt(0);
            }
            return true;
        }

        public void Update()
        {

        }

        private void Clear()
        {
            if (path.Count > 0) path.Clear();
        }
    }
}
