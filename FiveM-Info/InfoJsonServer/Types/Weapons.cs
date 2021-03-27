using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GHMatti.InfoJson.Types {
#pragma warning disable IDE1006 // Naming Styles

    [Serializable()]
    public class Weapon {

        [XmlElement("weaponHash")]
        public uint weaponHash { get; set; }

        [XmlElement("nameHash")]
        public string nameHash { get; set; }

        [XmlElement("textLabel")]
        public string textLabel { get; set; }

        [XmlElement("weaponDesc")]
        public string weaponDesc { get; set; }

        [XmlElement("weaponTT")]
        public string weaponTT { get; set; }

        [XmlElement("weaponUppercase")]
        public string weaponUppercase { get; set; }

        [XmlElement("weaponComponents")]
        public WeaponComponents weaponComponents { get; set; }
    }

    [Serializable()]
    public class WeaponComponents {

        [XmlElement("Item")]
        public List<WeaponComponent> components { get; set; }
    }

    [Serializable()]
    public class WeaponComponent {

        [XmlElement("componentName")]
        public string componentName { get; set; }

        [XmlElement("textLabel")]
        public string textLabel { get; set; }

        [XmlElement("componentDesc")]
        public string componentDesc { get; set; }
    }

    [Serializable()]
    [XmlRoot("WeaponShopItemArray")]
    public class WeaponShopItemArray {

        [XmlElement("weaponShopItems")]
        public WeaponShopItems weaponShopItems { get; set; }
    }

    [Serializable()]
    public class WeaponShopItems {

        [XmlElement("Item")]
        public List<Weapon> weapons { get; set; }
    }

#pragma warning restore IDE1006 // Naming Styles
}