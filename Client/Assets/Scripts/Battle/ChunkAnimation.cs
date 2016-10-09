using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkAnimation : MonoBehaviour {
    public class Define
    {
        static public float TIME_IDLE = 2.5f;
        static public float TIME_ATTACK_START = 0.1f;
        static public float TIME_ATTACK_DO = 0.05f;
        static public float TIME_ATTACK_END = 0.1f;
        static public float TIME_HITTED = 0.1f;
        static public float TIME_DEFEND = 0.2f;
        static public float TIME_SKIPED = 0.0f;

        static public float TIME_ATTACK = TIME_ATTACK_START + TIME_ATTACK_DO + TIME_ATTACK_END / 2.0f;
    };

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
