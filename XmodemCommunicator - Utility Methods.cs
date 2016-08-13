﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpToolkit;

namespace XModemProtocol {
    public partial class XModemCommunicator {

        /// <summary>
        /// Holds logic for when an operation has completed.
        /// </summary>
        private void CompleteOperation() {
            Task.Run(() => { Reset();});
            Completed?.Invoke(this, new CompletedEventArgs());
            State = XModemStates.Idle;
        }

        private List<byte> CheckSum(List<byte> packetInfo) {
            if (Mode == XModemMode.Checksum)
                return new List<byte> { (byte)packetInfo.Sum(b => b) };
            else
                // This needs to produce CRC checksum.
                return new List<byte>();
        }

        /// <summary>
        /// This overload of the Abort method assesses whether the
        /// CAN should be sent or not using a general rule.
        /// </summary>
        /// <param name="e">An instance of the AbortedEventArgs class.</param>
        private void Abort(AbortedEventArgs e) {
            bool sendCAN = e.Reason != XModemAbortReason.CancelRequestReceived ||
                e.Reason != XModemAbortReason.InitializationFailed;
            Abort(e, sendCAN);
        }

        /// <summary>
        /// This overload of the Abort method can override the general rule of when to initiate a cancel or not.
        /// </summary>
        /// <param name="e">An instance of the AbortedEventArgs class.</param>
        /// <param name="sendCAN">Whether to initiate cancel or not.</param>
        private void Abort(AbortedEventArgs e, bool sendCAN) {
            if (sendCAN) Port.Write(Enumerable.Repeat(CAN, CANSentDuringAbort).ToArray());
            Task.Run(() => { Reset(); });
            Aborted?.Invoke(this, e);
            State = XModemStates.Idle;
        }

        /// <summary>
        /// Resets variables, and some cleanup in the instance in order to prepare for an operation.
        /// </summary>
        private void Reset() {
            _tempBuffer = new List<byte>();
            _initializationTimeOut?.Dispose();
            ResetConsecutiveNAKs();
            _cancellationWaitHandle.Reset();
            _initializationWaitHandle.Reset();
            _packetIndexToSend = 0;
        }

        private bool IncrementConsecutiveNAKs() {
            _consecutiveNAKs++;
            if (_consecutiveNAKs > NAKBytesRequired) {
                _consecutiveNAKs = 0;
                return true;
            }
            return false;
        }

        private void ResetConsecutiveNAKs() => _consecutiveNAKs = 0;

        /// <summary>
        /// Method used to cancel operation. If State is idle, method does nothing.
        /// </summary>
        /// <returns>If instance was in position to be cancelled, returns true. Otherwise, false.</returns>
        public void CancelOperation() {
            if (State == XModemStates.Idle) return;
            _cancellationWaitHandle.Set();
        }

    }
}
