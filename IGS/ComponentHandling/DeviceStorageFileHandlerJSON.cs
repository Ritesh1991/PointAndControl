﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IGS.Server.Devices;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

namespace IGS.ComponentHandling
{
    public class DeviceStorageFileHandlerJSON
    {
        readonly string DEVICE_SAVE_PATH = AppDomain.CurrentDomain.BaseDirectory + "\\devices.txt";
        private JsonSerializerSettings setting { get; set; }

        public DeviceStorageFileHandlerJSON()
        {
            setting = new JsonSerializerSettings();
            setting.TypeNameHandling = TypeNameHandling.All;
        }

        public void addDevice(Device dev)
        {
            if (!File.Exists(DEVICE_SAVE_PATH))
            {
                File.Create(DEVICE_SAVE_PATH).Close();
            }

            JsonSerializer serializer = new JsonSerializer();
            serializer.TypeNameHandling = TypeNameHandling.All;
            serializer.Formatting = Formatting.Indented;

            using (FileStream stream = File.Open(DEVICE_SAVE_PATH, FileMode.Append))
            using (StreamWriter sw = new StreamWriter(stream))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, dev);
            }

            //List<Device> devices = readDevices();

            //devices.Add(dev);

            //writeDevicesToFile(devices);
        }

        public string addDeviceCoord(string devId,  Ball ball)
        {
            
            List<Device> devices = readDevices();

            if (devices == null || devices.Count == 0)
                return "No Devices Available";

            foreach(Device dev in devices)
            {
                if(dev.Id == devId)
                {
                    dev.Form.Add(ball);
                    writeDevicesToFile(devices);
                    return "Coordinates added";
                }
            }

           

            return "No Device with that ID Found - Coordinates not added";
        }

        public List<Device> readDevices()
        {
            if (!File.Exists(DEVICE_SAVE_PATH))
            {
                File.Create(DEVICE_SAVE_PATH).Close();
            }

            String devices = File.ReadAllText(DEVICE_SAVE_PATH);

            //String[] splitDevices = devices.Split(new string[] { "}{" }, StringSplitOptions.None);

            //splitDevices[0] = splitDevices[0] + "}";

            //for (int i = 1; i < (splitDevices.Count() - 2); i++)
            //{
            //    splitDevices[i] = "{" + splitDevices[i] + "}";
            //}

            //splitDevices[splitDevices.Count() - 1] = "{" + splitDevices[splitDevices.Count() - 1];

            List<Device> devs = JsonConvert.DeserializeObject<List<Device>>(devices);
            //List<Device> devs = new List<Device>();
            //foreach (String dev in splitDevices)
            //{
                
            //}

            if (devs == null)
            {
                devs = new List<Device>();
            }

            return new List<Device>();
        }


        public void replaceExistingDevices(List<Device> devices)
        {
            writeDevicesToFile(devices);
        }

        private void writeDevicesToFile(List<Device> devices)
        { 
            string devicesString = JsonConvert.SerializeObject(devices, Formatting.Indented, setting);
            File.WriteAllText(DEVICE_SAVE_PATH, devicesString);
        }

        
        

        
    }
}
