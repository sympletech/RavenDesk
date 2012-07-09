using System;
using System.Collections.Generic;
using System.Linq;
using TeamMgmtCal.Core.Calendar;
using TeamMgmtCal.Core.Data;
using TeamMgmtCal.Core.Data.Models;
using TeamMgmtCal.Core.UserManagement;
using TeamMgmtCal.Core.Shared;

namespace TeamMgmtCal.Core.TeamManagement
{
    public class TeamWorker
    {
        public IDataContext Db { get; set; }
        public TeamWorker(IDataContext db)
        {
            this.Db = db;
        }

        public TeamDetail GetTeam(int id)
        {
            return Db.Session.Load<TeamDetail>(id);
        }
        public TeamDetail GetTeam(string id)
        {
            return Db.Session.Load<TeamDetail>(id);
        }

        public IEnumerable<TeamDetail> GetAllTeams()
        {
            return Db.Session.Query<TeamDetail>();
        }

        public IEnumerable<TeamDetail> GetVisibleCalendars(AccountUser agent)
        {
            if (agent.Admin)
            {
                return Db.Session.Query<TeamDetail>().OrderBy(x => x.Name);
            }

            return from x in Db.Session.Query<TeamDetail>()
                   where x.Manager.Id == agent.Id
                         || x.AssistantManager.Id == agent.Id
                         || x.HrAdmin.Id == agent.Id
                         || x.TeamMembers.Any(y => y.Id == agent.Id)
                   select x;
        }

        public IEnumerable<TeamShift> GetTeamShifts(int teamId)
        {
            var team = Db.Session.Load<TeamDetail>(teamId);
            return team.Shifts;
        }

        public IEnumerable<string> DayOffChoices()
        {
            var days = new List<string>();
            var dt = new DateTime(2012, 6, 10);
            for (int i = 0; i < 7; i++)
            {
                days.Add(dt.AddDays(i).DayOfWeek.ToString());
            }
            return days;
        }

        public IEnumerable<CalendarEntry> GetAgentsOffOnDate(TeamDetail team, DateTime lookup)
        {
            var calDay = new CalendarDay(Db, team, lookup);
            return calDay.GetCalendarEntries();
        }

        public IEnumerable<CalendarEntry> GetPendingRequests(TeamDetail team)
        {
            return Db.Session.Query<CalendarEntry>()
                .Where(x => x.Approved == null && x.Team == team)
                .OrderBy(x => x.StartDate);
        }

        public IEnumerable<TeamSummayEntry> GetTeamSummary(TeamDetail team, DateTime startDate, DateTime endDate)
        {
            var results = new List<TeamSummayEntry>();

            var members = team.TeamMembers;
            
            var uWorker = new UserWorker(Db);

            foreach (var teamMember in members)
            {
                var agentSummary = uWorker.ComputeAttendanceHourSummary(teamMember, startDate, endDate);
                if (agentSummary != null)
                {
                    var entry = new TeamSummayEntry
                        {
                            Agent = teamMember,
                            HoursByReqestType = new Dictionary<string, double>(),
                            TotalHours = agentSummary.Sum(x => x.Hours)
                        };
                    foreach (var tSummary in agentSummary)
                    {
                        entry.HoursByReqestType.Add(tSummary.Name, tSummary.Hours);
                    }
                    results.Add(entry);
                }
            }

            return results;
        }

        public IEnumerable<WeeklyReportEntry> GetWeeklyReport(TeamDetail team, DateTime weekStart)
        {
            var results = new List<WeeklyReportEntry>();
            for (int i = 0; i < 7; i++)
            {
                var lookupDate = weekStart.AddDays(i);
                var calDay = new CalendarDay(Db, team, lookupDate);
                var rEntry = new WeeklyReportEntry
                                 {
                                     ReportDay = lookupDate,
                                     AgentsOff = calDay.GetCalendarEntries()
                                 };
                results.Add(rEntry);
            }

            return results;
        }

        public TeamShift GetTeamShift(int id)
        {
            return Db.Session.Load<TeamShift>(id);
        }

        public OperationResult CreateEditShift(TeamShift tShift)
        {
            var result = new OperationResult();
            try
            {
                tShift.Save();
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
    }
}
