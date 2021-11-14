using System;
using System.Collections.Generic;
using UnityEngine;

namespace PpsPro
{
    public static class GridMapFuncs
    {
        public static Func<Vector2Int, BaseGrid> GetGridUnitById;
        public static Func<Vector3, Vector3, List<BaseGrid>> GetGridPath;
        public static Func<Vector3, Vector3, List<BaseGrid>> GetNearestGridPath;

        public static Action<BaseGrid, bool> TurnGrid;
        public static Action<bool> ShowTurnGrid;
    }
}
