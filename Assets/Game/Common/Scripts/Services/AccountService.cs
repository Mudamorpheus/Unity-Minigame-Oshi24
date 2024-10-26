using System.Collections.Generic;

using UnityEngine;

namespace Game.Common.Scripts.Services
{
    public class AccountService
    {
        //======SUBCLASSES

        [System.Serializable]
        public enum ItemState
        {
            Available,
            Unselected,
            Selected,            
        }

        [System.Serializable]
        public class SkinData
        {
            public string Id;
            public ItemState State;

            public SkinData(string id, ItemState state)
            {
                Id = id;
                State = state;
            }
        }

        [System.Serializable]
        public class StickData
        {
            public string Id;
            public ItemState State;

            public StickData(string id, ItemState state)
            {
                Id = id;
                State = state;
            }
        }

        [System.Serializable]
        public class ShopData
        {
            public List<SkinData> Skins;
            public List<StickData> Sticks;

            public void AddSkin(string id, ItemState state)
            {
                var skin = new SkinData(id, state);
                Skins.Add(skin);
            }

            public void AddStick(string id, ItemState state)
            {
                var stick = new StickData(id, state);
                Sticks.Add(stick);
            }

            public ShopData()
            {
                Skins = new List<SkinData>();
                AddSkin("Default", ItemState.Selected);
                Sticks = new List<StickData>();
                AddStick("Default", ItemState.Selected);
            }
        }

        [System.Serializable]
        public class AccountData
        {
            public string   Name;
            public int      Score;
            public int      Balance;
            public bool     Sound;
            public bool     Music;
            public bool     Tutorial;
            public ShopData Shop;

            public AccountData(string name, int score, int balance, bool sound, bool music, bool tutorial, ShopData shop)
            {
                Name     = name;
                Score    = score;
                Balance  = balance;
                Sound    = sound;
                Music    = music;
                Tutorial = tutorial;
                Shop     = shop;
            }
        }

        //======FIELDS

        private string      code = "XwUMhfb6PKnXW3BD8CskF";
        private AccountData data;

        //======PROPERTIES

        public AccountData     Data   { get { return data; } }
        public List<SkinData>  Skins  { get { return data.Shop.Skins; } }
        public List<StickData> Sticks { get { return data.Shop.Sticks; } }

        //======PLAYER

        public void Init()
        {
            var name = "Player#" + Random.Range(10000, 99999);
            var shop = new ShopData();
            data = new AccountData(name, 0, 0, true, true, true, shop);
        }

        public void Save()
        {
            string key = code;
            string json = JsonUtility.ToJson(Data);
            PlayerPrefs.SetString(key, json);
        }

        public void Load()
        {
            string key = code;
            string json = PlayerPrefs.GetString(key);
            if(json != "")
            {
                data = JsonUtility.FromJson<AccountData>(json);
            }            
        }                     
    }
}