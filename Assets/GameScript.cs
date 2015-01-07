using UnityEngine;
using System.Collections;

public class GameScript : MonoBehaviour
{

		//create a prefab to hold the cube
		public GameObject cubePrefab;
	

		//create variables to hold the grid size, grid array, color array, the turn time
		public int gridWidth = 8;
		public int gridHeight = 5;
		public Color[] colorArray;
		public int numberOfColors = 6;
		public GameObject[,] cubeGrid;
		public GameObject pusher1;
		public GameObject pusher2;
		public Vector3[] pusherPositions;
		public int pusherPosArraySize;
		public int gridSpacing = 2;
		public Color c1, c2, c3, c4, c5;
		public int actionTime = 4;
		public float actionTimer;
		public int pusherOneCurrent;
		public int pusherTwoCurrent;
		public bool planningGUIenabled;
		public bool actionGUIenabled;
		public bool timeToFillGrid;
	public bool alreadyFilled;	
		
		//create variables for scoring
		public int minInARowToScore = 3;

		// Use this for initialization
		void Start ()
		{
				//dont instantiate, instead use move method on cubes
				//fill in the colors in the array
				colorArray = new Color[numberOfColors];
				colorArray [0] = Color.black;
				colorArray [1] = Color.blue;
				colorArray [2] = Color.green;
				colorArray [3] = Color.red;
				colorArray [4] = Color.yellow;
				colorArray [5] = Color.white;

				pusherPosArraySize = gridWidth * 2 + gridHeight * 2;
				pusherPositions = new Vector3[pusherPosArraySize];
				for (int i = 0; i < gridWidth; i++) {
						pusherPositions [i] = new Vector3 (i * gridSpacing, gridHeight * gridSpacing, 0);
				}
				int u = gridHeight;
				for (int i = gridWidth; i < gridHeight + gridWidth; i++) {
						u -= 1;
						pusherPositions [i] = new Vector3 (gridWidth * gridSpacing, u * gridSpacing, 0);
				}
				u = gridWidth;
				for (int i = gridWidth + gridHeight; i < gridHeight + gridWidth + gridWidth; i++) {
						u -= 1;
						pusherPositions [i] = new Vector3 (u * gridSpacing, 0 - gridSpacing, 0);
				}
				u = -1;
				for (int i = gridWidth + gridHeight + gridWidth; i < gridHeight + gridWidth + gridWidth + gridHeight; i++) {
						u += 1;
						pusherPositions [i] = new Vector3 (0 - gridSpacing, u * gridSpacing, 0);
				}


				//instantiate the grid of gridwidth by gridheight
				cubeGrid = new GameObject[gridWidth, gridHeight];
				for (int x = 0; x < gridWidth; x++) {
						for (int y = 0; y < gridHeight; y++) {
								cubeGrid [x, y] = (GameObject)Instantiate (cubePrefab, new Vector3 (x * gridSpacing, y * gridSpacing, 0), Quaternion.identity);
								//sets the x and y components in the cube behavior script
								cubeGrid [x, y].GetComponent<CubeBehavior> ().x = x;
								cubeGrid [x, y].GetComponent<CubeBehavior> ().y = y;
								cubeGrid [x, y].renderer.material.color = colorArray [(Random.Range (0, numberOfColors))];

								//color the grid randomly, making sure that the two to the left and two below are not colored the same
								if (x >= minInARowToScore) {
										do {
												cubeGrid [x, y].renderer.material.color = colorArray [(Random.Range (0, numberOfColors))];
												c1 = cubeGrid [x, y].renderer.material.color;
												c2 = cubeGrid [x - 1, y].renderer.material.color;
												c3 = cubeGrid [x - 2, y].renderer.material.color;
										} while (c1 == c2 && c2 == c3);

								}
								if (y >= minInARowToScore) {
										do {
												cubeGrid [x, y].renderer.material.color = colorArray [(Random.Range (0, numberOfColors))];
												c1 = cubeGrid [x, y].renderer.material.color;
												c4 = cubeGrid [x, y - 1].renderer.material.color;
												c5 = cubeGrid [x, y - 2].renderer.material.color;
										} while ((c1 == c2 && c1 == c3) || (c1 == c4 && c1 == c5));					
								}
						}
				}

				CreatePushers ();
				timeToFillGrid = false;
		}
	
		// Update is called once per frame
		void Update ()
		{

		timeToFillGrid = false;
				ProcessKeyboardInput ();
				actionTimer = Mathf.Max (0, actionTimer - Time.deltaTime);
				if (actionTimer > 0) {
						actionGUIenabled = true;
						planningGUIenabled = false;
						alreadyFilled = false;
				} else {
						actionGUIenabled = false;
						planningGUIenabled = true;
			if (alreadyFilled == false){
						timeToFillGrid = true;
				alreadyFilled = true;
			}
						//PushRowsHorizontal();
				}
		if (timeToFillGrid == true) {
			FallToFillGrid();		
		}

		}
	

		//instantiate two pusher cubes labeled one and two of different colors
		void CreatePushers ()
		{

				pusher1 = (GameObject)Instantiate (cubePrefab, pusherPositions [0], Quaternion.identity);
				pusher1.renderer.material.color = colorArray [(Random.Range (0, numberOfColors))];
				pusher1.GetComponent<CubeBehavior> ().x = 0;
				pusher1.GetComponent<CubeBehavior> ().y = gridHeight;
				pusherOneCurrent = 0;
				pusher2 = (GameObject)Instantiate (cubePrefab, pusherPositions [pusherPosArraySize - 1], Quaternion.identity);
				pusher2.renderer.material.color = colorArray [(Random.Range (0, numberOfColors))];
				pusher2.GetComponent<CubeBehavior> ().x = 0 - 2;
				pusher2.GetComponent<CubeBehavior> ().y = gridHeight - 2;
				pusherTwoCurrent = pusherPosArraySize - 1;
		}
		//let the player move them, four keys (both 1 and 2 and clockwise and counterclockwise, skipping corners
		//if a pusher tries to go to the same location as another, it will skip over it (make them able to become a giant cube and push together)

		void ProcessKeyboardInput ()
		{

				if (Input.GetKeyDown (KeyCode.UpArrow)) {
						if (pusherOneCurrent != pusherPosArraySize - 1) {
								pusherOneCurrent++;
						} else {
								pusherOneCurrent = 0;
						}
						if (pusherOneCurrent == pusherTwoCurrent) {
								pusherOneCurrent++;
								if (pusherOneCurrent == pusherPosArraySize) {
										pusherOneCurrent = 0;

								}
						}
						pusher1.transform.position = pusherPositions [pusherOneCurrent];

						//move down
						//Move(cubeGrid[1,1], 0, -1);
				}
				if (Input.GetKeyDown (KeyCode.DownArrow)) {
						if (pusherOneCurrent != 0) {
								pusherOneCurrent--;
						} else {
								pusherOneCurrent = pusherPosArraySize - 1;
						}
						if (pusherOneCurrent == pusherTwoCurrent) {
								pusherOneCurrent--;
								if (pusherOneCurrent == 0 - 1) {
										pusherOneCurrent = pusherPosArraySize - 1;
					
								}
			
						}
						pusher1.transform.position = pusherPositions [pusherOneCurrent];
			
				}
				if (Input.GetKeyDown (KeyCode.RightArrow)) {
						if (pusherTwoCurrent != pusherPosArraySize - 1) {
								pusherTwoCurrent++;
						} else {
								pusherTwoCurrent = 0;
						}
						if (pusherOneCurrent == pusherTwoCurrent) {
								pusherTwoCurrent++;
							if (pusherOneCurrent == pusherPosArraySize - 1) {
								pusherTwoCurrent = 0;
					
				}
						}
						pusher2.transform.position = pusherPositions [pusherTwoCurrent];
				}

				if (Input.GetKeyDown (KeyCode.LeftArrow)) {
						if (pusherTwoCurrent != 0) {
								pusherTwoCurrent--;
						} else {
								pusherTwoCurrent = pusherPosArraySize - 1;
						}
						if (pusherOneCurrent == pusherTwoCurrent) {
								pusherTwoCurrent--;
				if (pusherOneCurrent == 0) {
					pusherTwoCurrent = pusherPosArraySize - 1;
					
				}
							
						}
						pusher2.transform.position = pusherPositions [pusherTwoCurrent];
			
				}

		}


	void Move (GameObject obj, int xDir, int yDir){
		//change xdir and ydir to *gridspacing
		Vector3 endPos = obj.transform.position + new Vector3(xDir * gridSpacing, yDir * gridSpacing, 0);	

		int newX = (obj.GetComponent<CubeBehavior> ().x + xDir);
		int newY = (obj.GetComponent<CubeBehavior> ().y + yDir);
		int oldX = obj.GetComponent<CubeBehavior> ().x;
		int oldY = obj.GetComponent<CubeBehavior> ().y;

		cubeGrid [newX, newY].renderer.material.color = cubeGrid [oldX, oldY].renderer.material.color;
		cubeGrid [newX, newY].GetComponent<CubeBehavior> ().x = cubeGrid [oldX, oldY].GetComponent<CubeBehavior> ().x;
		cubeGrid [newX, newY].GetComponent<CubeBehavior> ().y = cubeGrid [oldX, oldY].GetComponent<CubeBehavior> ().y;
		cubeGrid [newX, newY] = cubeGrid [oldX, oldY];

		if (obj.GetComponent<CubeBehavior>().isMoving == false) {
			StartCoroutine(MoveOverTime(obj.transform, obj.transform.position, endPos, 1f, obj, xDir, yDir));

		}
	}
	
	IEnumerator MoveOverTime (Transform transform, Vector3 startPos, Vector3 endPos, float t, GameObject obj, int xDir, int yDir) {
		obj.GetComponent<CubeBehavior> ().isMoving = true;
		float percentComplete = 0.00f;
		
		while (percentComplete < 1.00f) {
			// make progress as a ratio of deltaTime and desired total time
			percentComplete += Time.deltaTime / t;
			
			// update the position based on our percentage complete
			obj.transform.position = Vector3.Lerp(startPos, endPos, percentComplete);
			
			// stop processing for now, and continue next frame
			yield return null;
		}

		cubeGrid [obj.GetComponent<CubeBehavior> ().x + xDir, obj.GetComponent<CubeBehavior> ().y + yDir].renderer.enabled = true;
			obj.GetComponent<CubeBehavior> ().isMoving = false;
		cubeGrid [obj.GetComponent<CubeBehavior> ().x, gridHeight - 1].renderer.enabled = true; 
		          
		//FillTopRow ();
	}


		//once in the proper place, have the player click an action button relative to screen.width to make things happen
		void OnGUI ()
		{

				if (planningGUIenabled) {
						if (GUI.Button (new Rect (Screen.width / 10, Screen.height / 5, 200, 50), "Action")) {
								StartAction ();
						}
				}

				if (actionGUIenabled) {
						GUI.Label (new Rect (Screen.width / 10, Screen.height / 5, 100, 50), "Time left: " + actionTimer.ToString ()); 
						GUI.Label (new Rect (Screen.width / 10, Screen.height / 4, 100, 50), "You may click cubes to destroy them "); 
				}
		}

		void StartAction ()
		{
				actionTimer = actionTime;
		}

		//the player has turntime seconds to do their actions, as shown by a countdown timer which appeared where the action button was
		//instructional text changes to read "you may click cubes to destroy them"
		//pushers cannot be moved during action, but they can be destroyed
		//players can click as many cubes as they want
		public void ProcessClickedcube (int x, int y)
		{
				cubeGrid [x, y].renderer.enabled = false;
		print (y);
	
		}


		//when the timer reaches zero, resolution phase starts
		//the countdown timer disappears as well as the instrucional text


		//the first pusher moves everything if it wasnt destroyed, this happens over pusherSeconds (1.5)
		//the second pusher moves the same way
		void PushRowsHorizontal ()
		{
				for (int x = 1; x < gridWidth; x++) {
						for (int y = 0; y < gridHeight; y++) {
								if (cubeGrid [x, y].renderer.enabled == false) {
								}
						}
				}
		}
	


		//the cubes outside the grid are loot, and they will be collected by disappearing/moving off screen over dissapearingSeconds (1)
		//if the player has no loot, skip this step (0 seconds)

		//within the grid, group of three in a row/column of the same color
		//scored cubes should fade out over fadeSeconds(0.5), in order of top to bottom


		//all cubes with an empty space below them fall to the bottom of the grid until all spaces are filled in
	void FallToFillGrid (){
		ScoreGameHorizontal ();
		bool isFull = CheckForFullGrid();
		if (isFull == false) {
						for (int x = 0; x < gridWidth; x++) {
								for (int y = 1; y < gridHeight; y++) {
										for (int z = y - 1; z > 0; z--) {
												if (cubeGrid [x, z].renderer.enabled == false) {
														Move (cubeGrid [x, y], 0, -1);
														if(y == gridHeight - 1){
								cubeGrid [x, y] = (GameObject)Instantiate (cubePrefab, new Vector3 (x * gridSpacing, y * gridSpacing, 0), Quaternion.identity);
								cubeGrid [x, y].renderer.enabled = false;
								cubeGrid [x, y].renderer.material.color = colorArray [(Random.Range (0, numberOfColors))];
								//sets the x and y components in the cube behavior script
								cubeGrid [x, y].GetComponent<CubeBehavior> ().x = x;
								cubeGrid [x, y].GetComponent<CubeBehavior> ().y = y;
								cubeGrid [x, y].renderer.material.color = colorArray [(Random.Range (0, numberOfColors))];
								
							}
						}
					}
								}
						}
			isFull = CheckForFullGrid();

		}
	}
		//add random colors to the top of the grid as needed over fallSeconds(1), then score as needed
		//repeat until no more scoring will occur
	void FillTopRow (){

						for (int y = 0; y < gridHeight; y++) {
								for (int x = 0; x < gridWidth; x++) {
										if (cubeGrid [x, y].renderer.enabled == false) {
												cubeGrid [x, gridHeight - 1].renderer.enabled = true;
												cubeGrid [x, gridHeight - 1].renderer.material.color = colorArray [(Random.Range (0, numberOfColors))];
												cubeGrid [x, gridHeight - 1] = (GameObject)Instantiate (cubePrefab, new Vector3 (x * gridSpacing, y * gridSpacing, 0), Quaternion.identity);
												//sets the x and y components in the cube behavior script
												cubeGrid [x, gridHeight - 1].GetComponent<CubeBehavior> ().x = x;
												cubeGrid [x, gridHeight - 1].GetComponent<CubeBehavior> ().y = y;
												cubeGrid [x, gridHeight - 1].renderer.material.color = colorArray [(Random.Range (0, numberOfColors))];

										}
								}
						}
	}

		//go back to planning phase, with instructions and action button and 2 new random pushercubes

	void ScoreGameHorizontal (){
		for (int y = 0; y < gridHeight; y++) {
						for (int x = 0; x < gridWidth - minInARowToScore + 1; x++) {
				if (cubeGrid[x,y].renderer.material.color == cubeGrid[x + 1,y].renderer.material.color && cubeGrid[x+1, y] == cubeGrid [x+2, y]){
					cubeGrid [x, y].renderer.enabled = false;
			
				}
						}
				}

	}

	bool CheckForFullGrid (){
		for (int x = 0; x < gridWidth; x++) {
			for (int y = 0; y < gridHeight; y++) {
				if (cubeGrid[x,y].renderer.enabled == false){
					return false;
				}

			}}
		return true;	
			}
		//scoring:
		//players get x in a row, and points are 10x-20
		//if multiple scoring events happen when things fall into the grid, there is a +1x multiplier for each scoring phase
		//loot will give a scoring bonus for that color, display numbers somewhere on screen.  The bonus is + 10% for each loot of the scored color
		
}
