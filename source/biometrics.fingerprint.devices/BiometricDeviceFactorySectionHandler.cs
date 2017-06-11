using System;
using System.Configuration;
using System.Reflection;

namespace NoID.Biometrics.Managers
{
    public sealed class BiometricDevices : ConfigurationSection
    {
        Configuration config;
        string configPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\noid.device.config";

        public BiometricDevices()
        {
            config = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap() { ExeConfigFilename = configPath }, ConfigurationUserLevel.None);
            ConfigurationSection sec  = config.GetSection("BiometricDevices");
            Name = (string)config.AppSettings.Settings["SelectedDevice"].Value;
        }

        [ConfigurationProperty("name")]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("DeviceType")]
        public string DeviceType
        {
            get { return (string)base["deviceType"]; }
        }

        [ConfigurationProperty("ManufacturerName")]
        public string ManufacturerName
        {
            get { return (string)base["manufacturerName"]; }
        }

        [ConfigurationProperty("DriverName")]
        public string DriverName
        {
            get { return (string)base["driverName"]; }
        }

        [ConfigurationProperty("DriverLocation")]
        public string DriverLocation
        {
            get { return (string)base["driverLocation"]; }
        }
        /*
        public string DeviceInformation
        {
            get
            {
                try
                {
                   
                    ConfigurationSection section = config.GetSection("BiometricDevices");
                    BiometricDevices devices = (BiometricDevices)section.ElementInformation[];

                    int selectedIndex = 0;
                    return string.Concat("Name = ", devices[selectedIndex].Name, ", Type = ", devices[selectedIndex].DeviceType, ", ManufacturerName = ", devices[selectedIndex].ManufacturerName, ", DriverName = ", devices[selectedIndex].DriverName);
                }
                catch (Exception except)
                {
                    throw new Exception("Device definition for " + ConfigurationManager.AppSettings["SelectedDevice"] + " was not found in noid.devoice.config file. Error = " + except.Message);
                }
            }
        }
        */
    }
}