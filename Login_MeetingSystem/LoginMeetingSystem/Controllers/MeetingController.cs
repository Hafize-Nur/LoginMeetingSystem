using LoginMeetingSystem.Data;
using LoginMeetingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace LoginMeetingSystem.Controllers
{
    [Authorize]
    public class MeetingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public MeetingController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        //Veritabanındaki kayıtlı toplantıları gösterme


        public IActionResult Index()
        {


            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return RedirectToAction("Login", "User");

            int userId = int.Parse(userIdClaim.Value);
            var meetings = _context.Meetings
                            .Where(m => m.UserId == userId)
                            .ToList();

            return View(meetings);
        }
        // GET: Create form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Meeting meeting, List<IFormFile> documents)
        {// toplantı bilgilerini kişi bilgisiyle ekliyor
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            meeting.UserId = userId;

            _context.Meetings.Add(meeting);
            _context.SaveChanges();

            // Proje kök dizini (wwwroot yerine burada kök klasör)
            var projectRoot = Directory.GetCurrentDirectory();
            var documentsPath = Path.Combine(projectRoot, "Documents");

            // Eğer klasör yoksa oluştur
            if (!Directory.Exists(documentsPath))
            {
                Directory.CreateDirectory(documentsPath);
            }

            foreach (var doc in documents)
            {// toplantı için dosya ekleme
                var filePath = Path.Combine(documentsPath, doc.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    doc.CopyTo(stream);
                }

                _context.MeetingDocuments.Add(new MeetingDocument
                {
                    MeetingId = meeting.Id,
                    FilePath = Path.Combine("Documents", doc.FileName), // Veritabanına kaydetmek için
                    OriginalName = doc.FileName,
                    ContentType = doc.ContentType,
                    Size = doc.Length
                });
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        //kayıtlı toplantıları görmek için
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var meeting = _context.Meetings.Find(id);
            return View(meeting);
        }
        //toplantı güncellemek için
        [HttpPost]
        public IActionResult Edit(Meeting meeting)
        {
            var existingMeeting = _context.Meetings.Find(meeting.Id);
            if (existingMeeting == null)
                return NotFound();


            existingMeeting.Title = meeting.Title;
            existingMeeting.Description = meeting.Description;
            existingMeeting.StartDate = meeting.StartDate;
            existingMeeting.EndDate = meeting.EndDate;

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        //toplantı silmek için
        public IActionResult Delete(int id)
        {
            var meeting = _context.Meetings.Find(id);
            if (meeting != null)
            {
                _context.Meetings.Remove(meeting);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
