using System;
using Newtonsoft.Json;
using static UnityEngine.PlayerPrefs;

public class UserSettingsStorage {
    private static UserSettingsStorage _instance;

    public static UserSettingsStorage Instance {
        get {
            _instance ??= new UserSettingsStorage();
            return _instance;
        }
    }

    public int BestScore {
        get => Get<int>("bestScore");
        set => Set("bestScore", value);
    }

    public bool SoundState {
        get => Get("soundState", true);
        set => Set("soundState", value);
    }

    public bool MusicState {
        get => Get("musicState", true);
        set => Set("musicState", value);
    }

    public float MusicVolume {
        get => Get<float>("musicVolume", 1);
        set => Set("musicVolume", value);
    }

    public float SoundVolume {
        get => Get<float>("soundVolume", 1);
        set => Set("soundVolume", value);
    }

    #region private common methods

    private static void Set(string key, object value) {
        if (value == null) return;

        var serializedObj = JsonConvert.SerializeObject(value);
        SetString(key, serializedObj);
        Save();
    }

    public static T Get<T>(string key, T defaultValue = default)
    {
        object value = default(T);
        try
        {
            var data = GetString(key, defaultValue == null ? "" : defaultValue.ToString());
            if (value is bool) data = data.ToLower();
            if (!string.IsNullOrEmpty(data))
            {
                value = JsonConvert.DeserializeObject<T>(data, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            }
        }
        catch (Exception)
        {
            DeleteKey(key);
            return defaultValue == null ? (T)value : defaultValue;
        }

        return (T)value;
    }

    #endregion
}
