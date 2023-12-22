using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUISingletonMaker : MonoBehaviour
{
    private static GameUISingletonMaker instance;

    // Propiedad pública para acceder a la instancia del singleton
    public static GameUISingletonMaker Instance
    {
        get
        {
            // Si no hay instancia existente, intenta encontrar una en la escena
            if (instance == null)
            {
                instance = FindObjectOfType<GameUISingletonMaker>();

                // Si no se encuentra en la escena, crea una nueva instancia
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("MySingleton");
                    instance = singletonObject.AddComponent<GameUISingletonMaker>();
                }
            }
            return instance;
        }
    }

    // Asegura que solo haya una instancia al inicio
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
