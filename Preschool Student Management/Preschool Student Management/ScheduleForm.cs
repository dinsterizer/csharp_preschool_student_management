﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Preschool_Student_Management.Models;

namespace Preschool_Student_Management
{
	public partial class ScheduleForm : Form
	{
		private int week = 0;
		private DateTime from{
			get 
			{
				return DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + this.week*7);
			}
		}
		private int startedHour = 6;
		private int	hourPeriod = 12;

		private List<Schedule> schedules = new List<Schedule>();

		private int schedulableId;
		private string schedulableType;

		private List<Schedule> Schedules
		{
			set
			{
				this.schedules = value.OrderBy((schedule) => schedule.StartedAt).ToList();
			}
			get
			{
				return this.schedules;
			}
		}

		public ScheduleForm(Classroom classroom)
		{
			this.classroom = classroom;
			this.schedulableId = int.Parse(classroom.Key);
			this.schedulableType = classroom.TableName;

			this.LoadSchedule();

			InitializeComponent();
		}

		public ScheduleForm(Student student)
		{
			this.student = student;
			this.schedulableId = int.Parse(student.Key);
			this.schedulableType = student.TableName;
			this.LoadSchedule();

			InitializeComponent();
		}

		private Classroom classroom;
		private Student student;
		private void LoadSchedule() {
			this.ReloadSchedule();
		}
		private void ReloadSchedule() {
			if (this.classroom != null)
			{
				this.Schedules = Classroom.Query.WithSchedules(this.from, this.from.AddDays(7)).Find(this.classroom.Key).Schedules;
			}
			else 
			{
				this.Schedules = Student.Query.WithSchedules(this.from, this.from.AddDays(7)).Find(this.student.Key).Schedules;
			}
		}

		/// <summary>
		/// Create a empty space use for period between 2 schedule
		/// </summary>
		private FlowLayoutPanel CreateScheduleEmpty(int parentWidth, int parentHeight, DateTime latestEndedAt, Schedule schedule)
		{
			var period = schedule.StartedAt.Subtract(latestEndedAt);

			var empty = new FlowLayoutPanel();
			empty.Size = new Size(parentWidth, (int)((period.TotalHours / this.hourPeriod) * parentHeight));
			empty.Margin = new Padding(0);

			return empty;
		}

		/// <summary>
		/// Create a button represent for schedule
		/// </summary>
		private Button CreateScheduleButton(int parentWidth, int parentHeight, Schedule schedule) 
		{
			var period = schedule.EndedAt - schedule.StartedAt;

			var btn = new Button();
			btn.Text = schedule.GetAttribute("name") + "\n" + schedule.StartedAt.ToString("hh:mm") + " - " + schedule.EndedAt.ToString("hh:mm");
			btn.Size = new Size(parentWidth, (int)((period.TotalHours / this.hourPeriod) * parentHeight));
			btn.Margin = new Padding(0);
			btn.Tag = schedule;
			btn.Click += Btn_Click;

			return btn;
		}

		private void Btn_Click(object sender, EventArgs e)
		{
			var updateScheduleForm = new UpdateScheduleForm((Schedule)(((Button)sender).Tag));
			updateScheduleForm.StartPosition = FormStartPosition.CenterParent;
			updateScheduleForm.ShowDialog();

			this.RerenderSchedules();
		}

		private void RerenderSchedules()
		{
			this.ReloadSchedule();
			this.flpMonday.Controls.Clear();
			this.flpTuesday.Controls.Clear();
			this.flpWednesday.Controls.Clear();
			this.flpThursday.Controls.Clear();
			this.flpFriday.Controls.Clear();
			this.flpSaturday.Controls.Clear();
			this.flpSunday.Controls.Clear();

			this.RenderSchedules();
		}

		private void RenderSchedules() 
		{
			DateTime latestEndedAt = this.from;
			FlowLayoutPanel empty;
			Button btn;
			foreach (var schedule in this.schedules)
			{
				if ((int)latestEndedAt.DayOfWeek != (int)schedule.StartedAt.DayOfWeek)
				{
					latestEndedAt = schedule.StartedAt.AddHours(-schedule.StartedAt.Hour + this.startedHour).AddMinutes(-schedule.StartedAt.Minute).AddSeconds(-schedule.StartedAt.Second);
					Console.WriteLine(latestEndedAt);
				}

				empty = this.CreateScheduleEmpty(this.flpMonday.Size.Width, this.flpMonday.Size.Height, latestEndedAt, schedule);
				btn = this.CreateScheduleButton(this.flpMonday.Size.Width, this.flpMonday.Size.Height, schedule);

				switch (schedule.StartedAt.DayOfWeek)
				{
					// Monday
					case DayOfWeek.Monday:
						this.flpMonday.Controls.Add(empty);
						this.flpMonday.Controls.Add(btn);
						break;

					// Tuesday
					case DayOfWeek.Tuesday:
						this.flpTuesday.Controls.Add(empty);
						this.flpTuesday.Controls.Add(btn);
						break;

					// Wednesday
					case DayOfWeek.Wednesday:
						this.flpWednesday.Controls.Add(empty);
						this.flpWednesday.Controls.Add(btn);
						break;

					// Thursday
					case DayOfWeek.Thursday:
						this.flpThursday.Controls.Add(empty);
						this.flpThursday.Controls.Add(btn);
						break;

					// Friday
					case DayOfWeek.Friday:
						this.flpFriday.Controls.Add(empty);
						this.flpFriday.Controls.Add(btn);
						break;

					// Saturday
					case DayOfWeek.Saturday:
						this.flpSaturday.Controls.Add(empty);
						this.flpSaturday.Controls.Add(btn);
						break;

					// Sunday
					case DayOfWeek.Sunday:
						this.flpSunday.Controls.Add(empty);
						this.flpSunday.Controls.Add(btn);
						break;
				}


				latestEndedAt = schedule.EndedAt;
			}

			this.btnSunday.Text = "SUN - " + this.from.ToString("d/M");
			this.btnMonday.Text = "MON - " + this.from.AddDays(1).ToString("d/M");
			this.btnTuesday.Text = "TUE - " + this.from.AddDays(2).ToString("d/M");
			this.btnWednesday.Text = "WED - " + this.from.AddDays(3).ToString("d/M");
			this.btnThursday.Text = "THU - " + this.from.AddDays(4).ToString("d/M");
			this.btnFriday.Text = "FRI - " + this.from.AddDays(5).ToString("d/M");
			this.btnSaturday.Text = "SAT - " + this.from.AddDays(6).ToString("d/M");
		}

		private void ScheduleForm_Load(object sender, EventArgs e)
		{
			this.RenderSchedules();
		}

		private void btnNextWeek_Click(object sender, EventArgs e)
		{
			this.week += 1;
			this.RerenderSchedules();
		}

		private void btnPreWeek_Click(object sender, EventArgs e)
		{
			this.week -= 1;
			this.RerenderSchedules();
		}

		private void thêmLịchHọcToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var createLearningScheduleForm = new CreateLearningScheduleForm(this.schedulableType, this.schedulableId);
			createLearningScheduleForm.StartPosition = FormStartPosition.CenterParent;
			createLearningScheduleForm.ShowDialog();

			this.RerenderSchedules();
		}

		private void thêmLịchKhácToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var createVaccineScheduleForm = new CreateVaccineSchedule(this.schedulableType, this.schedulableId);
			createVaccineScheduleForm.StartPosition = FormStartPosition.CenterParent;
			createVaccineScheduleForm.ShowDialog();

			this.RerenderSchedules();
		}
	}
}
