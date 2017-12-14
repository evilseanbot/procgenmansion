using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcExpo : MonoBehaviour {

	public GameObject procMansionTemplate;
	public List<GameObject> mansions;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < 16; i++) {
			GameObject mansion = Instantiate (procMansionTemplate, Vector3.zero, Quaternion.identity, null);
			mansions.Add (mansion);
		}
		Invoke ("MoveMansions", 0.1f);
	}	

	void MoveMansions() {
		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 4; j++) {
				mansions [i+(j*4)].transform.position += Vector3.right * (i * 72) + Vector3.forward * (j*72);
			}
		}
	}
}
