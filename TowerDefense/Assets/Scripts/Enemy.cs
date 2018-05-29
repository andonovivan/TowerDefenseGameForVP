using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	[SerializeField]
	private Transform exitPoint;

	[SerializeField]
	private Transform[] wayPoints;

	[SerializeField]
	private float navigationUpdate;

	private Transform enemy;
	private float navigationTime = 0;
	private int target = 0;


	// Use this for initialization
	void Start () {
		enemy = GetComponent<Transform>();
		GameManager.Instance.RegisterEnemy(this);
	}
	
	// Update is called once per frame
	void Update () {
		if (wayPoints != null)
		{
			navigationTime += Time.deltaTime;
			if(navigationTime > navigationUpdate)
			{
				if(target < wayPoints.Length)
				{
					enemy.position = Vector2.MoveTowards(enemy.position, wayPoints[target].position, navigationTime);
				}
				else
				{
					enemy.position = Vector2.MoveTowards(enemy.position, exitPoint.position, navigationTime);   
				}
				navigationTime = 0;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag== "checkpoint")
		{
			target++;
		}   
		else if (other.tag == "Finish")
		{
			GameManager.Instance.UnregisterEnemy(this);
		}
	}
}
