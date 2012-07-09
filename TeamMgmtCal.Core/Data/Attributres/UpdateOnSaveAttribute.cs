using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMgmtCal.Core.Data.Attributres
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    class UpdateOnSaveAttribute : Attribute
    {
    }
}
