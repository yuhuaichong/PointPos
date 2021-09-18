using System;
using System.Collections.Generic;

namespace PpsPro
{
    public class BaseComponent
    {
        protected BaseActor owner;
        public BaseActor Owner { get { return owner; } }

        public void Load(BaseActor actor)
        {
            owner = actor;
        }

        protected virtual void OnLoad()
        {

        }

        public void Dispose()
        {

        }
        protected virtual void OnDispose()
        {

        }
    }
}
