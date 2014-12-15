﻿using IGS.KNN;
using IGS.Server.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Xml;
using System.Xml.Linq;


namespace IGS.Helperclasses
{
    static class XMLComponentHandler
    {
        /// <summary>
        /// This method reads the device information in the configuration.xml and creates for each device a device object 
        /// with the specified parameters of the read file entry.
        /// </summary>
        /// <returns>A list with device objects</returns>
        public static List<Device> readDevices()
        {
            List<Device> devices = new List<Device>();
            XmlTextReader reader = new XmlTextReader(AppDomain.CurrentDomain.BaseDirectory + "\\configuration.xml");
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "device")
                {
                    String type = reader.GetAttribute(0);
                    reader.ReadToDescendant("name");
                    String name = reader.ReadString();
                    reader.ReadToFollowing("id");
                    String id = reader.ReadString();
                    reader.ReadToFollowing("form");
                    String[] ballParams = new String[4];
                    List<Ball> form = new List<Ball>();
                    int count = Convert.ToInt32(reader.GetAttribute(0));
                    for (int o = 0; o < count; o++)
                    {

                        reader.ReadToFollowing("ball");
                        for (int i = 0; i < ballParams.Length; i++)
                        {
                            ballParams[i] = reader.GetAttribute(i);
                        }
                        Vector3D vec = new Vector3D(Convert.ToDouble(ballParams[1]), Convert.ToDouble(ballParams[2]),
                                               Convert.ToDouble(ballParams[3]));
                        float radius = (float)Convert.ToDouble(ballParams[0]);
                        Ball ball = new Ball(vec, radius);
                        form.Add(ball);
                    }
                    reader.ReadToFollowing("address");
                    String address = reader.ReadString();
                    reader.ReadToFollowing("port");
                    String port = reader.ReadString();

                    Type typeObject = Type.GetType("IGS.Server.Devices." + type);
                    try
                    {
                        object instance = Activator.CreateInstance(typeObject, name, id, form, address, port);
                        devices.Add((Device)instance);
                        Debug.WriteLine(instance.GetType().ToString());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Beim Erzeugen eines Gerätes ist ein Fehler aufgetreten:");
                        Console.WriteLine(e.StackTrace);
                    }
                }

            }
            reader.Close();

            return devices;
        }
        /// <summary>
        ///         Adds an device entry with the given parameters and id_number to the deviceConfiguration in the configuration.xml
        /// </summary>
        ///         <param name="parameter">the parameters of the device which will be added
        ///                         parameters: Type, name, id, form, adress
        ///         </param>
        /// <param name="id_number">The calculated number to attach to the id to have a unique id</param>
        public static void addDeviceToXML(String[] parameter, int id_number)
        {
            XmlDocument docConfig = new XmlDocument();
            docConfig.Load(AppDomain.CurrentDomain.BaseDirectory + "\\configuration.xml");
            XmlNode rootNode = docConfig.SelectSingleNode("/config/deviceConfiguration");

            // creates an device node
            XmlElement device = docConfig.CreateElement("device");
            device.SetAttribute("type", parameter[0]);
            rootNode.AppendChild(device);

            XmlNodeList deviceNodes = docConfig.SelectNodes("config/deviceConfiguration/device");

            XmlElement name = docConfig.CreateElement("name");
            name.InnerText = parameter[1];
            deviceNodes[deviceNodes.Count - 1].AppendChild(name);

            XmlElement id = docConfig.CreateElement("id");

            id.InnerText = parameter[0] + "_" + id_number;
            deviceNodes[deviceNodes.Count - 1].AppendChild(id);

            XmlElement form = docConfig.CreateElement("form");
            form.SetAttribute("count", "0");
            deviceNodes[deviceNodes.Count - 1].AppendChild(form);

            XmlElement address = docConfig.CreateElement("address");
            address.InnerText = parameter[2];
            deviceNodes[deviceNodes.Count - 1].AppendChild(address);

            XmlElement port = docConfig.CreateElement("port");
            port.InnerText = parameter[3];
            deviceNodes[deviceNodes.Count - 1].AppendChild(port);

            docConfig.Save(AppDomain.CurrentDomain.BaseDirectory + "\\configuration.xml");
        }

        /// <summary>
        /// This methods writes for a specific existing device a new ball (position and radius)
        /// in its XML entry.
        /// </summary>
        /// <param name="devId">the id of the device, the new ball will be added to</param>
        /// <param name="radius">the radius of the ball</param>
        /// <param name="ball">the ball of the device</param>
        /// <returns>returns the message if the write process was successful</returns>
        public static String addDeviceCoordToXML(String devId, String radius, Ball ball)
        {
            String ret = "keine Koordinaten hinzugefügt";
            bool added = false;
            XmlDocument docConfig = new XmlDocument();
            docConfig.Load(AppDomain.CurrentDomain.BaseDirectory + "\\configuration.xml");
            XmlNodeList nodeList = docConfig.SelectNodes("/config/deviceConfiguration/device");

            foreach (XmlNode node in nodeList)
            {
                for (int i = 0; i < node.ChildNodes.Count && !added; i++)
                {
                    if (node.ChildNodes[i].InnerText.Equals(devId))
                    {
                        XmlElement newElement = docConfig.CreateElement("ball");
                        node.ChildNodes[i + 1].Attributes[0].Value =
                            (Convert.ToInt32(node.ChildNodes[i + 1].Attributes[0].Value) + 1).ToString();

                        newElement.SetAttribute("radius", radius);
                        newElement.SetAttribute("centerX", ball.Centre.X.ToString());
                        newElement.SetAttribute("centerY", ball.Centre.Y.ToString());
                        newElement.SetAttribute("centerZ", ball.Centre.Z.ToString());
                        node.ChildNodes[i + 1].AppendChild(newElement);

                        ret = "Koordinaten hinzugefügt";
                        added = true;
                    }
                }
            }
            docConfig.Save(AppDomain.CurrentDomain.BaseDirectory + "\\configuration.xml");


            return ret;
        }

        /// <summary>
        /// Reads the kinect configuration in the configuration.xml and returns the components as string attributes through an array.
        /// </summary>
        /// <returns>An string array with the kinect configurations in the order: x, y, z-coordinates, tiltangle, HorizontalorientationAngel</returns>
        public static string[] readKinectComponents()
        {
            String[] kinectComponents = new String[5];
            bool found = false;

            XmlTextReader reader = new XmlTextReader(AppDomain.CurrentDomain.BaseDirectory + "\\configuration.xml");
            while (reader.Read() && !found)
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "KinectConfiguration")
                {
                    reader.ReadToDescendant("X");
                    kinectComponents[0] = reader.ReadString();
                    reader.ReadToFollowing("Y");
                    kinectComponents[1] = reader.ReadString();
                    reader.ReadToFollowing("Z");
                    kinectComponents[2] = reader.ReadString();
                    reader.ReadToFollowing("tiltingAngle");
                    kinectComponents[3] = reader.ReadString();
                    reader.ReadToFollowing("HorizontalOrientationAngle");
                    kinectComponents[4] = reader.ReadString();
                    found = true;
                }
            }
            reader.Close();
            return kinectComponents;
        }

        /// <summary>
        /// Reads the plugwise adress components in the configuration.xml file and returns them in a string array.
        /// </summary>
        /// <returns>A string array with the plugwise adress komponents in the order: host, port, path</returns>
        public static string[] readPlugwiseComponents()
        {
            String[] PlugwiseComponents = new String[3];
            bool found = false;

            XmlTextReader reader = new XmlTextReader(AppDomain.CurrentDomain.BaseDirectory + "\\configuration.xml");

            while (reader.Read() && !found)
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "PlugwiseAdress")
                {
                    reader.ReadToDescendant("host");
                    PlugwiseComponents[0] = reader.ReadString();
                    reader.ReadToFollowing("port");
                    PlugwiseComponents[1] = reader.ReadString();
                    reader.ReadToFollowing("path");
                    PlugwiseComponents[2] = reader.ReadString();
                    found = true;
                }
            }
            reader.Close();
            return PlugwiseComponents;
        }
        /// <summary>
        /// reads the room specification in the configuaration.xml and returns the width, height, and depth in an array
        /// </summary>
        /// <returns>A string array with the specification of the room in the order: width, heigth, depth</returns>
        public static string[] readRoomComponents()
        {
            String[] roomComponents = new String[3];
            bool found = false;

            XmlTextReader reader = new XmlTextReader(AppDomain.CurrentDomain.BaseDirectory + "\\configuration.xml");
            while (reader.Read() && !found)
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Roomsize")
                {
                    reader.ReadToDescendant("width");
                    roomComponents[0] = reader.ReadString();
                    reader.ReadToFollowing("height");
                    roomComponents[1] = reader.ReadString();
                    reader.ReadToFollowing("depth");
                    roomComponents[2] = reader.ReadString();
                    found = true;
                }
            }
            reader.Close();
            return roomComponents;
        }

        /// <summary>
        /// Writes the given adress components for the plugwise adress in the configuration.xml file.
        /// </summary>
        /// <param name="adressComponents"> the plugwise adress components which should be written in the configuration.xml</param>
        public static void writePlugwiseAdresstoXML(String[] adressComponents)
        {
            XmlDocument docConfig = new XmlDocument();
            docConfig.Load(AppDomain.CurrentDomain.BaseDirectory + "\\configuration.xml");
            XmlNode node = docConfig.SelectSingleNode("/config/environment/PlugwiseAdress/host");
            node.InnerText = adressComponents[0];
            node = docConfig.SelectSingleNode("/config/environment/PlugwiseAdress/port");
            node.InnerText = adressComponents[1];
            node = docConfig.SelectSingleNode("/config/environment/PlugwiseAdress/path");
            node.InnerText = adressComponents[2];

            docConfig.Save(AppDomain.CurrentDomain.BaseDirectory + "\\configuration.xml");
        }


        /// <summary>
        /// Writes the given new kinect placement components in the configuration.xml file
        /// </summary>
        /// <param name="newPosition">the new kinect placement information which should be written in the configuration.xml</param>
        public static void saveKinectPosition(String[] newPosition)
        {
            XmlDocument docConfig = new XmlDocument();
            docConfig.Load(AppDomain.CurrentDomain.BaseDirectory + "\\configuration.xml");
            XmlNode node = docConfig.SelectSingleNode("/config/environment/KinectConfiguration/X");
            node.InnerText = newPosition[0];
            node = docConfig.SelectSingleNode("/config/environment/KinectConfiguration/Y");
            node.InnerText = newPosition[1];
            node = docConfig.SelectSingleNode("/config/environment/KinectConfiguration/Z");
            node.InnerText = newPosition[2];
            node = docConfig.SelectSingleNode("/config/environment/KinectConfiguration/tiltingAngle");
            node.InnerText = newPosition[3];
            node = docConfig.SelectSingleNode("/config/environment/KinectConfiguration/HorizontalOrientationAngle");
            node.InnerText = newPosition[4];
            docConfig.Save(AppDomain.CurrentDomain.BaseDirectory + "\\configuration.xml");
        }

        /// <summary>
        /// Writes the given room measurment components in the configuration.xml file.
        /// </summary>
        /// <param name="roomData">an array with the new measurement specifications which should be written in the configuration.xml</param>
        public static void saveRoomPosition(String[] roomData)
        {
            XmlDocument docConfig = new XmlDocument();
            docConfig.Load(AppDomain.CurrentDomain.BaseDirectory + "\\configuration.xml");
            XmlNode node = docConfig.SelectSingleNode("/config/environment/Roomsize/width");
            node.InnerText = roomData[0];
            node = docConfig.SelectSingleNode("/config/environment/Roomsize/height");
            node.InnerText = roomData[1];
            node = docConfig.SelectSingleNode("/config/environment/Roomsize/depth");
            node.InnerText = roomData[2];
            docConfig.Save(AppDomain.CurrentDomain.BaseDirectory + "\\configuration.xml");
        }

        public static void writeKNNSampleToXML(KNNSample sample)
        {
            XmlDocument docConfig = new XmlDocument();
            docConfig.Load(AppDomain.CurrentDomain.BaseDirectory + "\\KNNSamples.xml");
            XmlNode node = docConfig.SelectSingleNode("/devices");

            foreach (XmlNode deviceNode in node.ChildNodes)
            {

                if (deviceNode.FirstChild.InnerText == sample.sampleDeviceName && deviceNode.FirstChild.Name == "deviceName")
                {
                    if (deviceNode.ChildNodes[1].Name == "samplePositions")
                    {
                        XmlElement xmlPosition = docConfig.CreateElement("position");
                        XmlElement posX = docConfig.CreateElement("X");
                        posX.InnerText = sample.x.ToString();
                        xmlPosition.AppendChild(posX);
                        XmlElement posY = docConfig.CreateElement("Y");
                        posY.InnerText = sample.y.ToString();
                        xmlPosition.AppendChild(posY);
                        XmlElement posZ = docConfig.CreateElement("Z");
                        posZ.InnerText = sample.z.ToString();
                        xmlPosition.AppendChild(posZ);
                        //xmlPosition.SetAttribute("X:", sample.x.ToString());
                        //xmlPosition.SetAttribute("Y:", sample.y.ToString());
                        //xmlPosition.SetAttribute("Z:", sample.z.ToString());
                        deviceNode.ChildNodes[1].AppendChild(xmlPosition);
                        docConfig.Save(AppDomain.CurrentDomain.BaseDirectory + "\\KNNSamples.xml");
                        return;
                    }
                }
            }

            XmlElement newDev = docConfig.CreateElement("device");
            XmlElement deviceName = docConfig.CreateElement("deviceName");
            deviceName.InnerText = sample.sampleDeviceName;
            XmlElement samplePositions = docConfig.CreateElement("samplePositions");
            XmlElement position = docConfig.CreateElement("position");
            XmlElement X = docConfig.CreateElement("X");
            X.InnerText = sample.x.ToString();
            position.AppendChild(X);
            XmlElement Y = docConfig.CreateElement("Y");
            Y.InnerText = sample.y.ToString();
            position.AppendChild(Y);
            XmlElement Z = docConfig.CreateElement("Z");
            Z.InnerText = sample.z.ToString();
            position.AppendChild(Z);

            samplePositions.AppendChild(position);
            newDev.AppendChild(deviceName);
            newDev.AppendChild(samplePositions);

            node.AppendChild(newDev);
            docConfig.Save(AppDomain.CurrentDomain.BaseDirectory + "\\KNNSamples.xml");
            return;
        }


        public static List<KNNSample> readKNNSamplesFromXML()
        {
            List<KNNSample> sampleList = new List<KNNSample>();

            XmlDocument docConfig = new XmlDocument();
            docConfig.Load(AppDomain.CurrentDomain.BaseDirectory + "\\KNNSamples.xml");
           
            XmlNodeList devices = docConfig.SelectSingleNode("/devices").ChildNodes;

            foreach (XmlNode sample in devices)
            {
                String knnDeviceName = "";
                foreach (XmlNode prop in sample)
                {
                    if (prop.Name.Equals("deviceName"))
                    {
                        knnDeviceName = prop.InnerText;
                    }
                    else if (prop.Name.Equals("samplePositions"))
                    {
                        foreach (XmlNode position in prop.ChildNodes)
                        {
                            KNNSample s = new KNNSample(new Point3D(
                                double.Parse(position.ChildNodes[0].InnerText),
                                double.Parse(position.ChildNodes[1].InnerText),
                                double.Parse(position.ChildNodes[2].InnerText)), knnDeviceName);

                            sampleList.Add(s);
                        }
                    }
                }
                
            }

            return sampleList;
        }
        public static void writeSampleToXML(Vector3D[] positions, String deviceName)
        {
            XmlDocument docConfig = new XmlDocument();
            docConfig.Load(AppDomain.CurrentDomain.BaseDirectory + "\\samples.xml");
            XmlNode node = docConfig.SelectSingleNode("/devices");
            Vector3D dir = Vector3D.Subtract(positions[3], positions[2]);
            foreach (XmlNode deviceNode in node.ChildNodes)
            {
                if (deviceNode.FirstChild.InnerText == deviceName 
                    && deviceNode.FirstChild.Name == "deviceName"
                    && deviceNode.ChildNodes[1].Name == "samples")
                {
                   
                        XmlElement sample = docConfig.CreateElement("sample");
                        XmlNode xmlSamples = deviceNode.ChildNodes[1];
                       
                        XmlElement xmlPosition = docConfig.CreateElement("position");
                        XmlElement posX = docConfig.CreateElement("X");
                                   posX.InnerText = positions[2].X.ToString();
                                   xmlPosition.AppendChild(posX);
                        XmlElement posY = docConfig.CreateElement("Y");
                                   posY.InnerText = positions[2].Y.ToString();
                                   xmlPosition.AppendChild(posY);
                        XmlElement posZ = docConfig.CreateElement("Z");
                                   posZ.InnerText = positions[2].Z.ToString();
                        xmlPosition.AppendChild(posZ);
                        sample.AppendChild(xmlPosition);
                                
                               
                        XmlElement xmlDirection = docConfig.CreateElement("direction");
                        XmlElement dirX = docConfig.CreateElement("X");
                                   dirX.InnerText = dir.X.ToString();
                                   xmlDirection.AppendChild(dirX);
                        XmlElement dirY = docConfig.CreateElement("Y");
                                   dirY.InnerText = dir.Y.ToString();
                                   xmlDirection.AppendChild(dirY);
                        XmlElement dirZ = docConfig.CreateElement("Z");
                                   dirZ.InnerText = dir.Z.ToString();
                        xmlDirection.AppendChild(dirZ);
                        sample.AppendChild(xmlDirection);
                                
                            

                        
                        deviceNode.ChildNodes[1].AppendChild(sample);

                        docConfig.Save(AppDomain.CurrentDomain.BaseDirectory + "\\samples.xml");
                        return;
                        //xmlPosition.SetAttribute("X:", sample.x.ToString());
                        //xmlPosition.SetAttribute("Y:", sample.y.ToString());
                        //xmlPosition.SetAttribute("Z:", sample.z.ToString());
                    
                }
            }

            XmlElement newDev = docConfig.CreateElement("device");
            XmlElement devName = docConfig.CreateElement("deviceName");
            devName.InnerText = deviceName;
            XmlElement samplePositions = docConfig.CreateElement("samples");
            XmlElement newSample = docConfig.CreateElement("sample");


            XmlElement position = docConfig.CreateElement("position");
            XmlElement pX = docConfig.CreateElement("X");
            pX.InnerText = positions[2].X.ToString();
            position.AppendChild(pX);
            XmlElement pY = docConfig.CreateElement("Y");
            pY.InnerText = positions[2].Y.ToString();
            position.AppendChild(pY);
            XmlElement pZ = docConfig.CreateElement("Z");
            pZ.InnerText = positions[2].Z.ToString();
            position.AppendChild(pZ);
            newSample.AppendChild(position);


            XmlElement direction = docConfig.CreateElement("direction");
            XmlElement dX = docConfig.CreateElement("X");
            dX.InnerText = dir.X.ToString();
            direction.AppendChild(dX);
            XmlElement dY = docConfig.CreateElement("Y");
            dY.InnerText = dir.Y.ToString();
            direction.AppendChild(dY);
            XmlElement dZ = docConfig.CreateElement("Z");
            dZ.InnerText = dir.Z.ToString();
            direction.AppendChild(dZ);
            newSample.AppendChild(direction);

            samplePositions.AppendChild(newSample);
            newDev.AppendChild(devName);
            newDev.AppendChild(samplePositions);

            node.AppendChild(newDev);
            docConfig.Save(AppDomain.CurrentDomain.BaseDirectory + "\\samples.xml");
            return;
        }
    }
}
