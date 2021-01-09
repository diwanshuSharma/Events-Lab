using Events.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Events.Web.Models
{
    public class CommentViewModel
    {
        public string Text { get; set; }
        public string Author { get; set; }
        public string AuthorId { get; set; }
        public int Id { get; set; }
        public static Expression<Func<Comment, CommentViewModel>> ViewModel
        {
            get
            {
                return c => new CommentViewModel()
                {
                    Text = c.Text,
                    Author = c.Author.FullName,
                    AuthorId = c.AuthorId,
                    Id = c.Id
                };
            }
        }
    }
}