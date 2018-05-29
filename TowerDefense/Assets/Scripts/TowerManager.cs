using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerManager : Singleton<TowerManager> {

	private TowerButton towerButtonPressed;
	private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
		{
			Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

			if (hit.collider.tag == "buildSite")
			{
				hit.collider.tag = "buildSiteFull";
				PlaceTower(hit);
			}
		}
		if (spriteRenderer.enabled)
		{
			FollowMouse();
		}
	}

	public void PlaceTower(RaycastHit2D hit)
	{
		if (!EventSystem.current.IsPointerOverGameObject() && towerButtonPressed != null)
		{
			GameObject newTower = Instantiate(towerButtonPressed.TowerObject);
			newTower.transform.position = hit.transform.position;
			DisableDragSprite();
		}
	}

	public void SelectedTower(TowerButton towerSelected) // which tower button is pressed
	{
		towerButtonPressed = towerSelected;
		EnableDragSprite(towerButtonPressed.DragSprite); // enable the drag from the button press
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
