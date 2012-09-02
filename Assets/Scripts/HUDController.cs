using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour
{
	private Rect timeLabelPos;
	private Rect bonusItemsPos;
	
	void Start()
	{
		timeLabelPos = new Rect(Screen.width -110f, 0f, 110f, 30f);
		bonusItemsPos = new Rect(Screen.width -110f, 20f, 110f, 30f);
	}
	
	void OnGUI()
	{
		GUI.Label(timeLabelPos, "Time: " + LevelController.LevelTimeString);
		GUI.Label(bonusItemsPos, "Bonus Object: " + LevelController.BonusObjectsCollected);
	}
}
