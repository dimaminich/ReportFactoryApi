using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportFactoryApi.Data;
using ReportFactoryApi.Models;
using ReportFactoryApi.Services;
using ReportFactoryApi.Utilities;

namespace ReportFactoryApi.Controllers
{
    public class ReportBuilderController : ReportController
    {
        private readonly ReportService _reportService;
        private readonly DataService _dataService;
        private readonly IConfiguration _config;
        private readonly VerificationService _verificationService;
        private readonly DataContext _context;
        private readonly ILogger<ReportBuilderController> _logger;


        public ReportBuilderController(
            ReportService reportService,
            DataService dataService,
            IConfiguration config,
            VerificationService verificationService,
            DataContext context,
            ILogger<ReportBuilderController> logger
            )
        {
            _reportService = reportService;
            _dataService = dataService;
            _config = config;
            _verificationService = verificationService;
            _context = context;
            _logger = logger;
        }


        [AllowAnonymous]
        [HttpPost("server")]
        public async Task<ActionResult> CreateReportOnServer([FromBody] ReportRequest request)
        {
            (int statuscode, string message) status = await _verificationService.Verify(request.device!, _context);
            if (status.statuscode == StatusCodes.Status200OK)
            {
                ReportData reportData = await _dataService.TakeData(request, _config);
                string reportFilename = request.device!.Name + "_" + request.ReportFilename;
                string reportFullFileName = await _reportService.CreateReport(reportData, request.RDLCTemplate!, reportFilename, _config);
                return Ok(reportFullFileName);
            }
            else
                return StatusCode(status.statuscode, status.message);
        }


        [AllowAnonymous]
        [HttpPost("server/{reportTemplate}/{reportFilename}")]
        public async Task<ActionResult> CreateReportOnServer([FromBody]ReportData reportData, [FromRoute] string reportTemplate, [FromRoute] string reportFilename)
        {
            string reportFullFileName = await _reportService.CreateReport(reportData, reportTemplate, reportFilename, _config);
            return Ok(reportFullFileName);
        }


        [AllowAnonymous]
        [HttpPost("client")]
        public async Task<ActionResult> CreateReportStreamToClient([FromBody] ReportRequest request)
        {
            (int statuscode, string message) status = await _verificationService.Verify(request.device!, _context);
            if (status.statuscode == StatusCodes.Status200OK)
            {
                string reportFilename = request.device!.Name + "_" + request.ReportFilename;
                reportFilename = FileUtility.AddTimeStampToFilename(reportFilename);
                reportFilename = $"{reportFilename}.pdf";
                ReportData reportData = await _dataService.TakeData(request, _config);
                byte[] reportBytes = await _reportService.CreateReportBytes(reportData, request.RDLCTemplate!, _config);
                return File(reportBytes, "application/pdf", reportFilename);
            }
            else
                return StatusCode(status.statuscode, status.message);
        }


        [AllowAnonymous]
        [HttpGet("client/{reportFilename}")]
        public async Task<IActionResult> CreateReportStreamToClient(string reportFilename)
        {
            string reportPath = (string)_config.GetValue(typeof(string), "ReportPath");
            string fullFileName = FileUtility.FindFileInDirectory(reportPath, reportFilename)
                ?? throw new NullReferenceException("No report found according given filename");

            var bytes = await System.IO.File.ReadAllBytesAsync(fullFileName);
            return File(bytes, "application/pdf", Path.GetFileName(fullFileName));
        }
    }
}
