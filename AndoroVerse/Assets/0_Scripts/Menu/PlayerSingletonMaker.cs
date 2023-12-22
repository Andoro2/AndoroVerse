using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSingletonMaker : MonoBehaviour
{
    private static PlayerSingletonMaker instance;

    // Propiedad pública para acceder a la instancia del singleton
    public static PlayerSingletonMaker Instance
    {
        get
        {
            // Si no hay instancia existente, intenta encontrar una en la escena
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerSingletonMaker>();

                // Si no se encuentra en la escena, crea una nueva instancia
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject("MySingleton");
                    instance = singletonObject.AddComponent<PlayerSingletonMaker>();
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
