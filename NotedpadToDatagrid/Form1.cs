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
using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NotedpadToDatagrid
{
    public partial class Form1 : Form
    {
        String connetionString = @"Data Source=192.168.2.9\SQLEXPRESS;Initial Catalog=hr_bak;User ID=sa;Password=Nescafe3in1;MultipleActiveResultSets=true";
        SqlConnection con = new SqlConnection();
        SqlCommand cmd = new SqlCommand();
        SqlDataReader dr;
        public Form1()
        {
            InitializeComponent();
        }
        private void con_on()
        {
            con = new SqlConnection();
            con.ConnectionString = connetionString;
            con.Open();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = Helper.DataTableFromTextFile("LogsHW 136.txt", ' ');
            label1.Text = dataGridView1.Rows.Count.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //for (int i = 0; i < dataGridView1.Rows.Count; i++)
            //{

            //    string empid = dataGridView1.Rows[i].Cells[2].Value.ToString().Trim();
            //    string date = Convert.ToDateTime(dataGridView1.Rows[i].Cells[3].Value.ToString().Trim()).ToString("yyyy-MM-dd");
            //    string time = dataGridView1.Rows[i].Cells[4].Value.ToString().Trim() + dataGridView1.Rows[i].Cells[5].Value.ToString().Trim();
            //    string timeconcat = Convert.ToDateTime(time).ToString("hh:mmttt");

            //    ActivitiesLogs(textBox1.Text + " | " + empid + " | " + date + " | " + timeconcat);
            //}
            //MessageBox.Show("Saved");

            con_on();
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                try {
                    string q = "UPDATE dbo.DTRecords SET dbo.DTRecords.LunchIn = @a WHERE dbo.DTRecords.Employee_ID = @b AND dbo.DTRecords.Attendance_Date = @c";
                    cmd = new SqlCommand(q, con);
                    cmd.Parameters.AddWithValue("@a", dataGridView2.Rows[i].Cells[3].Value.ToString());
                    cmd.Parameters.AddWithValue("@b", dataGridView2.Rows[i].Cells[0].Value.ToString());
                    cmd.Parameters.AddWithValue("@c", "2018-11-10");

                    cmd.ExecuteNonQuery();

                    ActivitiesLogs(dataGridView2.Rows[i].Cells[3].Value.ToString() + " | " + dataGridView2.Rows[i].Cells[0].Value.ToString() + " | " + "INSERTED");

                }

           
                    catch 
                {

                    ActivitiesLogs(dataGridView2.Rows[i].Cells[3].Value.ToString() + " | " + dataGridView2.Rows[i].Cells[0].Value.ToString() + " | " + "FAILED");
                  

                    }
            }
            con.Close();

            MessageBox.Show("SAVED");
        }
        public void ActivitiesLogs(string logs)
        {

            try
            {

                //@"c:\a\UserName.txt"
                const string location = @"SAVELOGS";

                if (!File.Exists(location))
                {
                    var createText = "LOG : " + Environment.NewLine;
                    File.WriteAllText(location, createText);

                }
                var appendLogs = "Logs: " + logs + " " + Environment.NewLine;
                File.AppendAllText(location, appendLogs);
            }
            catch (Exception ex)
            {
                const string location = @"SAVELOGS";
                if (!File.Exists(location))
                {
                    TextWriter file = File.CreateText(@"C:\SAVELOGS");
                    var createText = "New Activities Logs" + Environment.NewLine;

                    File.WriteAllText(location, createText);

                }

                var appendLogs = ex.Message + logs + Environment.NewLine;
                File.AppendAllText(location, appendLogs);


            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            int count = 0;
            con_on();

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {

                string empid = dataGridView1.Rows[i].Cells[2].Value.ToString().Trim();
                //string date = Convert.ToDateTime(dataGridView1.Rows[i].Cells[3].Value.ToString().Trim()).ToString("yyyy-MM-dd");
                //string time = dataGridView1.Rows[i].Cells[4].Value.ToString().Trim() + dataGridView1.Rows[i].Cells[5].Value.ToString().Trim();
                //string timeconcat = Convert.ToDateTime(time).ToString("hh:mmttt");

                string q = "SELECT * FROM dbo.Health_wellnessDTRecords hwd WHERE hwd.employee_number = '" + empid + "' AND hwd.Attendance_Date = '2018-12-18'";
                cmd = new SqlCommand(q, con);
                dr = cmd.ExecuteReader();
                //dr.Read();

                if (!dr.HasRows)
                {
                    string qwe = "INSERT INTO dbo.Health_wellnessDTRecords (dbo.Health_wellnessDTRecords.employee_number, dbo.Health_wellnessDTRecords.Attendance_Date, dbo.Health_wellnessDTRecords.Attendance_Time) VALUES (@a, @b, @c)";
                    cmd = new SqlCommand(qwe, con);
                    cmd.Parameters.AddWithValue("@a", empid);
                    cmd.Parameters.AddWithValue("@b", "2018-12-18");
                    cmd.Parameters.AddWithValue("@c", "2018-12-18 07:00:00.00");
                    cmd.ExecuteNonQuery();
                    count++;
                }

                //dataGridView2.Rows.Add(dr[0].ToString(), empid, date, timeconcat);

            }
            MessageBox.Show("Saved " + count.ToString());
            label4.Text = dataGridView2.Rows.Count.ToString();
            con.Close();


        }
        public class Helper
        {
            public static DataTable DataTableFromTextFile(string location, char delimiter = ',')
            {
                DataTable result;

                string[] LineArray = File.ReadAllLines(location);

                result = FormDataTable(LineArray, delimiter);

                return result;
            }

            private static DataTable FormDataTable(string[] LineArray, char delimiter)
            {
                DataTable dt = new DataTable();

                AddColumnToTable(LineArray, delimiter, ref dt);

                AddRowToTable(LineArray, delimiter, ref dt);

                return dt;
            }

            private static void AddRowToTable(string[] valueCollection, char delimiter, ref DataTable dt)
            {

                for (int i = 1; i < valueCollection.Length; i++)
                {
                    string[] values = valueCollection[i].Split(delimiter);
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < values.Length; j++)
                    {
                        dr[j] = values[j];
                    }
                    dt.Rows.Add(dr);
                }
            }

            private static void AddColumnToTable(string[] columnCollection, char delimiter, ref DataTable dt)
            {
                string[] columns = columnCollection[0].Split(delimiter);
                foreach (string columnName in columns)
                {
                    DataColumn dc = new DataColumn(columnName, typeof(string));
                    dt.Columns.Add(dc);
                }
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                string machineName = "192.168.2.171";
                Process proc = new Process();
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.FileName = "net";
                proc.StartInfo.Arguments = @"time \\" + machineName;
                proc.Start();
                proc.WaitForExit();

                List<string> results = new List<string>();

                while (!proc.StandardOutput.EndOfStream)
                {
                    string currentline = proc.StandardOutput.ReadLine();

                    if (!string.IsNullOrEmpty(currentline))
                    {
                        results.Add(currentline);
                    }
                }

                string currentTime = string.Empty;

                if (results.Count > 0 && results[0].ToLower().StartsWith(@"current time at \\" + machineName.ToLower() + " is "))
                {
                    currentTime = results[0].Substring((@"current time at \\" +
                                  machineName.ToLower() + " is ").Length);

                    label5.Text = currentTime;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }

            ////

            try
            {
                string machineName = "192.168.2.136";
                Process proc = new Process();
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.FileName = "net";
                proc.StartInfo.Arguments = @"time \\" + machineName;
                proc.Start();
                proc.WaitForExit();

                List<string> results = new List<string>();

                while (!proc.StandardOutput.EndOfStream)
                {
                    string currentline = proc.StandardOutput.ReadLine();

                    if (!string.IsNullOrEmpty(currentline))
                    {
                        results.Add(currentline);
                    }
                }

                string currentTime = string.Empty;

                if (results.Count > 0 && results[0].ToLower().StartsWith(@"current time at \\" + machineName.ToLower() + " is "))
                {
                    currentTime = results[0].Substring((@"current time at \\" +
                                  machineName.ToLower() + " is ").Length);

                    label6.Text = currentTime;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
    }
}
