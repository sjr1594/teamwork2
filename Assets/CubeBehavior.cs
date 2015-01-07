using UnityEngine;
using System.Collections;

public class CubeBehavior : MonoBehaviour {

	// Use this for initialization
	public int x, y;
	public bool isExistant;
	public bool isMoving;
	
	//lets this script access the GameController object
	GameScript aGameController;
	
	// Use this for initialization
	void Start () {
		//lets this script access the GameController script
		aGameController = GameObject.Find ("GameController").GetComponent<GameScript> ();
		
		
		
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	/*void OnMouseOver ()
	{
		aGameController.ProcessMouseOver (this.gameObject, x, y);
		
	}
	
	void OnMouseExit ()
	{
		aGameController.ProcessMouseExit (this.gameObject, x, y);
		
	}
	
	*/
	
	
	void OnMouseDown ()
	{
		//every time a cube is clicked, it will call the method ProcessClickedCube on the clicked game object
		if (aGameController.actionTimer > 0) {
						aGameController.ProcessClickedcube (x, y);
				}
		
	}
	
	
	
	
}
