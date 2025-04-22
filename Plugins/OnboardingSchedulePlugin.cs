using Microsoft.SemanticKernel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Obnoarding.Plugins;

public class OnboardingSchedulePlugin
{
    [KernelFunction]
    public Task<List<Meeting>> GetSchedule(string date, string department)
    {
        var it = new List<Meeting>()
        {
            new Meeting {
                Topic = "Introduction to company",
                Agenda = "Overview of company policies and procedures",
                Organizer = "HR Department",
                Location = "online",
                Date = date,
                HourStart = "9:00",
                HourEnd = "9:45"
            },
            new Meeting {
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
                Topic = "Introduction to company",
                Agenda = "Overview of company policies and procedures",
                Organizer = "HR Department",
                Location = "online",
                Date = date,
                HourStart = "9:00",
                HourEnd = "9:45"
            },
            new Meeting {
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

        return Task.FromResult(schedule);
    }
}
