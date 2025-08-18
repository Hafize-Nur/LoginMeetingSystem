using LoginMeetingSystem.Data;
using LoginMeetingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetingApp.Controllers
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

        public IActionResult Index()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var meetings = _context.Meetings.Where(m => m.UserId == userId).ToList();
            return View(meetings);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Meeting meeting, List<IFormFile> documents)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            meeting.UserId = userId;

            _context.Meetings.Add(meeting);
            _context.SaveChanges();

            foreach (var doc in documents)
            {
                var path = Path.Combine(_env.WebRootPath, "uploads", doc.FileName);
                using var stream = new FileStream(path, FileMode.Create);
                doc.CopyTo(stream);

                _context.MeetingDocuments.Add(new MeetingDocument
                {
                    MeetingId = meeting.Id,
                    FilePath = "/uploads/" + doc.FileName,
                    OriginalName = doc.FileName,
                    ContentType = doc.ContentType,
                    Size = doc.Length
                });
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var meeting = _context.Meetings.Find(id);
            return View(meeting);
        }

        [HttpPost]
        public IActionResult Edit(Meeting meeting)
        {
            _context.Meetings.Update(meeting);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

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
