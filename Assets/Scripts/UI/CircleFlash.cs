using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleFlash : MonoBehaviour
{
    public float flashSpeed;
    private bool flash;

    private float growCounter;
    private SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sortingLayerName = "Particles";
        flash = false;
        flashSpeed = 2;
        growCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!flash)
            return;

        float s = 1 + Mathf.Sin(growCounter);

        transform.localScale = new Vector3(s, s, transform.localScale.z);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (sr.color.a - Time.deltaTime * flashSpeed).Clamp(0, 1));

        if (sr.color.a <= 0)
        {
            growCounter = 0;
            flash = false;
            this.gameObject.SetActive(false);
            return;
        }

        growCounter = (growCounter + 0.05f).Clamp(0f, 1.5f); // max at 1.5 because sin( 1.5 + ) starts getting smaller
    }

    public void ShowFlash(float x, float y, float z, float speed)
    {
        growCounter = 0;
        flashSpeed = speed;
        transform.localScale = new Vector3(1, 1, 1);
        transform.position = new Vector3(x, y, z);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.9f);
        gameObject.SetActive(true);
        flash = true;
    }
}
