using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportFactoryApi.Data;
using ReportFactoryApi.Models;
using ReportFactoryApi.Services;

namespace ReportFactoryApi.Controllers
{
    public class ReportDataController: ReportController
    {
        private readonly DataService _dataService;
        private readonly IConfiguration _config;
        private readonly DataContext _context;
        private readonly VerificationService _verificationService;
        private readonly ILogger<ReportDataController> _logger;

        public ReportDataController(
            DataService dataService,
            IConfiguration config,
            DataContext context,
            VerificationService verificationService,
            ILogger<ReportDataController> logger
            )
        {
            _dataService = dataService;
            _config = config;
            _context = context;
            _verificationService = verificationService;
            _logger = logger;
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<ReportData>> Data([FromBody] ReportRequest request)
        {
            _logger.LogInformation("Request data", nameof(ReportBuilderController));

            (int statuscode, string message) status = await _verificationService.Verify(request.device!, _context);
            if (status.statuscode == StatusCodes.Status200OK)
                return await _dataService.TakeData(request, _config);
            else
                return StatusCode(status.statuscode, status.message);
        }
    }
}
