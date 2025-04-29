using Microsoft.Agents.Storage;
using Microsoft.SemanticKernel;
using Onboardy.Contracts;

namespace Obnoarding.Plugins;


public class Meetign
{

}

public class OnboardingSchedulePlugin
{
    IStorage _storage;

    public OnboardingSchedulePlugin(IStorage storage)
    {
        _storage = storage;
    }

    [KernelFunction]
    public async Task<List<MeetingDto>> GetSchedule(string department, string userId)
    {
        var date = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var now = DateTime.UtcNow;

        var it = new List<MeetingDto>()
        {
            new MeetingDto {
                UserId = userId,
                Topic = "Introduction to company",
                Agenda = "Overview of company policies and procedures",
                Organizer = "HR Department",
                Location = "online",
                Date = date,
                StartHour = 9,
                StartMinute = 0,
                EndHour = 9,
                EndMinute = 30,
                Resources = new List<string>
                {
                    "https://www.microsoft.com/en-us/about",
                },
                FollowUpQuestions =
                [
                    new()
                    {
                        Text = $"Welcome to the company. I'm very happy that you joined us. Please read the following article: https://www.microsoft.com/en-us/about and then we can talk about the contents!",
                        Hour = now.AddMinutes(0).Hour,
                        Minute = now.AddMinutes(0).Minute
                    },
                    new()
                    {
                        Text = "Let's check what you've learnt. What is our company mission?",
                        Hour = now.AddMinutes(2).Hour,
                        Minute = now.AddMinutes(2).Minute
                    },
                    new ()
                    {
                        Text = "Let's check your knowledge. What are our company values?",
                        Hour = now.AddMinutes(3).Hour,
                        Minute = now.AddMinutes(3).Minute
                    },
                ]
            },
            new() {
                UserId = userId,
                Topic = "SCRUM basics",
                Agenda = "The framwork used in development teams, team structure, basics of scrum guide",
                Organizer = "Software Development Manager",
                Location = "online",
                Date = date,
                StartHour = 10,
                StartMinute = 0,
                EndHour = 10,
                EndMinute = 30,
                Resources = new List<string>
                {
                    "https://www.scrum.org/",
                },
                FollowUpQuestions =
                [
                    new()
                    {
                        Text = "Let's talk about SCRUM - the way we work. Please read the document called SCRUM GUIDE that you can find on the website https://www.scrum.org/ and prepare for a short quiz",
                        Hour = now.AddMinutes(0).Hour,
                        Minute = now.AddMinutes(0).Minute
                    },
                    new()
                    {
                        Text = "Let's check what you've learnt. Name 2 authors of SCRUM GUIDE",
                        Hour = now.AddMinutes(4).Hour,
                        Minute = now.AddMinutes(4).Minute
                    },
                    new ()
                    {
                        Text = "Let's check what your learning progress. What is the goal of daily standups in SCRUM?",
                        Hour = now.AddMinutes(5).Hour,
                        Minute = now.AddMinutes(5).Minute
                    },
                ]
            }
        };

        var finance = new List<MeetingDto>()
        {
            new() {
                UserId = userId,
                Topic = "Introduction to company",
                Agenda = "Overview of company policies and procedures",
                Organizer = "HR Department",
                Location = "online",
                Date = date,
                StartHour = 9,
                StartMinute = 0,
                EndHour = 9,
                EndMinute = 45,
                Resources = new List<string>
                {
                    "https://wp.pl",
                    "https://google.com"
                },
                FollowUpQuestions =
                [
                    new()
                    {
                        Text = "Welcome to the company. I'm happy that you joined us. Please read the following articles. Then I will ask you some questions related to the docs.",
                        Hour = now.AddMinutes(0).Hour,
                        Minute = now.AddMinutes(0).Minute
                    },
                    new()
                    {
                        Text = "Let's check your knowledge. ",
                        Hour = now.AddMinutes(1).Hour,
                        Minute = now.AddMinutes(1).Minute
                    }
                ]
            },
            new() {
                UserId = userId,
                Topic = "Finance framework",
                Agenda = "The framework used in the company, your responsibilities, common goal",
                Organizer = "Finance Department",
                Location = "online",
                Date = date,
                StartHour = 10,
                StartMinute = 0,
                EndHour = 11,
                EndMinute = 30,
                Resources = new List<string>
                {
                    "https://finance.ec.europa.eu/digital-finance/framework-financial-data-access_en",
                    "https://www.bdo.com/insights/assurance/accounting-for-business-combinations-asc-805"
                },
                FollowUpQuestions =
                [
                    new()
                    {
                        Text = "Time to talk about the accounting and finance procedures. We are happy that you joined us. Please read the following documents. Then I will ask you some questions related to the docs.",
                        Hour = now.AddMinutes(0).Hour,
                        Minute = now.AddMinutes(0).Minute
                    },
                    new()
                    {
                        Text = "Let's check your knowledge. ",
                        Hour = now.AddMinutes(2).Hour,
                        Minute = now.AddMinutes(2).Minute
                    },
                    new ()
                    {
                        Text = 
                        "Which two major initiatives were included in the EU’s June 2023 legislative package to modernize the financial sector?" +
                        "A) Introduction of cryptocurrency regulation and a digital euro" +
                        "B) Amendments to the Payment Services Directive (PSD2) and a new Payment Services Regulation (PSR)" +
                        "C) Abolishment of open banking and reinforcement of traditional banking services" +
                        "D) Establishment of a European Financial Surveillance Authority and an Open Finance Taskforce",
                        Hour = now.AddMinutes(3).Hour,
                        Minute = now.AddMinutes(3).Minute
                    },
                ]
            }
        };

        var schedule = department == "it" ? it : finance;

        foreach(var meeting in schedule)
        {
            await SaveMeeting(meeting, userId);
        }

        //CLEANUP FOLLOWUP QUESTIONS:
        schedule.ForEach(s => s.FollowUpQuestions = new List<FollowUpQuestion>());

        return schedule;
    }

    public Task<string> SaveMeeting(MeetingDto meeting, string userId)
    {
        var meetingToSave = new Dictionary<string, MeetingDto>()
        {
            { $"meeting:{userId}:{Guid.NewGuid()}", meeting}
        };
        _storage.WriteAsync(meetingToSave);

        return Task.FromResult("meeting added correctly");
    }

}
