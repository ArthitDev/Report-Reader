using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Report_View.Models;
using Microsoft.EntityFrameworkCore;
using Report_View.Data;
using System.IO;
using FastReport.Web;
using FastReport.Data;

namespace Report_View.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("Home/ViewReport")]
        public async Task<IActionResult> ViewReport(IFormFile reportFile, string reportCode)
        {
            try
            {
                if (string.IsNullOrEmpty(reportCode))
                {
                    throw new Exception("Report code is required");
                }

                if (reportFile == null || reportFile.Length == 0)
                {
                    throw new Exception("No file uploaded");
                }

                if (!reportFile.FileName.EndsWith(".frx"))
                {
                    throw new Exception("Invalid file type. Please upload a .frx file");
                }

                WebReport webReport = new WebReport();
                
                using (var ms = new MemoryStream())
                {
                    await reportFile.CopyToAsync(ms);
                    ms.Position = 0;
                    webReport.Report.Load(ms);
                }

                var postgresConn = new PostgresDataConnection();
                postgresConn.ConnectionString = _context.Database.GetConnectionString();
                postgresConn.CreateAllTables();
                webReport.Report.Dictionary.Connections.Clear();
                webReport.Report.Dictionary.Connections.Add(postgresConn);

                webReport.Report.Prepare();

                ViewBag.WebReport = webReport;
                return View("ViewReport");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing report");
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> TestConnection()
        {
            try
            {
                bool canConnect = await _context.Database.CanConnectAsync();
                string message = canConnect ? "Database connection successful!" : "Database connection failed!";
                Console.WriteLine($"Database Connection Test: {message}");
                ViewBag.Message = message;
            }
            catch (Exception ex)
            {
                string errorMessage = $"Connection failed: {ex.Message}";
                Console.WriteLine($"Database Connection Error: {errorMessage}");
                ViewBag.Message = errorMessage;
            }
            return View();
        }
    }
}
