using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Events.Web.Models
{
    public class EventInputModel
    {
        [Required(ErrorMessage ="Event title is Required.")]
        [StringLength(200, ErrorMessage ="the {0} must be betweeen {2} and {1} characters long.", MinimumLength =1)]
        [Display(Name ="Title")]
        public string Title { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name ="Date and Time *")]
        public DateTime StartdateTime { get; set; }

        public TimeSpan? Duration { get; set; }
        public string Description { get; set; }

        [MaxLength(200)]
        public string Location { get; set; }

        [Display(Name ="Is Public ?")]
        public bool IsPublic { get; set; }
    }
}