using UnityEngine;
using System.Collections;

public class TempTile : GroundTile {

	private int life;
	public int Life{
		get {return life;}
	}

	void Start () {
		life = 3;
	}

	// Update is called once per frame
	void Update ()
	{
	}


	public override void PlayerLand()
	{
		life--;
		if(life > 0)
		{
			GetComponent<Renderer>().material =
										Resources.Load("Crack_Face_" + life) as Material;
		}
	}
	public override void PlayerLeave()
	{
		if(life <= 1)
		{
			StartCoroutine(Fall());
		}
	}
	IEnumerator Fall()
	{
		float distance = 10f;
		Vector3 startPos = transform.position;
		Vector3 endPos = transform.position; endPos.y -= distance;
		float startTime = Time.time;
		while (transform.position != endPos) {
			float distCovered = (Time.time - startTime) * 3f;
			float fracJourney = distCovered / distance;
			transform.position = Vector3.Lerp(startPos, endPos, fracJourney);
			yield return 0;
		}
		Destroy(gameObject);
	}
}

