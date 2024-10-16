using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clock
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
			this.TransparencyKey = Color.Empty;
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			labelTime.Text = DateTime.Now.ToString("hh:mm:ss tt");
			if (cbShowDate.Checked)
			{
				labelTime.Text += $"\n{DateTime.Today.ToString("yyyy.MM.dd")}";
			}
			//notifyIconSystemTray.Text = "Curret time " + labelTime.Text;
		}
		private void SetVisibility(bool visible)
		{
			this.TransparencyKey = visible ? Color.Empty : this.BackColor;
			this.FormBorderStyle = visible ? FormBorderStyle.Sizable : FormBorderStyle.None;
			this.ShowInTaskbar = visible;
			cbShowDate.Visible = visible;
			btnHideControls.Visible = visible;
			labelTime.BackColor = visible ? Color.Empty : Color.Coral;
		}
		private void btnHideControls_Click(object sender, EventArgs e)
		{
			SetVisibility(false);
			notifyIconSystemTray.ShowBalloonTip(3, "Alerts!", "Для того чтобы вернуть как было, нужно ткнуть 2 раза мышей по часам, или по этой иконке", ToolTipIcon.Warning);
		}

		private void labelTime_DoubleClick(object sender, EventArgs e)
		{
			SetVisibility(true);
		}

		private void notifyIconSystemTray_MouseMove(object sender, MouseEventArgs e)
		{
			notifyIconSystemTray.Text = "Curret time:\n" + labelTime.Text;
		}
	}
}
