using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridHelper
{
    /// <summary>
    /// 计算两点间经过的格子
    /// </summary>
    public static List<Vector2Int> GetTouchedPosBetweenTwoPoints(Vector2Int from, Vector2Int to)
    {
        List<Vector2Int> touchedGrids = GetTouchedPosBetweenOrigin2Target(to - from);
        touchedGrids.Offset(from);
        return touchedGrids;
    }

    /// <summary>
    /// 计算目标位置到原点所经过的格子
    /// </summary>
    static List<Vector2Int> GetTouchedPosBetweenOrigin2Target(Vector2Int target)
    {
        List<Vector2Int> touched = new List<Vector2Int>();
        bool steep = Mathf.Abs(target.y) > Mathf.Abs(target.x);
        int x = steep ? target.y : target.x;
        int y = steep ? target.x : target.y;

        //斜率
        float tangent = (float)y / x;

        float delta = x > 0 ? 0.5f : -0.5f;

        for (int i = 1; i < 2 * Mathf.Abs(x); i++)
        {
            float tempX = i * delta;
            float tempY = tangent * tempX;
            bool isOnEdge = Mathf.Abs(tempY - Mathf.FloorToInt(tempY)) == 0.5f;

            //偶数 格子内部判断
            if ((i & 1) == 0)
            {
                //在边缘,则上下两个格子都满足条件
                if (isOnEdge)
                {
                    touched.AddUnique(new Vector2Int(Mathf.RoundToInt(tempX), Mathf.CeilToInt(tempY)));
                    touched.AddUnique(new Vector2Int(Mathf.RoundToInt(tempX), Mathf.FloorToInt(tempY)));
                }
                //不在边缘就所处格子满足条件
                else
                {
                    touched.AddUnique(new Vector2Int(Mathf.RoundToInt(tempX), Mathf.RoundToInt(tempY)));
                }
            }

            //奇数 格子边缘判断
            else
            {
                //在格子交点处,不视为阻挡,忽略
                if (isOnEdge)
                {
                    continue;
                }
                //否则左右两个格子满足
                else
                {
                    touched.AddUnique(new Vector2Int(Mathf.CeilToInt(tempX), Mathf.RoundToInt(tempY)));
                    touched.AddUnique(new Vector2Int(Mathf.FloorToInt(tempX), Mathf.RoundToInt(tempY)));
                }
            }
        }

        if (steep)
        {
            //镜像翻转 交换 X Y
            for (int i = 0; i < touched.Count; i++)
            {
                Vector2Int v = touched[i];
                v.x = v.x ^ v.y;
                v.y = v.x ^ v.y;
                v.x = v.x ^ v.y;

                touched[i] = v;
            }
        }
        touched.Except(new List<Vector2Int>() { Vector2Int.zero, target });

        return touched;
    }
}
public static class GridsExtension
{
    //添加元素(如果已经有了则不需要重复添加)
    public static void AddUnique(this List<Vector2Int> self, Vector2Int other)
    {
        if (!self.Contains(other))
        {
            self.Add(other);
        }
    }

    //添加元素(如果已经有了则不需要重复添加)
    public static void AddUnique(this List<Vector2Int> self, List<Vector2Int> others)
    {
        if (others == null)
            return;

        for (int i = 0; i < others.Count; i++)
        {
            if (!self.Contains(others[i]))
            {
                self.Add(others[i]);
            }
        }
    }

    //偏移
    public static void Offset(this List<Vector2Int> self, Vector2Int offset)
    {
        for (int i = 0; i < self.Count; i++)
        {
            self[i] += offset;
        }
    }

    //移除操作
    public static void Except(this List<Vector2Int> self, List<Vector2Int> other)
    {
        if (other == null)
            return;

        for (int i = 0; i < other.Count; i++)
        {
            if (self.Contains(other[i]))
            {
                self.Remove(other[i]);
            }
        }
    }
}