using System;
using UnityEngine;

namespace Assets.PpsPro
{
    public static class GridMapFuncs
    {
        public static Func<Vector2Int, BaseGrid> GetGridUnitById;
        public static Action<BaseGrid, bool> TurnGrid;
    }
}
