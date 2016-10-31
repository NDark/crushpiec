/**
@date 20161029 by NDark 
. add class member m_MorphingData at Character.
. add class method ForceEndMorphing()
. add class method StartMorphingModel()
. add class method UpdateMorphing()
. add class method isMorphingEnded()
. add class method GenerateChunkKeyFromIndex()
. add class method GenerateModelStringFromTemplateSetting()


*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic ;

public enum MODELTYPE
{
    E_DEFENSE,
    E_ATTACK,
    E_CONCENTRATE,

    E_MODELTYPE_NUM
}

public class Character : MonoBehaviour {
    // private
    IMesh m_ShareMeshData = null;
    UnitData m_ShareUnitData = null;
    ChunkAnimation m_ShareAnimation = null;

    // manager reference 
    static MeshCreator s_ShareMeshCreator = GlobalSingleton.GetMeshCreator();
    // static DataManager s_DataManager = GlobalSingleton.GetDataManager();

    // public
    public string m_MeshName = "Unit_1";
    public GameObject m_UIHPBar = null;

    // attribute
    public Attribute hp { get { return m_ShareUnitData.m_HP; } }
    public Attribute atk { get { return m_ShareUnitData.m_ATK; } }
    public Attribute def { get { return m_ShareUnitData.m_DEF; } }
    public IMesh mesh { get { return m_ShareMeshData; } }

	// the morphing animation controller, will automatically been created in StartMorphingModel()
	Dictionary<int,MorphingStruct> m_MorphingData =
		 new Dictionary<int, MorphingStruct>() ;
	
    public void ReCreate() {
        if (m_ShareMeshData != null) {
            s_ShareMeshCreator.DestroyMesh(ref m_ShareMeshData);
        }

        //if (m_ShareUnitData != null){
        //   s_DataManager.GetUnitData(m_MeshName, ref m_ShareUnitData);
        //}

        DoRender();
    }

    public void DoRender() {
        if (m_ShareMeshData == null) {
            m_ShareMeshData = s_ShareMeshCreator.CreateMesh(m_MeshName);
        }

        if (m_ShareMeshData != null) {
            m_ShareMeshData.Draw(this.gameObject);
        }
    }

	public void ForceEndMorphing()
	{
		foreach( var data in m_MorphingData.Values )
		{
			data.isInMorphing = false ;
		}
	}
	
	public void StartMorphingModel(int _ChunkIndex, MODELTYPE _ModelType)
	{
		
		if( false == m_MorphingData.ContainsKey( _ChunkIndex ) )
		{
			m_MorphingData.Add( _ChunkIndex , new MorphingStruct() ) ;
		}
		
		MorphingStruct morphData = null ;
		morphData = m_MorphingData[ _ChunkIndex ] ;
		morphData.isInMorphing = true ;
		
		
		string modelstr = GenerateModelStringFromTemplateSetting( _ChunkIndex , _ModelType );
		
		Mesh_VoxelChunk Mesh = m_ShareMeshData as Mesh_VoxelChunk;
		if (null != Mesh)
		{
			string chunkKey = GenerateChunkKeyFromIndex( _ChunkIndex ) ;
			Mesh.StartMorphingModel( chunkKey , modelstr , morphData );
		}
	}
	
	public void UpdateMorphing()
	{
		foreach( var morphChunk in this.m_MorphingData.Values )
		{
			morphChunk.UpdateMorphData() ;
		}
	}
	
	public bool isMorphingEnded()
	{
		bool isMorphing = false ;
		foreach( MorphingStruct data in this.m_MorphingData.Values )
		{
			if( true == data.isInMorphing )
			{
				isMorphing = true ;
				break ;
			}
		}
		return false == isMorphing  ;
	}
	
	private string GenerateChunkKeyFromIndex( int _ChunkIndex )
	{
		return "bone" + _ChunkIndex ;
	}
	
	private string GenerateModelStringFromTemplateSetting( int _ChunkIndex, MODELTYPE _ModelType )
	{
		string ret = "body_" + _ChunkIndex;
		string[] def_models = { "shield" };
		string[] atk_models = { "axe", "sword" };
		
		switch (_ModelType)
		{
		case MODELTYPE.E_DEFENSE:
			ret = def_models[Random.Range(0, 100) % def_models.Length];
			break;
			
		case MODELTYPE.E_ATTACK:
			ret = atk_models[Random.Range(0, 100) % atk_models.Length];
			break;
			
		case MODELTYPE.E_CONCENTRATE:
			ret = "body_" + _ChunkIndex;
			break;
		}
		return ret ;
	}
	
    public void DoChangeModel(int _ChunkIndex, MODELTYPE _ModelType)
    {
		string model = GenerateModelStringFromTemplateSetting( _ChunkIndex , _ModelType ) ;
        
        Mesh_VoxelChunk Mesh = m_ShareMeshData as Mesh_VoxelChunk;
        if (null != Mesh)
        {
            // GlobalSingleton.DEBUG("ChangeModel : " + _ChunkIndex + "," + model);
			string chunkKey = GenerateChunkKeyFromIndex( _ChunkIndex ) ;
			Mesh.ChangeModel( chunkKey , model);
        }
    }

    public void DoAction(int _ChunkIndex, AnimationState _State, float _Delay = 0.0f) {
        float dx = gameObject.transform.localScale.x;
        string sx = (dx > 0.0f) ? "R" : "L";
        m_ShareAnimation.DoAnimation(this,
            string.Format("Chunk-{0}-{1}", sx, _ChunkIndex),
            _ChunkIndex,
            _State,
            _Delay);
    }
	
	public void SetOpponentTarget(int _ChunkIndex )  
	{
		Debug.Log("SetOpponentTarget");
		float dx = gameObject.transform.localScale.x;
		string sx = (dx > 0.0f) ? "R" : "L";
		string attackerSide = (sx == "R" ) ? "L" : "R";
		string attackerChunckString = string.Format("Chunk-{0}-{1}", attackerSide, _ChunkIndex) ; 
		m_ShareAnimation.SetOpponentTarget( _ChunkIndex , attackerChunckString ) ;
		
	}
	
	public void ClearOpponentTarget( int _ChunkIndex )  
	{
		Debug.Log("ClearOpponentTarget");
		m_ShareAnimation.ClearOpponentTarget( _ChunkIndex ) ;
		
	}
	public void ClearAllOpponentTargets()  
	{
		m_ShareAnimation.ClearAllOpponentTargets() ;
	}
	
    public void DoUpdateHP() {
        if (m_UIHPBar != null ) {
            float hpRatio = Mathf.Max(0.0f, (float)m_ShareUnitData.m_HP.value / m_ShareUnitData.m_HP.max);
            Vector3 localScale = m_UIHPBar.transform.localScale;
            localScale.x = hpRatio;
            m_UIHPBar.transform.localScale = localScale;
        }
    }

    // Use this for initialization
    void Start () {
        if (m_ShareAnimation == null) {
            // m_ShareAnimation = GetComponent<TransformAnimation>();
            m_ShareAnimation = GetComponent<ChunkAnimation>();
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
