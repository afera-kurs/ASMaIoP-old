using System;
using System.Collections.Generic;
using System.Text;

namespace ASMaIoP.General.Client
{
    enum ProtocolId
    { 
        Auth = 0,
        DataTransfer = 1
    }

    class Session : ASMaIoP.General.Client.Client
    {
        public Session(string Address)
        {
            SetupAddress(Address);
        }

        public ErrorCode Open() => Connect();

    }
}
