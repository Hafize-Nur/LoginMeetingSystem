namespace LoginMeetingSystem.Models
{
    public class MeetingDocument
    {

        public int Id { get; set; }
        public int MeetingId { get; set; }
        public Meeting? Meeting { get; set; }
        public string FilePath { get; set; }
        public string OriginalName { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }

    }
}