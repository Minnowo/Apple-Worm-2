using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RankText : MonoBehaviour
{
    public float flashSpeed;
    public bool show;
    private float growCounter;
    public Text t;
    // Start is called before the first frame update
    void Start()
    {
        t = GetComponent<Text>();
        show = false;
        flashSpeed = 1;
        growCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!show)
            return;

        float s = 1 + Mathf.Sin(growCounter);

        transform.localScale = new Vector3(s, s, transform.localScale.z);
        t.color = new Color(t.color.r, t.color.g, t.color.b, (t.color.a - Time.deltaTime * flashSpeed).Clamp(0, 1));

        if (t.color.a <= 0)
        {
            show = false;
            this.gameObject.SetActive(false);
            return;
        }

        growCounter = (growCounter + 0.05f).Clamp(0f, 1.5f); // max at 1.5 because sin( 1.5 + ) starts getting smaller
    }

    public void ShowRank(Rank r, float x, float y, float z, float speed)
    {
        t.text = r.ToString();

        growCounter = 0;
        flashSpeed = speed;

        transform.localScale = new Vector3(1, 1, 1);
        transform.position =   new Vector3(x, y, z);
        t.color = new Color(t.color.r, t.color.g, t.color.b, 1f);

        gameObject.SetActive(true);
        show = true;
    }

    public void ShowRank(Rank r, float speed)
    {
        t.text = r.ToString();

        growCounter = 0;
        flashSpeed = speed;

        transform.localScale = new Vector3(1, 1, 1);
        t.color = new Color(t.color.r, t.color.g, t.color.b, 1f);

        gameObject.SetActive(true);
        show = true;
    }
}
