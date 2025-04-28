namespace Onboardy.Contracts;

public class MeetingDto
{
    public string UserId { get; set; }
    public string Topic { get; set; }
    public string Agenda { get; set; }
    public string Organizer { get; set; }
    public string Location { get; set; }
    public string Date { get; set; }
    public int StartHour { get; set; }
    public int StartMinute { get; set; }
    public int EndHour { get; set; }
    public int EndMinute { get; set; }
    public List<string> Resources { get; set; }
    public List<FollowUpQuestion> FollowUpQuestions { get; set; }
}

public class FollowUpQuestion
{
    public string Text { get; set; }
    public string Time { get; set; }
    public int Hour { get; set; }
    public int Minute { get; set; }
}