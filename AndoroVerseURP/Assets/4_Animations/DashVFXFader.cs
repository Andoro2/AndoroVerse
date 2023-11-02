using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashVFXFader : MonoBehaviour
{
    private MeshRenderer MRenderer;
    private float rate = 0.1f, refreshRate = 0.05f;
    // Start is called before the first frame update
    void Start()
    {
        MRenderer = GetComponent<MeshRenderer>();
        StartCoroutine(FadeDashVFX());
        Destroy(gameObject, 0.5f);
    }

    IEnumerator FadeDashVFX()//Material mat, float goal, float rate, float refreshRate)
    {
        float Value = MRenderer.material.GetFloat("_Alpha");//Reference en el valor que sale en Reference en la ventana de edición del shader

        while (Value > 0f)
        {
            Value -= rate;
            MRenderer.material.SetFloat("_Alpha", Value);
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
