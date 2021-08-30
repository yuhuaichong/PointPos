using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScene : IObject
{
    protected BaseActor role;
    public BaseActor Role { get { return role; } }

    public void Load() { OnLoad(); }
    protected virtual void OnLoad()
    {
        role = new BaseActor();
        role.Load();
    }
    public void Dispose() { OnDispose(); }
    protected virtual void OnDispose()
    {

    }
}
