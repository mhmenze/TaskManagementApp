using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementApp.Domain
{
    public enum TaskCurrentStatus
    {
        ToDo = 0,
        InProgress = 1,
        Completed = 2,
        UnAssigned = 3,
        Deleted = 4
    }
}
