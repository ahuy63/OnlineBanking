using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBanking.MyClass
{
    public interface IReportService
    {
        public byte[] GeneratePdfReport();
    }
}
