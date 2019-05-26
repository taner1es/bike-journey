using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MatchingAreaTrigger : MonoBehaviour
{
    public Sprite closedSprite;
    public GameObject closedTextGO;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(gameObject.name == collision.gameObject.name)
        {
            Destroy(collision.gameObject);

            gameObject.GetComponent<SpriteRenderer>().sprite = closedSprite;

            string text = transform.GetChild(0).GetComponent<TextMeshPro>().text;

            Destroy(transform.GetChild(0).gameObject);

            GameObject newTextGO = Instantiate(closedTextGO, transform);
            newTextGO.GetComponent<TextMeshPro>().SetText(text);

            gameObject.GetComponentInParent<HorizontalLayoutGroup>().enabled = false;
            gameObject.GetComponentInParent<HorizontalLayoutGroup>().enabled = true;

            /*Destroy(collision.gameObject);
            //gameObject.GetComponent<Renderer>().material.color = Color.green;

            gameObject.GetComponent<SpriteRenderer>().sprite = matchingAreaClosedPrefab.GetComponent<SpriteRenderer>().sprite;
            gameObject.GetComponentInChildren<RectTransform>().localPosition = matchingAreaClosedPrefab.GetComponent<RectTransform>().localPosition;
            


            Destroy(gameObject.GetComponent<Collider2D>());




            //gameObject.GetComponent<Collider2D>().enabled = false;
            */
        }
    }
}
