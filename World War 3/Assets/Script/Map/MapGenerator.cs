using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance;

    public GameObject nodePrefab;
    public int rows = 6;
    public int nodesPerRow = 3;
    public float xSpacing = 4f, ySpacing = 3f;

    [HideInInspector] public List<List<MapNode>> allNodes = new();

    void Awake() => Instance = this;

    void Start() => GenerateMap();

    void GenerateMap()
    {
        for (int row = 0; row < rows; row++)
        {
            List<MapNode> rowNodes = new();
            int count = (row == rows - 1) ? 1 : nodesPerRow;

            for (int i = 0; i < count; i++)
            {
                Vector3 pos = new(i * xSpacing - (count - 1) * xSpacing / 2, row * ySpacing, 0);
                var obj = Instantiate(nodePrefab, pos, Quaternion.identity, transform);
                var node = obj.GetComponent<MapNode>();
                MapNode.NodeType type = GetTypeForRow(row);
                node.Init(row, i, type);
                rowNodes.Add(node);
            }

            allNodes.Add(rowNodes);
        }

        ConnectNodes();
    }

    MapNode.NodeType GetTypeForRow(int row)
    {
        if (row == rows - 1) return MapNode.NodeType.Boss;
        if (row == 0) return MapNode.NodeType.Combat;

        int r = Random.Range(0, 100);
        if (r < 50) return MapNode.NodeType.Combat;
        if (r < 70) return MapNode.NodeType.Shop;
        if (r < 85) return MapNode.NodeType.Campfire;
        return MapNode.NodeType.Elite;
    }

    void ConnectNodes()
    {
        for (int row = 0; row < allNodes.Count - 1; row++)
        {
            foreach (var from in allNodes[row])
            {
                foreach (var to in allNodes[row + 1])
                {
                    if (Mathf.Abs(from.index - to.index) <= 1)
                        DrawLine(from.transform.position, to.transform.position);
                }
            }
        }
    }

    void DrawLine(Vector3 start, Vector3 end)
    {
        var lineObj = new GameObject("Line");
        var lr = lineObj.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPositions(new[] { start, end });
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
    }
}
