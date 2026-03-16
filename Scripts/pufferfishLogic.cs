using System.Collections;
using UnityEngine;

public class pufferfishLogic : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject fish;
    public Sprite p1;
    public Sprite p2;
    private Coroutine animCoroutine;
    void Awake()
    {
        fish = this.gameObject;
    }
    IEnumerator animationCoroutine()
    {
        while (true) 
        {
            fish.GetComponent<SpriteRenderer>().sprite = p1;
            fish.transform.localScale = Vector3.one;
            yield return new WaitForSeconds(2f);
            
            fish.GetComponent<SpriteRenderer>().sprite = p2;
            fish.transform.localScale = Vector3.one * 2f;
            yield return new WaitForSeconds(2f);
        }
    }
    void OnEnable()
    {
        if (animCoroutine != null)        {
            StopCoroutine(animCoroutine);
        }
        animCoroutine = StartCoroutine(animationCoroutine());
    }
}
