using System;
using System.Collections.Generic;

namespace PpsPro
{
    //组件基类
    public class BaseComponent
    {
        protected BaseActor owner;
        public BaseActor Owner { get { return owner; } }

        public void Load(BaseActor actor)
        {
            owner = actor;
            OnLoad();
        }
        protected virtual void OnLoad(){}
        public void Dispose(){ OnDispose(); }
        protected virtual void OnDispose(){}
        public void Update() { OnUpdate(); }
        protected virtual void OnUpdate() { }
    }
}
