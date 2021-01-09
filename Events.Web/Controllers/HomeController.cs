using Events.Data;
using Events.Web.Extensions;
using Events.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;

namespace Events.Web.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            var events = this.db.Events
                .OrderBy(e => e.StartDateTime)
                .Where(e => e.IsPublic)
                .Select(EventViewModel.ViewModel);

            var upcomingEvents = events.Where(e => e.StartDateTime > DateTime.Now);
            var passedEvents = events.Where(e => e.StartDateTime <= DateTime.Now);

            return View(new UpcomingPassedEventsViewModel()
            {
                UpComingEvents = upcomingEvents,
                PassedEvents = passedEvents
            });
        }

        public ActionResult EventDetailsById(int id)
        {
            var currentUserId = this.User.Identity.GetUserId();
            var isAdmin = this.IsAdmin();
            var eventDetails = this.db.Events
                .Where(e => e.Id == id)
                .Where(e => e.IsPublic || isAdmin || (e.AuthorId != null && e.AuthorId == currentUserId))
                .Select(EventDetailsViewModel.ViewModel)
                .FirstOrDefault();

            var isOwner = (eventDetails != null && eventDetails.AuthorId != null && eventDetails.AuthorId == currentUserId);

            this.ViewBag.CanEdit = isOwner || isAdmin;
            this.ViewBag.CurrentUserId = currentUserId;

            return this.PartialView("_EventDetails", eventDetails);
        }

        [HttpGet]
        public ActionResult AddCommentById(int? id)
        {
            ViewBag.Id = id;
            return View();
        }

        [HttpPost]
        public ActionResult AddCommentById(int id, string comment)
        {
            var currentuserId = this.User.Identity.GetUserId();
            Comment Addcomment = new Comment()
            {
                Text = comment,
                AuthorId = currentuserId,
                EventId = id,
                Event = this.db.Events.Find(id)
            };

            this.db.Comments.Add(Addcomment);
            this.db.SaveChanges();
            this.AddNotification("Comment Added Successfully", NotificationType.SUCCESS);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult DeleteComment(int id)
        {
            var deleteComment = this.db.Comments
                .Where(x => x.Id == id)
                .Select(CommentViewModel.ViewModel).FirstOrDefault();
            
            return View(deleteComment);
        }

        [HttpPost]
        public ActionResult DeleteComment(int? id)
        {
            if (id != null)
            {
                var deleteComment = this.db.Comments.FirstOrDefault(x => x.Id == id);
                this.db.Comments.Remove(deleteComment);
                this.db.SaveChanges();
                this.AddNotification("Comment Deleted Successfully", NotificationType.INFO);
                return RedirectToAction("Index");
            }

            return RedirectToAction("DeleteComment", new { id = id});
        }
    }
}