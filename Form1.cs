//
// N-tier C# and SQL program to analyze CTA Ridership data.
//
// <<Ankita Tank>>
// U. of Illinois, Chicago
// CS341, Fall2016
// Homework 7
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CTA
{

    public partial class Form1 : Form
    {
        private string BuildConnectionString()
        {
            string version = "MSSQLLocalDB";
            string filename = this.txtDatabaseFilename.Text;
            string connectionInfo = String.Format(@"Data Source=(LocalDB)\{0};AttachDbFilename={1};Integrated Security=True;", version, filename);
            return connectionInfo;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //
            // setup GUI:
            //
            this.lstStations.Items.Add("");
            this.lstStations.Items.Add("[ Use File>>Load to display L stations... ]");
            this.lstStations.Items.Add("");
            this.lstStations.ClearSelected();
            toolStripStatusLabel1.Text = string.Format("Number of stations:  0");

            // 
            // open-close connect to get SQL Server started:
            //

            try
            {
                BusinessTier.Business biztier = new BusinessTier.Business(this.txtDatabaseFilename.Text);
                biztier.TestConnection();
            }
            catch
            {
                //
                // ignore any exception that occurs, goal is just to startup
                //
            }

        }

        //
        // File>>Exit:
        //
        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //
        // File>>Load Stations:
        //
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //
            // clear the UI of any current results:
            //
            ClearStationUI(true /*clear stations*/);

            //
            // now load the stations from the database:
            //
            try
            {
                BusinessTier.Business biztier;
                biztier = new BusinessTier.Business(this.txtDatabaseFilename.Text);
                var stations = biztier.GetStations();

                foreach (BusinessTier.CTAStation station in stations)
                {
                    this.lstStations.Items.Add(station.Name);
                }
                toolStripStatusLabel1.Text = string.Format("Number of stations:  {0:#,##0}", stations.Count);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error: '{0}'.", ex.Message);
                MessageBox.Show(msg);
            }

        }


        //
        // User has clicked on a station for more info:
        //
        private void lstStations_SelectedIndexChanged(object sender, EventArgs e)
        {
            // sometimes this event fires, but nothing is selected...
            if (this.lstStations.SelectedIndex < 0)   // so return now in this case:
                return;

            //
            // clear GUI in case this fails:
            //
            ClearStationUI();

            //
            // now display info about selected station:
            //
            var stationName = this.lstStations.Text;

            try
            {
                BusinessTier.Business biztier;
                biztier = new BusinessTier.Business(this.txtDatabaseFilename.Text);
                var s = biztier.GetStations();
                foreach (var d in s)
                {
                    if (d.Name.Equals(stationName))
                    {
                        txtStationID.Text = d.ID.ToString();
                    }
                }

                DataSet stop_set = biztier.set_stops(stationName, Convert.ToInt32(this.txtStationID.Text));
                foreach (DataRow row in stop_set.Tables["TABLE"].Rows)
                {
                    string msg = String.Format(Convert.ToString(row["Name"]));
                    Console.Write(msg);
                    this.lstStops.Items.Add(msg);
                }

                string string_Answer_Weekday = biztier.getweekend(stationName, Convert.ToInt32(this.txtStationID.Text));
                txtWeekdayRidership.Text = string_Answer_Weekday;

                string string_Answer_Saturday = biztier.getSaturday(stationName, Convert.ToInt32(this.txtStationID.Text));
                txtSaturdayRidership.Text = string_Answer_Saturday;

                string string_Answer_Sunday = biztier.getSunday(stationName, Convert.ToInt32(this.txtStationID.Text));
                txtSundayHolidayRidership.Text = string_Answer_Sunday;

                string total_ridership_count = biztier.total_ridership(stationName, Convert.ToInt32(this.txtStationID.Text));
                txtTotalRidership.Text = total_ridership_count;

                string AvG_ridership = biztier.average_ridership(stationName, Convert.ToInt32(this.txtStationID.Text));
                string day = "/day";
                txtAvgDailyRidership.Text = AvG_ridership + day;

                string peecent_ridership = biztier.get_percent(stationName, Convert.ToInt32(this.txtStationID.Text));
                txtPercentRidership.Text = peecent_ridership;
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error: '{0}'.", ex.Message);
                MessageBox.Show(msg);
            }
        }


        private void ClearStationUI(bool clearStatations = false)
        {
            ClearStopUI();

            this.txtTotalRidership.Clear();
            this.txtTotalRidership.Refresh();

            this.txtAvgDailyRidership.Clear();
            this.txtAvgDailyRidership.Refresh();

            this.txtPercentRidership.Clear();
            this.txtPercentRidership.Refresh();

            this.txtStationID.Clear();
            this.txtStationID.Refresh();

            this.txtWeekdayRidership.Clear();
            this.txtWeekdayRidership.Refresh();

            this.txtSaturdayRidership.Clear();
            this.txtSaturdayRidership.Refresh();

            this.txtSundayHolidayRidership.Clear();
            this.txtSundayHolidayRidership.Refresh();

            this.lstStops.Items.Clear();
            this.lstStops.Refresh();

            this.textBox2.Clear();

            if (clearStatations)
            {
                this.lstStations.Items.Clear();
                this.lstStations.Refresh();
            }
        }

        //
        // user has clicked on a stop for more info:
        //
        private void lstStops_SelectedIndexChanged(object sender, EventArgs e)
        {
            // sometimes this event fires, but nothing is selected...
            if (this.lstStops.SelectedIndex < 0)   // so return now in this case:
                return;

            //
            // clear GUI in case this fails:
            //
            ClearStopUI();

            //
            // now display info about this stop:
            //
            string stopName = this.lstStops.Text;

            try
            {
                BusinessTier.Business biztier;
                biztier = new BusinessTier.Business(this.txtDatabaseFilename.Text);

                string handicap = biztier.get_ADA(stopName, Convert.ToInt32(this.txtStationID.Text));
                txtAccessible.Text = handicap;

                string Direction = biztier.get_direction(stopName, Convert.ToInt32(this.txtStationID.Text));
                txtDirection.Text = Direction;

                string Location = biztier.get_Location(stopName, Convert.ToInt32(this.txtStationID.Text));
                txtLocation.Text = Location;

                DataSet Lines = biztier.getLines(stopName, Convert.ToInt32(this.txtStationID.Text));
                foreach (DataRow row in Lines.Tables["TABLE"].Rows)
                {
                    string msg = String.Format(Convert.ToString(row["Color"]));
                    this.lstLines.Items.Add(msg);
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error: '{0}'.", ex.Message);
                MessageBox.Show(msg);
            }
        }

        private void ClearStopUI()
        {
            this.txtAccessible.Clear();
            this.txtAccessible.Refresh();

            this.txtDirection.Clear();
            this.txtDirection.Refresh();

            this.txtLocation.Clear();
            this.txtLocation.Refresh();

            this.lstLines.Items.Clear();
            this.lstLines.Refresh();

            this.textBox2.Clear();
        }
        //
        // Top-10 stations in terms of ridership:
        //
        private void top10StationsByRidershipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearStationUI(true /*clear stations*/);
            try
            {
                BusinessTier.Business Topten;
                Topten = new BusinessTier.Business(this.txtDatabaseFilename.Text);
                var Stations = Topten.GetTopStations(10);
                foreach (var TopStation in Stations)
                {
                    this.lstStations.Items.Add(TopStation.Name);
                    this.lstStations.Items.Add(TopStation.ID);
                }
                toolStripStatusLabel1.Text = string.Format("Number of stations:  {0:#,##0}", Stations.Count);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error: '{0}'.", ex.Message);
                MessageBox.Show(msg);
            }
        }

        private void txtStationID_TextChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void txtWeekdayRidership_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSaturdayRidership_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSundayHolidayRidership_TextChanged(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void txtTotalRidership_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtAvgDailyRidership_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtPercentRidership_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtAccessible_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtDirection_TextChanged(object sender, EventArgs e)
        {

        }

        private void lstLines_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            lstStations.Items.Clear();
            string string_to_Find = textBox1.Text;
            BusinessTier.Business biztier;
            biztier = new BusinessTier.Business(this.txtDatabaseFilename.Text);
            DataSet Matched_Stations = biztier.find_station(string_to_Find);
            foreach (DataRow row in Matched_Stations.Tables["TABLE"].Rows)
            {
                string msg = String.Format(Convert.ToString(row["Name"]));
                this.lstStations.Items.Add(msg);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void top10StationsBySaturdayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lstStations.Items.Clear();
            BusinessTier.Business biztier;
            biztier = new BusinessTier.Business(this.txtDatabaseFilename.Text);
            var Stations = biztier.GetTopStationsSaturday(10);
            foreach (var TopStation in Stations)
            {
                this.lstStations.Items.Add(TopStation.Name);
                this.lstStations.Items.Add(TopStation.ID);
            }
        }


        private void top10StationsBySundayHolidayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lstStations.Items.Clear();
            BusinessTier.Business biztier;
            biztier = new BusinessTier.Business(this.txtDatabaseFilename.Text);
            var Stations = biztier.GetTopStationsSunday(10);
            foreach (var TopStation in Stations)
            {
                this.lstStations.Items.Add(TopStation.Name);
                this.lstStations.Items.Add(TopStation.ID);
            }
        }

        private void top10StationsByWeekdayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lstStations.Items.Clear();
            BusinessTier.Business biztier;
            biztier = new BusinessTier.Business(this.txtDatabaseFilename.Text);
            var Stations = biztier.GetTopStationsWeekday(10);
            foreach (var TopStation in Stations)
            {
                this.lstStations.Items.Add(TopStation.Name);
                this.lstStations.Items.Add(TopStation.ID);
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            BusinessTier.Business biztier;
            biztier = new BusinessTier.Business(this.txtDatabaseFilename.Text);

            String change = textBox2.Text;
            bool ada;
            if (change.Equals("Y") || change.Equals("y") || change.Equals("yes") || change.Equals("YES"))
                ada = true;
            else ada = false;

            string stopName = this.lstStops.Text;
            biztier.update_ada(stopName, Convert.ToInt32(this.txtStationID.Text), ada);
            MessageBox.Show("ADA changed to " + ada);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }
    }//class
}//namespace
