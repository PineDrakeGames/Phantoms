using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(ShakyImage))]
public class MyImageEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();//Draw inspector UI of ImageEditor
    }
}
#endif

public class ShakyImage : Image
{
    /////////////////////////
    /// Serialized Fields ///
    /////////////////////////

    [Header("Shaky Variables")]
    public float ShakeDistance = 5f;
    public RangedFloat ShakeInterval = new RangedFloat(0.2f, 0.5f);

    // For a sliced image, the imaged is divided into 9, with 36 verts, so positioning them is weirder.
    // Chart of the verts for future reference
    /*
                                  1
    ____________________________________________________________
    |9                10|21               22|33               34|
    |                   |                   |                   |
    |                   |                   |                   |
    |                   |                   |                   |
    |8                11|20               23|32               35|
    ____________________________________________________________
    |5                 6|17               18|29               30|
    |                   |                   |                   |
  0 |                   |                   |                   | 3
    |                   |                   |                   |
    |4                 7|16               19|28               31|
    ____________________________________________________________
    |1                 2|13               14|25               26|
    |                   |                   |                   |
    |                   |                   |                   |
    |                   |                   |                   |
    |0                 3|12               15|24               27|
    ____________________________________________________________
                                   2
    */
    // Making dictionaries mapping vertex indexes
    private Dictionary<int, int> m_sliceCornerVertices = new Dictionary<int, int>()
    {
        {0, 0},
        {9, 1},
        {27, 2},
        {34, 3}
    };
    private Dictionary<int, int> m_sliceCornerVerticesReversed = new Dictionary<int, int>()
    {
        {0, 0},
        {1, 9},
        {2, 27},
        {3, 34}
    };
    // Mapping vertices to sides - 0 for left, 1 for top, 2 for bot, 3 for right
    private Dictionary<int, int> m_sliceEdgeVertices = new Dictionary<int, int>()
    {
        {1, 0},
        {4, 0},
        {5, 0},
        {8, 0},
        {10, 1},
        {21, 1},
        {22, 1},
        {33, 1},
        {3, 2},
        {12, 2},
        {15, 2},
        {24, 2},
        {26, 3},
        {31, 3},
        {30, 3},
        {35, 3},
    };


    private class ImageCornerOffset
    {
        public Vector3 PrevPos = Vector3.zero;
        public Vector3 TargetPos = Vector3.zero;
        public float CurrentTime = 0f;
        public float TotalTime = 1f;

        public Vector3 CurrentPos
        {
            get
            {
                return Vector3.Lerp(PrevPos, TargetPos, (Mathf.Cos(Mathf.Clamp01(CurrentTime / TotalTime) * Mathf.PI) - 1f) / -2f);
            }
        }
    }

    private const int CORNERS = 4;

    private ImageCornerOffset[] m_cornerOffsets = null;
    private ImageCornerOffset[] CornerOffsets
    {
        get
        {
            if (m_cornerOffsets == null)
            {
                CreateCornerOffsets();
            }
            return m_cornerOffsets;
        }
    }

    ///////////////////////
    /// Unity Functions ///
    ///////////////////////
    protected override void Start()
    {
        base.Start();
        CreateCornerOffsets();
    }

    private void Update()
    {
        if (Application.isPlaying && gameObject.activeInHierarchy)
        {
            UpdateCorners();
            UpdateGeometry();
        }
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        base.OnPopulateMesh(vh);

        if (overrideSprite == null)
        {
            return;
        }

        if (type == Type.Sliced)
        {
            VertexManipSlice(ref vh);
        }
        else
        {
            VertexManipDefault(ref vh);
        }
    }


    ////////////////////////////////////
    /// Private Vertex Helper Manips ///
    ////////////////////////////////////

    public void VertexManipDefault(ref VertexHelper vh)
    {
        for (int i = 0; i < vh.currentVertCount; i++)
        {
            UIVertex vert = UIVertex.simpleVert;
            vh.PopulateUIVertex(ref vert, i);
            Vector3 position = vert.position;
            //
            //manipulate position
            //
            Vector3 offset = CornerOffsets[i % CORNERS].CurrentPos;
            position += offset;

            vert.position = position;
            vh.SetUIVertex(vert, i);
        }
    }

    public void VertexManipSlice(ref VertexHelper vh)
    {
        Vector3[] corners = new Vector3[4];
        Vector3[] offsetCorners = new Vector3[4];
        for (int i = 0; i < CORNERS; i++)
        {
            UIVertex vert = UIVertex.simpleVert;
            vh.PopulateUIVertex(ref vert, m_sliceCornerVerticesReversed[i]);
            Vector3 position = vert.position;
            corners[i] = position;
            offsetCorners[i] = position + CornerOffsets[i].CurrentPos;
        }

        for (int i = 0; i < vh.currentVertCount; i++)
        {
            UIVertex vert = UIVertex.simpleVert;
            vh.PopulateUIVertex(ref vert, i);
            Vector3 position = vert.position;

            if (m_sliceCornerVertices.ContainsKey(i))
            {
                position = offsetCorners[m_sliceCornerVertices[i]];
            }
            else if (m_sliceEdgeVertices.ContainsKey(i))
            {
                int edge = m_sliceEdgeVertices[i];
                int corner1Index = 0;
                int corner2Index = 0;
                switch (edge)
                {
                    case 0:
                        corner1Index = 0;
                        corner2Index = 1;
                        break;
                    case 1:
                        corner1Index = 1;
                        corner2Index = 3;
                        break;
                    case 2:
                        corner1Index = 0;
                        corner2Index = 2;
                        break;
                    case 3:
                        corner1Index = 3;
                        corner2Index = 2;
                        break;
                }
                Vector3 corner1 = corners[corner1Index];
                Vector3 corner2 = corners[corner2Index];
                Vector3 newCorner1 = offsetCorners[corner1Index];
                Vector3 newCorner2 = offsetCorners[corner2Index];

                float parameter = InverseLerp(corner1, corner2, position);
                position = Vector3.Lerp(newCorner1, newCorner2, parameter);
            }


            vert.position = position;
            vh.SetUIVertex(vert, i);
        }
    }

    ////////////////////////////////
    /// Private Helper Functions ///
    ////////////////////////////////

    private void CreateCornerOffsets()
    {
        m_cornerOffsets = new ImageCornerOffset[CORNERS];
        for (int i = 0; i < CORNERS; i++)
        {
            m_cornerOffsets[i] = new ImageCornerOffset();
            GenerateNewCornerOffset(m_cornerOffsets[i]);
        }
    }

    private void UpdateCorners()
    {
        for (int i = 0; i < CORNERS; i++)
        {
            CornerOffsets[i].CurrentTime += Time.unscaledDeltaTime;
            if (CornerOffsets[i].CurrentTime >= CornerOffsets[i].TotalTime)
            {
                GenerateNewCornerOffset(CornerOffsets[i]);
            }
        }
    }

    private void GenerateNewCornerOffset(ImageCornerOffset corner)
    {
        Vector3 newPosition = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f).normalized * Random.Range(0f, ShakeDistance);
        corner.PrevPos = corner.CurrentPos;
        corner.TargetPos = newPosition;
        corner.CurrentTime = 0f;
        corner.TotalTime = Random.Range(ShakeInterval.minValue, ShakeInterval.maxValue);
    }

    public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
    {
        Vector3 AB = b - a;
        Vector3 AV = value - a;
        return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
    }
}
