using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    public static IEnumerator Delay(float duration)
    {
        yield return new WaitForSeconds(Mathf.Abs(duration));
    }
}
