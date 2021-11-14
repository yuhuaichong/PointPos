using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defines{}

public enum EGridState
{
    EActive,              //激活
    EDisactive,           //未激活
    EBreak,               //断开
    EGround,              //草地
    EFull,                //满载
}

public enum EMoveState
{
    EStop,                //停止
    EMoving,              //移动
    EPause,               //暂停
    EChoke,               //被阻塞
    EBreak,               //断开
}
