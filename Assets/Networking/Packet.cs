using System;
using System.Collections.Generic;
using System.Text;

namespace AssetFactory.Networking
{
    /// <summary>
    /// Class to put data to send to peer in
    /// </summary>
	public class Packet
    {
        /// <summary>
        /// Codes for packets sent by server.
        /// </summary>
        public static class ServerCodes
        {
            public const byte START_GAME = 249;
            public const byte ROOM_UPDATE = 250;
            public const byte PLAYER_JOINED = 251;
            public const byte PLAYER_LEFT = 252;
            public const byte ROOM_OPERATION_RESULT = 253;
            public const byte DISCONNECT = 254;
            public const byte WELCOME = 255;
        }
        /// <summary>
        /// Codes for packets sent by clients.
        /// </summary>
        public static class ClientCodes
        {
            public const byte CHECK = 248;
            public const byte UPDATE_STATUS = 249;
            public const byte REQUEST_START_GAME = 250;
            public const byte REQUEST_KICK = 251;
            public const byte REQUEST_CREATE_ROOM = 252;
            public const byte REQUEST_JOIN_ROOM = 253;
            public const byte DISCONNECT = 254;
            public const byte WELCOME_CONFIRM = 255;
        }
        public byte Code { get => Data[0]; }
		public byte[] Data { get; private set; }
		public int Length { get => Data.Length; }

        public int sender = -1;

		public Packet(byte[] data)
		{
			Data = data;
		}

        public byte this[int i] { get => Data[i]; set => Data[i] = value; }

        public class Writer
        {
            public List<byte> data;
            public Writer(byte code)
            {
                data = new List<byte>(1) { code };
            }

            public void WriteLength()
            {
                data.InsertRange(1, BitConverter.GetBytes(data.Count - 1)); // Insert the byte length of the packet at the very beginning
            }

#region Write
            /// <summary>Adds a byte to the packet.</summary>
            /// <param name="value">The byte to add.</param>
            public void Write(byte value)
            {
                data.Add(value);
            }
            /// <summary>Adds an array of bytes to the packet.</summary>
            /// <param name="value">The byte array to add.</param>
            public void Write(byte[] value)
            {
                data.AddRange(value);
            }
            /// <summary>Adds a short to the packet.</summary>
            /// <param name="value">The short to add.</param>
            public void Write(short value)
            {
                data.AddRange(BitConverter.GetBytes(value));
            }
            /// <summary>Adds an int to the packet.</summary>
            /// <param name="value">The int to add.</param>
            public void Write(int value)
            {
                data.AddRange(BitConverter.GetBytes(value));
            }
            /// <summary>Adds a long to the packet.</summary>
            /// <param name="value">The long to add.</param>
            public void Write(long value)
            {
                data.AddRange(BitConverter.GetBytes(value));
            }
            /// <summary>Adds a float to the packet.</summary>
            /// <param name="value">The float to add.</param>
            public void Write(float value)
            {
                data.AddRange(BitConverter.GetBytes(value));
            }
            /// <summary>Adds a bool to the packet.</summary>
            /// <param name="value">The bool to add.</param>
            public void Write(bool value)
            {
                data.AddRange(BitConverter.GetBytes(value));
            }
            /// <summary>Adds a string to the packet.</summary>
            /// <param name="value">The string to add.</param>
            public void Write(string value)
            {
                Write(value.Length); // Add the length of the string to the packet
                data.AddRange(Encoding.UTF8.GetBytes(value)); // Add the string itself
            }
#endregion
            public Packet Finish()
            {
                return new Packet(data.ToArray());
            }
        }
        public class Reader
        {
            Packet packet;
            public int pointer;

            public Reader(Packet packet)
            {
                this.packet = packet;
                pointer = 1;
            }

            #region Read Data
            /// <summary>Reads a byte from the packet.</summary>
            /// <param name="movePointer">Whether or not to move the packet.Data's read position.</param>
            public byte ReadByte(bool movePointer = true)
            {
                // If there are unread bytes
                if (packet.Length > pointer)
                {
                    byte value = packet.Data[pointer]; // Get the byte at pointer' position
                    if (movePointer)
                    {
                        pointer += sizeof(byte);
                    }
                    return value;
                }
                else
                {
                    throw new Exception("Could not read value of type 'byte'!");
                }
            }

            /// <summary>Reads a short from the packet.</summary>
            /// <param name="movePointer">Whether or not to move the packet.Data's read position.</param>
            public short ReadShort(bool movePointer = true)
            {
                if (packet.Length > pointer)
                {
                    // If there are unread bytes
                    short value = BitConverter.ToInt16(packet.Data, pointer); // Convert the bytes to a short
                    if (movePointer)
                    {
                        // If movePointer is true and there are unread bytes
                        pointer += sizeof(short); // Increase pointer by 2
                    }
                    return value; // Return the short
                }
                else
                {
                    throw new Exception("Could not read value of type 'short'!");
                }
            }

            /// <summary>Reads an int from the packet.</summary>
            /// <param name="movePointer">Whether or not to move the packet.Data's read position.</param>
            public int ReadInt(bool movePointer = true)
            {
                if (packet.Length > pointer)
                {
                    // If there are unread bytes
                    int value = BitConverter.ToInt32(packet.Data, pointer); // Convert the bytes to an int
                    if (movePointer)
                    {
                        // If movePointer is true
                        pointer += sizeof(int); // Increase pointer by 4
                    }
                    return value; // Return the int
                }
                else
                {
                    throw new Exception("Could not read value of type 'int'!");
                }
            }

            /// <summary>Reads a long from the packet.</summary>
            /// <param name="movePointer">Whether or not to move the packet.Data's read position.</param>
            public long ReadLong(bool movePointer = true)
            {
                if (packet.Length > pointer)
                {
                    // If there are unread bytes
                    long value = BitConverter.ToInt64(packet.Data, pointer); // Convert the bytes to a long
                    if (movePointer)
                    {
                        // If movePointer is true
                        pointer += sizeof(long); // Increase pointer by 8
                    }
                    return value; // Return the long
                }
                else
                {
                    throw new Exception("Could not read value of type 'long'!");
                }
            }

            /// <summary>Reads a float from the packet.</summary>
            /// <param name="movePointer">Whether or not to move the packet.Data's read position.</param>
            public float ReadFloat(bool movePointer = true)
            {
                if (packet.Length > pointer)
                {
                    // If there are unread bytes
                    float value = BitConverter.ToSingle(packet.Data, pointer); // Convert the bytes to a float
                    if (movePointer)
                    {
                        // If movePointer is true
                        pointer += sizeof(float); // Increase pointer by 4
                    }
                    return value; // Return the float
                }
                else
                {
                    throw new Exception("Could not read value of type 'float'!");
                }
            }

            /// <summary>Reads a bool from the packet.</summary>
            /// <param name="movePointer">Whether or not to move the packet.Data's read position.</param>
            public bool ReadBool(bool movePointer = true)
            {
                if (packet.Length > pointer)
                {
                    // If there are unread bytes
                    bool value = BitConverter.ToBoolean(packet.Data, pointer); // Convert the bytes to a bool
                    if (movePointer)
                    {
                        // If movePointer is true
                        pointer += sizeof(bool); // Increase pointer by 1
                    }
                    return value; // Return the bool
                }
                else
                {
                    throw new Exception("Could not read value of type 'bool'!");
                }
            }

            /// <summary>Reads a string from the packet.</summary>
            /// <param name="movePointer">Whether or not to move the packet.Data's read position.</param>
            public string ReadString(bool movePointer = true)
            {
                try
                {
                    int length = ReadInt(); // Get the length of the string
                    string value = Encoding.UTF8.GetString(packet.Data, pointer, length); // Convert the bytes to a string
                    if (movePointer && value.Length > 0)
                    {
                        // If movePointer is true string is not empty
                        pointer += length; // Increase pointer by the length of the string
                    }
                    return value; // Return the string
                }
                catch
                {
                    throw new Exception("Could not read value of type 'string'!");
                }
            }
#endregion
        }
    }
}