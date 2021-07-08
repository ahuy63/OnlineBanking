using Microsoft.AspNetCore.Mvc;
using OnlineBanking.MyClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace OnlineBanking.Controllers
{
    [Route("a/a")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var pdfFile = _reportService.GeneratePdfReport();
            return File(pdfFile,  "application/octet-stream", "SimplePdf.pdf");
        }
    }
}
