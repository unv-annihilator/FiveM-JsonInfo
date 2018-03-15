using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GHMatti.InfoJson.Types
{
    [Serializable()]
    public class Vehicle
    {
        [XmlElement("modelHash")]
        public uint modelHash { get; set; }

        [XmlElement("modelName")]
        public string modelName { get; set; }

        [XmlElement("gameName")]
        public string gameName { get; set; }

        [XmlElement("vehicleMakeName")]
        public string vehicleMakeName { get; set; }

        [XmlElement("flags")]
        public string flags { get; set; }

        [XmlElement("plateType")]
        public string plateType { get; set; }

        [XmlElement("vehicleClass")]
        public string vehicleClass { get; set; }

        [XmlElement("type")]
        public string type { get; set; }
    }

    [Serializable()]
    public class Data
    {
        [XmlElement("Item")]
        public Vehicle[] vehicle { get; set; }
    }

    [Serializable()]
    [XmlRoot("CVehicleModelInfo__InitDataList")]
    public class CVehicleModelInfo
    {
        [XmlElement("InitDatas")]
        public Data data { get; set; }
    }

    [Serializable()]
    [XmlRoot("MVehicleData")]
    public class MVehicleData
    {
        public MVehicleData() { vehicles = new List<Vehicle>(); }
        [XmlElement("vehicle")]
        public List<Vehicle> vehicles { get; set; }
    }

}
