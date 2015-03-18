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
		            								PlayerPrefab.transform.position.y, 
		            								startZ),
		                                            PlayerPrefab.transform.rotation);

		//Set the faces of the model.
		PlayerCube model = PlayerInst.GetComponent<PlayerCube> ();
		model.Top = top;
		model.Bottom = bottom;
		model.PosZ = posZ;
		model.NegZ = negZ;
		model.NegX = negX;
		model.PosX = posX;

		//Texture the cube appropriately.
		PlayerInst.transform.FindChild("Top").GetComponent<Renderer>().material.mainTexture = Resources.Load("player_" + top.ToString()) as Texture2D;
		PlayerInst.transform.FindChild("Bottom").GetComponent<Renderer>().material.mainTexture = Resources.Load("player_" + bottom.ToString()) as Texture2D;
		PlayerInst.transform.FindChild("NegZ").GetComponent<Renderer>().material.mainTexture = Resources.Load("player_" + negZ.ToString()) as Texture2D;
		PlayerInst.transform.FindChild("PosZ").GetComponent<Renderer>().material.mainTexture = Resources.Load("player_" + posZ.ToString()) as Texture2D;
		PlayerInst.transform.FindChild("NegX").GetComponent<Renderer>().material.mainTexture = Resources.Load("player_" + negX.ToString()) as Texture2D;
		PlayerInst.transform.FindChild("PosX").GetComponent<Renderer>().material.mainTexture = Resources.Load("player_" + posX.ToString()) as Texture2D;
		PlayerInst.GetComponent<Controller> ().SetFaces ();

		//open start UI panel
		StartLevel();

		//Allow World to Respond to input.
		//IsPlaying = true;

	}

	void Update() 
	{
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

	public void LoadNextLevel()
	{
		Application.LoadLevel(NextSceneName);
	}

	private void HandleInput() {
		float curX = PlayerInst.transform.position.x;
		float curZ = PlayerInst.transform.position.z;
		Controller controller = PlayerInst.GetComponent<Controller> ();

		//Move the player to corresponding direction if the level allows them to.
		if (Input.GetButtonDown ("Left")) {
			if (CanMove (curX - 1, curZ)) {
				controller.RollLeft();
			}
		} else if (Input.GetButtonDown ("Right")) {
			if (CanMove (curX + 1, curZ)) {
				controller.RollRight ();
			}
		} else if (Input.GetButtonDown ("Forward")) {
			if (CanMove (curX, curZ + 1)) {
				controller.RollForward();
			}
		} else if (Input.GetButtonDown ("Backward")) {
			if (CanMove (curX, curZ - 1)) {
				controller.RollBackward ();
			}
		}
	}

	//Essentially Checks if there is a block in the given x, z location.
	private bool CanMove(float x, float z)
	{
		return System.Array.Exists (tiles, 
		                   tile => tile.transform.position.x == x
		                          && tile.transform.position.z == z);
	}

	private TerrainType GetTileAtLoc(float x, float z) 
	{
		return System.Array.Find (tiles, tile => tile.transform.position.x == x
		                          && tile.transform.position.z == z).TerrType;
	}

	private void CheckFaces() {
		float x = PlayerInst.transform.position.x;
		float z = PlayerInst.transform.position.z;
		switch (GetTileAtLoc(x, z)) {
			case TerrainType.fire:
				if(PlayerInst.GetComponent<PlayerCube>().Bottom != PlayerFaceType.ice) {
					PlayerInst.GetComponent<Controller>().Sink ();
				}
				break;
			case TerrainType.ice:
				if(PlayerInst.GetComponent<PlayerCube>().Bottom != PlayerFaceType.spikes) {
					switch(PlayerInst.GetComponent<Controller>().LastMove) {
						case Direction.negX:
							PlayerInst.GetComponent<Controller>().SlideNegX();
							break;
						case Direction.posX:
							PlayerInst.GetComponent<Controller>().SlidePosX();
							break;
						case Direction.negZ:
							PlayerInst.GetComponent<Controller>().SlideNegZ();
							break;
						case Direction.posZ:
							PlayerInst.GetComponent<Controller>().SlidePosZ();
							break;
					}
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
	
}