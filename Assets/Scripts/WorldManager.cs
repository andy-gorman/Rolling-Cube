using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public enum Direction
{
	posX,
	negX,
	posZ,
	negZ,
	negY
}

public class WorldManager : MonoBehaviour
{
	
	private int direction;
	private CameraMovement CM;
	
	public GameObject PlayerPrefab;
	public GameObject TextPrefab;
	//These hold the players face types. Settablex in editor.
	public PlayerFaceType top, bottom, posX, negX, posZ, negZ;
	
	public Image BlackScreen;
	private bool firstime = true;
	
	public Canvas WinUI;
	private int levelNum;
	
	public string[] endMessages;
	
	public Canvas LevelCanvas;
	public Text moveCount;
	
	
	//This is the instance of the player that is created from PlayerPrefab.
	private GameObject PlayerInst;
	private GameObject endText;
	private bool IsPlaying;
	public bool CurPlaying{
		get { return IsPlaying;}
	}
	private float startX, startZ;
	private GroundTile[] tiles;
	private GroundTile ToRemove;
	
	private GameObject indicator;
	
	void Start()
	{
		
		if (WinUI == null) {
			WinUI = GameObject.Find ("Level_Win_Canvas").GetComponent<Canvas>();
			WinUI.gameObject.SetActive (false);
		}
		if (BlackScreen == null) {
			BlackScreen = Instantiate ( Resources.Load ("Black_Screen") as GameObject).GetComponentInChildren<Image>();
		}
		BlackScreen.gameObject.SetActive (false);
		if (LevelCanvas == null) {
			LevelCanvas = (Instantiate (Resources.Load ("Level_Canvas")) as GameObject).GetComponent<Canvas>();
			moveCount = GameObject.Find ("scoreText").GetComponent<Text>();
			Button reset =  GameObject.Find ("Reset").GetComponent<Button>();
			Button toggle = GameObject.Find ("Toggle_Indicator").GetComponent<Button>();;
			reset.onClick.AddListener(() =>{ResetPlayer ();});
			toggle.onClick.AddListener(() => {ToggleIndicator();});
			LevelCanvas.gameObject.SetActive(true);
		}
		
		if (GameObject.Find ("Background_Music") == null) {
			Instantiate(Resources.Load ("Background_Music") as GameObject);
		}
		
		
		//Parse level name into the level number.
		string name = Application.loadedLevelName;
		int index = name.LastIndexOf ('_'); 
		levelNum = System.Convert.ToInt32(name.Substring (index + 1, name.Length - (index + 1)));
		
		//Load the level into an array.
		//This isn't the most efficient way to parse the level,
		//But they won't get to a size where it'll be a problem.
		tiles = GameObject.FindObjectsOfType<GroundTile> ();
		
		//Find the start cube and add the player to it.
		Transform start = System.Array.Find(tiles, tile => tile.TerrType == TerrainType.start).transform;
		startX = start.position.x; startZ = start.position.z;
		//Initialize Player at given start position
		PlayerInst = (GameObject)Object.Instantiate(PlayerPrefab,
		                                            new Vector3(startX,
		            start.position.y + 1f,
		            startZ),
		                                            PlayerPrefab.transform.rotation);
		PlayerInst.name = "Player";
		
		//the direction indicator
		indicator = Instantiate (Resources.Load ("indicator") as GameObject);
		Vector3 indicatorTemp;
		
		indicatorTemp.x = PlayerInst.transform.position.x;
		indicatorTemp.y = PlayerInst.transform.position.y + 1;
		indicatorTemp.z = PlayerInst.transform.position.z;
		
		indicator.transform.position = indicatorTemp;
		
		//Set the faces of the model.
		PlayerCube model = PlayerInst.GetComponent<PlayerCube> ();
		model.Top = top;
		model.Bottom = bottom;
		model.PosZ = posZ;
		model.NegZ = negZ;
		model.NegX = negX;
		model.PosX = posX;
		
		//Texture the cube appropriately.
		TextureCubeFaces ();
		
		//open start UI panel
		Debug.Log ("Replay: " + PlayerPrefs.GetInt("Replay"));
		//if (PlayerPrefs.GetInt ("Replay") == 0) {
		//		StartLevel ();
		//}
		StartLevel ();
		
		//Allow World to Respond to input.
		//IsPlaying = true;
		
		//determine the operating way ,1 for positive is z+, 2 for positive is x+ , 3 for positive is z-, 4 for positive is x-;
		direction = 1;
		CM = GameObject.Find ("Main Camera").GetComponent<CameraMovement> ();
		
		//resetCamera
		CM.resetCamera ();
		
		
		//Set the
		ToRemove = null;
	}
	
	void Update()
	{
		direction = CM.direction;
		
		//update the indicator position and angle
		Vector3 indicatorTemp;
		
		indicatorTemp.x = PlayerInst.transform.position.x;
		indicatorTemp.y = PlayerInst.transform.position.y + 1;
		indicatorTemp.z = PlayerInst.transform.position.z;
		
		indicator.transform.position = indicatorTemp;
		
		if (direction == 1) {
			indicator.transform.eulerAngles = new Vector3 (0, 180, 0);
		} else if (direction == 2) {
			indicator.transform.eulerAngles = new Vector3 (0, 270, 0);
		} else if (direction == 3) {
			indicator.transform.eulerAngles = new Vector3 (0, 0, 0);
		} else if (direction == 4) {
			indicator.transform.eulerAngles = new Vector3 (0, 90, 0);
		}
		
		//Ignore Input until GameState is set to playing.
		if (IsPlaying) {
			if(!PlayerInst.GetComponent<Controller>().Moving) {
				CheckFaces ();
			}
			if(!PlayerInst.GetComponent<Controller>().Moving) {
				HandleInput ();
			}
			if(PlayerInst.GetComponent<Controller>().Dead) {
				ResetPlayer ();
			}
		}
		moveCount.text = "Moves: " + PlayerInst.GetComponent<Controller> ().moveCounter;
	}
	
	void updateDirection(int dir){
		direction = dir;
	}
	
	private void HandleInput() {
		float curX = PlayerInst.transform.position.x;
		float curY = PlayerInst.transform.position.y - 1.0f;
		float curZ = PlayerInst.transform.position.z;
		Controller controller = PlayerInst.GetComponent<Controller> ();
		GroundTile curTile = GetTileAtLoc (curX, curY, curZ);
		
		//Move the player to corresponding direction if the level allows them to.
		if ( (Input.GetButtonDown ("Left") && direction == 1) ||
		    (Input.GetButtonDown ("Backward") && direction == 2) ||
		    (Input.GetButtonDown ("Right") && direction == 3) ||
		    (Input.GetButtonDown ("Forward") && direction == 4) ) {
			if (CanMove (curX - 1, curY, curZ) == 0) {
				controller.RollLeft();
				curTile.PlayerLeave();
				if(ToRemove != null)
				{
					tiles = tiles.Where(tile=>tile != ToRemove).ToArray ();
					ToRemove = null;
				}
				GetTileAtLoc (curX - 1, curY, curZ).PlayerLand ();
			}
			else if(CanMove (curX - 1, curY, curZ) == 1) {
				controller.RollLeftUp();
				curTile.PlayerLeave();
				if(ToRemove != null)
				{
					tiles = tiles.Where(tile=>tile != ToRemove).ToArray();
					ToRemove = null;
				}
				GetTileAtLoc(curX - 1, curY + 1, curZ).PlayerLand ();
			}
			else if(CanMove (curX - 1, curY, curZ) == -1) {
				controller.RollLeftDown();
				curTile.PlayerLeave();
				if(ToRemove != null)
				{
					tiles = tiles.Where(tile=>tile != ToRemove).ToArray();
					ToRemove = null;
				}
				GetTileAtLoc(curX - 1, curY - 1, curZ).PlayerLand ();
			}
			
		} else if ((Input.GetButtonDown ("Right") && direction == 1) ||
		           (Input.GetButtonDown ("Forward") && direction == 2) ||
		           (Input.GetButtonDown ("Left") && direction == 3) ||
		           (Input.GetButtonDown ("Backward") && direction == 4)) {
			if (CanMove (curX + 1, curY, curZ) == 0) {
				controller.RollRight ();
				curTile.PlayerLeave();
				if(ToRemove != null)
				{
					tiles = tiles.Where(tile=>tile != ToRemove).ToArray();
					ToRemove = null;
				}
				GetTileAtLoc (curX + 1, curY, curZ).PlayerLand ();
			}
			else if (CanMove (curX + 1, curY, curZ) == 1) {
				controller.RollRightUp ();
				curTile.PlayerLeave();
				if(ToRemove != null)
				{
					tiles = tiles.Where(tile=>tile != ToRemove).ToArray();
					ToRemove = null;
				}
				GetTileAtLoc (curX + 1, curY + 1, curZ).PlayerLand ();
			}
			else if (CanMove (curX + 1, curY, curZ) == -1) {
				controller.RollRightDown ();
				curTile.PlayerLeave();
				if(ToRemove != null)
				{
					tiles = tiles.Where(tile=>tile != ToRemove).ToArray();
					ToRemove = null;
				}
				GetTileAtLoc(curX + 1, curY - 1, curZ).PlayerLand();
			}
		} else if ((Input.GetButtonDown ("Forward") && direction == 1) ||
		           (Input.GetButtonDown ("Left") && direction == 2) ||
		           (Input.GetButtonDown ("Backward") && direction == 3) ||
		           (Input.GetButtonDown ("Right") && direction == 4)) {
			if (CanMove (curX, curY, curZ + 1) == 0) {
				controller.RollForward();
				curTile.PlayerLeave();
				if(ToRemove != null)
				{
					tiles = tiles.Where(tile=>tile != ToRemove).ToArray();
					ToRemove = null;
				}
				GetTileAtLoc(curX, curY, curZ + 1).PlayerLand();
			}
			else if (CanMove (curX, curY, curZ + 1) == 1) {
				controller.RollForwardUp();
				curTile.PlayerLeave();
				if(ToRemove != null)
				{
					tiles = tiles.Where(tile=>tile != ToRemove).ToArray();
					ToRemove = null;
				}
				GetTileAtLoc(curX, curY + 1, curZ + 1).PlayerLand();
			}
			else if (CanMove (curX, curY, curZ + 1) == -1) {
				controller.RollForwardDown();
				curTile.PlayerLeave();
				if(ToRemove != null)
				{
					tiles = tiles.Where(tile=>tile != ToRemove).ToArray();
					ToRemove = null;
				}
				GetTileAtLoc(curX, curY - 1, curZ + 1).PlayerLand();
			}
		} else if ((Input.GetButtonDown ("Backward") && direction == 1) ||
		           (Input.GetButtonDown ("Right") && direction == 2) ||
		           (Input.GetButtonDown ("Forward") && direction == 3) ||
		           (Input.GetButtonDown ("Left") && direction == 4)) {
			if (CanMove (curX, curY, curZ - 1) == 0) {
				controller.RollBackward ();
				curTile.PlayerLeave();
				if(ToRemove != null)
				{
					tiles = tiles.Where(tile=>tile != ToRemove).ToArray();
					ToRemove = null;
				}
				GetTileAtLoc(curX, curY, curZ - 1).PlayerLand();
			}
			else if (CanMove (curX, curY, curZ - 1) == 1) {
				controller.RollBackwardUp ();
				curTile.PlayerLeave();
				if(ToRemove != null)
				{
					tiles = tiles.Where(tile=>tile != ToRemove).ToArray();
					ToRemove = null;
				}
				GetTileAtLoc(curX, curY + 1, curZ - 1).PlayerLand();
			}
			else if (CanMove (curX, curY, curZ - 1) == -1) {
				controller.RollBackwardDown ();
				curTile.PlayerLeave();
				if(ToRemove != null)
				{
					tiles = tiles.Where(tile=>tile != ToRemove).ToArray();
					ToRemove = null;
				}
				GetTileAtLoc(curX, curY - 1, curZ - 1).PlayerLand();
			}
		}
	}
	
	//Essentially Checks if there is a block in the given x, z location.
	private int CanMove(float x, float y, float z)
	{
		RaycastHit hit;
		if (System.Array.Exists (tiles,
		                         tile => tile.transform.position.x == x
		                         && tile.transform.position.z == z
		                         && tile.transform.position.y == (y + 1.0f))) {
			if (!System.Array.Exists (tiles,
			                          tile => Mathf.Approximately(tile.transform.position.x, x)
			                          && Mathf.Approximately(tile.transform.position.z, z)
			                          && Mathf.Approximately(tile.transform.position.y, (y + 2.0f)))) {
				return 1;
			} else {
				return 9;
			}
		} else if (System.Array.Exists (tiles,
		                                tile => Mathf.Approximately(tile.transform.position.x, x)
		                                && Mathf.Approximately(tile.transform.position.z,z)
		                                && Mathf.Approximately(tile.transform.position.y, y))) {
			//		Debug.Log ("find parallel");
			return 0;
		} else if (Physics.Raycast (new Vector3(x, y, z), Vector3.down * 100, out hit)) {
			return 0;
		} else if (System.Array.Exists (tiles,
		                                tile => tile.transform.position.x == x
		                                && tile.transform.position.z == z
		                                && tile.transform.position.y == (y - 1.0f))) {
			//		Debug.Log ("find lower");
			return -1;
		} else {
			//means no block adjacent
			Debug.Log ("no adjacent");
			return 9;
		}
	}
	
	private GroundTile GetTileAtLoc(float x, float y, float z)
	{
		return System.Array.Find (tiles, tile => Mathf.Approximately(tile.transform.position.x, x)
		                          && Mathf.Approximately(tile.transform.position.y, y)
		                          && Mathf.Approximately(tile.transform.position.z, z));
	}
	
	private TerrainType GetTileTerrAtLoc(float x, float y, float z)
	{
		
		GroundTile tile_ = System.Array.Find (tiles, tile => Mathf.Approximately(tile.transform.position.x, x)
		                                      && Mathf.Approximately(tile.transform.position.y, y)
		                                      && Mathf.Approximately(tile.transform.position.z, z));
		if (tile_ != null) {
			return tile_.TerrType;
		} else {
			return TerrainType.null_exist;
		}
	}
	
	private void CheckFaces() {
		float x = PlayerInst.transform.position.x;
		float z = PlayerInst.transform.position.z;
		float y = PlayerInst.transform.position.y - 1f;
		switch (GetTileTerrAtLoc(x, y, z)) {
		case TerrainType.fire:
			if(PlayerInst.GetComponent<PlayerCube>().Bottom != PlayerFaceType.ice) {
				PlayerInst.GetComponent<Controller>().Sink ();
			}
			break;
		case TerrainType.ice:
			y += 1f;
			if(PlayerInst.GetComponent<PlayerCube>().Bottom != PlayerFaceType.spikes) {
				switch(PlayerInst.GetComponent<Controller>().LastMove) {
				case Direction.negX:
					if(GetTileTerrAtLoc(x - 1, y, z) == TerrainType.null_exist) {
						PlayerInst.GetComponent<Controller>().SlideNegX();
						GetTileAtLoc (x - 1, y-1, z).PlayerLand();
					}
					break;
				case Direction.posX:
					if(GetTileTerrAtLoc(x + 1, y, z) == TerrainType.null_exist) {
						PlayerInst.GetComponent<Controller>().SlidePosX();
						GetTileAtLoc(x + 1, y-1, z).PlayerLand();
					}
					break;
				case Direction.negZ:
					if(GetTileTerrAtLoc(x, y, z - 1) == TerrainType.null_exist) {
						PlayerInst.GetComponent<Controller>().SlideNegZ();
						GetTileAtLoc (x, y-1, z - 1).PlayerLand();
					}
					break;
				case Direction.posZ:
					if(GetTileTerrAtLoc(x, y, z + 1) == TerrainType.null_exist) {
						PlayerInst.GetComponent<Controller>().SlidePosZ();
						GetTileAtLoc(x, y-1, z + 1).PlayerLand();
					}
					break;
				}
			}
			break;
		case TerrainType.teleport:
			TeleportTile paired = (GetTileAtLoc(x, y, z) as TeleportTile).OtherEnd;
			if(paired != null && !PlayerInst.GetComponent<Controller>().JustTeleported) {
				Vector3 pairedLoc = paired.transform.position;
				pairedLoc.y += 1;
				StartCoroutine(PlayerInst.GetComponent<Controller>().TeleportTo(pairedLoc));
			}/* else {
				Debug.Log ("Doesn't have a paired teleporter");
			}*/
			break;
		case TerrainType.temporary:
			TempTile tile = GetTileAtLoc(x, y, z) as TempTile;
			if(tile.life <= 0) {
				ToRemove = tile;
			}
			break;
		case TerrainType.null_exist:
			RaycastHit hit;
			if(Physics.Raycast (PlayerInst.transform.position, Vector3.down * 100, out hit)) {
				float fallDist = hit.distance - 0.5f;
				StartCoroutine(PlayerInst.GetComponent<Controller>().Fall(fallDist));
			} else {
				StartCoroutine(PlayerInst.GetComponent<Controller>().FallToDeath());
			}
			break;
		case TerrainType.finish:
			IsPlaying = false;
			StartCoroutine(WinLevel());
			break;
		}
	}
	
	public void ResetPlayer() {
		PlayerPrefs.SetInt ("Replay", 1);
		Application.LoadLevel (Application.loadedLevel);
	}
	
	/*
	 * Start UI Panel
	 */
	private void StartLevel()
	{
		IsPlaying = true;
		/*&if (firstime == true) {
			StartUI.gameObject.SetActive (true);
			firstime = false;
		} else {
			IsPlaying = true;
		}*/
	}
	
	
	public void QuitGame()
	{
		Application.Quit ();
	}
	
	public void ToggleIndicator()
	{
		foreach (MeshRenderer mr in indicator.GetComponentsInChildren<MeshRenderer> ()) {
			mr.enabled = !mr.enabled;
		}
	}
	
	
	private IEnumerator WinLevel()
	{
		BlackScreen.gameObject.SetActive (true);
		LevelCanvas.gameObject.SetActive (false);
		//Play the level ending sound.s
		AudioSource audio = GetComponent < AudioSource > ();
		if (audio != null) {
			yield return new WaitForSeconds (0.5f);
			audio.Play ();
		}
		
		//Fade in a black screen.
		float t = 0f;
		while (BlackScreen.color.a < 1f) { 
			t += Time.deltaTime * 0.1f;
			BlackScreen.color = Color.Lerp (BlackScreen.color, Color.black, t);
			yield return 0;
		}
		BlackScreen.color = Color.black;
		if (audio != null) {
			while (audio.isPlaying) {
				yield return 0;
			}
		}
		
		if (endMessages.Length > 0) {
			endText = Instantiate (TextPrefab);
			TextBox box = endText.GetComponentInChildren<TextBox> ();
			box.messages = endMessages;
			
			endText.transform.SetParent (BlackScreen.transform);
			
			RectTransform rect = endText.GetComponent<RectTransform> ();
			
			//Reset the anchors of the rect.
			rect.anchoredPosition = new Vector2 (0, 0);
			rect.offsetMax = new Vector2 (0, 0);
			rect.offsetMin = new Vector2 (0, 0);
			yield return 0;
			
			//Wait for the textbox to run through its text.
			
			while (!box.Done) {
				yield return 0;
			}
		}
		
		//TODO: EndGame UI popup?
		
		//Load the next Level
		//yield return new WaitForSeconds (1);
		PlayerPrefs.DeleteKey ("Replay");
		Application.LoadLevel ("Level_" + (levelNum + 1));
	}
	
	private void TextureCubeFaces()
	{
		TextureCubeFace ("Top", top);
		TextureCubeFace ("Bottom", bottom);
		TextureCubeFace ("NegZ", negZ);
		TextureCubeFace ("PosZ", posZ);
		TextureCubeFace ("PosX", posX);
		TextureCubeFace ("NegX", negX);
		PlayerInst.GetComponent<Controller> ().SetFaces ();
	}
	
	private void TextureCubeFace(string dir, PlayerFaceType type)
	{
		if (type != PlayerFaceType.none) {
			//PlayerInst.transform.FindChild (dir).GetComponent<Renderer> ().material.mainTexture = Resources.Load ("player_" + type.ToString ()) as Texture2D;
			PlayerInst.transform.FindChild(dir).GetComponent<Renderer>().material = Resources.Load ("Materials/player_" + type.ToString ()) as Material;
		}
	}
	
}