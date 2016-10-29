/**
@date 20161029 by NDark
. add class method StartMorphingModel()
. add class method Shuffle()
. add class method Swap()
. add class method CalculatePositionFromRootPosition()
	
*/
#define VERTEX_COLOR_256
// #define VERTEX_COLOR_8
// #define GRAY_SCALE_COLOR

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Mesh_VoxelChunk : IMesh
{
    public class Chunk
    {
        public Vector3  LocalPos;
        public Vector2  Scale;
        public Vector2  Size;
        public string   Model;
        public int[]    Vertex;
    }

    private static string s_VoxelPrefab = "Battle/Prefabs/Voxel";
    private string typename = typeof(Mesh_VoxelChunk).ToString();
    private GameObject root = null;

    Dictionary<string, Chunk> m_ChunkMap = new Dictionary<string, Chunk>();
    Dictionary<string, Chunk> m_ModelMap = new Dictionary<string, Chunk>();
    Dictionary<string, List<GameObject>> m_ChunkCache = new Dictionary<string, List<GameObject>>();
    
    public bool HasChunk
    {
        get { return m_ChunkMap.Count() > 0; }
    } 

    public Dictionary<string, Chunk> ChunkMap
    {
        get { return m_ChunkMap; }
    }

    public bool HasModel
    {
        get { return m_ModelMap.Count() > 0; }
    }

    public Dictionary<string, Chunk> ModelMap
    {
        get { return m_ModelMap; }
    }

    public Dictionary<string, List<GameObject>> Chunks
    {
        get { return m_ChunkCache; }
    }

    // copy
    public Mesh_VoxelChunk(IMesh _CopyMesh) : base(_CopyMesh)
    {
        Mesh_VoxelChunk _copyVoxelChunk = _CopyMesh as Mesh_VoxelChunk;
        if (_copyVoxelChunk.HasChunk)
        {
            m_ChunkMap = new Dictionary<string, Chunk>(_copyVoxelChunk.ChunkMap);
        }
        if (_copyVoxelChunk.HasModel)
        {
            m_ModelMap = new Dictionary<string, Chunk>(_copyVoxelChunk.ModelMap);
        }
    }

    public Mesh_VoxelChunk(string _SourceStr) : base(_SourceStr)
    {
        GlobalSingleton.DEBUG("Ctor for String");

        m_SourceStr = _SourceStr;
    }

    public void ChangeModel(string chunk, string model)
    {
        Chunk c;
        if (m_ChunkMap.TryGetValue(chunk, out c))
        {
            AttachModel(chunk, model, c);
        }
        else
        {
            GlobalSingleton.ERROR("Find no model:" + chunk + "," + model);
        }
    }
    
	public void StartMorphingModel(string chunkKey 
		, string newModelStr 
		, MorphingStruct _MorphingData )
	{
		// Debug.Log("MoveChunkModel chunk=" + chunkKey + " model=" + newModelStr );
		
		Chunk attachedChunkData = null ;
		if ( false == m_ChunkMap.TryGetValue(chunkKey, out attachedChunkData))
		{
			Debug.LogError("StartMorphingModel() false == m_ChunkMap.TryGetValue chunkKey=" + chunkKey );
			return ;
		}
		
		var listObjCache = m_ChunkCache ;
		List<GameObject> allOrgModelList;
		if ( false == listObjCache.TryGetValue(chunkKey, out allOrgModelList))
		{
			Debug.LogError("StartMorphingModel() false == listObjCache.TryGetValue chunkKey=" + chunkKey );
			return; 
		}
		
		var storageModelDataMap = m_ModelMap ;
		Chunk newModelChunkData = null ;
		if ( false == storageModelDataMap.TryGetValue(newModelStr, out newModelChunkData))
		{
			Debug.LogError("StartMorphingModel() false == storageModelDataMap.TryGetValue newModelStr=" + newModelStr );
			return ;
		}
		
		
		// every time start morphing, we clear old game object.
		_MorphingData.morphVec.Clear() ;
		foreach (GameObject o in allOrgModelList) 
		{
			var morphObj = new MorphingObj() ;
			morphObj.GameObj = o ;
			_MorphingData.morphVec.Add( morphObj ) ;
		}
		// Debug.LogWarning("StartMorphingModel() _MorphingData.morphVec.Count=" + _MorphingData.morphVec.Count );
		
		Vector2 gridWidthHeight = newModelChunkData.Size;
		Vector2 newModelScale = newModelChunkData.Scale;
		Vector3 referencePos = attachedChunkData.LocalPos + newModelChunkData.LocalPos;
		int[] gridAttributes = newModelChunkData.Vertex;
		
		Vector3 localScale = root.transform.localScale;
		int _w = (int)gridWidthHeight.x;
		int _h = (int)gridWidthHeight.y;
		int _TotalGridSize = _w * _h;
		
		// first phase we all all possible position.
		List<Vector3> validPositionVec = new List<Vector3>() ;
		for (int i = 0; i < _TotalGridSize ; ++i)
		{
			int voxel = gridAttributes[i];
			Vector3 targetPos = Vector3.zero ;
			if (voxel > 0 )
			{
				targetPos = this.CalculatePositionFromRootPosition( root.transform.position 
				                                                   , localScale 
				                                                   , newModelScale 
				                                                   , referencePos , _w , _h , i ) ;
				
				validPositionVec.Add( targetPos ) ;
			}		
		}
		// Debug.LogWarning("StartMorphingModel() validPositionVec.Count=" + validPositionVec.Count );
		
		// second phase we randomize the vector
		CollectionUtility.List_Vector3_Shuffle( validPositionVec ) ;
		
		// third phase we try mapping morph object to those shuffled position.
		int mappingIndex = 0 ;
		for (int i = 0; i < _MorphingData.morphVec.Count && validPositionVec.Count > 0 ; ++i)
		{
			_MorphingData.morphVec[ i ].Target = validPositionVec[ mappingIndex ] ;
			
			if ( i < validPositionVec.Count )// if our morph object more than valid position
			{
				++mappingIndex ;// everty time we used one slot, plus one.
			}
		}
		// Debug.LogWarning("StartMorphingModel() indexValidPosition=" + indexValidPosition );
	}
	
	
    public void AttachModel(string chunk, string model, Chunk c)
    {
        // if (m_ChunkCache.ContainsKey(chunk) && c.Model == model)
        //    return;

        List<GameObject> Purge;
        if (m_ChunkCache.TryGetValue(chunk, out Purge))
        {
            foreach (GameObject o in Purge) {
                GameObject.Destroy(o);
            }
            m_ChunkCache.Remove(chunk);
        }

        Chunk c0;
        if (m_ModelMap.TryGetValue(model, out c0))
        {
            Vector2 size = c0.Size;
            Vector2 scale = c0.Scale;
            Vector3 objectPos = c.LocalPos + c0.LocalPos;
            int[] vertex = c0.Vertex;
            m_ChunkMap[chunk].Model = model;
            m_ChunkCache.Add(chunk, new List<GameObject>());

            GlobalSingleton.DEBUG("AttachModel:"
                + chunk + ","
                + model + ","
                + scale  + ","
                + objectPos + ","
                + size);

            int _w = (int)size.x;
            int _h = (int)size.y;
            int _size = _w * _h;
            for (int i = 0; i < _size /*vertex.Length*/ ; ++i)
            {
                int voxel = vertex[i];
                if (voxel > 0)
                {
                    GameObject obj = Object.Instantiate(Resources.Load(s_VoxelPrefab)) as GameObject;
                    m_ChunkCache[chunk].Add(obj);

                    Vector3 localScale = root.transform.localScale;
                    obj.transform.parent = root.transform;
                    obj.transform.localScale = new Vector3(
                            localScale.x * scale.x,
                            localScale.y * scale.y,
                            localScale.z);
					obj.transform.position = this.CalculatePositionFromRootPosition( root.transform.position 
						, localScale , scale , objectPos , _w , _h , i ) ;
					

                    // use Sprites-Default material
#if VERTEX_COLOR_256 || VERTEX_COLOR_8
                    MeshFilter cube = obj.GetComponent<MeshFilter>();
                    Vector3[] vertices = cube.mesh.vertices;
#if VERTEX_COLOR_256
                    int colorScale = 256;
#elif VERTEX_COLOR_8
                int colorScale = 8;

#endif
#if GRAY_SCALE_COLOR
                    float grayScale = (voxel % colorScale) / (float)colorScale;
                    float r = grayScale, g = grayScale, b = grayScale;
#else
                    float r = (voxel % colorScale) / (float)colorScale;
                    float g = (voxel % colorScale) / (float)colorScale;
                    float b = (voxel % colorScale) / (float)colorScale;
#endif
                    Color[] colors = Enumerable.Repeat(
                        new Color(r, g, b, 1.0f),
                        vertices.Length).ToArray();
                    cube.mesh.colors = colors;
#endif
                }
            }
        }
    }
    
    public Vector3 CalculatePositionFromRootPosition( Vector3 _RootPosition 
	                                                 , Vector3 _ParentScale 
	                                                 , Vector3 _TargetScale 
	                                                 , Vector3 _ReferencePosition
	                                                 , int _GridSizeInWidth 
	                                                 , int _GridSizeInHeight 
	                                                 , int _Index )
    {
		Vector3 ret = _RootPosition
			+ new Vector3(
				_ParentScale.x * (_TargetScale.x * ( _Index % _GridSizeInWidth - _GridSizeInHeight * 0.5f) + _ReferencePosition.x),
				_ParentScale.y * (_TargetScale.y * (_GridSizeInHeight *0.5f - _Index / _GridSizeInWidth) + _ReferencePosition.y),
				_ReferencePosition.z);
		return ret ;
    }

    public override void Draw(GameObject _Root)
    {
        root = _Root;

        foreach (KeyValuePair<string, Chunk> entry in m_ChunkMap)
        {
            string name = entry.Key;
            Chunk chunk = entry.Value;
            AttachModel(name, chunk.Model, chunk);
        }
    }

    public override void BreakPart(int _PartIndex, GameObject _Root)
    {
        GlobalSingleton.DEBUG(typename + "BreakPart, part index = " + _PartIndex);
    }

    public override bool DoParse()
    {
        GlobalSingleton.DEBUG(typename + "DoParse, SourceStr = " + m_SourceStr);

        // remove unused char(space, new line, etc...)
        m_SourceStr = System.Text.RegularExpressions.Regex.Replace(m_SourceStr, @"\t|\n|\r| ", string.Empty);
        string[] attrSegment = m_SourceStr.Split(';');

        for (int i = 0; i < attrSegment.Length; ++i)
        {
            string[] element = attrSegment[i].Split(':');
            if (element[0] == "name")
            {
                m_Name = element[1];
            }
            else if (element[0] == "chunk")
            {
                char[] trim = {'[', ']'};
                string[] chunks = element[1].Trim(trim).Split('#');

                GlobalSingleton.DEBUG(element[1]);

                for (int j = 1; j < chunks.Length; ++j)
                {
                    string[] chunkSegment = chunks[j].Split('|');
                    // if (chunkSegment.Length < 5)
                    //    continue;
                    
                    string name = chunkSegment[0];
                    string model = chunkSegment[1];
                    string[] vector3String = chunkSegment[2].Split(',');
                    Vector3 localPosition = new Vector3
                    (
                        float.Parse(vector3String[0]),
                        float.Parse(vector3String[1]),
                        float.Parse(vector3String[2])
                    );

                    GlobalSingleton.DEBUG("chunk = "
                        + name + ","
                        + localPosition);

                    Chunk c = new Chunk();
                    c.Model = model;
                    c.LocalPos = localPosition;
                    // c.Scale = localScale;
                    // c.Size = size;
                    m_ChunkMap.Add(name, c);
                }
            }
            else if (!m_ModelMap.ContainsKey(element[0]))
            {
                if (element.Length < 5)
                    continue;

                string[] pos3String = element[1].Split(',');
                Vector3 localPosition = new Vector3
                (
                    float.Parse(pos3String[0]),
                    float.Parse(pos3String[1]),
                    float.Parse(pos3String[2])
                );

                string[] scale2String = element[2].Split(',');
                Vector2 localScale = new Vector2
                (
                    float.Parse(scale2String[0]),
                    float.Parse(scale2String[1])
                );

                string[] size2String = element[3].Split(',');
                Vector2 size = new Vector2
                (
                    float.Parse(size2String[0]),
                    float.Parse(size2String[1])
                );

                GlobalSingleton.DEBUG("add model: " + element[0]);

                string valueStr = element[4];
                string[] verticesStr = valueStr.Split(',');

                Chunk c = new Chunk();
                c.Model = element[0];
                c.LocalPos = localPosition;               
                c.Scale = localScale;
                c.Size = size;
                c.Vertex = System.Array.ConvertAll(verticesStr, s => int.Parse(s));
                m_ModelMap.Add(element[0], c);                
            }
        }

        GlobalSingleton.DEBUG(typename + "DoParse End : " + m_ChunkMap.Count());

        return true;
    }
}
