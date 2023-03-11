using Google.Protobuf.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public interface ILoader<key, Value>
{
    Dictionary<key, Value> MakeDict();

}

//TODO
//스테이지 정보 파일을 만들어보자.

public class DataManager
{
    public Dictionary<int, Skill> SkillDict { get; private set; } = new Dictionary<int, Skill>();
    public Dictionary<string, StatInfo> StatDict { get; private set; } = new Dictionary<string, StatInfo>();

    public void Init()
    {
        SkillDict = LoadJson<SkillData, int, Skill>("SkillData").MakeDict();
        StatDict = LoadJson<StatData, string, StatInfo>("StatData").MakeDict();

    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        //TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        //return JsonUtility.FromJson<Loader>(textAsset.text);


        var jsonSerializerSettings = new JsonSerializerSettings();
        jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        //string text = File.ReadAllText($"{ConfigManager.Config.dataPath}/{path}.json");
        return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(textAsset.text, jsonSerializerSettings);

    }

}
