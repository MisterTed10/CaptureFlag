using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Core
{

    public class GameConfigurator
    {

        private Dictionary<string, string> m_internalStorage;
        public GameConfigurator()
        {
            RestoreToDefaultValue();
        }
        public void SetItemValue(string key, string value)
        {
            m_internalStorage[key] = value;
        }
        public string GetItemValue(string key)
        {
            if (m_internalStorage.ContainsKey(key))
            {
                return m_internalStorage[key];
            }
            else
                return "";
        }
        public void LoadFileConfiguration(string path, bool overwrite)
        {
            //TODO
        }
        public void RestoreToDefaultValue()
        {
            m_internalStorage = new Dictionary<string, string>();
            m_internalStorage["TeamCount"] = "2";
            m_internalStorage["Team[0].KnightCount"] = "5";
            m_internalStorage["Team[0].SamouraiCount"] = "0";
            m_internalStorage["Team[0].SpawnPosition"] = "0.0 -20.0";
            m_internalStorage["Team[0].FlagPosition"] = "-12.75 -20.0";
            m_internalStorage["Team[0].Color"] = "0 255 0";//255
            m_internalStorage["Team[1].KnightCount"] = "0";
            m_internalStorage["Team[1].SamouraiCount"] = "8";
            m_internalStorage["Team[1].SpawnPosition"] = "0.0 20.0";
            m_internalStorage["Team[1].FlagPosition"] = "12.75 20.0";
            m_internalStorage["Team[1].Color"] = "0 0 255";//255
            m_internalStorage["KnightStamina"] = "3";
            m_internalStorage["SamouraiStamina"] = "1";
            m_internalStorage["KnightSpeed"] = "8"; //unit.sec-1
            m_internalStorage["SamouraiSpeed"] = "15";//unit.sec-1
            m_internalStorage["KnightVisibilityRange"] = "5"; //unit
            m_internalStorage["SamouraiVisibilityRange"] = "5";//unit
            m_internalStorage["MapWidth"] = "50";
            m_internalStorage["MapHeight"] = "50";
            m_internalStorage["PointToWin"] = "5";
            m_internalStorage["BaseRespawnTime"] = "10";

        }

    }
    
}
