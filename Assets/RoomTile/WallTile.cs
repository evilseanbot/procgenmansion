using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTile : MonoBehaviour {
	public GameObject solidWallTemplate;
	public GameObject doorWallTemplate;
	public GameObject windowWallTemplate;

	public Type type;

	public enum Type
	{
		none,
		solid,
		door,
		window
	}

	public void Init() {
		if (type == Type.solid) {
			Instantiate (solidWallTemplate, transform.position, transform.rotation, transform);
		} else if (type == Type.door) {
			Instantiate (doorWallTemplate, transform.position, transform.rotation, transform);
		} else if (type == Type.window) {
			Instantiate (windowWallTemplate, transform.position, transform.rotation, transform);
		}
	}
}
