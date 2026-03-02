using System.Collections;
using UnityEngine;

public class pufferfishLogic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject fish;
    public Sprite p1;
    public Sprite p2;
    void Start()
    {
        fish = this.gameObject;
        StartCoroutine(animationCoroutine());
    }
    IEnumerator animationCoroutine()
    {
        Debug.Log("start p1");
        fish.GetComponent<SpriteRenderer>().sprite = p1;
        fish.transform.localScale = Vector3.one;
        yield return new WaitForSeconds(2f);
        Debug.Log("start p2");
        fish.GetComponent<SpriteRenderer>().sprite = p2;
        fish.transform.localScale = Vector3.one * 2f;
        yield return new WaitForSeconds(2f);
        StartCoroutine(animationCoroutine());
    }
    void OnEnable()
    {
        StartCoroutine(animationCoroutine());
    }
}
