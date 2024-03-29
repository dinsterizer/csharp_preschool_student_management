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
	public partial class CreateLearningScheduleForm : Form
	{
		private string schedulableType;
		private int schedulableId;

		public CreateLearningScheduleForm(string schedulableType, int schedulableId)
		{
			this.schedulableType = schedulableType;
			this.schedulableId = schedulableId;
			InitializeComponent();
		}

		private Schedule CreateSchedule(string name, string description, DateTime timeFrom, DateTime timeTo, DateTime date)
		{
			return Schedule.Create(this.schedulableType, this.schedulableId, name, description, date, timeFrom, timeTo);
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void btnCreate_Click(object sender, EventArgs e)
		{
			var name = this.txtName.Text;
			var description = this.txtDescription.Text;
			var timeFrom = DateTime.Today.AddHours(this.dtpTimeFrom.Value.Hour).AddMinutes(this.dtpTimeFrom.Value.Minute);
			var timeTo = DateTime.Today.AddHours(this.dtpTimeTo.Value.Hour).AddMinutes(this.dtpTimeTo.Value.Minute);
			var dateFrom = new DateTime(this.dtpDateFrom.Value.Year, this.dtpDateFrom.Value.Month, this.dtpDateFrom.Value.Day);
			var dateTo = new DateTime(this.dtpDateTo.Value.Year, this.dtpDateTo.Value.Month, this.dtpDateTo.Value.Day);

			if (name == "") 
			{
				MessageBox.Show("Tên là bắt buộc", "Error!");
				return;
			}
			if ((timeTo - timeFrom).TotalMinutes < 30)
			{
				MessageBox.Show("Khung giờ không hợp lệ", "Error!");
				return;
			}
			if (dateFrom > dateTo)
			{
				MessageBox.Show("Thời gian diễn ra không hợp lệ", "Error!");
				return;
			}

			var currentDate = dateFrom;
			var createdSchedules = new List<Schedule>();

			while (currentDate <= dateTo)
			{
				// Monday
				if (
					currentDate.DayOfWeek == DayOfWeek.Monday && this.cbMonday.Checked
					|| currentDate.DayOfWeek == DayOfWeek.Tuesday && this.cbTuesday.Checked
					|| currentDate.DayOfWeek == DayOfWeek.Wednesday && this.cbWednesday.Checked
					|| currentDate.DayOfWeek == DayOfWeek.Thursday && this.cbThursday.Checked
					|| currentDate.DayOfWeek == DayOfWeek.Friday && this.cbFriday.Checked
					|| currentDate.DayOfWeek == DayOfWeek.Saturday && this.cbSaturday.Checked
					|| currentDate.DayOfWeek == DayOfWeek.Sunday && this.cbSunday.Checked
					)
				{
					createdSchedules.Add(
						this.CreateSchedule(name, description, timeFrom, timeTo, currentDate)
					);
				}


				currentDate = currentDate.AddDays(1);
			}

			MessageBox.Show("Tạo thành công " + createdSchedules.Count.ToString() + " schedules!", "Success!");
		}
	}
}
