using System;
using UnityEngine;

public enum PlayerFaceType
{
	none,
	ice,
	spikes
}

public class PlayerCube : MonoBehaviour
{
	private PlayerFaceType top;
	public PlayerFaceType Top {
		get { return top;}
		set { top = value; }
	}
	private PlayerFaceType bottom;
	public PlayerFaceType Bottom {
		get { return bottom; }
		set { bottom = value; }
	}
	private PlayerFaceType negX;
	public PlayerFaceType NegX {
		get { return negX; }
		set { negX = value; }
	}
	private PlayerFaceType posX;
	public PlayerFaceType PosX {
		get { return posX; }
		set { posX = value; }
	}
	private PlayerFaceType posZ;
	public PlayerFaceType PosZ {
		get { return posZ; }
		set { posZ = value; }
	}
	private PlayerFaceType negZ;
	public PlayerFaceType NegZ {
		get { return negZ; }
		set { negZ = value; }
	}
	private PlayerFaceType temp;

	void Start() {
	}
	void Update() {

	}
	
	public void roll( int direction )
	{
		switch(direction)
		{
		case 1://left
			temp = top;
			top = posX;
			posX = bottom;
			bottom = negX;
			negX = temp;
			break;
		case 2://right
			temp = top;
			top = negX;
			negX = bottom;
			bottom = posX;
			posX = temp;
			break;
		case 3://front
			temp = top;
			top = negZ;
			negZ = bottom;
			bottom = posZ;
			posZ = temp;
			break;
		case 4://back
			temp = top;
			top = posZ;
			posZ = bottom;
			bottom = negZ;
			negZ =temp;
			break;
		default:
			break;
		}
	}

	public PlayerFaceType bottomIfTurn (int direction){
		switch (direction) {
		case 1:
			return negX;
		case 2:
			return posX;
		case 3:
			return posZ;
		case 4:
			return negZ;
		default:
			return PlayerFaceType.none;
		}
	}

	
	public PlayerFaceType InBottom()
	{
		return bottom;
	}
	
	public void PrintFaces ()
	{
		Debug.Log("###################");
		Debug.Log (top);
		Debug.Log (negX);
		Debug.Log (posZ);
		Debug.Log (posX);
		Debug.Log (negZ);
		Debug.Log (bottom);
		Debug.Log("###################");
	}
}

