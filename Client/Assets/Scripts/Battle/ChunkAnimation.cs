/**

@date 20161031 by NDark
. add class member m_OpponentTargetMap
. add class member m_ActuallyBeenHittedMap
. add class method SetOpponentTarget()
. add class method ClearOpponentTarget()
. add class method ClearAllOpponentTargets()

*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
show shield in init (done)
no defense animation (done)
after battle change back (done)
hit animation
animation speed (changed)
attack hit event
 */
public class ChunkAnimation : MonoBehaviour
{
    public List<GameObject> ChunkNodeRef;

    List<GameObject>[] ChunkMapRef = new List<GameObject>[3];
    GameObject[] ChunkRef = new GameObject[3];
    GameObject RootRef = null;
    float m_ScaleFactor = 1.0f;
    float m_ElapsedTime = 0.0f;
    TransformCache[] m_TransformRef = new TransformCache[3];
    AnimationState m_State = AnimationState.InValid;
    AnimationState m_NextState = AnimationState.InValid;
    
    Dictionary<int , GameObject> m_OpponentTargetMap = new Dictionary<int, GameObject>() ;
	Dictionary<int , bool> m_ActuallyBeenHittedMap = new Dictionary<int, bool>() ;
	
	
    IEnumerator PerformChangeAnimationState(int _ChunkIndex, AnimationState _State, float _DelayTime, bool _RestoreTransform)
    {
        yield return new WaitForSeconds(_DelayTime);
        m_ElapsedTime = 0.0f;

        if (_RestoreTransform)
        {
            m_TransformRef[_ChunkIndex].Restore();

            foreach (GameObject go in ChunkMapRef[_ChunkIndex])
            {
                if (null != go) {
                    go.transform.parent = RootRef.transform;
                }
            }
            // ChunkMapRef.Clear();
            ChunkRef[_ChunkIndex] = null;
        }
    }
    
	public void SetOpponentTarget( int _ChunkIndex , string _ChunkString )  
	{
		if( !m_OpponentTargetMap.ContainsKey( _ChunkIndex ) )
		{
			m_OpponentTargetMap.Add( _ChunkIndex , null ) ;
		}
		m_OpponentTargetMap[ _ChunkIndex ] = GlobalSingleton.Find(_ChunkString, true);
		if( null == m_OpponentTargetMap[ _ChunkIndex ] )
		{
			Debug.LogWarning("SetOpponentTarget null == null == m_OpponentTargetMap _ChunkString" + _ChunkString );
		}
		
		if( !m_ActuallyBeenHittedMap.ContainsKey( _ChunkIndex ) )
		{
			m_ActuallyBeenHittedMap.Add( _ChunkIndex , false ) ;
		}
		m_ActuallyBeenHittedMap[ _ChunkIndex ] = false ;
		
	}
	public void ClearOpponentTarget( int _ChunkIndex )  
	{
		if( m_OpponentTargetMap.ContainsKey( _ChunkIndex ) )
		{
			m_OpponentTargetMap[ _ChunkIndex ] = null ;
		}
		if( m_ActuallyBeenHittedMap.ContainsKey( _ChunkIndex ) )
		{
			m_ActuallyBeenHittedMap[ _ChunkIndex ] = false ;
		}
	}
	
	
	public void ClearAllOpponentTargets()  
	{
		foreach( var key1 in m_OpponentTargetMap.Keys )
		{
			m_OpponentTargetMap[key1] = null ;
		}
		
		foreach( var key2 in m_ActuallyBeenHittedMap.Keys )
		{
			m_ActuallyBeenHittedMap[key2] = false ;
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
            // GlobalSingleton.DEBUG("Ready to change parent:"+ ChunkMapRef[_ChunkIndex].Count);

            foreach (GameObject go in ChunkMapRef[_ChunkIndex])
            {
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
            
			if( true == m_OpponentTargetMap.ContainsKey(i)
			   && null != m_OpponentTargetMap[ i ] 
			   && true == m_ActuallyBeenHittedMap.ContainsKey( i ) 
			   && false == m_ActuallyBeenHittedMap[ i ] )
			{
				Vector3 distanceVec = m_OpponentTargetMap[ i ].transform.position - pos ;
				// Debug.Log("distanceVec" + distanceVec.magnitude);
				
				if( distanceVec.magnitude < 2.0f )
				{
					m_ActuallyBeenHittedMap[ i ] = true ;
				}
			}
			
            switch (m_State)
            {
                case AnimationState.Attack:
                    {
						float dx = Mathf.Abs(chunk.transform.localScale.x) / chunk.transform.localScale.x;
						float Speed = dx * Time.deltaTime * 40.0f;
						if( true == m_ActuallyBeenHittedMap.ContainsKey( i ) 
						   && true == m_ActuallyBeenHittedMap[ i ] )
						{
							Speed *= -2.0f;
						}
                        chunk.transform.Translate(new Vector3(Speed, 0, 0));
	                    
                    }
                    break;

                case AnimationState.Hitted:
                    {
						
	                    
						if( true == m_ActuallyBeenHittedMap.ContainsKey( i ) 
				   			&& true == m_ActuallyBeenHittedMap[ i ] )
						{
							// Debug.LogWarning("vibration");
							Vector3 originPos = m_TransformRef[i].m_PositionRef;
							float dx = Random.Range(-1.0f, 1.0f);
							chunk.transform.position =
								originPos + (new Vector3(dx, 0, 0));	
						}
                        
                    }
                    break;

                case AnimationState.Defend:
                    {
                        // float dx = Mathf.Abs(chunk.transform.localScale.x) / chunk.transform.localScale.x;
                        // float Speed = dx * Time.deltaTime * 2.5f;
                        // chunk.transform.Translate(new Vector3(Speed, 0, 0));
                    }
                    break;
            }
        }
    }
}
