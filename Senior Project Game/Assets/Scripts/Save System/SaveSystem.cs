using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System;
using Cyrcadian.WorldTime;

namespace Cyrcadian.PlayerSystems
{
    public class SaveSystem 
    {   
        /**************************************************************************
            </> Summary </>

            There is a serializable struct made, SaveData, which holds other structs of data.
            (Example: PlayerData holds vector3 data for their last position)

        
        */
        [System.Serializable]
        public struct SaveData
        {
            public PlayerSaveData playerData;
            public WorldTimeSaveData TimeSaveData;
            public SaveData[] ScenesData;
        }            
        
        private static SaveData s_CurrentData = new SaveData();

        [System.Serializable]
        public struct SceneData
        {
            public string SceneName;
        }


        private static Dictionary<string, SceneData> s_ScenesDataLookup = new Dictionary<string, SceneData>();


        public static void Save()
        {
            GameManager.Instance.PlayerData.Save(ref s_CurrentData.playerData);
            GameManager.Instance.Time_World.Save(ref s_CurrentData.TimeSaveData);

            string savefile = Application.persistentDataPath + "/save.sav";

                try
                {   // Check if the file we want to save even exists.
                    // If yes, delete that save as we will be writing over it.
                    if (File.Exists(savefile))
                    {
                        Debug.Log("Data exists. Deleting old file and writing a new one!");
                        File.Delete(savefile);
                    }
                    else
                        {Debug.Log("Writing file for the first time!");}
                    
                    // Using creating a new savefile with Filestream. Make sure to immediately close it afterwards.
                    using FileStream stream = File.Create(savefile);
                    stream.Close();
                        {Debug.Log("Closed stream!");}

                    // This "Serializes" or turns our struct of s_CurrentData into a json readable file.
                    File.WriteAllText(savefile, JsonConvert.SerializeObject(s_CurrentData, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
                }
                catch (Exception e)
                {
                    Debug.LogError($"Unable to save data due to: {e.Message} {e.StackTrace}");
                }
        }
        

        public static void Load()
        {
            string savefile = Application.persistentDataPath + "/save.sav";


            if (!File.Exists(savefile))
            {
                Debug.LogError($"Cannot load file at {savefile}. File does not exist!");
                throw new FileNotFoundException($"{savefile} does not exist!");
            }   
                try
                {     
                    s_CurrentData = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(savefile));
                }  
                catch (Exception e)
                {
                        Debug.LogError($"Failed to load data due to: {e.Message} {e.StackTrace}");
                        throw e;
                }
            SceneManager.sceneLoaded += SceneLoaded;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }

        static void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            GameManager.Instance.PlayerData.Load(s_CurrentData.playerData);
            GameManager.Instance.Time_World.Load(s_CurrentData.TimeSaveData);
            //GameManager.Instance.Terrain.Load(s_CurrentData.TerrainData);

            SceneManager.sceneLoaded -= SceneLoaded;
        }

        public static void SaveSceneData()
        {

        }

        public static void LoadSceneData()
        {

        }
    }

}
