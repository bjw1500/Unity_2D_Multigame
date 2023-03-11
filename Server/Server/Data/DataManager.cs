using Google.Protobuf.Protocol;
using Newtonsoft.Json;
using Server.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server
{
    public interface ILoader<key, Value>
    {
        Dictionary<key, Value> MakeDict();
    }

    //TODO
    //스테이지 정보 파일을 만들어보자.

    public class DataManager
    {
        public static Dictionary<string, StatInfo> StatDict { get; private set; } = new Dictionary<string, StatInfo>();
        public static Dictionary<int, Skill> SkillDict { get; private set; } = new Dictionary<int, Skill>();

        public static void LoadData()
        {
            StatDict = LoadJson<StatData, string, StatInfo>("StatData").MakeDict();
            SkillDict = LoadJson<SkillData, int, Skill>("SkillData").MakeDict();
        }

        static Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
        {

            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            string text = File.ReadAllText($"{ConfigManager.Config.dataPath}/{path}.json");
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(text, jsonSerializerSettings);

        }

    }

}
