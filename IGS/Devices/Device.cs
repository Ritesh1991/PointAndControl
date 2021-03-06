﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media.Media3D;

namespace PointAndControl.Devices
{
    /// <summary>
    ///     The abstract class represents a device with a name, a shape made out of balls(spheres), 
    ///     and a transmit method which has to be specified.
    ///     @author Florian Kinn
    /// </summary>
    public abstract class Device
    {
        /// <summary>
        ///     Constructor for a device
        ///     <param name="name">name of a device.</param>
        ///     <param name="id">id of a device.</param>
        ///     <param name="form">Shape of a device.</param>
        /// </summary>
        protected Device(String name, String id, String path, List<Ball> form)
        {
            Name = name;
            Id = id;
            Form = form;
            Path = path;
            skelPositions = new List<Point3D[]>();

            parentID = null;
        }

        ///     Name of the device.
        ///     With the "set"-method the name can be set.
        ///     With the "get"-method the name can be returned.
        ///     <returns>Returns the name of the device</returns>
        public String Name { get; set; }

        ///     The ID of the device.
        ///     With the "set"-method the ID of the device can be set.
        ///     With the "get"-method the ID of the device can be returned.
        ///     <returns>Returns the ID of the device</returns>
        public String Id { get; set; }

        public String Path { get; set; }
        /// <summary>
        ///     Die Form des Geräts wird durch einen oder mehrere Ballkörper,
        ///     welche in einer Liste gespeichert werden, dargestellt.
        ///     Mit der "set"-Methode kann die Liste gesetzt werden.
        ///     Mit der "get"-Methode kann die Liste zurückgegeben werden.
        ///     <returns>Gibt die Liste der Bälle zurück.</returns>
        /// </summary>
        /// 
        public Connection connection { get; set; }

        public String CommandString { get; set; }

        public List<Ball> Form { get; set; }

        public List<Point3D[]> skelPositions { get; set; }

        public string parentID { get; set; }

        public Color color { get; set; }

        public static List<Type> deviceTypes = getAllDerivedDeviceTypes();

        public String[] splitPathToIPAndPort()
        {
            String ipAndPortPattern = "[1-9]{1,3}[.]{1}[0-9]{1,3}[.]{1}[0-9]{1,3}[.]{1}[1-9]{0,3}[:]{1}[0-9]{1,5}";

            Regex regex = new Regex(ipAndPortPattern);

            String ipPort = regex.Match(Path).ToString();

            String[] ipAndPort = ipPort.Split(':');

            return ipAndPort;
        }

        public static bool checkForIpAndPort(String s)
        {
            String ipAndPortPattern = "[1-9]{1,3}[.]{1}[0-9]{1,3}[.]{1}[0-9]{1,3}[.]{1}[1-9]{0,3}[:]{1}[0-9]{1,5}";
            Regex regex = new Regex(ipAndPortPattern);

            return regex.IsMatch(s);
        }

        public string putPrefixHTTP(string post)
        {
            return "http://" + post;
        }

        //Base-Code from User "Yahoo Serious" on Stackoverflow: http://stackoverflow.com/questions/857705/get-all-derived-types-of-a-type 
        //AccessDate: 18.04.2016 - 09:16 am UTC+1
        public static List<Type> getAllDerivedDeviceTypes()
        {
            var listOfBs = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                            from assemblyType in domainAssembly.GetTypes()
                            where typeof(Device).IsAssignableFrom(assemblyType)
                            select assemblyType).ToArray();

            return listOfBs.ToList();
        }

        public static List<String> getAllDerivedDeviceTypesAsStrings()
        {
            IEnumerable<Type> types = Device.deviceTypes;
            List<String> result = new List<string>();

            foreach (var t in types)
            {
                if (!t.Name.Equals("Device"))
                {
                    result.Add(t.Name);
                }
                
            }

            return result;
        }

        public bool hasParent()
        {
            return parentID != null;
        }

        public void addParent(Device dev)
        {
            parentID = dev.Id;
        }
    }
}