using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GHMatti.InfoJson {

    public class Client : BaseScript {

        public Client() {
            EventHandlers["playerSpawned"] += new Action(StartXMLParse);
            EventHandlers["RequestVehicleHashDictionary"] += new Action<string>(CreateVehicleHashDictionary);
            EventHandlers["RequestWeaponHashDictionary"] += new Action<string>(CreateWeaponHashDictionary);
        }

        public void StartXMLParse() {
            TriggerServerEvent("ParseXMLData");
        }

        public void CreateVehicleHashDictionary(string modelNamesJSON) {
            List<string> modelNames = JsonConvert.DeserializeObject<List<string>>(modelNamesJSON);
            Dictionary<string, uint> modelHashDictionary = new Dictionary<string, uint>();
            foreach (string name in modelNames) {
                modelHashDictionary[name] = (uint)API.GetHashKey(name);
                if (!API.IsModelValid(modelHashDictionary[name])) {
                    modelHashDictionary[name] = 0;
                }
            }
            Debug.WriteLine($"Sending list of {modelHashDictionary.Count} to Server");
            TriggerServerEvent("RecieveHash", JsonConvert.SerializeObject(modelHashDictionary), "Vehicle");
        }

        public void CreateWeaponHashDictionary(string modelNamesJSON) {
            List<string> modelNames = JsonConvert.DeserializeObject<List<string>>(modelNamesJSON);
            Dictionary<string, uint> modelHashDictionary = new Dictionary<string, uint>();
            foreach (string name in modelNames) {
                modelHashDictionary[name] = (uint)API.GetHashKey(name);
                if (!API.IsWeaponValid(modelHashDictionary[name])) {
                    modelHashDictionary[name] = 0;
                }
            }
            Debug.WriteLine($"Sending list of {modelHashDictionary.Count} to Server");
            TriggerServerEvent("RecieveHash", JsonConvert.SerializeObject(modelHashDictionary), "Weapon");
        }
    }
}