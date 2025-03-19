using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InstallTool
{
    class AppCommCmd : IAppCommRespListener, IAppCommCmdListener
    {
        protected AppComm mAppComm;
        protected InstallToolDefs.AppCommCmdID mCmdId;

        private readonly ConcurrentQueue<byte[]> mQueue;

        private Semaphore mSemaphore;

        public AppCommCmd(AppComm appComm, InstallToolDefs.AppCommCmdID cmdId)
        {
            mAppComm = appComm;
            mCmdId = cmdId;
            mQueue = new ConcurrentQueue<byte[]>();
            mSemaphore = new Semaphore(0, int.MaxValue);
            mAppComm.registerRespListener(this);
            mAppComm.registerCmdListener(this);
        }

        public InstallToolDefs.AppCommCmdID CmdId
        {
            get { return mCmdId; }
        }

        public void ResponseReceived(byte[] data)
        {
            lock (mQueue) mQueue.Enqueue(data);
            mSemaphore.Release();

        }

        public virtual void CmdReceived(byte[] data)
        {
        }

        protected byte[] waitResponse()
        {
            byte[] payload = null;

            if (mSemaphore.WaitOne(10000))
            {
                lock (mQueue) mQueue.TryDequeue(out payload);
            }            

            return payload;
        }

    }
}
