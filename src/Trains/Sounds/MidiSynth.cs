using System;
using System.Threading.Tasks;
using Midi;

namespace Trains.Sounds
{
    internal sealed class MidiSynth : IDisposable
    {
        private readonly object _lock = new object();
        private Win32API.HMIDIOUT _handle;

        public void Init()
        {
            // Find the built in MIDI synth
            uint num = Win32API.midiOutGetNumDevs();
            Win32API.MIDIOUTCAPS caps = default;
            var deviceId = UIntPtr.Zero;
            bool deviceFound = false;
            for (uint i = 0; i < num; i++)
            {
                Win32API.midiOutGetDevCaps((UIntPtr)i, out caps);
                if (caps.szPname == "Microsoft GS Wavetable Synth")
                {
                    deviceId = (UIntPtr)i;
                    deviceFound = true;
                    break;
                }
            }

            if (deviceFound)
            {
                lock (_lock)
                {
                    Win32API.midiOutOpen(out _handle, deviceId, null, (UIntPtr)0);
                }
                // Swtich to GS mode
                GsReset();
            }
        }

        internal void Train()
        {
            const byte channel = 11;
            SetGS(channel, 125, 6, 60);
        }

        internal async Task SoundAsync(HornModel horn, int milliseconds)
        {
            var hornDef = MidiHornDef.GetHorn(horn);
            for (byte i = 0; i < hornDef.Notes.Length; i++)
            {
                SendShortMessage(EncodeProgramChange(i, 110));
                SendShortMessage(EncodeNoteOn(i, (int)hornDef.Notes[i], 127));
            }
            await Task.Delay(milliseconds).ConfigureAwait(false);
            for (byte i = 0; i < hornDef.Notes.Length; i++)
            {
                SendShortMessage(EncodeNoteOff(i, (int)hornDef.Notes[i], 0));
            }
        }

        internal async Task TweetAsync()
        {
            const byte channel = 12;
            if (Environment.TickCount % 2 == 0)
                SetGS(channel, 123, 0, 60);
            else
                SetGS(channel, 123, 3, 60);

            await Task.Delay(150).ConfigureAwait(false);
            SendShortMessage(EncodeNoteOff(channel, 60, 127));
        }

        internal async Task WhistleAsync()
        {
            const byte channel = 13;
            SendShortMessage(EncodeProgramChange(channel, 78));
            SendShortMessage(EncodeNoteOn(channel, 60, 127));

            await Task.Delay(1500).ConfigureAwait(false);
            SendShortMessage(EncodeNoteOff(channel, 60, 127));
        }

        private void GsReset()
        {
            var bytes = new byte[] { 0xF0, 0x41, 0x10, 0x42, 0x12, 0x40, 0x00, 0x7F, 0x00, 0x41, 0xF7 };
            SendSysEx(bytes);
        }

        ////http://battleofthebits.org/lyceum/View/Specification+of+General+MIDI+and+Roland+MT-32+patches/#Patch%20List%20(channel%2010%20only)
        private void SetGS(byte channel, byte patch, byte bank, byte note)
        {
            SendShortMessage(EncodeControlChange(channel, 0, bank));
            SendShortMessage(EncodeProgramChange(channel, patch));
            SendShortMessage(EncodeNoteOn(channel, (int)note, 127));
        }

        private void SendShortMessage(uint message)
        {
            lock (_lock)
            {
                if (_handle.handle != IntPtr.Zero)
                {
                    Win32API.midiOutShortMsg(_handle, message);
                }
            }
        }

        private static uint EncodeProgramChange(int channel, int instrument)
        {
            return (uint)(0xC0 | (int)channel | ((int)instrument << 8));
        }

        private static uint EncodeNoteOn(int channel, int pitch, int velocity)
        {
            return (uint)(0x90 | (int)channel | ((int)pitch << 8) | (velocity << 16));
        }

        private static uint EncodeNoteOff(int channel, int pitch, int velocity)
        {
            return (uint)(0x80 | (int)channel | ((int)pitch << 8) | (velocity << 16));
        }

        private static uint EncodeControlChange(int channel, int control, int value)
        {
            return (uint)(0xB0 | (int)channel | ((int)control << 8) | (value << 16));
        }

        //https://csharp.hotexamples.com/examples/Midi/Win32API.MIDIHDR/-/php-win32api.midihdr-class-examples.html
        /// <summary>
        /// Sends a System Exclusive (sysex) message to this MIDI output device.
        /// </summary>
        /// <param name="data">The message to send (as byte array)</param>
        /// <exception cref="DeviceException">The message cannot be sent.</exception>
        private void SendSysEx(byte[] data)
        {
            if (_handle.handle == IntPtr.Zero)
                return;

            lock (_lock)
            {
                //Win32API.MMRESULT result;
                IntPtr ptr;
                uint size = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(Win32API.MIDIHDR));
                Win32API.MIDIHDR header = new Win32API.MIDIHDR();
                header.lpData = System.Runtime.InteropServices.Marshal.AllocHGlobal(data.Length);
                for (int i = 0; i < data.Length; i++)
                    System.Runtime.InteropServices.Marshal.WriteByte(header.lpData, i, data[i]);
                header.dwBufferLength = data.Length;
                header.dwBytesRecorded = data.Length;
                header.dwFlags = 0;

                try
                {
                    ptr = System.Runtime.InteropServices.Marshal.AllocHGlobal(System.Runtime.InteropServices.Marshal.SizeOf(typeof(Win32API.MIDIHDR)));
                }
                catch (Exception)
                {
                    System.Runtime.InteropServices.Marshal.FreeHGlobal(header.lpData);
                    throw;
                }

                try
                {
                    System.Runtime.InteropServices.Marshal.StructureToPtr(header, ptr, false);
                }
                catch (Exception)
                {
                    System.Runtime.InteropServices.Marshal.FreeHGlobal(header.lpData);
                    System.Runtime.InteropServices.Marshal.FreeHGlobal(ptr);
                    throw;
                }

                var result = Win32API.midiOutPrepareHeader(_handle, ptr, size);
                if (result == 0) result = Win32API.midiOutLongMsg(_handle, ptr, size);
                if (result == 0) result = Win32API.midiOutUnprepareHeader(_handle, ptr, size);

                System.Runtime.InteropServices.Marshal.FreeHGlobal(header.lpData);
                System.Runtime.InteropServices.Marshal.FreeHGlobal(ptr);
            }
        }

        public void Close()
        {
            lock (_lock)
            {
                if (_handle.handle != IntPtr.Zero)
                {
                    Win32API.midiOutClose(_handle);
                    _handle.handle = IntPtr.Zero;
                }
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
