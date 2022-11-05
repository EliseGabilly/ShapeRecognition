using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// Save and load information fro player in the persistente data directory
/// </summary>
public static class SaveSystem {

    private static readonly string PATH = Application.persistentDataPath + "/player.txt";

    public static void SaveGame(Game player) {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(PATH, FileMode.Create);

        GameData data = new GameData(player);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static GameData LoadData() {
        if (File.Exists(PATH)) {
            return LoadDataFromPath();
        } else {
            try {
                //try to write a new player if first conection
                Debug.Log("Save a new file for Game data (" + PATH + ") ");
                SaveGame(Game.Instance);
                return LoadDataFromPath();
            } catch (Exception e) {
                Debug.LogError("Save file not found (" + PATH + ") "+e.HelpLink);
                return null;
            }
        }
    }

    public static GameData LoadDataFromPath() {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(PATH, FileMode.Open);

        GameData data = formatter.Deserialize(stream) as GameData;
        stream.Close();

        Game.Instance.ChangeData(data);
        return data;
    }
}
