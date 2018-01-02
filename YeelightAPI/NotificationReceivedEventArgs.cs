﻿using System;
using System.Collections.Generic;
using System.Text;
using YeelightClient.Models;

namespace YeelightAPI
{
    /// <summary>
    /// Notification event Argument
    /// </summary>
    public class NotificationReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public NotificationReceivedEventArgs() { }

        /// <summary>
        /// Constructor with notification result
        /// </summary>
        /// <param name="result"></param>
        public NotificationReceivedEventArgs(NotificationResult result)
        {
            this.Result = result;
        }

        /// <summary>
        /// Notification Result
        /// </summary>
        public NotificationResult Result { get; set; }
    }
}
