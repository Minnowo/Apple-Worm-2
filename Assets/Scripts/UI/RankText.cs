using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RankText : MonoBehaviour
{
    public float flashSpeed;
    public bool show;

    public Text t;
    // Start is called before the first frame update
    void Start()
    {
        t = GetComponent<Text>();
        show = false;
        flashSpeed = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (!show)
            return;

        //this.gameObject.SetActive(true);

        if (t.color.a > 0.5f)
        {
            transform.localScale = new Vector3(
                (transform.localScale.x / t.color.a).Clamp(0, 2),
                (transform.localScale.y / t.color.a).Clamp(0, 2),
                transform.localScale.z);
        }
        else if (t.color.a < 0.5f && t.color.a > 0.3f)
        {
            transform.localScale = new Vector3(
                transform.localScale.x * t.color.a,
                transform.localScale.y * t.color.a,
                transform.localScale.z);
        }
        t.color = new Color(t.color.r, t.color.g, t.color.b, (t.color.a - Time.deltaTime * flashSpeed).Clamp(0, 1));

        if (t.color.a <= 0)
        {
            show = false;
            //this.gameObject.SetActive(false);
        }
    }

    public void ShowRank(Rank r, float x, float y, float z, float speed)
    {
        t.text = r.ToString();
        this.flashSpeed = speed;
        transform.localScale = new Vector3(1, 1, 1);
        transform.position = new Vector3(x, y, z);
        t.color = new Color(t.color.r, t.color.g, t.color.b, 0.7f);
        this.show = true;
    }

    public void ShowRank(Rank r, float speed)
    {
        t.text = r.ToString();
        this.flashSpeed = speed;
        transform.localScale = new Vector3(1, 1, 1);
        t.color = new Color(t.color.r, t.color.g, t.color.b, 0.7f);
        this.show = true;
    }
}
