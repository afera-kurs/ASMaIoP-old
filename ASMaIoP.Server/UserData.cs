using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ASMaIoP.Server
{
    internal class UserData
    {
        public string sCardId { get; set; }
        public string sEmployeeId { get; set; }
        public string sRoleId { get; set; }
        public int nRoleLevel { get; set; }

        private void thrd_LoadInfo(DatabaseInterface database, string CardID)
        {
            sEmployeeId = database.GetEmployeeId(CardID);
        }

        public void LoadInfo(DatabaseInterface database)
        {
            Thread thrd = new Thread(() => thrd_LoadInfo(database, sCardId));
            
            thrd.Start();
        }

    }
}
