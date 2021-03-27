using CitizenFX.Core;
using CitizenFX.Core.Native;
using GHMatti.Core;
using GHMatti.Data.XML;
using GHMatti.InfoJson.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GHMatti.InfoJson {

    public class Server : BaseScript {
        private bool runOnce = false;
        private Dictionary<string, uint> vehicleHashDictionary = null;
        private Dictionary<string, uint> weaponHashDictionary = null;
        private Dictionary<string, uint> componentHashDictionary = null;
        private readonly Dictionary<string, bool> dictRecieved = new Dictionary<string, bool>();
        private readonly GHMattiTaskScheduler scheduler = new GHMattiTaskScheduler();

        public Server() {
            dictRecieved.Add("Vehicle", false);
            dictRecieved.Add("Weapon", false);
            dictRecieved.Add("Component", false);
            EventHandlers["RecieveHash"] += new Action<Player, string, string>(RecieveHashDictionary);
            EventHandlers["ParseXMLData"] += new Action<Player>(ParseXMLAsync);
        }

        private void ParseXMLAsync([FromSource] Player p) {
            if (!runOnce) {
                ParseVehicles(p);
                ParseWeapons(p);

                runOnce = !runOnce;
            }
        }

        private async void ParseWeapons(Player p) {
            string resourcePath = Path.Combine("resources", API.GetCurrentResourceName(), "xml");
            DirectoryInfo dir = new DirectoryInfo(Path.GetFullPath(resourcePath));
            FileInfo[] files = dir.GetFiles("*shop_weapon*");
            Dictionary<string, Weapon> weaponDict = new Dictionary<string, Weapon>();
            List<string> modelNames = new List<string>();

            foreach (FileInfo file in files) {
                Debug.WriteLine($"Serializing: {file.Name}");
                WeaponShopItemArray xmlData = await Serializer.Deserialize<WeaponShopItemArray>(file.Name, scheduler);
                foreach (Weapon weapon in xmlData.weaponShopItems.weapons) {
                    modelNames.Add(weapon.nameHash);
                    weaponDict[weapon.nameHash] = weapon;
                }
            }

            if (await RequestHashDictionary(p, "Weapon", modelNames)) {
                WeaponShopItems xmlOutData = new WeaponShopItems {
                    weapons = new List<Weapon>()
                };
                Dictionary<uint, Weapon> weaponHashDict = new Dictionary<uint, Weapon>();
                foreach (KeyValuePair<string, Weapon> entry in weaponDict) {
                    if (weaponHashDictionary[entry.Value.nameHash] != 0) {
                        entry.Value.weaponHash = weaponHashDictionary[entry.Value.nameHash];
                        xmlOutData.weapons.Add(entry.Value);
                        weaponHashDict[weaponHashDictionary[entry.Value.nameHash]] = entry.Value;
                    }
                }

                Serializer.SerializeJSON("ghmatti_weapon_data.json", xmlOutData);
                Serializer.SerializeJSON("ghmatti_weapon_data_simple.json", weaponHashDict);
            }
        }

        private async void ParseVehicles(Player p) {
            string resourcePath = Path.Combine("resources", API.GetCurrentResourceName(), "xml");
            DirectoryInfo dir = new DirectoryInfo(Path.GetFullPath(resourcePath));
            FileInfo[] files = dir.GetFiles("*vehicle*");
            Dictionary<string, Types.Vehicle> vehicleDict = new Dictionary<string, Types.Vehicle>();
            List<string> modelNames = new List<string>();

            foreach (FileInfo file in files) {
                Debug.WriteLine($"Serializing: {file.Name}");
                CVehicleModelInfo xmlData = await Serializer.Deserialize<CVehicleModelInfo>(file.Name, scheduler);
                foreach (Types.Vehicle vehicle in xmlData.data.vehicle) {
                    modelNames.Add(vehicle.modelName);
                    vehicleDict[vehicle.modelName] = vehicle;
                }
            }

            if (await RequestHashDictionary(p, "Vehicle", modelNames)) {
                MVehicleData xmlOutData = new MVehicleData();
                Dictionary<uint, Types.Vehicle> vehicleHashDict = new Dictionary<uint, Types.Vehicle>();
                foreach (KeyValuePair<string, Types.Vehicle> entry in vehicleDict) {
                    if (vehicleHashDictionary[entry.Value.modelName] != 0) {
                        entry.Value.modelHash = vehicleHashDictionary[entry.Value.modelName];
                        xmlOutData.vehicles.Add(entry.Value);
                        vehicleHashDict[vehicleHashDictionary[entry.Value.modelName]] = entry.Value;
                    }
                }

                Serializer.SerializeJSON("ghmatti_vehicle_data.json", xmlOutData);
                Serializer.SerializeJSON("ghmatti_vehicle_data_simple.json", vehicleHashDict);
            }
        }

        public void RecieveHashDictionary([FromSource] Player p, string strHashDictionary, string s) {
            if (s == "Vehicle")
                vehicleHashDictionary = JsonConvert.DeserializeObject<Dictionary<string, uint>>(strHashDictionary);
            else if (s == "Weapon")
                weaponHashDictionary = JsonConvert.DeserializeObject<Dictionary<string, uint>>(strHashDictionary);
            else if (s == "Component")
                componentHashDictionary = JsonConvert.DeserializeObject<Dictionary<string, uint>>(strHashDictionary);
            dictRecieved[s] = true;
        }

        public void AskForDictionary(Player p, string s, List<string> modelNames) {
            p.TriggerEvent($"Request{s}HashDictionary", JsonConvert.SerializeObject(modelNames));
        }

        public async Task<bool> RequestHashDictionary(Player p, string s, List<string> modelNames) {
            Debug.WriteLine($"Sending {modelNames.Count} {s} Model Names to Hash to Client");
            await Delay(0);
            AskForDictionary(p, s, modelNames);
            while (!dictRecieved[s]) {
                await Delay(0);
            }
            return true;
        }
    }
}