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
        public int      Size;
        public string   Model;
    }

    private static string s_VoxelPrefab = "Battle/Prefabs/Voxel";
    private string typename = typeof(Mesh_VoxelChunk).ToString();
    private GameObject root = null;

    Dictionary<string, Chunk> m_ChunkMap = new Dictionary<string, Chunk>();
    Dictionary<string, int[]> m_ModelMap = new Dictionary<string, int[]>();
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

    public Dictionary<string, int[]> ModelMap
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
            m_ModelMap = new Dictionary<string, int[]>(_copyVoxelChunk.ModelMap);
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
    }

    public void AttachModel(string chunk, string model, Chunk c)
    {
        if (m_ChunkCache.ContainsKey(chunk) && c.Model == model)
            return;

        List<GameObject> Purge;
        if (m_ChunkCache.TryGetValue(chunk, out Purge))
        {
            foreach (GameObject o in Purge) {
                GameObject.Destroy(o);
            }
            m_ChunkCache.Remove(chunk);
        }

        if (m_ModelMap.ContainsKey(model))
        {
            int size = c.Size;
            Vector2 scale = c.Scale;
            Vector3 objectPos = c.LocalPos;
            int[] vertex = m_ModelMap[model];
            m_ChunkMap[chunk].Model = model;
            m_ChunkCache.Add(chunk, new List<GameObject>());

            GlobalSingleton.DEBUG("AttachModel:"
                + chunk + ","
                + model + ","
                + scale  + ","
                + objectPos + ","
                + size);

            for (int i = 0; i < vertex.Length; ++i)
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

                    obj.transform.position = root.transform.position
                        + new Vector3(
                            localScale.x * (scale.x * (i % size - size / 2) + objectPos.x),
                            localScale.y * (scale.y * (size / 2 - i / size) + objectPos.y),
                            objectPos.z);

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
                    if (chunkSegment.Length < 5)
                        continue;
                    
                    string name = chunkSegment[0];
                    string model = chunkSegment[1];
                    string[] vector3String = chunkSegment[2].Split(',');
                    Vector3 localPosition = new Vector3
                    (
                        float.Parse(vector3String[0]),
                        float.Parse(vector3String[1]),
                        float.Parse(vector3String[2])
                    );

                    string[] vector2String = chunkSegment[3].Split(',');
                    Vector2 localScale = new Vector2
                    (
                        float.Parse(vector2String[0]),
                        float.Parse(vector2String[1])
                    );
                   
                    int size = int.Parse(chunkSegment[4]);
                    GlobalSingleton.DEBUG("chunk = " 
                        + name + "," 
                        + localPosition + ","
                        + localScale + ","
                        + size);

                    Chunk c = new Chunk();
                    c.Model = model;
                    c.LocalPos = localPosition;
                    c.Scale = localScale;
                    c.Size = size;
                    m_ChunkMap.Add(name, c);
                }
            }
            else if (!m_ModelMap.ContainsKey(element[0]))
            {
                if (element.Length < 2)
                    continue;

                GlobalSingleton.DEBUG("add model: " + element[0]);

                string valueStr = element[1];
                string[] verticesStr = valueStr.Split(',');
                m_ModelMap.Add(
                    element[0],
                    System.Array.ConvertAll(verticesStr, s => int.Parse(s))
                    );
            }
        }

        GlobalSingleton.DEBUG(typename + "DoParse End : " + m_ChunkMap.Count());

        return true;
    }
}
