using System;

namespace OrchardLite.Web.Models
{
    public class ContentItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Body { get; set; }
        public string ContentType { get; set; }
        public int AuthorId { get; set; }
        public DateTime PublishedDate { get; set; }
        public int ViewCount { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
