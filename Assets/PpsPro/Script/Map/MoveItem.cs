using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.PpsPro
{
    public class MoveItem
    {
        public Transform Item;
        public List<BaseGrid> Path;

        public bool MoveTo()
        {
            if (Path.Count == 0) return false;
            Item.position = Vector3.MoveTowards(Item.position, Path[0].Center, Time.deltaTime * 2);
            if (Vector3.Distance(Item.position, Path[0].Center) < .2f)
            {
                Path.RemoveAt(0);
            }
            return true;
        }
    }
}
