using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseActor 
{
    private GameObject model;
    private Transform transform;
    private Vector3 position;
    private Vector3 targetPos;


    public Transform _Transform { get { return transform; } }
    public Vector3 Position { get { return transform.position; } }
    public void Load()
    {
        model = GameObject.Instantiate(Resources.Load("Role")) as GameObject;
        transform = model.transform;
        transform.position = new Vector3(5, .5f, 5);
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
