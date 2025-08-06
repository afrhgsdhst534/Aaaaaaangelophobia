using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hand : MonoBehaviour
{
    public List<Transform> cards = new List<Transform>();

    [Header("��������� �������")]
    public float cardSpacing ;        // ���������� ����� ������� �� X
    public float yPos ;                 // ������� �� Y

    [Header("��������")]
    public float animationSpeed;

    [Header("����� �����")]
    public float spawnPosX;          // ������ ����� ���������
    public GameObject cardPrefab;

    [ContextMenu("AddCard")]
    public void Add()
    {
        Vector3 startPos = new Vector3(transform.position.x + spawnPosX, transform.position.y, transform.position.z);
        AddCard(Instantiate(cardPrefab.transform, startPos, Quaternion.identity, transform));
    }

    public void AddCard(Transform card)
    {
        card.SetParent(transform, false);
        card.localRotation = Quaternion.identity;
        cards.Add(card);
        UpdateHandPositions();
    }

    private IEnumerator MoveCard(Transform card, Vector3 targetPos)
    {
        Vector3 startPos = card.localPosition;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * animationSpeed;
            card.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
    }

    public void UpdateHandPositions()
    {
        int count = cards.Count;
        if (count == 0) return;

        for (int i = 0; i < count; i++)
        {
            // ����������� ������������� ���� �� X ������������ ������
            float middle = (count - 1) / 2f;
            float x = (i - middle) * cardSpacing;

            Vector3 targetPos = new Vector3(x, yPos, 0);
            StartCoroutine(MoveCard(cards[i], targetPos));
        }
    }

    public void RemoveCard(Transform card)
    {
        cards.Remove(card);
        Destroy(card.gameObject);
        UpdateHandPositions();
    }
}
