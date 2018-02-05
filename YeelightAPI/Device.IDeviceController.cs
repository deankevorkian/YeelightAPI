﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using YeelightAPI.Models;

namespace YeelightAPI
{
    public partial class Device : IDeviceController
    {

        /// <summary>
        /// Disconnect the current device
        /// </summary>
        /// <returns></returns>
        public void Disconnect()
        {
            if (this.tcpClient != null)
            {
                this.tcpClient.Dispose();
                this.tcpClient = null;
            }
        }

        #region synchronous

        /// <summary>
        /// Connects to a device
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            return ConnectAsync().Result;
        }

        /// <summary>
        /// Toggle the device power
        /// </summary>
        /// <returns></returns>
        public bool Toggle()
        {
            return ToggleAsync().Result;
        }

        /// <summary>
        /// Set the device power state
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool SetPower(bool state = true)
        {
            return SetPowerAsync(state).Result;
        }

        /// <summary>
        /// Change the device brightness
        /// </summary>
        /// <param name="value"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public bool SetBrightness(int value, int? smooth = null)
        {
            return SetBrightnessAsync(value, smooth).Result;
        }

        /// <summary>
        /// Change the device RGB color
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public bool SetRGBColor(int r, int g, int b, int? smooth)
        {
            return SetRGBColorAsync(r, g, b, smooth).Result;
        }

        /// <summary>
        /// Change Color Temperature
        /// </summary>
        /// <param name="saturation"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public bool SetColorTemperature(int temperature, int? smooth)
        {
            return SetColorTemperatureAsync(temperature, smooth).Result;
        }

        #endregion synchronous

        #region asynchronous

        /// <summary>
        /// Connects to a device asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ConnectAsync()
        {
            this.Disconnect();

            this.tcpClient = new TcpClient();
            //IPEndPoint endPoint = GetIPEndPointFromHostName(this.Hostname, this.Port);
            await this.tcpClient.ConnectAsync(this.Hostname, this.Port);

            if (!this.tcpClient.Connected)
            {
                return false;
            }

            //continuous receiving
#pragma warning disable 4014
            this.Watch();
#pragma warning restore 4014

            //initialiazing all properties
            foreach (KeyValuePair<PROPERTIES, object> property in this.GetAllProps())
            {
                this[property.Key] = property.Value;
            }

            return true;
        }

        /// <summary>
        /// Toggle the device power asynchronously
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ToggleAsync()
        {
            CommandResult result = await ExecuteCommandWithResponseAsync(METHODS.Toggle, id: (int)METHODS.Toggle);

            return result.IsOk();
        }

        /// <summary>
        /// Set the device power state asynchronously
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task<bool> SetPowerAsync(bool state = true)
        {
            CommandResult result = await ExecuteCommandWithResponseAsync(
                method: METHODS.SetPower,
                id: (int)METHODS.SetPower,
                parameters: new List<object>() { state ? "on" : "off" }
            );

            return result.IsOk();
        }

        /// <summary>
        /// Change the device brightness asynchronously
        /// </summary>
        /// <param name="value"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public async Task<bool> SetBrightnessAsync(int value, int? smooth = null)
        {
            List<object> parameters = new List<object>() { value };

            HandleSmoothValue(ref parameters, smooth);

            CommandResult result = await ExecuteCommandWithResponseAsync(
                method: METHODS.SetBrightness,
                id: (int)METHODS.SetBrightness,
                parameters: parameters);

            return result.IsOk();
        }

        /// <summary>
        /// Change the device RGB color asynchronously
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public async Task<bool> SetRGBColorAsync(int r, int g, int b, int? smooth)
        {
            //Convert RGB into integer 0x00RRGGBB
            int value = ((r) << 16) | ((g) << 8) | (b);
            List<object> parameters = new List<object>() { value };

            HandleSmoothValue(ref parameters, smooth);

            CommandResult result = await ExecuteCommandWithResponseAsync(
                method: METHODS.SetRGBColor,
                id: (int)METHODS.SetRGBColor,
                parameters: parameters);

            return result.IsOk();
        }

        /// <summary>
        /// Change Color Temperature asynchronously
        /// </summary>
        /// <param name="saturation"></param>
        /// <param name="smooth"></param>
        /// <returns></returns>
        public async Task<bool> SetColorTemperatureAsync(int temperature, int? smooth)
        {
            List<object> parameters = new List<object>() { temperature };

            HandleSmoothValue(ref parameters, smooth);

            CommandResult result = await ExecuteCommandWithResponseAsync(
                method: METHODS.SetColorTemperature,
                id: (int)METHODS.SetColorTemperature,
                parameters: parameters);

            return result.IsOk();
        }

        #endregion asynchronous

    }
}