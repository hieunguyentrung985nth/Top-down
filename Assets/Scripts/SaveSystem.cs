using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    private static string path = Path.Combine(Application.persistentDataPath, "sound.save");

    public static void SaveSoundSettings(SoundSettings data)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        FileStream stream = new FileStream(path, FileMode.Create);

        var json = JsonUtility.ToJson(data);

        formatter.Serialize(stream, json);

        stream.Close();
    }

    public static SoundSettings LoadSoundSettings()
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(path, FileMode.Open);

            var json = formatter.Deserialize(stream) as string;

            SoundSettings data = JsonUtility.FromJson<SoundSettings>(json);

            stream.Close();

            return data;
        }
        else
        {
            //Debug.LogWarning("No sound settings save file");

            return null;
        }
    }

    public static void DeleteSoundSettingsSave()
    {
        File.Delete(path);
    }
}
