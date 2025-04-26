using Microsoft.Agents.Storage;
using Microsoft.SemanticKernel;
using Onboardy.Contracts;

namespace Obnoarding.Plugins;

public class OnboardingSchedulePlugin
{
    IStorage _storage;

    public OnboardingSchedulePlugin(IStorage storage)
    {
        _storage = storage;
    }

    [KernelFunction]
    public Task<List<MeetingDto>> GetSchedule(string date, string department, string userId)
    {
        var it = new List<MeetingDto>()
        {
            new MeetingDto {
                UserId = userId,
                Topic = "Introduction to company",
                Agenda = "Overview of company policies and procedures",
                Organizer = "HR Department",
                Location = "online",
                Date = date,
                HourStart = "9:00",
                HourEnd = "9:45",
                Message = "Welcome to the company. We are happy that you joined us. Please read the following articles and let me know when you are ready to talk about the contents!",
                Resources = new List<string>
                {
                    "https://www.wp.pl",
                    "https://www.google.com"
                }
            },
            new MeetingDto {
                UserId = userId,
                Topic = "SCRUM basics",
                Agenda = "The framwork used in development teams, team structure, basics of scrum guide",
                Organizer = "Software Development Manager",
                Location = "online",
                Date = date,
                HourStart = "10:00",
                HourEnd = "11:30",
                Message = "Let's talk about SCRUM - the way we work. Please read the document called SCRUM GUIDE and let me know when you are ready for a shord quiz.",
                Resources = new List<string>
                {
                    "https://www.scrum.org/",
                }
            }
        };

        var finance = new List<MeetingDto>()
        {
            new MeetingDto {
                UserId = userId,
                Topic = "Introduction to company",
                Agenda = "Overview of company policies and procedures",
                Organizer = "HR Department",
                Location = "online",
                Date = date,
                HourStart = "9:00",
                HourEnd = "9:45",
                Message = "Welcome to the company. We are happy that you joined us. Please read the following articles and let me know when you are ready to talk about the contents!",
                Resources = new List<string>
                {
                    "https://wp.pl",
                    "https://google.com"
                }
            },
            new MeetingDto {
                UserId = userId,
                Topic = "Finance framework",
                Agenda = "The framework used in the company, your responsibilities, common goal",
                Organizer = "Finance Department",
                Location = "online",
                Date = date,
                HourStart = "10:00",
                HourEnd = "11:30",
                Message = "Time to talk about the accounting and finance procedures. We are happy that you joined us. Please read the following websites and let me know when you are ready to talk about the contents!",
                Resources = new List<string>
                {
                    "https://finance.ec.europa.eu/digital-finance/framework-financial-data-access_en",
                    "https://www.bdo.com/insights/assurance/accounting-for-business-combinations-asc-805"
                }
            }
        };

        var schedule = department == "it" ? it : finance;

        foreach(var meeting in schedule)
        {
            SaveMeeting(meeting, userId);
        }

        return Task.FromResult(schedule);
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
