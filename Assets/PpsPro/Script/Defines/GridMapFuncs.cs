using System;
using UnityEngine;

namespace PpsPro
{
    public static class GridMapFuncs
    {
        public static Func<Vector2Int, BaseGrid> GetGridUnitById;
        public static Action<BaseGrid, bool> TurnGrid;
    }
}
