using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleFlash : MonoBehaviour
{
    public float flashSpeed;
    private bool flash;

    private SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sortingLayerName = "Particles";
        flash = false;
        flashSpeed = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (!flash)
            return;

        if(sr.color.a > 0.5f)
        {
            transform.localScale = new Vector3(
                (transform.localScale.x / sr.color.a).Clamp(0, 2), 
                (transform.localScale.y / sr.color.a).Clamp(0, 2), 
                transform.localScale.z);
        }
        else if (sr.color.a < 0.5f && sr.color.a > 0.3f)
        {
            transform.localScale = new Vector3(
                transform.localScale.x * sr.color.a,
                transform.localScale.y * sr.color.a,
                transform.localScale.z);
        }
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (sr.color.a - Time.deltaTime * flashSpeed).Clamp(0, 1));

        if (sr.color.a <= 0)
            flash = false;
    }

    public void ShowFlash(float x, float y, float z, float flashSpeed)
    {
        this.flashSpeed = flashSpeed;
        transform.localScale = new Vector3(1, 1, 1);
        transform.position = new Vector3(x, y, z);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.7f);
        this.flash = true;
    }
}
