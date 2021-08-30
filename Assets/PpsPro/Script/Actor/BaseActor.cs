using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseActor 
{
    private GameObject model;
    private Transform transform;
    private Vector3 position;
    private Vector3 targetPos;


    public Vector3 Position { get { return transform.position; } }
    public void Load()
    {
        model = GameObject.Instantiate(Resources.Load("ACube")) as GameObject;
        transform = model.transform;
        transform.position = new Vector3(0, .5f, 0);
        targetPos = transform.position;
    }

    public void Update()
    {
        if (Vector3.Distance(targetPos,Position) > .1f)
        {
            Move();
        }
    }

    public void Move()
    {

    }
}
