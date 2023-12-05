using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FadeOutText : MonoBehaviour
{
    public TMP_Text m_Text;
    public float m_FadeOutTime = 2f; // Duración en segundos antes de empezar a desvanecer
    public float m_TimeToStart = 3f; // Tiempo en segundos antes de empezar a desvanecer

    private float timer;
    private bool isFading = false;

    void OnEnable()
    {
        // Inicializa el temporizador cuando el GameObject se activa
        timer = 0f;
    }

    void Update()
    {
        // Incrementa el temporizador
        timer += Time.deltaTime;

        // Comienza a desvanecer después de startFadeAfter segundos
        if (timer >= m_TimeToStart && !isFading)
        {
            isFading = true;
            StartCoroutine(FadeTextOverTime());
        }
    }

    IEnumerator FadeTextOverTime()
    {
        // Espera antes de comenzar a desvanecer
        yield return new WaitForSeconds(m_FadeOutTime);

        // Reduzca gradualmente el valor alpha del texto durante fadeDuration segundos
        float elapsedTime = 0f;
        Color originalColor = m_Text.color;

        while (elapsedTime < m_FadeOutTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / m_FadeOutTime);
            Color newColor = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0f, t));
            m_Text.color = newColor;
            yield return null;
        }
        Destroy(this.gameObject);
        // Restaura el texto a su color original
        //textToFade.color = originalColor;

        // Reinicia el temporizador para permitir la repetición
        //timer = 0f;
        //isFading = false;

        // Desactiva el script para evitar la repetición continua
        //this.enabled = false;

    }
}
