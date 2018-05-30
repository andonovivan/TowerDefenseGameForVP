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

	[SerializeField]
	private int healthPoints;

	[SerializeField]
	private int rewardAmt;

	private Transform enemy;
	private float navigationTime = 0;
	private int target = 0;
	private bool isDead = false;
	private Collider2D enemyCollider;
	private Animator anim;

	public bool IsDead { get { return isDead; } }

	// Use this for initialization
	void Start () {
		enemy = GetComponent<Transform>();
		enemyCollider = GetComponent<Collider2D>();
		anim = GetComponent<Animator>();
		GameManager.Instance.RegisterEnemy(this);
	}
	
	// Update is called once per frame
	void Update () {
		if (wayPoints != null && !isDead)
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
			GameManager.Instance.RoundEscaped++;
			GameManager.Instance.TotalEscaped++;
			GameManager.Instance.UnregisterEnemy(this);
			GameManager.Instance.IsWaveOver();
		}
		else if(other.tag == "projectile")
		{
			Projectile newP = other.gameObject.GetComponent<Projectile>();
			EnemyHit(newP.AttackStrength);
			Destroy(other.gameObject);
		}
	}

	public void EnemyHit(int hitpoints)
	{
		if(healthPoints - hitpoints > 0)
		{
			healthPoints -= hitpoints;
			anim.Play("Hurt");
		}
		else
		{
			anim.SetTrigger("didDie");
			Die();
		}
	}

	public void Die()
	{
		isDead = true;
		enemyCollider.enabled = false;
		GameManager.Instance.TotalKilled++;
		GameManager.Instance.AddMoney(rewardAmt);
		GameManager.Instance.IsWaveOver();
	}
}
