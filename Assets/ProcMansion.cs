using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcMansion : MonoBehaviour {

	public GameObject grassTemplate;
	public GameObject streetTemplate;
	public GameObject roomTileTemplate;

	int[,,] map;

	private int xLength;
	private int zLength;
	private int yLength = 20;
	private float unitLength = 3;

	public class Direction	{
		public string name;
		public Vector3 vector;

		public Direction(string newName, Vector3 newVector) {
			name = newName;
			vector = newVector;
		}
	}

	public class Exit
	{
		public Vector3 pos;
		public Vector3 direction;

		public Exit(Vector3 newPos, Vector3 newDirection) {
			pos = newPos;
			direction = newDirection;
		}

	}

	public List<Direction> directions;

	// Use this for initialization
	void Start () {
		map = new int[20, 20, 20];

		directions = new List<Direction> ();
		directions.Add(new Direction("north", new Vector3(0, 0, -1)));
		directions.Add(new Direction("south", new Vector3(0, 0, 1)));
		directions.Add(new Direction("west", new Vector3(-1, 0, 0)));
		directions.Add(new Direction("east", new Vector3(1, 0, 0)));

		xLength = Random.Range (5, 20);
		zLength = Random.Range (5, 20);

		// Place grass tiles
		for (int x = 0; x < xLength; x++) {
			for (int z = 0; z < zLength; z++) {
				//Instantiate (grassTemplate, (Vector3.right * (unitLength * x)) + (Vector3.forward * (unitLength * z)), Quaternion.identity, transform);
				map[x, 10, z] = 1;
			}
		}

		PlaceOutsideStreets ();

		PlaceRooms ();

		// Place templates on map.
		PlaceTemplatesFromMap();
	}

	void PlaceTemplatesFromMap() {
		for (int x = 0; x < 20; x++) {
			for (int y = 0; y < 20; y++) {
				for (int z = 0; z < 20; z++) {
					Vector3 templatePos = (Vector3.right * (unitLength * x)) + (Vector3.forward * (unitLength * z)) + (Vector3.up * (unitLength * y));
					if (map [x, y, z] == 1) {
						Instantiate (grassTemplate, templatePos, Quaternion.identity, transform);
					} else if (map [x, y, z] == 2) {
						RoomTile roomTile = Instantiate (roomTileTemplate, templatePos, Quaternion.identity, transform).GetComponent<RoomTile> ();

						if (y == 19) {
							roomTile.hasRoof = true;
						} else if (map [x, y + 1, z] != 2) {
							roomTile.hasRoof = true;
						} else {
							roomTile.hasRoof = false;
						}

						if (y == 0) {
							roomTile.hasFloor = true;
						} else if (map [x, y - 1, z] != 2) {
							roomTile.hasFloor = true;
						} else {
							roomTile.hasFloor = false;
						}
							
						roomTile.Init ();

						foreach (Direction direction in directions) {
							if (!IsOnMap (new Vector3 (x, y, z) + direction.vector)) {
								if (roomTile.hasFloor) {
									roomTile.walls [direction.name].type = RandomWallType (true);
								} else {
									roomTile.walls [direction.name].type = RandomWallType (false);
								}
							} else if (map [x + (int)direction.vector.x, y + (int)direction.vector.y, z + (int)direction.vector.z] != 2) {

								// Test if we're in the air
								if (y == 11) {
									roomTile.walls [direction.name].type = RandomWallType (true);
									// Test whether there's a roof to stand on.
								} else if (map [x + (int)direction.vector.x, y + (int)direction.vector.y - 1, z + (int)direction.vector.z] != 2) {
									roomTile.walls [direction.name].type = RandomWallType (false);
								} else if (roomTile.hasFloor) {
									roomTile.walls [direction.name].type = RandomWallType (true);
								} else {
									roomTile.walls [direction.name].type = RandomWallType (false);
								}
							} else {
								roomTile.walls [direction.name].type = WallTile.Type.none;
							}
						}

						roomTile.InitWalls ();
					} else if (map [x, y, z] == 3 || map [x, y, z] == 4 || map [x, y, z] == 5 || map [x, y, z] == 6) {
						RoomTile roomTile = Instantiate (roomTileTemplate, templatePos, Quaternion.identity, transform).GetComponent<RoomTile> ();
						roomTile.isStairs = true;

						if (map [x, y, z] == 3) {
							roomTile.stairsDirectionName = "north";
						} else if (map [x, y, z] == 4) {
							roomTile.stairsDirectionName = "south";
						} else if (map [x, y, z] == 5) {
							roomTile.stairsDirectionName = "west";
						} else if (map [x, y, z] == 6) {
							roomTile.stairsDirectionName = "east";
						}

						roomTile.Init ();


					}
				}
			}
		}
	}

	void PlaceOutsideStreets() {
		int staticY = 10;
		int staticZ = -1;
		for (int x = -1; x < xLength + 1; x++) {
			Instantiate (streetTemplate, (Vector3.right * (unitLength * x)) + (Vector3.forward * (unitLength * staticZ)) + (Vector3.up * (unitLength * staticY)), Quaternion.identity, transform);
		}

		staticZ = zLength;
		for (int x = -1; x < xLength + 1; x++) {
			Instantiate (streetTemplate, (Vector3.right * (unitLength * x)) + (Vector3.forward * (unitLength * staticZ))+ (Vector3.up * (unitLength * staticY)), Quaternion.identity, transform);
		}

		int staticX = -1;
		for (int z = -1; z < zLength + 1; z++) {
			Instantiate (streetTemplate, (Vector3.right * (unitLength * staticX)) + (Vector3.forward * (unitLength * z))+ (Vector3.up * (unitLength * staticY)), Quaternion.identity, transform);
		}

		staticX = xLength;
		for (int z = -1; z < zLength + 1; z++) {
			Instantiate (streetTemplate, (Vector3.right * (unitLength * staticX)) + (Vector3.forward * (unitLength * z))+ (Vector3.up * (unitLength * staticY)), Quaternion.identity, transform);
		}
	}

	void PlaceRooms() {
		Vector3 pos = new Vector3 ((int)Random.Range (0, xLength), 11, (int)Random.Range (0, zLength));
		PlaceRoom (pos, directions[0].vector, true);
	}

	void PlaceRoom(Vector3 pos, Vector3 exitDirection, bool isInitialRoom) {

		int choice = (int)Random.Range (0, 3);
		if (choice == 0) {
			PlaceHallway (pos, exitDirection, isInitialRoom);
		} else if (choice == 1) {
			PlaceFoyer (pos, exitDirection, isInitialRoom);
		} else if (choice == 2) {
			PlaceStairs (pos, exitDirection, isInitialRoom);
		}
	}

	void PlaceHallway(Vector3 pos, Vector3 exitDirection, bool isInitialRoom) {
		int initialLength = (int)Random.Range (2, 9);
		int maxOpenSpaces = MaxOpenSpaces (pos, exitDirection);
		int length = Mathf.Min (initialLength, maxOpenSpaces);

		Vector3 tilePos = pos;
		for (int i = 0; i < length; i++) {
			map [(int)tilePos.x , (int)tilePos.y, (int)tilePos.z] = 2;
			tilePos += exitDirection;
		}
		
		// Find the upper-left corner for this room.
		Vector3 startingPos;
		if (exitDirection.x > 0 || exitDirection.z > 0) {
			startingPos = pos;
		} else {
			startingPos = (pos + (exitDirection * (length - 1)));
		}

		int roomXLength = exitDirection.x != 0 ? length : 1;
		int roomZLength = exitDirection.z != 0 ? length : 1;

		List<Exit> possibleExits = GetPossibleExits (startingPos, roomXLength, roomZLength);

		int exitsUsed;
		if (isInitialRoom) {
			exitsUsed = Random.Range (2, 4);
		} else {
			exitsUsed = Random.Range (0, 3);
		}

		for (int i = 0; i < Mathf.Min(exitsUsed, possibleExits.Count); i++) {
			Exit exit = possibleExits [(int)Random.Range (0, possibleExits.Count)];
			possibleExits.Remove (exit);
			PlaceRoom (exit.pos, exit.direction, false);
		}
	}

	void PlaceFoyer(Vector3 pos, Vector3 exitDirection, bool isInitialRoom) {
		int initialNorthLength = (int)Random.Range (0, 4);
		int maxOpenSpacesNorth = MaxOpenSpaces (pos, new Vector3 (0, 0, -1));
		int northLength = Mathf.Min (initialNorthLength, maxOpenSpacesNorth - 1);

		int initialSouthLength = (int)Random.Range (0, 4);
		int maxOpenSpacesSouth = MaxOpenSpaces (pos, new Vector3 (0, 0, 1));
		int southLength = Mathf.Min (initialSouthLength, maxOpenSpacesSouth - 1);

		int initialWestLength = (int)Random.Range (0, 4);
		int maxOpenSpacesWest = MaxOpenSpaces (pos, new Vector3 (-1, 0, 0));
		int westLength = Mathf.Min (initialWestLength, maxOpenSpacesWest - 1);

		int initialEastLength = (int)Random.Range (0, 4);
		int maxOpenSpacesEast = MaxOpenSpaces (pos, new Vector3 (1, 0, 0));
		int eastLength = Mathf.Min (initialEastLength, maxOpenSpacesEast - 1);

		int initialUpLength = (int)Random.Range (0, 10);
		int maxOpenSpacesUp = MaxOpenSpaces (pos, new Vector3 (0, 1, 0));
		int upLength = Mathf.Min (initialUpLength, maxOpenSpacesUp - 1);

		Vector3 startingPos = pos + (Vector3.back * northLength) + (Vector3.left * westLength);

		int roomXLength = westLength + eastLength + 1;
		int roomZLength = northLength + southLength + 1;
		int roomYLength = upLength + 1;

		Vector3 tilePos = startingPos;
		for (int i = 0; i < roomXLength; i++) {
			for (int j = 0; j < roomZLength; j++) {
				for (int k = 0; k < roomYLength; k++) {
					tilePos = startingPos + (Vector3.forward * j) + (Vector3.right * i) + (Vector3.up * k);
					map [(int)tilePos.x, (int)tilePos.y, (int)tilePos.z] = 2;
				}
			}
		}

		List<Exit> possibleExits = GetPossibleExits (startingPos, roomXLength, roomZLength);

		int exitsUsed;
		if (isInitialRoom) {
			exitsUsed = Random.Range (2, 4);
		} else {
			exitsUsed = Random.Range (0, 3);
		}

		for (int i = 0; i < Mathf.Min(exitsUsed, possibleExits.Count); i++) {
			Exit exit = possibleExits [(int)Random.Range (0, possibleExits.Count)];
			possibleExits.Remove (exit);
			PlaceRoom (exit.pos, exit.direction, false);
		}
	}

	void PlaceStairs(Vector3 pos, Vector3 exitDirection, bool isInitialRoom) {
		int maxOpenSpaces = MaxOpenSpaces (pos, exitDirection);
		int maxOpenSpacesAbove = MaxOpenSpaces (pos + Vector3.up, exitDirection);
		int maxOpenSpacesBelow = MaxOpenSpaces (pos + Vector3.down, exitDirection);

		// Decide which way stairs is going.
		bool goingUp;
		if (maxOpenSpaces < 2) {
			return;
		} else if (maxOpenSpacesAbove > 2 && maxOpenSpacesBelow > 2) {
			int choice = Random.Range (0, 2);
			if (choice == 0) {
				goingUp = true;
			} else {
				goingUp = false;
			}
		} else if (maxOpenSpacesAbove > 2 && maxOpenSpacesBelow < 2) {
			goingUp = true;
		} else if (maxOpenSpacesBelow > 2 && maxOpenSpacesAbove < 2) {
			goingUp = false;
		} else {
			return;
		}
			
		// Initial space
		Vector3 tilePos = pos;
		map [(int)tilePos.x , (int)tilePos.y, (int)tilePos.z] = 2;

		if (goingUp) {
			tilePos = pos + Vector3.up;
			map [(int)tilePos.x , (int)tilePos.y, (int)tilePos.z] = 2;
		}
			
		int stairsCode = 2;
		if (exitDirection == Vector3.back) {
			if (goingUp) {
				stairsCode = 3;
			} else {
				stairsCode = 4;
			}
		} else if (exitDirection == Vector3.forward) {
			if (goingUp) {
				stairsCode = 4;
			} else {
				stairsCode = 3;
			}
		} else if (exitDirection == Vector3.left) {
			if (goingUp) {
				stairsCode = 5;
			} else {
				stairsCode = 6;
			}
		} else if (exitDirection == Vector3.right) {
			if (goingUp) {
				stairsCode = 6;
			} else {
				stairsCode = 5;
			}
		}

		if (goingUp) {
			tilePos = pos + exitDirection;
		} else {
			tilePos = pos + exitDirection + Vector3.down;
		}
		map [(int)tilePos.x , (int)tilePos.y, (int)tilePos.z] = stairsCode;

		// space above stairs
		tilePos += Vector3.up;
		map [(int)tilePos.x , (int)tilePos.y, (int)tilePos.z] = 2;

		// Space beyond stairs
		if (goingUp) {
			tilePos = pos + (exitDirection * 2) + (Vector3.up);
			map [(int)tilePos.x, (int)tilePos.y, (int)tilePos.z] = 2;
		} else {
			tilePos = pos + (exitDirection * 2);
			map [(int)tilePos.x, (int)tilePos.y, (int)tilePos.z] = 2;
			tilePos = pos + (exitDirection * 2) + (Vector3.down);
			map [(int)tilePos.x, (int)tilePos.y, (int)tilePos.z] = 2;
		}
			
		/*// Find the upper-left corner for this room.
		Vector3 startingPos;
		if (exitDirection.x > 0 || exitDirection.z > 0) {
			startingPos = pos;
		} else {
			startingPos = (pos + (exitDirection * (length - 1)));
		}

		int roomXLength = exitDirection.x != 0 ? length : 1;
		int roomZLength = exitDirection.z != 0 ? length : 1;
		*/

		List<Exit> possibleExits;

		if (goingUp) {
			possibleExits = GetPossibleExits (pos + (exitDirection * 2) + (Vector3.up), 1, 1);
		} else {
			possibleExits = GetPossibleExits (pos + (exitDirection * 2) + (Vector3.down), 1, 1);
		}
		
		int exitsUsed;
		if (isInitialRoom) {
			exitsUsed = Random.Range (2, 4);
		} else {
			exitsUsed = Random.Range (0, 3);
		}

		for (int i = 0; i < Mathf.Min(exitsUsed, possibleExits.Count); i++) {
			Exit exit = possibleExits [(int)Random.Range (0, possibleExits.Count)];
			possibleExits.Remove (exit);
			PlaceRoom (exit.pos, exit.direction, false);
		}
	}

	int MaxOpenSpaces(Vector3 pos, Vector3 exitDirection) {
		int openSpaces = 1;
		while (true) {
			pos += exitDirection;
			if (!IsOnMap (pos)) {
				return openSpaces;
			} else if (map [(int)pos.x, (int)pos.y, (int)pos.z] == 2) {
				return openSpaces;
			} else {
				openSpaces++;
			}
		}
	}

	List<Exit> GetPossibleExits(Vector3 startingPos, int roomXLength, int roomZLength) {
		Vector3 checkPos;
		List<Exit> possibleExits = new List<Exit> ();

		// Check northward
		checkPos = startingPos + new Vector3 (0, 0, -1);
		for (int i = 0; i < roomXLength; i++) {
			if (IsOnMap (checkPos)) {
				possibleExits = AddExitIfPossible (possibleExits, new Vector3(0, 0, -1), checkPos);
			}
			checkPos += new Vector3 (1, 0, 0);
		}

		// Check southward
		checkPos = startingPos + new Vector3 (0, 0, 1);
		for (int i = 0; i < roomXLength; i++) {
			if (IsOnMap (checkPos)) {
				possibleExits = AddExitIfPossible (possibleExits, new Vector3(0, 0, 1), checkPos);
			}
			checkPos += new Vector3 (1, 0, 0);
		}

		// Check westward
		checkPos = startingPos + new Vector3 (-1, 0, 0);
		for (int i = 0; i < roomZLength; i++) {
			if (IsOnMap (checkPos)) {
				possibleExits = AddExitIfPossible (possibleExits, new Vector3(-1, 0, 0), checkPos);
			}
			checkPos += new Vector3 (0, 0, 1);
		}

		// Check eastward
		checkPos = startingPos + new Vector3 (1, 0, 0);
		for (int i = 0; i < roomZLength; i++) {
			if (IsOnMap (checkPos)) {
				possibleExits = AddExitIfPossible (possibleExits, new Vector3(1, 0, 0), checkPos);
			}
			checkPos += new Vector3 (0, 0, 1);
		}			

		return possibleExits;
	}

	List<Exit> AddExitIfPossible(List<Exit> possibleExits, Vector3 newExitDirection, Vector3 newExitPos) {
		if (map[(int)newExitPos.x, (int)newExitPos.y, (int)newExitPos.z] != 2) {
			possibleExits.Add(new Exit(newExitPos, newExitDirection) );
		}
		return possibleExits;
	}	

	WallTile.Type RandomWallType( bool isIncludingDoors) {
		int choice = (int)Random.Range (1, 4);
		if (choice == 1) {
			return WallTile.Type.solid;
		} else if (choice == 2) {
			return WallTile.Type.window;
		} else if (choice == 3 && isIncludingDoors) {
			return WallTile.Type.door;
		} else {
			return WallTile.Type.solid;
		}
	}

	bool IsOnMap(Vector3 pos) {
		if (pos.x < 0) {
			return false;
		} else if (pos.x+1 > xLength) {
			return false;
		} else if (pos.z < 0) {
			return false;
		} else if (pos.z+1 > zLength) {
			return false;
		} if (pos.y < 0) {
			return false;
		} else if (pos.y + 1 > 20) {
			return false;
		} else {
			return true;
		}
	}
}
