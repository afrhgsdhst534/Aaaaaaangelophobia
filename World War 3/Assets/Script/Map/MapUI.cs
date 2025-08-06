using UnityEngine;

public class MapUI : MonoBehaviour
{
    public static MapUI Instance;
    public MapNode currentNode;

    void Awake() => Instance = this;

    public void SelectNode(MapNode node)
    {
        if (node.row == currentNode.row + 1 && Mathf.Abs(node.index - currentNode.index) <= 1)
        {
            currentNode = node;
            Debug.Log("Moved to: " + node.nodeType);
        }
    }
}
