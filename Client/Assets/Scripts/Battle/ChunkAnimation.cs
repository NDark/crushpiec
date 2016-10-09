using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkAnimation : MonoBehaviour {
    List<GameObject> ChunkMapRef;
    AnimationState ChunkState = AnimationState.Idle;
    string ChunkName;

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {



	}

    void AddAnimation(string name, AnimationState state, List<GameObject> chunk)
    {
        ChunkName = name;
        ChunkState = state;
        ChunkMapRef = chunk;
    }

}
