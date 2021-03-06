﻿using System;
using System.Collections.Generic;

namespace PointAndControl.Devices
{
    /// <summary>
    ///     This class specializes the device class to the class plugwise
    ///     It contains all information as well as functions available for a plugwise.
    ///     Follwing functions are available:
    ///     On
    ///     Off
    ///     @author Florian Kinn
    /// </summary>
    public class Plugwise : NativeTransmittingDevice
    {
        private readonly String _commandString = "http://cumulus.teco.edu:5000/plugwise/";


        /// <summary>
        ///     Constructor of a plugwise object.
        ///     <param name="id">ID of the object for identifying it</param>
        ///     <param name="name">Userdefined name of the device</param>
        ///     <param name="form">Shape of the device in the room</param>
        ///     <param name="path">The Path to communicate with the device</param>  
        /// </summary>

        public Plugwise(String name, String id, String path, List<Ball> form)
                : base(name, id, path, form)
            {
            String[] ipAndPort = splitPathToIPAndPort();
            connection = new Http(Convert.ToInt32(ipAndPort[1]), "127.0.0.1");

            _commandString += getPlugId();

            CommandString = _commandString;
            }

        /// <summary>
        ///     The Transmit method is responsible for the correct invocation of a function of the plugwise 
        ///     which is implicated by the "commandID"
        ///     <param name="cmdId">
        ///         With the commandID the Transmit-method recieves which command
        ///         should be send to the device (plugwise)
        ///     </param>
        ///     <param name="value">
        ///         The value belonging to the command
        ///     </param>
        ///     <returns>
        ///     If execution was successful
        ///     </returns>
        /// </summary>
        public override String Transmit(String cmdId, String value)
        {
            String response = Properties.Resources.InvalidCommand;
            switch (cmdId)
            {

                case "on":
                    if (connection.Send(CommandString + "/on").StartsWith("{\n  \"plugwise\": {\n    \"state\":"))
                        response = "true";
                    break;
                case "off":
                    if (connection.Send(CommandString + "/off").StartsWith("{\n  \"plugwise\": {\n    \"state\":"))
                        response = "true";
                    break;
            }
            return response;
        }

        public string getPlugId()
        {
            String[] splitArray = Path.Split('/');
            

            if(splitArray.Length != 1)
            {
                return Path.Split('/')[1];
            }

            return "NoPlugwiseID";

        }
    }
}