using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTile : MonoBehaviour {

	//public GameObject floorTemplate;
	public GameObject wallTileTemplate;
	public GameObject roofTemplate;
	public GameObject floorTemplate;
	public GameObject stairsTemplate;

	public WallTile northWall;
	public WallTile southWall;
	public WallTile eastWall;
	public WallTile westWall;

	public Dictionary<string, WallTile> walls;

	public GameObject northWallPlace;
	public GameObject southWallPlace;
	public GameObject eastWallPlace;
	public GameObject westWallPlace;

	public bool hasRoof;
	public bool hasFloor;
	public bool isStairs;
	public string stairsDirectionName;

	// Use this for initialization
	void Start () {
		if (hasRoof) {
			Instantiate (roofTemplate, transform.position + Vector3.up * (1.4f), Quaternion.identity, transform);
		}

		if (hasFloor) {
			Instantiate (floorTemplate, transform.position + Vector3.down * (1.4f), Quaternion.identity, transform);
		}

	}	

	public void Init() {
		northWall = Instantiate (wallTileTemplate, northWallPlace.transform.position, northWallPlace.transform.rotation, transform).GetComponent<WallTile>();
		southWall = Instantiate (wallTileTemplate, southWallPlace.transform.position, southWallPlace.transform.rotation, transform).GetComponent<WallTile>();
		eastWall = Instantiate (wallTileTemplate, eastWallPlace.transform.position, eastWallPlace.transform.rotation, transform).GetComponent<WallTile>();
		westWall = Instantiate (wallTileTemplate, westWallPlace.transform.position, westWallPlace.transform.rotation, transform).GetComponent<WallTile>();

		walls = new Dictionary<string, WallTile> ();

		walls ["north"] = northWall;
		walls ["south"] = southWall;
		walls ["east"] = eastWall;
		walls ["west"] = westWall;

		Quaternion stairsRotation = Quaternion.identity;
		if (isStairs) {
			if (stairsDirectionName == "north") {
				stairsRotation = Quaternion.identity;
			} else if (stairsDirectionName == "south") {
				stairsRotation = Quaternion.Euler(new Vector3(0, 180, 0));
			} else if (stairsDirectionName == "west") {
				stairsRotation = Quaternion.Euler(new Vector3(0, 90, 0));
			} else if (stairsDirectionName == "east") {
				stairsRotation = Quaternion.Euler(new Vector3(0, -90, 0));
			} 

			Instantiate (stairsTemplate, transform.position, stairsRotation, transform);
		}

	}

	public void InitWalls() {
		northWall.Init ();
		southWall.Init ();
		eastWall.Init ();
		westWall.Init ();
	}
}
