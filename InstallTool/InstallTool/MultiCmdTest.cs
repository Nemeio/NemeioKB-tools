using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstallTool
{
    class MultiCmdTest
    {
        AppComm mAppComm;
        KeepAlive mKeepAlive;
        ConfigurationList mConfigurationList;

        public MultiCmdTest(AppComm appComm)
        {
            mAppComm = appComm;
            mKeepAlive = new KeepAlive(appComm);
            mConfigurationList = new ConfigurationList(appComm);
        }

        public void sendMultiCmd()
        {
            byte[] keepAliveFrame = mKeepAlive.getFrame();
            byte[] configListFrame = mConfigurationList.getFrame();
            byte[] multipleCmdFrame = keepAliveFrame.Concat(configListFrame).ToArray();

            mAppComm.SendBytes(multipleCmdFrame);

            mKeepAlive.getResponse();
            mConfigurationList.getResponse();
        }
    }
}
