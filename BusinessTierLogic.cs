//
// BusinessTier:  business logic, acting as interface between UI and data store.
//

using System;
using System.Collections.Generic;
using System.Data;


namespace BusinessTier
{

    //
    // Business:
    //
    public class Business
    {
        //
        // Fields:
        //
        private string _DBFile;
        private DataAccessTier.Data dataTier;


        ///
        /// <summary>
        /// Constructs a new instance of the business tier.  The format
        /// of the filename should be either |DataDirectory|\filename.mdf,
        /// or a complete Windows pathname.
        /// </summary>
        /// <param name="DatabaseFilename">Name of database file</param>
        /// 
        public Business(string DatabaseFilename)
        {
            _DBFile = DatabaseFilename;

            dataTier = new DataAccessTier.Data(DatabaseFilename);
        }


        ///
        /// <summary>
        ///  Opens and closes a connection to the database, e.g. to
        ///  startup the server and make sure all is well.
        /// </summary>
        /// <returns>true if successful, false if not</returns>
        /// 
        public bool TestConnection()
        {
            return dataTier.OpenCloseConnection();
        }


        ///
        /// <summary>
        /// Returns all the CTA Stations, ordered by name.
        /// </summary>
        /// <returns>Read-only list of CTAStation objects</returns>
        /// 


        public IReadOnlyList<CTAStation> GetStations()
        {
            List<CTAStation> stations = new List<CTAStation>();

            try
            {

                DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
                string sql = "Select Name,StationId from stations order by name";
                DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

                foreach (DataRow row in ds.Tables["TABLE"].Rows)
                {
                    String a = Convert.ToString(row["Name"]);
                    int b = Convert.ToInt32(row["StationID"]);

                    CTAStation newstation = new CTAStation(b, a);
                    stations.Add(newstation);
                }

            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetStations: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }

            return stations;
        }


        ///
        /// <summary>
        /// Returns the CTA Stops associated with a given station,
        /// ordered by name.
        /// </summary>
        /// <returns>Read-only list of CTAStop objects</returns>
        ///



        public string get_ADA(String name, int stationID)
        {
            name = name.Replace("'", "''");
            DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
            string sql = string.Format(@"Select ADA from stops where name ='{0}';", name);
            object result = dataTier.ExecuteScalarQuery(sql);
            string ans = Convert.ToString(result);
            if (ans.Equals("True"))
                return "Yes";
            else
                return "False";
        }



        public string get_direction(String name, int stationID)
        {
            name = name.Replace("'", "''");
            DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
            string sql = string.Format(@"Select direction from stops where name ='{0}';", name);
            object result = dataTier.ExecuteScalarQuery(sql);
            string dir = Convert.ToString(result);
            return dir;
        }


        public string get_Location(String name, int stationID)
        {
            name = name.Replace("'", "''");
            DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
            string sql1 = string.Format(@"Select latitude from stops where name ='{0}';", name);
            string sql2 = string.Format(@"Select longitude from stops where name ='{0}';", name);

            object result1 = dataTier.ExecuteScalarQuery(sql1);
            object result2 = dataTier.ExecuteScalarQuery(sql2);

            double latitude = Math.Round(Convert.ToDouble(result1), 4);
            double longitude = Math.Round(Convert.ToDouble(result2), 4);

            string Location = "(" + latitude.ToString() + "," + longitude.ToString() + ")";

            return Location;
        }

        public DataSet getLines(String name, int stationID)
        {
            name = name.Replace("'", "''");
            DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
            string sql = string.Format(@"select color from lines where lineid = any(select lineid from StopDetails where StopID = (select stopid from stops where name ='{0}'));", name);
            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);
            return ds;
        }

        public string getweekend(String name, int stationID)
        {
            name = name.Replace("'", "''");
            DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
            string sql = string.Format(@"select SUM(DailyTotal) from Riderships where StationID = (select stationID from stations where name = '{0}') AND TypeOfDay = 'W';", name);
            object result = dataTier.ExecuteScalarQuery(sql);
            int Answer_Weekday = Convert.ToInt32(result);
            string string_Answer_Weekday = String.Format("{0:#,##0}", Answer_Weekday);
            return string_Answer_Weekday;
        }


        public string getSaturday(String name, int stationID)
        {
            name = name.Replace("'", "''");
            DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
            string sql = string.Format(@"select  SUM(DailyTotal) from Riderships where StationID = (select stationID from stations where name = '{0}') AND TypeOfDay = 'A';", name);
            object result = dataTier.ExecuteScalarQuery(sql);
            int Answer_Saturday = Convert.ToInt32(result);
            string string_Answer_Saturday = String.Format("{0:#,##0}", Answer_Saturday);
            return string_Answer_Saturday;
        }


        public string getSunday(String name, int stationID)
        {
            name = name.Replace("'", "''");
            DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
            string sql = string.Format(@"select  SUM(DailyTotal) from Riderships where StationID = (select stationID from stations where name = '{0}') AND TypeOfDay = 'U';", name);
            object result = dataTier.ExecuteScalarQuery(sql);
            int Answer_Sunday = Convert.ToInt32(result);
            string string_Answer_Sunday = String.Format("{0:#,##0}", Answer_Sunday);
            return string_Answer_Sunday;
        }


        public string total_ridership(String name, int stationID)
        {
            name = name.Replace("'", "''");
            DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
            string sql1 = string.Format(@"select  SUM(DailyTotal) from Riderships where StationID = (select stationID from stations where name = '{0}') AND TypeOfDay = 'U';", name);
            string sql2 = string.Format(@"select  SUM(DailyTotal) from Riderships where StationID = (select stationID from stations where name = '{0}') AND TypeOfDay = 'A';", name);
            string sql3 = string.Format(@"select SUM(DailyTotal) from Riderships where StationID = (select stationID from stations where name = '{0}') AND TypeOfDay = 'W';", name);
            object result1 = dataTier.ExecuteScalarQuery(sql1);
            object result2 = dataTier.ExecuteScalarQuery(sql2);
            object result3 = dataTier.ExecuteScalarQuery(sql3);
            int result = Convert.ToInt32(result1) + Convert.ToInt32(result2) + Convert.ToInt32(result3);
            string total_ridershipcount = String.Format("{0:#,##0}", result);
            double result_double = Convert.ToDouble(result);
      
            string sql4 = string.Format(@" select sum(cast(dailytotal as bigint)) from Riderships;");
            object result4 = dataTier.ExecuteScalarQuery(sql4);
            double result4_double = Convert.ToDouble(result4);

            double percentage = (result_double * 100) / result4_double;
            percentage = Math.Round(percentage, 2);

            return total_ridershipcount;
        }


        public string get_percent(String name, int stationID)
        {
            name = name.Replace("'", "''");
            DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
            string sql1 = string.Format(@"select  SUM(DailyTotal) from Riderships where StationID = (select stationID from stations where name = '{0}') AND TypeOfDay = 'U';", name);
            string sql2 = string.Format(@"select  SUM(DailyTotal) from Riderships where StationID = (select stationID from stations where name = '{0}') AND TypeOfDay = 'A';", name);
            string sql3 = string.Format(@"select SUM(DailyTotal) from Riderships where StationID = (select stationID from stations where name = '{0}') AND TypeOfDay = 'W';", name);
            object result1 = dataTier.ExecuteScalarQuery(sql1);
            object result2 = dataTier.ExecuteScalarQuery(sql2);
            object result3 = dataTier.ExecuteScalarQuery(sql3);
            int result = Convert.ToInt32(result1) + Convert.ToInt32(result2) + Convert.ToInt32(result3);
            string total_ridershipcount = String.Format("{0:#,##0}", result);
            double result_double = Convert.ToDouble(result);

            string sql4 = string.Format(@" select sum(cast(dailytotal as bigint)) from Riderships;");
            object result4 = dataTier.ExecuteScalarQuery(sql4);
            double result4_double = Convert.ToDouble(result4);

            double per = (result_double * 100) / result4_double;
            per = Math.Round(per, 2);

            string percent_ridership = Convert.ToString(per) + "%";
            return percent_ridership;
        }

        public DataSet find_station(string name)
        {
            name = name.Replace("'", "''");
            DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
            string sql = string.Format(@"select name from stations where name like '%{0}%';", name);
            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);
            return ds;
        }


        public DataSet set_stops(String name, int stationID)
        {
            name = name.Replace("'", "''");
            DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
            string sql = string.Format(@" SELECT name FROM stops  WHERE  stationID = (SELECT stationID FROM stations WHERE name = '{0}') order by name ;", name);
            DataSet ds = dataTier.ExecuteNonScalarQuery(sql);
            return ds;
        }

        public string average_ridership(String name, int stationID)
        {
            name = name.Replace("'", "''");
            DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
            string sql = string.Format(@"select AVG(DailyTotal)from Riderships where StationID = (select stationID from stations where name = '{0}'); ", name);
            object result = dataTier.ExecuteScalarQuery(sql);
            string AVG_ridership = String.Format("{0:#,##0}", result);
            return AVG_ridership;
        }

        public void update_ada(String name, int stationID, bool pass)
        {
            name = name.Replace("'", "''");
            DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
            string sql = string.Format(@"update stops set  ada = '{0}' where name= '{1}';", pass, name);
            var ds = dataTier.ExecuteNonScalarQuery(sql);
        }

        public IReadOnlyList<CTAStop> GetStops(int stationID)
        {
            List<CTAStop> stops = new List<CTAStop>();
            try
            {
                DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
                string sql = "Select stopID,Name, stationID,direction,ada,latitude,longitude from stops";
                DataSet ds = dataTier.ExecuteNonScalarQuery(sql);

                foreach (DataRow row in ds.Tables["TABLE"].Rows)
                {
                    int id = Convert.ToInt32(row["StopID"]);
                    String a = Convert.ToString(row["Name"]);
                    int b = Convert.ToInt32(row["StationID"]);
                    string dir = Convert.ToString(row["Direction"]);
                    bool ada = Convert.ToBoolean(row["ADA"]);
                    double lati = Convert.ToDouble(row["Latitude"]);
                    double longi = Convert.ToDouble(row["Longitude"]);

                    CTAStop newstops = new CTAStop(id, a, b, dir, ada, lati, longi);
                    stops.Add(newstops);
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetStops: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }
            return stops;
        }
        ///
        /// <summary>
        /// Returns the top N CTA Stations by ridership, 
        /// ordered by name.
        /// </summary>
        /// <returns>Read-only list of CTAStation objects</returns>
        /// 
        public IReadOnlyList<CTAStation> GetTopStations(int N)
        {
            if (N < 1)
                throw new ArgumentException("GetTopStations: N must be positive");

            List<CTAStation> stations = new List<CTAStation>();

            try
            {
                DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
                string sql = string.Format(@"select st.name as name,daily_total from stations st,(select top 10 StationID,sum(DailyTotal) as daily_total from Riderships group by StationID order by sum(dailytotal)DESC) as temp where st.stationid = temp.StationID;");
                DataSet ds = dataTier.ExecuteNonScalarQuery(sql);
                foreach (DataRow row in ds.Tables["TABLE"].Rows)
                {
                    String a = Convert.ToString(row["name"]);
                    int b = Convert.ToInt32(row["daily_total"]);
                    CTAStation newstation = new CTAStation(b, a);
                    stations.Add(newstation);
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetTopStations: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }
            return stations;
        }


        public IReadOnlyList<CTAStation> GetTopStationsSaturday(int N)
        {
            if (N < 1)
                throw new ArgumentException("GetTopStations: N must be positive");

            List<CTAStation> stations = new List<CTAStation>();
            try
            {
                DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
                string sql = string.Format(@"select st.name as name,daily_total from stations st,(select top 10 StationID,sum(DailyTotal) as daily_total from Riderships where TypeofDay = 'A' group by StationID order by sum(dailytotal)DESC) as temp where st.stationid = temp.StationID;");
                DataSet ds = dataTier.ExecuteNonScalarQuery(sql);
                foreach (DataRow row in ds.Tables["TABLE"].Rows)
                {
                    String a = Convert.ToString(row["name"]);
                    int b = Convert.ToInt32(row["daily_total"]);
                    CTAStation newstation = new CTAStation(b, a);
                    stations.Add(newstation);
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetTopStations: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }

            return stations;
        }

        public IReadOnlyList<CTAStation> GetTopStationsSunday(int N)
        {
            if (N < 1)
                throw new ArgumentException("GetTopStations: N must be positive");

            List<CTAStation> stations = new List<CTAStation>();
            try
            {
                DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
                string sql = string.Format(@"select st.name as name,daily_total from stations st,(select top 10 StationID,sum(DailyTotal) as daily_total from Riderships where TypeofDay = 'U' group by StationID order by sum(dailytotal)DESC) as temp where st.stationid = temp.StationID;");
                DataSet ds = dataTier.ExecuteNonScalarQuery(sql);
                foreach (DataRow row in ds.Tables["TABLE"].Rows)
                {
                    String a = Convert.ToString(row["name"]);
                    int b = Convert.ToInt32(row["daily_total"]);
                    CTAStation newstation = new CTAStation(b, a);
                    stations.Add(newstation);
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetTopStations: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }
            return stations;
        }

        public IReadOnlyList<CTAStation> GetTopStationsWeekday(int N)
        {
            if (N < 1)
                throw new ArgumentException("GetTopStations: N must be positive");

            List<CTAStation> stations = new List<CTAStation>();
            try
            {
                DataAccessTier.Data dataTier = new DataAccessTier.Data(_DBFile);
                string sql = string.Format(@"select st.name as name,daily_total from stations st,(select top 10 StationID,sum(DailyTotal) as daily_total from Riderships where TypeofDay = 'W' group by StationID order by sum(dailytotal)DESC) as temp where st.stationid = temp.StationID;");
                DataSet ds = dataTier.ExecuteNonScalarQuery(sql);
                foreach (DataRow row in ds.Tables["TABLE"].Rows)
                {
                    String a = Convert.ToString(row["name"]);
                    int b = Convert.ToInt32(row["daily_total"]);
                    CTAStation newstation = new CTAStation(b, a);
                    stations.Add(newstation);
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetTopStations: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }
            return stations;
        }

    }//class
}//namespace
