using Events.Data;
using Events.Web.Extensions;
using Events.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Events.Web.Controllers
{
    [Authorize]
    public class EventsController : BaseController
    {
        // GET: Event
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EventInputModel model)
        {
            string filename = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
            string extension = Path.GetExtension(model.ImageFile.FileName);
            filename = filename + extension;

            // saving Image
            var savefilename = Path.Combine(Server.MapPath("~/Image/"), filename);
            model.ImageFile.SaveAs(savefilename);
            
            filename =  "~/Image/" + filename;
            if(model != null && this.ModelState.IsValid)
            {
                var e = new Event()
                {
                    AuthorId = this.User.Identity.GetUserId(),
                    Title = model.Title,
                    StartDateTime = model.StartdateTime,
                    Duration = model.Duration,
                    Description = model.Description,
                    Location = model.Location,
                    IsPublic = model.IsPublic,
                    ImagePath = filename
                };

                // saving file into folder


                this.db.Events.Add(e);
                this.db.SaveChanges();

                // Display message
                this.AddNotification("Event created.", NotificationType.INFO);
                return this.RedirectToAction("My");
            }

            return this.View(model);
        }

        public ActionResult My()
        {
            string currentuserId = this.User.Identity.GetUserId();
            var events = this.db.Events
                .Where(e => e.AuthorId == currentuserId)
                .OrderBy(e => e.StartDateTime)
                .Select(EventViewModel.ViewModel);

            var upcomingEvents = events.Where(e => e.StartDateTime > DateTime.Now);
            var passedEvents = events.Where(e => e.StartDateTime <= DateTime.Now);

            return View(new UpcomingPassedEventsViewModel()
            {
                UpComingEvents = upcomingEvents,
                PassedEvents = passedEvents
            });
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var eventToEdit = this.LoadEvent(id);
            if(eventToEdit == null)
            {
                this.AddNotification("Cannot edit event # " + id,
                    NotificationType.ERROR);

                return this.RedirectToAction("My");
            }

            // var model = this.db.Events.Find(id);
            // var model = EventInputModel.CreateFromEvent(eventToEdit);
            
            var model = (from e in this.db.Events
            where e.Id == id
            select new EventInputModel()
            {
                    Title = e.Title,
                    StartdateTime = e.StartDateTime,
                    Duration = e.Duration,
                    Description = e.Description,
                    Location = e.Location,
                    IsPublic = e.IsPublic
            }).FirstOrDefault();
            
            return this.View(model);
        }

        private Event LoadEvent(int id)
        {
            var currentuserId = this.User.Identity.GetUserId();
            var isAdmin = this.IsAdmin();
            var eventToEdit = this.db.Events
                .Where(e => e.Id == id)
                .FirstOrDefault(e => e.AuthorId == currentuserId || isAdmin);
            return eventToEdit;
        }

        public ActionResult Edit(int id, EventInputModel model)
        {
            var eventToEdit = this.LoadEvent(id);
            if(eventToEdit == null)
            {
                this.AddNotification("Can not edit events # " + id, NotificationType.ERROR);
                return this.RedirectToAction("My");
            }

            if(model != null && this.ModelState.IsValid)
            {
                eventToEdit.Title = model.Title;
                eventToEdit.StartDateTime = model.StartdateTime;
                eventToEdit.Duration = model.Duration;
                eventToEdit.Description = model.Description;
                eventToEdit.Location = model.Location;
                eventToEdit.IsPublic = model.IsPublic;

                this.db.SaveChanges();
                this.AddNotification("Event edit.", NotificationType.INFO);
                return this.RedirectToAction("My");
            }

            return this.View(model);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var deleteEvent = (from e in this.db.Events
                              where e.Id == id
                              select new EventViewModel
                              {
                                  Id = e.Id,
                                  Title = e.Title,
                                  StartDateTime = e.StartDateTime,
                                  Duration = e.Duration,
                                  Author = e.Author.FullName,
                                  Location = e.Location
                              }).FirstOrDefault();
            return View(deleteEvent);
        }

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            var deleteEvent = this.db.Events.FirstOrDefault(x => x.Id == id);
            if (id != null && deleteEvent != null)
            {
                this.db.Events.Remove(deleteEvent);
                db.SaveChanges();
                this.AddNotification("Event Deleted.", NotificationType.INFO);
                return RedirectToAction("My");
            }

            this.AddNotification("Error occur while deleting.", NotificationType.ERROR);
            //var deletedEvent = this.db.Events.FirstOrDefault(x => x.Id == id);
            return View(deleteEvent);
        }

    }
}