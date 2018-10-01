﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Neosmartpen.Net
{
    public abstract class PenComm : IPenComm
    {
        private Socket mSock { get; set; }

        private Stream mStream { get; set; }

        private Thread mReadThread, mWriteThread;

        private byte[] mReadBuffer = new byte[1024*32];

        private Queue<byte[]> mWriteQueue = new Queue<byte[]>();

        private AutoResetEvent mLockHandleWrite;

        public IProtocolParser Parser
        {
            get;
            protected set;
        }

        public abstract uint DeviceClass
        {
            get;
        }

        public abstract string Version
        {
            get;
        }

        public abstract string Name
        {
            get;
            set;
        }

        protected internal PenComm( IProtocolParser _parser )
        {
            Parser = _parser;
        }

        public bool Alive
        {
            get
            {
                return mSock != null && mStream != null && mSock.Connected && mStream.CanWrite && mStream.CanRead;
            }
        }

        public void Bind( Socket soc, string name = null )
        {
            mSock = soc;

            mLockHandleWrite = new AutoResetEvent( false );

            mStream = new NetworkStream( mSock );

            Name = name != null ? name : mSock.RemoteEndPoint.ToString();

            Init();
        }

        private bool Init()
        {
            if ( Alive )
            {
                StartThread();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void StartThread()
        {
            mReadThread = new Thread( new ThreadStart( ReadRun ) );
            mReadThread.Name = "Read Thread";
            mReadThread.IsBackground = true;
            mReadThread.Start();

            mWriteThread = new Thread( new ThreadStart( WriteRun ) );
            mWriteThread.Name = "Write Thread";
            mWriteThread.IsBackground = true;
            mWriteThread.Start();

            OnConnected();
        }

        public void Clean()
        {
            System.Console.WriteLine( "[PenCommBase] Clean" );

            lock ( mLockHandleWrite )
            {
                mLockHandleWrite.Set();
            }

            try
            {
                mSock.Close();
            }
            catch
            {
            }
        }

        protected internal bool Write( byte[] mbyte )
        {
            if ( !Alive )
            {
                System.Console.WriteLine( "[PenCommBase] Write Thread is not alive." );
                return false;
            }

            mWriteQueue.Enqueue( mbyte );

            lock ( mLockHandleWrite )
            {
                mLockHandleWrite.Set();
            }

            return true;
        }

        private void WriteRun()
        {
            System.Console.WriteLine( "[PenCommBase] Write Thread is Started." );

            while ( Alive )
            {
                while ( mWriteQueue.Count > 0 )
                {
                    byte[] datas = mWriteQueue.Dequeue();

                    if ( datas != null && datas.Length > 0 )
                    {
                        try
                        {
                            mStream.Write( datas, 0, datas.Length );
                            mStream.Flush();
                        }
                        catch ( Exception e )
                        {
                            System.Console.WriteLine( e.StackTrace );
                            break;
                        }
                    }
                }

                mLockHandleWrite.WaitOne();
            }

            System.Console.WriteLine( "[PenCommBase] Write Thread is Terminated" );
        }

        private void ReadRun()
        {
            System.Console.WriteLine( "[PenCommBase] Read Thread is Started" );

            while ( Alive )
            {
                try
                {
                    int length = mStream.Read( mReadBuffer, 0, mReadBuffer.Length );
                    
                    if ( length == 0 )
                    {
                        Clean();
                        break;
                    }

                    Parser.Put( mReadBuffer, length );
                }
                catch ( Exception e )
                {
                    System.Console.WriteLine( e.StackTrace );
                    this.Clean();
                    break;
                }
            }

            System.Console.WriteLine( "[PenCommBase] Read Thread is Terminated" );

            FinalizeSocket();
            OnDisconnected();
        }

        private void FinalizeSocket()
        {
            try
            {
                mStream.Close();
            }
            catch
            {
            }

            try
            {
                mSock.Close();
            }
            catch
            {
            }
        }

        protected internal abstract void OnConnected();

        protected internal abstract void OnDisconnected();
    }
}
