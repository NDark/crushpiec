using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkAnimation : MonoBehaviour
{
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

    public List<GameObject> ChunkNodeRef;

    List<GameObject>[] ChunkMapRef = new List<GameObject>[3];
    GameObject[] ChunkRef = new GameObject[3];
    GameObject RootRef = null;
    float m_ScaleFactor = 1.0f;
    float m_ElapsedTime = 0.0f;
    TransformCache[] m_TransformRef = new TransformCache[3];
    AnimationState m_State = AnimationState.InValid;
    AnimationState m_NextState = AnimationState.InValid;

    IEnumerator PerformChangeAnimationState(int _ChunkIndex, AnimationState _State, float _DelayTime, bool _RestoreTransform)
    {
        yield return new WaitForSeconds(_DelayTime);

        // perform change
        // m_State = AnimationState.InValid;
        m_ElapsedTime = 0.0f;

        if (_RestoreTransform)
        {
            m_TransformRef[_ChunkIndex].Restore();

            foreach (GameObject go in ChunkMapRef[_ChunkIndex])
            {
                go.transform.parent = RootRef.transform;
            }
            // ChunkMapRef.Clear();
            ChunkRef[_ChunkIndex] = null;
        }
    }

    public void DoAnimation(
        Character _CharRef,
        string _ChunkNode,
        int _ChunkIndex,
        AnimationState _NextState,
        float _DelayTime = 0.0f)
    {
        RootRef = _CharRef.gameObject;
        ChunkRef[_ChunkIndex] = GlobalSingleton.Find(_ChunkNode, true);
        Mesh_VoxelChunk Mesh = _CharRef.mesh as Mesh_VoxelChunk;
        string BoneIndex = "bone" + _ChunkIndex;

        // ChunkRef
        if (null != ChunkRef
         && null != RootRef
         && Mesh.Chunks.ContainsKey(BoneIndex))
        {
            ChunkMapRef[_ChunkIndex] = Mesh.Chunks[BoneIndex];
            GlobalSingleton.DEBUG("Ready to change parent:"+ ChunkMapRef[_ChunkIndex].Count);

            foreach (GameObject go in ChunkMapRef[_ChunkIndex])
            {
                // go.transform = null;
                go.transform.SetParent(ChunkRef[_ChunkIndex].transform);
            }
        }

        ChangeAnimationState(_ChunkIndex, _NextState, _DelayTime, true);
    }

    public void ChangeAnimationState(
        int _ChunkIndex,
        AnimationState _NextState, 
        float _DelayTime, 
        bool _RestoreTransform = true)
    {
        m_TransformRef[_ChunkIndex] = new TransformCache(ChunkRef[_ChunkIndex].transform);
        // m_ScaleFactor = (-m_RootRef.transform.localScale.x) / m_RootRef.transform.localScale.x;
        m_State = _NextState;

        m_NextState = AnimationState.InValid;
        StartCoroutine(PerformChangeAnimationState(_ChunkIndex, _NextState, _DelayTime, _RestoreTransform));
    }


    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 3; ++i)
        {
            GameObject chunk = ChunkRef[i];
            if (null == chunk)
                continue;

            Vector3 pos = chunk.transform.position;
            switch (m_State)
            {
                case AnimationState.Idle:
                    pos.y = 0.5f * Mathf.Sin(Define.TIME_IDLE * m_ElapsedTime);
                    chunk.transform.position = pos;
                    break;

                case AnimationState.Attack:
                    {
                        float dx = Mathf.Abs(chunk.transform.localScale.x) / chunk.transform.localScale.x;
                        float Speed = dx * Time.deltaTime * 5.0f;
                        chunk.transform.Translate(new Vector3(Speed, 0, 0));
                    }
                    break;

                case AnimationState.Defend:
                    break;
            }
        }
    }
}
