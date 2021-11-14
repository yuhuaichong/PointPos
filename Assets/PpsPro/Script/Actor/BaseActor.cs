using UnityEngine;

namespace PpsPro
{
    public class BaseActor
    {
        private GameObject model;
        private Transform transform;
        private MoveComponent moveCom;
        private Vector3 position;
        private Vector3 targetPos;

        public BaseActor Target;


        public Transform _Transform { get { return transform; } }
        public Vector3 Position
        {
            get { return transform.position; }
            set { transform.position = value; }
        }
        public void Load(string actorName)
        {
            LoadModel(actorName);
            LoadComponent();
        }

        protected virtual void LoadModel(string actorName)
        {
            model = GameObject.Instantiate(Resources.Load(actorName)) as GameObject;
            transform = model.transform;
            transform.position = new Vector3(5, .5f, 5);
            targetPos = transform.position;
        }

        protected virtual void LoadComponent()
        {
            moveCom = new MoveComponent();
            moveCom.Load(this);
        }

        public void Update() { OnUpdate(); }
        protected virtual void OnUpdate()
        {
            moveCom?.Update();
        }

        public void MoveTo()
        {
            if (Target != null) MoveTo(Target.position);
        }

        //角色移动接口
        public void MoveTo(Vector3 targetPos)
        {
            if (moveCom == null) return;
            moveCom.MoveTo(targetPos);
        }

        //设置出生位置
        public void SetBirthPosition(Vector3 pos)
        {
            if (transform != null) transform.position = pos;
        }
    }
}