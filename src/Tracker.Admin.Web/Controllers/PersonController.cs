using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageProcessorCore;
using ImageProcessorCore.Formats;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Tracker.Admin.Web.Data;
using Tracker.Admin.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Tracker.Admin.Web.Controllers
{

    public class PersonController : Controller
    {
        private readonly TrackerDbContext _context;
        private readonly IHostingEnvironment _environment;

        public PersonController(TrackerDbContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        #region ---IMAGE UPLOAD ------

        private const int AvatarStoredWidth = 200;  // ToDo - Change the size of the stored avatar image
        private const int AvatarStoredHeight = 200; // ToDo - Change the size of the stored avatar image
        private const int AvatarScreenWidth = 400;  // ToDo - Change the value of the width of the image on the screen

        private const string TempFolder = "Temp";
        private readonly string[] _imageFileExtensions = { ".jpg", ".png", ".gif", ".jpeg" };

        [HttpGet]
        public ActionResult Upload()
        {
            return PartialView("_Upload");
        }

        [HttpPost]
        public ActionResult Upload(IFormFile files)
        {
            if (files == null) return Json(new { success = false, errorMessage = "No file uploaded." });

            var file = files;

            if (!IsImage(file)) return Json(new { success = false, errorMessage = "File is of wrong format." });

            if (file.Length <= 0) return Json(new { success = false, errorMessage = "File cannot be zero length." });

            var webPath = GetTempSavedFilePath(file).Replace("/", "\\");

            return Json(new { success = true, fileName = webPath }); // success
        }

        [HttpPost]
        public ActionResult Save(int id, string cropPointY, string cropPointX, string imageCropHeight, string imageCropWidth, string imagePath)
        {
            try
            {
                //Get person record to save image to
                var person = _context.Person.FirstOrDefault(p => p.Id == id);

                if (person == null)
                {
                    return Json(new { success = false, errorMessage = "Unable to upload file.\nERRORINFO: Person not found" });
                }

                // Calculate dimensions
                var top = Convert.ToInt32(cropPointY);
                var left = Convert.ToInt32(cropPointX);
                var height = Convert.ToInt32(imageCropHeight);
                var width = Convert.ToInt32(imageCropWidth);

                // Get file from temporary folder
                var tempPath = Path.Combine(_environment.WebRootPath, "temp");
                var fileName = Path.GetFileName(imagePath);

                if (fileName == null)
                {
                    return Json(new { success = false, errorMessage = "Unable to upload file.\nERRORINFO: Image File name null" });
                }

                var fn = Path.Combine(tempPath, fileName);
                // ...get image and resize it, ...

                using (var fileStream = new FileStream(fn, FileMode.Open))
                using (var cropedStream = new MemoryStream())
                {
                    var image = new Image(fileStream);

                    image.Crop(width, height, new Rectangle(left, top, width, height))
                     .Resize(AvatarStoredWidth, AvatarStoredHeight)
                    .Save(cropedStream);

                    var imageBytes = cropedStream.ToArray();

                    person.Image = imageBytes;

                    _context.SaveChanges();
                }

                return Json(new { success = true, avatar = Convert.ToBase64String(person.Image) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = "Unable to upload file.\nERRORINFO: " + ex.Message });
            }
        }

        private bool IsImage(IFormFile file)
        {
            if (file == null) return false;
            return file.ContentType.Contains("image") ||
                _imageFileExtensions.Any(item =>
                ContentDispositionHeaderValue.Parse(file.ContentDisposition)
                .FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        private string GetTempSavedFilePath(IFormFile file)
        {
            // Define destination
            var tempPath = Path.Combine(_environment.WebRootPath, "temp");
            if (Directory.Exists(tempPath) == false)
            {
                Directory.CreateDirectory(tempPath);
            }

            // Generate unique file name
            var filePath = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{DateTime.Now:yyyyMMddHHmmssfff}_{Path.GetFileName(filePath)}";

            fileName = SaveTemporaryAvatarFileImage(file, tempPath, fileName);

            // Clean up old files after every save
            CleanUpTempFolder(1);
            return Path.Combine($"\\{TempFolder}", fileName.Replace("\\", "/"));
        }

        private static string SaveTemporaryAvatarFileImage(IFormFile file, string serverPath, string fileName)
        {
            var fullFileName = Path.Combine(serverPath, fileName);
            if (System.IO.File.Exists(fullFileName))
            {
                System.IO.File.Delete(fullFileName);
            }

            using (var fileStream = new FileStream(fullFileName, FileMode.Create))
            {
                var image = new Image(file.OpenReadStream());
                var ratio = image.Height / (double)image.Width;
                image.Resize(AvatarScreenWidth, (int)(AvatarScreenWidth * ratio))
                    .Save(fileStream);
            }

            return fileName;
        }

        private void CleanUpTempFolder(int hoursOld)
        {
            try
            {
                var currentUtcNow = DateTime.UtcNow;
                var tempPath = Path.Combine(_environment.WebRootPath, "temp");
                if (!Directory.Exists(tempPath)) return;
                var fileEntries = Directory.GetFiles(tempPath);

                foreach (var fileEntry in fileEntries)
                {
                    var fileCreationTime = System.IO.File.GetCreationTimeUtc(fileEntry);
                    var res = currentUtcNow - fileCreationTime;
                    if (res.TotalHours > hoursOld)
                    {
                        System.IO.File.Delete(fileEntry);
                    }
                }
            }
            catch
            {
                // Deliberately empty.
            }
        }

        #endregion

        // GET: People / users of this device with the option to leave out people i know about
        [Produces("application/json")]
        [Route("api/people")]
        [ResponseCache(Duration = 0)]
        public async Task<IActionResult> Get(string excludeIds, int deviceId)
        {
            var data = await _context.Person.Include(p => p.PersonCards).ThenInclude(c => c.Card).ToListAsync();
            var location = (from d in _context.Device
                            join l in _context.Location on d.LocationId equals l.Id
                            where d.Id.Equals(deviceId)
                            select l).FirstOrDefault();

            dynamic res = data.Select(p => MapToResponse(p, location.Id));

            return Ok(new { People = res });
        }

        // GET: People
        [Produces("application/json")]
        [Route("api/person")]
        [ResponseCache(Duration = 0)]
        public async Task<IActionResult> GetPerson(string cardId)
        {
            var person = await (from p in _context.Person
                join pc in _context.PersonCard on p.Id equals pc.PersonId
                join c in _context.Card on pc.CardId equals c.Id
                where c.Uid.Equals(cardId)
                select p).LastOrDefaultAsync();
            
            return Ok(new { Id = person.Id, FullName = $"{person.FirstName} {person.LastName}" });
        }

        private object MapToResponse(Person person, int locationId)
        {
            var personCard = person.PersonCards.FirstOrDefault(c => c.Card.IsDeleted == false);
            var cardId = personCard == null ? string.Empty : personCard.Card.Uid;

            //Check ingress or egress
            var latestMovement =
                _context.Movement
                    .OrderByDescending(m => m.SwipeTime)
                    .FirstOrDefault(m => m.CardId == cardId);

            return new
            {
                Id = person.Id,
                Name = $"{person.FirstName} {person.LastName}",
                Image = person.Image,
                CardUid = cardId,
                InLocation = latestMovement != null && latestMovement.LocationId == locationId
            };
        }

        // GET: People
        public async Task<IActionResult> Index()
        {
            return View(await _context.Person.ToListAsync());
        }

        // GET: People/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Person.SingleAsync(m => m.Id == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // GET: People/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: People/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Person person)
        {
            if (ModelState.IsValid)
            {
                _context.Person.Add(person);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(person);
        }

        // GET: People/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Person.SingleAsync(m => m.Id == id);
            if (person == null)
            {
                return NotFound();
            }
            return View(person);
        }

        // POST: People/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Person person)
        {
            if (ModelState.IsValid)
            {
                var existPerson = await _context.Person.SingleAsync(p => p.Id == person.Id);

                if (existPerson == null)
                {
                    return NotFound();
                }

                existPerson.FirstName = person.FirstName;
                existPerson.LastName = person.LastName;
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(person);
        }

        // GET: People/Delete/5
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Person.SingleAsync(m => m.Id == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Person person = await _context.Person.SingleAsync(m => m.Id == id);
            _context.Person.Remove(person);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //GET: Person avatar by id
        [HttpGet]
        public async Task<FileContentResult> GetAvatar(int id)
        {
            var person = await _context.Person.SingleAsync(p => p.Id == id);
            var byteArray = person.Image;
            return byteArray != null
                ? new FileContentResult(byteArray, "image/jpeg")
                : new FileContentResult(new byte[0], "image/jpeg");
        }
    }
}