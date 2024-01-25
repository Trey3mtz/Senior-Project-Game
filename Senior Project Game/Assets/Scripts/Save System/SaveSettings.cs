using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cyrcadian
{

    [System.Serializable]
    public struct PlayerSavedSettings
    {
        public float MasterAudioVolume;
    }


    public class SaveSettings : MonoBehaviour
    {

        /// <summary>
        ///                         This script is attached to the Save Button in the Pause Menu. 
        ///                         It's job is to grab the settings we want to save. Place them in a stuct, 
        ///                         and send that to PlayerData.
        ///                         
        ///                         If you want more settings to be saved, SerializeField a type of UI element you want saved.
        ///                         And in the Save method (which gets called by PlayerData script, when it saves), add it to
        ///                         the method's data. Make sure the PlayerSavedSettings struct also has the piece of data you
        ///                         wanted to be saved as well.
        /// </summary>

        [SerializeField] Slider masterVolume;



        public void Save(ref PlayerSavedSettings data)
        {
            data.MasterAudioVolume = masterVolume.value;
        } 

        public void Load(PlayerSavedSettings data)
        {
            masterVolume.value = data.MasterAudioVolume;
            AudioManager.Instance.ChangeMasterVolume(data.MasterAudioVolume);
        }
    }
}
