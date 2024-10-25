using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Win32;

namespace Clock
{
	public partial class MainForm : Form
	{
		ColorDialog backgroundColorDialog;
		ColorDialog foregroundColorDialog;
		ChooseFont chooseFontDialog;
		string FontFile { get; set; }
		public MainForm()
		{
			InitializeComponent();
			SetFontDirectory();
			this.TransparencyKey = Color.Empty;
			backgroundColorDialog = new ColorDialog();
			foregroundColorDialog = new ColorDialog();

			chooseFontDialog = new ChooseFont();
			LoadSettings();

			//backgroundColorDialog.Color = Color.Black;
			//foregroundColorDialog.Color = Color.Blue;
			SetVisibility(false);
			this.Location = new Point
				(
					System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - this.Width,
					50
				);
			this.Text += $" Location: {this.Location.X}x{this.Location.Y}";
		}
		void SetFontDirectory()
		{
			string location = Assembly.GetEntryAssembly().Location;	//Получаем полный адрес исполняемого файла
			string path = Path.GetDirectoryName(location);			//Из адреса извлекаем путь к файлу
			//MessageBox.Show(path);
			Directory.SetCurrentDirectory($"{path}\\..\\..\\Fonts");//Переходим в каталог со шрифтами
			//MessageBox.Show(Directory.GetCurrentDirectory());
		}
		void LoadSettings()
		{
			StreamReader sr = new StreamReader("settings.txt");
			List<string> settings = new List<string>();
			while (!sr.EndOfStream)
			{
				settings.Add(sr.ReadLine());
			}
			sr.Close();
			backgroundColorDialog.Color = Color.FromArgb(Convert.ToInt32(settings.ToArray()[0]));
			foregroundColorDialog.Color = Color.FromArgb(Convert.ToInt32(settings.ToArray()[1]));
			FontFile = settings.ToArray()[2];
			topmostToolStripMenuItem.Checked = bool.Parse(settings.ToArray()[3]);
			showDateToolStripMenuItem.Checked = bool.Parse(settings.ToArray()[4]);
			labelTime.Font = chooseFontDialog.SetFontFile(FontFile);
			labelTime.ForeColor = foregroundColorDialog.Color;
			labelTime.BackColor = backgroundColorDialog.Color;

			RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
			object run = rk.GetValue("Clock318");
			if (run != null) loadOnWindowsStartupToolStripMenuItem.Checked = true;
			rk.Dispose();
		}
		void SaveSettings()
		{
			StreamWriter sw = new StreamWriter("settings.txt");
			sw.WriteLine(backgroundColorDialog.Color.ToArgb()); //ToArgb() врзвращает числовой код цвета
			sw.WriteLine(foregroundColorDialog.Color.ToArgb());
			sw.WriteLine(chooseFontDialog.FontFile.Split('\\').Last());
			sw.WriteLine(topmostToolStripMenuItem.Checked);
			sw.WriteLine(showDateToolStripMenuItem.Checked);
			sw.Close();
			Process.Start("notepad", "settings.txt");
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
			labelTime.BackColor = visible ? Color.Empty : backgroundColorDialog.Color;
		}
		private void btnHideControls_Click(object sender, EventArgs e)
		{
			//SetVisibility(false);
			showControlsToolStripMenuItem.Checked = false;
			notifyIconSystemTray.ShowBalloonTip(3, "Alerts!", "Для того чтобы вернуть как было, нужно ткнуть 2 раза мышей по часам, или по этой иконке", ToolTipIcon.Info);
		}

		private void labelTime_DoubleClick(object sender, EventArgs e)
		{
			//SetVisibility(true);
			showControlsToolStripMenuItem.Checked = true;
		}

		private void notifyIconSystemTray_MouseMove(object sender, MouseEventArgs e)
		{
			notifyIconSystemTray.Text = "Curret time:\n" + labelTime.Text;
		}

		private void closeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void topmostToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			this.TopMost = topmostToolStripMenuItem.Checked;
		}

		private void showControlsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			SetVisibility(((ToolStripMenuItem)sender).Checked);
		}

		private void showDateToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			cbShowDate.Checked = ((ToolStripMenuItem)sender).Checked;
		}

		private void cbShowDate_CheckedChanged(object sender, EventArgs e)
		{
			showDateToolStripMenuItem.Checked = ((CheckBox)sender).Checked;
		}

		private void notifyIconSystemTray_DoubleClick(object sender, EventArgs e)
		{
			if (!this.TopMost)
			{
				this.TopMost = true;
				this.TopMost = false;
			}
		}

		private void foregroundColorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (foregroundColorDialog.ShowDialog(this) == DialogResult.OK)
			{
				labelTime.ForeColor = foregroundColorDialog.Color;
			}
		}

		private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (backgroundColorDialog.ShowDialog(this) == DialogResult.OK)
			{
				labelTime.BackColor = backgroundColorDialog.Color;
			}
		}

		private void fontsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (chooseFontDialog.ShowDialog(this) == DialogResult.OK)
			{
				labelTime.Font = chooseFontDialog.ChosenFont;
			}
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			SaveSettings();
		}

		private void loadOnWindowsStartupToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			RegistryKey rk =
				Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);  //true - Writable
			if (loadOnWindowsStartupToolStripMenuItem.Checked)
					rk.SetValue("Clock318", Application.ExecutablePath);
			else	rk.DeleteValue("Clock318", false);//false - НЕ бросать исключение, если указанная запись отсутсвует.
			rk.Dispose();	//Освобождает ресурсы, занятые объектом.
		}
	}
}
