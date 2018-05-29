using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameStatus
{
	next , play , gameover , win
}

public class GameManager : Singleton<GameManager> {

	[SerializeField]
	private int totalWaves = 10;

	[SerializeField]
	private Text totalMoneyLbl;

	[SerializeField]
	private Text currentWaveLbl;

	[SerializeField]
	private Text totalEscapedLbl;

	[SerializeField]
	private Text playButtonLbl;

	[SerializeField]
	private GameObject spawnPoint;

	[SerializeField]
	private GameObject[] enemies;

	[SerializeField]
	private int maxEnemiesOnScreen;

	[SerializeField]
	private int totalEnemies;

	[SerializeField]
	private int enemiesPerSpawn;

	[SerializeField]
	private Button playButton;

	private int waveNumber = 0;
	private int totalMoney = 10;
	private int totalEscaped = 0;
	private int roundEscaped = 0;
	private int totalKilled = 0;
	private int whichEnemiesToSpawn = 0;
	private GameStatus currentState = GameStatus.play;

	public List<Enemy> EnemyList = new List<Enemy>();

	const float spawnDelay = 0.5f;

	public int TotalMoney {
		get
		{
			return totalMoney;
		}
		set
		{
			totalMoney = value;
			totalMoneyLbl.text = totalMoney.ToString();
		}
	}

	// Use this for initialization
	void Start()
	{
		playButton.gameObject.SetActive(false);
		ShowMenu();
	}

	private void Update()
	{
		HandleEscape(); // to drop the selected tower if we change our mind
	}

	IEnumerator Spawn()
	{
		if (enemiesPerSpawn > 0 && EnemyList.Count < totalEnemies)
		{
			for (int i = 0; i < enemiesPerSpawn; i++)
			{
				if (EnemyList.Count < maxEnemiesOnScreen)
				{
					GameObject newEnemy = Instantiate(enemies[2]) as GameObject;
					newEnemy.transform.position = spawnPoint.transform.position;
				}
			}

			yield return new WaitForSeconds(spawnDelay);
			StartCoroutine(Spawn());
		}
	}

	public void RegisterEnemy(Enemy enemy)
	{
		EnemyList.Add(enemy);
	}

	public void UnregisterEnemy(Enemy enemy)
	{
		EnemyList.Remove(enemy);
		Destroy(enemy.gameObject); // removing the gameObject from the screen
	}

	public void DestroyAllEnemies()
	{
		foreach (Enemy enemy in EnemyList)
		{
			Destroy(enemy.gameObject);
		}
		EnemyList.Clear();
	}


	public void AddMoney(int amount)
	{
		TotalMoney += amount;
	}

	public void SubtractMoney(int amount)
	{
		TotalMoney -= amount;
	}

	public void ShowMenu()
	{
		switch (currentState)
		{
			case GameStatus.gameover:
				playButtonLbl.text = "Play Again!";
				// add gameover
				break;
			case GameStatus.next:
				playButtonLbl.text = "Next Wave";
				break;
			case GameStatus.play:
				playButtonLbl.text = "Play";
				break;
			case GameStatus.win:
				playButtonLbl.text = "Play";
				break;

		}
		playButton.gameObject.SetActive(true);
	}

	private void HandleEscape()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			TowerManager.Instance.DisableDragSprite();
			TowerManager.Instance.TowerButtonPressed = null;
		}
	}
}
