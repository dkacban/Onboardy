using Microsoft.Agents.Storage;
using Microsoft.Azure.Cosmos;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Obnoarding.Plugins;

public class OnboardingSchedulePlugin
{
    IStorage _storage;

    public OnboardingSchedulePlugin(IStorage storage)
    {
        _storage = storage;
    }

    [KernelFunction]
    public Task<List<Meeting>> GetSchedule(string date, string department, string userId)
    {
        var it = new List<Meeting>()
        {
            new Meeting {
                UserId = userId,
                Topic = "Introduction to company",
                Agenda = "Overview of company policies and procedures",
                Organizer = "HR Department",
                Location = "online",
                Date = date,
                HourStart = "9:00",
                HourEnd = "9:45"
            },
            new Meeting {
                UserId = userId,
                Topic = "SCRUM basics",
                Agenda = "The framwork used in development teams, team structure, basics of scrum guide",
                Organizer = "Software Development Manager",
                Location = "online",
                Date = date,
                HourStart = "10:00",
                HourEnd = "11:30"
            }
        };

        var finance = new List<Meeting>()
        {
            new Meeting {
                UserId = userId,
                Topic = "Introduction to company",
                Agenda = "Overview of company policies and procedures",
                Organizer = "HR Department",
                Location = "online",
                Date = date,
                HourStart = "9:00",
                HourEnd = "9:45"
            },
            new Meeting {
                UserId = userId,
                Topic = "Finance framework",
                Agenda = "The framework used in the company, your responsibilities, common goal",
                Organizer = "Finance Department",
                Location = "online",
                Date = date,
                HourStart = "10:00",
                HourEnd = "11:30"
            }
        };

        var schedule = department == "it" ? it : finance;

        foreach(var meeting in schedule)
        {
            SaveMeeting(meeting, userId);
        }

        return Task.FromResult(schedule);
    }

    public Task<string> SaveMeeting(Meeting meeting, string userId)
    {
        var meetingToSave = new Dictionary<string, Meeting>()
        {
            { $"{userId}:{Guid.NewGuid()}", meeting}
        };
        _storage.WriteAsync(meetingToSave);

        return Task.FromResult("meeting added correctly");
    }

}
