﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Preschool_Student_Management.ORM;
using MySql.Data.MySqlClient;

namespace Preschool_Student_Management.Models
{
	public class Schedule : Eloquent<Schedule>
	{
		public override string TableName
		{
			get { return "schedules"; }
		}

		readonly public static int LearningType = 1;
		readonly public static int VacxinType = 2;

		public DateTime StartedAt {
			get { return DateTime.Parse(this.GetAttribute("started_at")); }
			set { this.SetAttribute("started_at", value.ToString("yyyy/MM/dd HH:mm:ss")); }
		}
		public DateTime EndedAt
		{
			get { return DateTime.Parse(this.GetAttribute("ended_at")); }
			set { this.SetAttribute("ended_at", value.ToString("yyyy/MM/dd HH:mm:ss")); }
		}

		public DateTime CreatedAt
		{
			get { return DateTime.Parse(this.GetAttribute("created_at")); }
			set { this.SetAttribute("created_at", value.ToString("yyyy/MM/dd HH:mm:ss")); }
		}

		public User User;
		/// <summary>
		/// With user who created the model
		/// </summary>
		public Schedule WithUser()
		{
			this.selectedQueues.Add((models) => {
				var keys = new List<string>();
				foreach (var model in models)
				{
					keys.Add(model.GetAttribute("user_id"));
				}

				var users = (new User()).WhereIn((new User()).KeyName, keys).Get();

				foreach (var model in models)
				{
					foreach (var user in users)
					{
						if (user.GetAttribute("id") == model.GetAttribute("user_id"))
						{
							model.User = user;
						}
					}
				}

				return models;
			});

			return this;
		}


		public static Schedule Create(string scheduleType, int scheduleId, string name, string description, DateTime date, DateTime timeFrom, DateTime timeTo)
		{
			var schedule = new Schedule();

			schedule.SetAttribute("schedulable_id", scheduleId.ToString());
			schedule.SetAttribute("schedulable_type", scheduleType);
			schedule.SetAttribute("name", name);
			schedule.SetAttribute("description", description);
			schedule.SetAttribute("type", Schedule.LearningType.ToString());
			schedule.SetAttribute("user_id", User.CurrentUsser.Key);

			schedule.StartedAt = new DateTime(date.Year, date.Month, date.Day, timeFrom.Hour, timeFrom.Minute, 0);
			schedule.EndedAt = new DateTime(date.Year, date.Month, date.Day, timeTo.Hour, timeTo.Minute, 0);
			schedule.CreatedAt = DateTime.Now;

			schedule.Save();


			return schedule;
		}
	}
}
