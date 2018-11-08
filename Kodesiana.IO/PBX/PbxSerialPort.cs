//     PbxSerialPort.cs is part of PBXListener.
//     Copyright (C) 2018  Fahmi Noor Fiqri
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <https://www.gnu.org/licenses/>.

#region

using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Text;

#endregion

namespace Kodesiana.IO.PBX
{
    /// <summary>
    /// <see cref="SerialPort"/> wrapper for PBX communication.
    /// </summary>
    [DesignerCategory("Code")]
    public abstract class PbxSerialPort : SerialPort
    {
        private readonly StringBuilder _buffer;

        /// <summary>
        /// Indicates that data has been received through a port represented. 
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        #region Properties
        /// <summary>
        /// Gets or sets the mapper class for current serial instance.
        /// </summary>
        [Browsable(false)] public ParserMapper Mapper { get; set; }

        /// <summary>
        /// Gets or sets EOF marker for serial transmission.
        /// </summary>
        /// <remarks>This property is used to check if the end-of-transmission is reached and the
        /// available data is ready to be processed. This function is necessary becuause of serial 
        /// communication behavior that may send data in chunks. This function is used to make sure the
        /// received data is complete and can be parsed through parsers.</remarks>
        [Browsable(false)] public Func<string, bool> EofMarker { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PbxSerialPort"/> class.
        /// </summary>
        public PbxSerialPort()
        {
            _buffer = new StringBuilder();
            EofMarker = x => Helpers.DurationParserLazy.Value.Parse(x) != null;

            DataReceived += BasePortDataReceived;
        }
        #endregion

        #region Event Subscriber
        private void BasePortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            _buffer.Append(ReadExisting());

            if (!EofMarker(_buffer.ToString())) return;
            OnMessageReceived(new MessageReceivedEventArgs { Message = OnParse(_buffer.ToString()) });
            _buffer.Clear();
        }
        #endregion

        #region Public Methods
        
        /// <summary>
        /// Opens a new serial port connection and start data receive.
        /// </summary>
        public new void Open()
        {
            Mapper.ClearMapping();
            base.Open();
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Invoked when a complete data is received and about to be parsed.
        /// </summary>
        /// <param name="buffer">Full transmission data.</param>
        /// <returns>Parsed object to be sent through <see cref="MessageReceived"/> event.</returns>
        protected virtual object OnParse(string buffer)
        {
            return Mapper.Parse(buffer);
        }

        /// <summary>
        /// Invoked when new message is parsed and ready to broadcast.
        /// </summary>
        /// <param name="e">Message body.</param>
        protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
        {
            MessageReceived?.Invoke(this, e);
        }
        #endregion
    }
}
