
// ReSharper disable All
/*
 * Do not adhere to naming standard in DCS-BIOS code, standard are based on DCS-BIOS json files and byte streamnaming
 */
namespace DCS_BIOS
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using ClassLibraryCommon;

    using DCS_BIOS.EventArgs;
    using DCS_BIOS.Interfaces;

    public enum DCSBiosStateEnum
    {
        WAIT_FOR_SYNC = 0,
        ADDRESS_LOW = 1,
        ADDRESS_HIGH = 2,
        COUNT_LOW = 3,
        COUNT_HIGH = 4,
        DATA_LOW = 5,
        DATA_HIGH = 6,
    }

    internal class DCSBIOSProtocolParser : IDisposable
    {
        public delegate void DcsDataAddressValueEventHandler(object sender, DCSBIOSDataEventArgs e);
        public event DcsDataAddressValueEventHandler OnDcsDataAddressValue;

        public delegate void DcsConnectionActiveEventHandler(object sender, DCSBIOSConnectionEventArgs e);
        public event DcsConnectionActiveEventHandler OnDcsConnectionActive;

        private List<string> _errorsLogged = new List<string>(10);

        public void AttachDataListener(IDcsBiosDataListener iDcsBiosDataListener)
        {
            //Try to remove it first so not to get duplicate
            OnDcsDataAddressValue -= iDcsBiosDataListener.DcsBiosDataReceived;

            OnDcsDataAddressValue += iDcsBiosDataListener.DcsBiosDataReceived;
        }

        public void DetachDataListener(IDcsBiosDataListener iDcsBiosDataListener)
        {
            OnDcsDataAddressValue -= iDcsBiosDataListener.DcsBiosDataReceived;
        }
        
        public void AttachConnectionStateListener(IDcsBiosConnectionListener connectionListener)
        {
            //Try to remove it first so not to get duplicate
            OnDcsConnectionActive -= connectionListener.DcsBiosConnectionActive;

            OnDcsConnectionActive += connectionListener.DcsBiosConnectionActive;
        }

        public void DetachConnectionStateListener(IDcsBiosConnectionListener connectionListener)
        {
            OnDcsConnectionActive -= connectionListener.DcsBiosConnectionActive;
        }

        private DCSBiosStateEnum _state;
        private uint _address;
        private uint _count;
        private uint _data;
        private byte _syncByteCount;
        private bool _shutdown;
        private static object _lockListOfAddressesToBroascastObject = new object();
        private readonly List<uint> _listOfAddressesToBroascast = new List<uint>();
        public static DCSBIOSProtocolParser DCSBIOSProtocolParserSO;
        private AutoResetEvent _autoResetEvent = new AutoResetEvent(false);


        //private object _lockArrayToProcess = new object();
        //private List<byte[]> _arraysToProcess = new List<byte[]>();
        private readonly ConcurrentQueue<byte[]> _arraysToProcess = new ConcurrentQueue<byte[]>();
        private Thread _processingThread;

        private DCSBIOSProtocolParser()
        {
            _state = DCSBiosStateEnum.WAIT_FOR_SYNC;
            _syncByteCount = 0;
            DCSBIOSProtocolParserSO = this;
            _shutdown = false;
        }
        
        protected virtual void Dispose(bool disposing)
        {
            _shutdown = true;
            if (disposing)
            {
                // dispose managed resources
                _autoResetEvent?.Dispose();
                _autoResetEvent = null;
            }
            // free native resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        public void Startup()
        {
            _shutdown = false;
            _processingThread = new Thread(ThreadedProcessArrays);
            _processingThread.Start();
        }

        public void Shutdown()
        {
            _shutdown = true;
            _autoResetEvent.Set();
        }

        private void ThreadedProcessArrays()
        {
            try
            {
                var interval = 0;
                while (!_shutdown)
                {
                    try
                    {
                        if (interval >= 100)
                        {
                            //Debug.Print("_arraysToProcess.Count = " + _arraysToProcess.Count);
                            interval = 0;
                        }
                        
                        byte[] array = null;
                        while (_arraysToProcess.TryDequeue(out array))
                        {
                            if (array != null)
                            {
                                for (int i = 0; i < array.Length; i++)
                                {
                                    ProcessByte(array[i]);
                                }
                            }
                        }
                        
                        interval++;
                    }
                    catch (Exception e)
                    {
                        Common.LogError( e, "DCSBIOSProtocolParser.ProcessArrays(), arrays to process : " + _arraysToProcess.Count);
                    }
                    _autoResetEvent?.WaitOne();
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception e)
            {
                Common.LogError( e, "DCSBIOSProtocolParser.ProcessArrays(), arrays to process : " + _arraysToProcess.Count);
            }
        }

        public static void RegisterAddressToBroadCast(uint address)
        {
            GetParser();
            lock (_lockListOfAddressesToBroascastObject)
            {
                if (!DCSBIOSProtocolParserSO._listOfAddressesToBroascast.Any(u => u == address))
                {
                    DCSBIOSProtocolParserSO._listOfAddressesToBroascast.Add(address);
                }
            }
        }

        public static DCSBIOSProtocolParser GetParser()
        {
            if (DCSBIOSProtocolParserSO == null)
            {
                DCSBIOSProtocolParserSO = new DCSBIOSProtocolParser();
            }
            return DCSBIOSProtocolParserSO;
        }

        public void AddArray(byte[] bytes)
        {
            _arraysToProcess.Enqueue(bytes);
            _autoResetEvent.Set();
        }

        private bool IsBroadcastable(uint address)
        {
            var result = false;
            lock (_lockListOfAddressesToBroascastObject)
            {
                if (_listOfAddressesToBroascast.Any(u => u == address))
                {
                    result = true;
                }
            }
            return result;
        }

        internal void ProcessByte(byte b)
        {
            try
            {
                switch (_state)
                {
                    case DCSBiosStateEnum.WAIT_FOR_SYNC:
                        /* do nothing */
                        break;
                    case DCSBiosStateEnum.ADDRESS_LOW:
                        _address = b;
                        _state = DCSBiosStateEnum.ADDRESS_HIGH;
                        break;
                    case DCSBiosStateEnum.ADDRESS_HIGH:
                        _address = (uint)(b << 8) | _address;
                        _state = _address != 0x5555 ? DCSBiosStateEnum.COUNT_LOW : DCSBiosStateEnum.WAIT_FOR_SYNC;
                        break;
                    case DCSBiosStateEnum.COUNT_LOW:
                        _count = b;
                        _state = DCSBiosStateEnum.COUNT_HIGH;
                        break;
                    case DCSBiosStateEnum.COUNT_HIGH:
                        _count = (uint)(b << 8) | _count;
                        _state = DCSBiosStateEnum.DATA_LOW;
                        break;
                    case DCSBiosStateEnum.DATA_LOW:
                        _data = b;
                        _count--;
                        _state = DCSBiosStateEnum.DATA_HIGH;
                        break;
                    case DCSBiosStateEnum.DATA_HIGH:
                        _data = (uint)(b << 8) | _data;
                        _count--;
                        //_iDcsBiosDataListener.DcsBiosDataReceived(_address, _data);

                        OnDcsConnectionActive?.Invoke(this, new DCSBIOSConnectionEventArgs()); // Informs main UI that data is coming

                        if (OnDcsDataAddressValue != null && IsBroadcastable(_address) && _data != 0x55)
                        {
                            /*if (_address == 25332)
                            {
                                Debug.Print("SENDING FROM DCS-BIOS address & value --> " + _address + "  " + _data);
                            }*/
                            try
                            {
                                OnDcsDataAddressValue?.Invoke(this, new DCSBIOSDataEventArgs() { Address = _address, Data = _data });

                                /*if (OnDcsDataAddressValue != null)
                                {
                                    Debug.WriteLine("OnDcsDataAddressValue : " + OnDcsDataAddressValue.GetInvocationList().Length);
                                }*/
                            }
                            catch (Exception e)
                            {
                                if (!_errorsLogged.Contains(e.Message))
                                {
                                    Common.LogError(e, "Error in DCS-BIOS stream. This error will be logged *just once*.");
                                    _errorsLogged.Add(e.Message);
                                }
                            }
                        }
                        _address += 2;
                        if (_count == 0)
                            _state = DCSBiosStateEnum.ADDRESS_LOW;
                        else
                            _state = DCSBiosStateEnum.DATA_LOW;
                        break;
                }

                if (b == 0x55)
                {
                    //Console.WriteLine(Environment.TickCount - ticks);
                    //ticks = Environment.TickCount;
                    _syncByteCount++;
                }
                else
                {
                    _syncByteCount = 0;
                }

                if (_syncByteCount == 4)
                {
                    _state = DCSBiosStateEnum.ADDRESS_LOW;
                    _syncByteCount = 0;
                }
            }
            catch (Exception e)
            {
                Common.LogError( e, "DCSBIOSProtocolParser.ProcessByte()");
            }
        }
    }
}
