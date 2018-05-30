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
	private Enemy[] enemies;

	[SerializeField]
	private int totalEnemies = 3;

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
	private int enemiesToSpawn = 0;
	private GameStatus currentState = GameStatus.play;
	private AudioSource audioSource;

	public List<Enemy> EnemyList = new List<Enemy>();

	const float spawnDelay = 0.5f;

	public int TotalEscaped { get { return totalEscaped; } set { totalEscaped = value; } }
	public int RoundEscaped { get { return roundEscaped; } set { roundEscaped = value; } }
	public int TotalKilled { get { return totalKilled; } set { totalKilled = value; } }
	public AudioSource AudioSource { get { return audioSource; } }

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
		audioSource = GetComponent<AudioSource>();
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
				if (EnemyList.Count < totalEnemies)
				{
					Enemy newEnemy = Instantiate(enemies[Random.Range(0, enemiesToSpawn)]);
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

	public void IsWaveOver()
	{
		totalEscapedLbl.text = "Escaped " + TotalEscaped + "/10";
		if((RoundEscaped + TotalKilled) == totalEnemies)
		{
			if(waveNumber <= enemies.Length)
			{
				enemiesToSpawn = waveNumber;
			}
			SetCurrentGameState();
			ShowMenu();
		}
	}

	public void SetCurrentGameState()
	{
		if (TotalEscaped >= 3)
		{
			currentState = GameStatus.gameover;
		}
		else if (waveNumber == 0 && (TotalKilled + RoundEscaped) == 0)
		{
			currentState = GameStatus.play;
		}
		else if (waveNumber >= totalWaves)
		{
			currentState = GameStatus.win;
		}
		else
			currentState = GameStatus.next;
	}

	public void ShowMenu()
	{
		switch (currentState)
		{
			case GameStatus.gameover:
				playButtonLbl.text = "Play Again!";
				AudioSource.PlayOneShot(SoundManager.Instance.Gameover);
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

	public void PlayButtonPressed()
	{
		switch (currentState)
		{
			case GameStatus.next:
				waveNumber++;
				totalEnemies += waveNumber;
				break;
			default:
				totalEnemies = 3;
				TotalEscaped = 0;
				TotalMoney = 10;
				TowerManager.Instance.DestroyAllTowers();
				TowerManager.Instance.RenameTagsBuildSites();
				enemiesToSpawn = 0;
				totalMoneyLbl.text = TotalMoney.ToString();
				totalEscapedLbl.text = "Escaped " + TotalEscaped + "/10";
				audioSource.PlayOneShot(SoundManager.Instance.NewGame); // complete the audio clip without cutting it off
				break;
		}
		DestroyAllEnemies();
		TotalKilled = 0;
		RoundEscaped = 0;
		currentWaveLbl.text = "Wave " + (waveNumber + 1);
		StartCoroutine(Spawn());
		playButton.gameObject.SetActive(false);
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
