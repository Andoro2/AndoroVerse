using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveLoadSystem
{
    public static void SaveGame(MainScript player)
    {
        BinaryFormatter m_Formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/game.fun";

        FileStream m_Stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(player);

        m_Formatter.Serialize(m_Stream, data);
        m_Stream.Close();
    }

    public static PlayerData LoadGame()
    {
        Time.timeScale = 1f;
        string path = Application.persistentDataPath + "/game.fun";
        if (File.Exists(path))
        {
            BinaryFormatter m_Formatter = new BinaryFormatter();
            FileStream m_Stream = new FileStream(path, FileMode.Open);

            PlayerData dataPlayer = m_Formatter.Deserialize(m_Stream) as PlayerData;
            m_Stream.Close();

            return dataPlayer;
        }
        else
        {
            Debug.LogError("Save file not found");
            return null;
        }
    }
    public static void SaveCollect(CollectibleInventory coleccionable)
    {
        BinaryFormatter m_Formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/collectible.fun";

        FileStream m_Stream = new FileStream(path, FileMode.Create);

        CollectibleData data = new CollectibleData(coleccionable);

        m_Formatter.Serialize(m_Stream, data);
        m_Stream.Close();
    }

    public static CollectibleData LoadCollect()
    {
        Time.timeScale = 1f;
        string path = Application.persistentDataPath + "/collectible.fun";
        if (File.Exists(path))
        {
            BinaryFormatter m_Formatter = new BinaryFormatter();
            FileStream m_Stream = new FileStream(path, FileMode.Open);

            CollectibleData dataCollectibple = m_Formatter.Deserialize(m_Stream) as CollectibleData;
            m_Stream.Close();

            return dataCollectibple;
        }
        else
        {
            Debug.LogError("Save file not found");
            return null;
        }
    }
}
