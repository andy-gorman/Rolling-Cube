using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum Direction
{
	posX,
	negX,
	posZ,
	negZ
}

public class WorldManager : MonoBehaviour
{

	private int direction;
	private CameraMovement CM;

	public GameObject PlayerPrefab;
	//These hold the players face types. Settable in editor.
	public PlayerFaceType top, bottom, posX, negX, posZ, negZ;

	public Canvas StartUI;
	private bool firstime = true;

	public Canvas WinUI;
	public string NextSceneName;


	//This is the instance of the player that is created from PlayerPrefab.
	private GameObject PlayerInst;
	private bool IsPlaying;
	public bool CurPlaying{
		get { return IsPlaying;}
	}
	private float startX, startZ;
	private GroundTile[] tiles;

	void Start()
	{

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
		StartLevel();

		//Allow World to Respond to input.
		//IsPlaying = true;

		//determine the operating way ,1 for positive is z+, 2 for positive is x+ , 3 for positive is z-, 4 for positive is x-;
		direction = 1;
		CM = GameObject.Find ("Main Camera").GetComponent<CameraMovement> ();

		//resetCamera
		CM.resetCamera ();

	}

	void Update()
	{
		direction = CM.direction;
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
	}

	void updateDirection(int dir){
		direction = dir;
	}

	public void LoadNextLevel()
	{
		Application.LoadLevel(NextSceneName);
	}

private void HandleInput() {
		float curX = PlayerInst.transform.position.x;
		float curY = PlayerInst.transform.position.y - 1.0f;
		float curZ = PlayerInst.transform.position.z;
		Controller controller = PlayerInst.GetComponent<Controller> ();

		//Move the player to corresponding direction if the level allows them to.
		if ( (Input.GetButtonDown ("Left") && direction == 1) ||
		     (Input.GetButtonDown ("Backward") && direction == 2) ||
		     (Input.GetButtonDown ("Right") && direction == 3) ||
		     (Input.GetButtonDown ("Forward") && direction == 4) ) {
			if (CanMove (curX - 1, curY, curZ) == 0) {
				controller.RollLeft();
			}
			else if(CanMove (curX - 1, curY, curZ) == 1) {
				controller.RollLeftUp();
			}
			else if(CanMove (curX - 1, curY, curZ) == -1) {
				controller.RollLeftDown();
			}

		} else if ((Input.GetButtonDown ("Right") && direction == 1) ||
		           (Input.GetButtonDown ("Forward") && direction == 2) ||
		           (Input.GetButtonDown ("Left") && direction == 3) ||
		           (Input.GetButtonDown ("Backward") && direction == 4)) {
			if (CanMove (curX + 1, curY, curZ) == 0) {
				controller.RollRight ();
			}
			else if (CanMove (curX + 1, curY, curZ) == 1) {
				controller.RollRightUp ();
			}
			else if (CanMove (curX + 1, curY, curZ) == -1) {
				controller.RollRightDown ();
			}
		} else if ((Input.GetButtonDown ("Forward") && direction == 1) ||
		           (Input.GetButtonDown ("Left") && direction == 2) ||
		           (Input.GetButtonDown ("Backward") && direction == 3) ||
		           (Input.GetButtonDown ("Right") && direction == 4)) {
			if (CanMove (curX, curY, curZ + 1) == 0) {
				controller.RollForward();
			}
			else if (CanMove (curX, curY, curZ + 1) == 1) {
				controller.RollForwardUp();
			}
			else if (CanMove (curX, curY, curZ + 1) == -1) {
				controller.RollForwardDown();
			}
		} else if ((Input.GetButtonDown ("Backward") && direction == 1) ||
		           (Input.GetButtonDown ("Right") && direction == 2) ||
		           (Input.GetButtonDown ("Forward") && direction == 3) ||
		           (Input.GetButtonDown ("Left") && direction == 4)) {
			if (CanMove (curX, curY, curZ - 1) == 0) {
				controller.RollBackward ();
			}
			else if (CanMove (curX, curY, curZ - 1) == 1) {
				controller.RollBackwardUp ();
			}
			else if (CanMove (curX, curY, curZ - 1) == -1) {
				controller.RollBackwardDown ();
			}
		}
	}

	//Essentially Checks if there is a block in the given x, z location.
	private int CanMove(float x, float y, float z)
	{
		if (System.Array.Exists (tiles,
		                        tile => tile.transform.position.x == x
			&& tile.transform.position.z == z
			&& tile.transform.position.y == (y + 1.0f))) {
	//		Debug.Log ("find upper");
			return 1;
		} else if (System.Array.Exists (tiles,
		                              tile => tile.transform.position.x == x
			&& tile.transform.position.z == z
			&& tile.transform.position.y == y)) {
	//		Debug.Log ("find parallel");
			return 0;
		} else if (System.Array.Exists (tiles,
		                              tile => tile.transform.position.x == x
			&& tile.transform.position.z == z
			&& tile.transform.position.y == (y - 1.0f))) {
	//		Debug.Log ("find lower");
			return -1;
		} else {
			//means no block adjacent
			return 9;
		}
	}

	private GroundTile GetTileAtLoc(float x, float y, float z)
	{
		return System.Array.Find (tiles, tile => tile.transform.position.x == x
		                   && tile.transform.position.y == y && tile.transform.position.z == z);	
	}
	
	private TerrainType GetTileTerrAtLoc(float x, float y, float z)
	{
		
		GroundTile tile_ = System.Array.Find (tiles, tile => tile.transform.position.x == x
		                          && tile.transform.position.y == y && tile.transform.position.z == z);
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
					}
					break;
				case Direction.posX:
					if(GetTileTerrAtLoc(x + 1, y, z) == TerrainType.null_exist) {
						PlayerInst.GetComponent<Controller>().SlidePosX();
					}
					break;
				case Direction.negZ:
					if(GetTileTerrAtLoc(x, y, z - 1) == TerrainType.null_exist) {
						PlayerInst.GetComponent<Controller>().SlideNegZ();
					}
					break;
				case Direction.posZ:
					if(GetTileTerrAtLoc(x, y, z + 1) == TerrainType.null_exist) {
						PlayerInst.GetComponent<Controller>().SlidePosZ();
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
			WinLevel();
			break;
		}
	}

	private void ResetPlayer() {
		IsPlaying = false;
		GameObject.Destroy(PlayerInst);
		Start ();

	}

	/*
	 * Start UI Panel
	 */
	private void StartLevel()
	{
		if (firstime == true) {
			StartUI.gameObject.SetActive (true);
			firstime = false;
		} else {
			IsPlaying = true;
		}
	}

	/*
	 * Onclick Start Button
	 */
	public void StartButton()
	{
		IsPlaying = true;
		StartUI.gameObject.SetActive(false);
	}

	/*
	 * TODO: Make a you win screen with next level button.
	 * Have it pop up here.
	 */

	private void WinLevel()
	{
		WinUI.gameObject.SetActive(true);
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
			PlayerInst.transform.FindChild (dir).GetComponent<Renderer> ().material.mainTexture = Resources.Load ("player_" + type.ToString ()) as Texture2D;
		}
	}
}