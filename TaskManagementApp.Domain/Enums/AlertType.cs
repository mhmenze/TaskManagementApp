using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementApp.Domain
{
    public enum AlertType
    {
        Email = 0,
        SMS = 1,
        BrowserAlert = 2,
        PushNotification = 3,
        InAppMessage = 4
    }
}
