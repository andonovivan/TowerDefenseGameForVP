using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerManager : Singleton<TowerManager> {

	public TowerButton TowerButtonPressed { get; set; }
	private SpriteRenderer spriteRenderer;
	private List<Tower> TowerList = new List<Tower>();
	private List<Collider2D> BuildList = new List<Collider2D>();
	private Collider2D buildTile;

	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		buildTile = GetComponent<Collider2D>();
		spriteRenderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
		{
			Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

			if (hit.collider.tag == "buildSite")
			{
				buildTile = hit.collider;
				buildTile.tag = "buildSiteFull";
				RegisterBuildSite(buildTile);
				PlaceTower(hit);
			}
		}
		if (spriteRenderer.enabled)
		{
			FollowMouse();
		}
	}

	public void RegisterBuildSite(Collider2D buildTag)
	{
		BuildList.Add(buildTag);
	}

	public void RegisterTower(Tower tower)
	{
		TowerList.Add(tower);
	}

	public void RenameTagsBuildSites()
	{
		foreach(Collider2D buildTag in BuildList)
		{
			buildTag.tag = "buildSite";
		}
		BuildList.Clear();
	}

	public void DestroyAllTowers()
	{
		foreach(Tower tower in TowerList)
		{
			Destroy(tower.gameObject);
		}
		TowerList.Clear();
	}

	public void PlaceTower(RaycastHit2D hit)
	{
		if (!EventSystem.current.IsPointerOverGameObject() && TowerButtonPressed != null)
		{
			Tower newTower = Instantiate(TowerButtonPressed.TowerObject);
			newTower.transform.position = hit.transform.position;
			BuyTower(TowerButtonPressed.TowerPrice);
			RegisterTower(newTower);
			DisableDragSprite();
		}
	}

	public void BuyTower(int price)
	{
		GameManager.Instance.SubtractMoney(price);
	}

	public void SelectedTower(TowerButton towerSelected) // which tower button is pressed
	{
		if (towerSelected.TowerPrice <= GameManager.Instance.TotalMoney)
		{
			TowerButtonPressed = towerSelected;
			EnableDragSprite(TowerButtonPressed.DragSprite); // enable the drag from the button press
		}
	}

	public void FollowMouse()
	{
		transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		transform.position = new Vector2(transform.position.x, transform.position.y);
	}

	public void EnableDragSprite(Sprite sprite)
	{
		spriteRenderer.enabled = true;
		spriteRenderer.sprite = sprite;
	}

	public void DisableDragSprite()
	{
		spriteRenderer.enabled = false;
	}
}
