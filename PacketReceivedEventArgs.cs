﻿using System;
using System.Collections.Generic;

namespace XModemProtocol {
    /// <summary>
    /// Provides data for the XModemProtocol.XModemCommunicator.PacketReceived event.
    /// </summary>
    public class PacketReceivedEventArgs : EventArgs {
        /// <summary>
        /// The packet number that was received.
        /// </summary>
        public int PacketNumber { get; private set; }
        /// <summary>
        /// The contents of the packet that was received.
        /// </summary>
        public List<byte> PacketReceived { get; private set; }
        /// <summary>
        /// Flag which denotes whether the packet was validated.
        /// </summary>
        public bool PacketVerified { get; private set; }
        internal PacketReceivedEventArgs(int packetNumber, List<byte> packetReceived, bool packetVerified) {
            PacketNumber = packetNumber;
            PacketVerified = packetVerified;
            PacketReceived = new List<byte>(packetReceived);
        }
    }
}