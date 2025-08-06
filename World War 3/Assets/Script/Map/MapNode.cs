using UnityEngine;
using UnityEngine.UI;

public class MapNode : MonoBehaviour
{
    public enum NodeType { Combat, Shop, Elite, Campfire, Boss }
    public NodeType nodeType;
    public int row, index;
    public Button button;

    public void Init(int row, int index, NodeType type)
    {
        this.row = row;
        this.index = index;
        nodeType = type;
    }

    void OnClick()
    {
        MapUI.Instance.SelectNode(this);
    }
}
