using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace TechJobsConsole
{
    class JobData
    {
        static List<Dictionary<string, string>> AllJobs = new List<Dictionary<string, string>>();
        static bool IsDataLoaded = false;

        public static List<Dictionary<string, string>> FindAll()
        {
            LoadData();
            return DeepCopyAllJobs(AllJobs);
        }

        /*
         * Returns a list of all values contained in a given column,
         * without duplicates. 
         */
        public static List<string> FindAll(string column)
        {
            LoadData();

            List<string> values = new List<string>();

            foreach (Dictionary<string, string> job in AllJobs)
            {
                string aValue = job[column];

                if (!values.Contains(aValue))
                {
                    values.Add(aValue);
                }
            }

            values.Sort();
            return values;
        }

        public static List<Dictionary<string, string>> FindByColumnAndValue(string column, string value)
        {
            // load data, if not already loaded
            LoadData();

            List<Dictionary<string, string>> jobs = new List<Dictionary<string, string>>();

            foreach (Dictionary<string, string> row in AllJobs)
            {
                string aValue = row[column];

                if (StringNoCaseContains(aValue, value))
                {
                    jobs.Add(DeepCopyJob(row));
                }
            }

            return jobs;
        }

        public static List<Dictionary<string, string>> FindByValue(string value)
        {
            // load data, if not already loaded
            LoadData();

            List<Dictionary<string, string>> jobs = new List<Dictionary<string, string>>();

            foreach (Dictionary<string, string> row in AllJobs) // for each job
            {
                foreach (KeyValuePair<string, string> kvp in row) // for field in a job
                {                                     
                    if (StringNoCaseContains(kvp.Value, value)) // ignore case
                    {
                        jobs.Add(DeepCopyJob(row));

                        // this field is a match, no need to check the other fields.  Break out and inspect next job.
                        break; 
                    }

                }
            }

            return jobs;
        }

        /*
         * Load and parse data from job_data.csv
         */
        private static void LoadData()
        {

            if (IsDataLoaded)
            {
                return;
            }

            List<string[]> rows = new List<string[]>();

            using (StreamReader reader = File.OpenText("job_data.csv"))
            {
                while (reader.Peek() >= 0)
                {
                    string line = reader.ReadLine();
                    string[] rowArrray = CSVRowToStringArray(line);
                    if (rowArrray.Length > 0)
                    {
                        rows.Add(rowArrray);
                    }
                }
            }

            string[] headers = rows[0];
            rows.Remove(headers);

            // Parse each row array into a more friendly Dictionary
            foreach (string[] row in rows)
            {
                Dictionary<string, string> rowDict = new Dictionary<string, string>();

                for (int i = 0; i < headers.Length; i++)
                {
                    rowDict.Add(headers[i], row[i]);
                }
                AllJobs.Add(rowDict);
            }

            IsDataLoaded = true;
        }

        /*
         * Parse a single line of a CSV file into a string array
         */
        private static string[] CSVRowToStringArray(string row, char fieldSeparator = ',', char stringSeparator = '\"')
        {
            bool isBetweenQuotes = false;
            StringBuilder valueBuilder = new StringBuilder();
            List<string> rowValues = new List<string>();

            // Loop through the row string one char at a time
            foreach (char c in row.ToCharArray())
            {
                if ((c == fieldSeparator && !isBetweenQuotes))
                {
                    rowValues.Add(valueBuilder.ToString());
                    valueBuilder.Clear();
                }
                else
                {
                    if (c == stringSeparator)
                    {
                        isBetweenQuotes = !isBetweenQuotes;
                    }
                    else
                    {
                        valueBuilder.Append(c);
                    }
                }
            }

            // Add the final value
            rowValues.Add(valueBuilder.ToString());
            valueBuilder.Clear();

            return rowValues.ToArray();
        }

        // Performs case sensitive check whether the value string contains the search string.
        private static bool StringNoCaseContains(string value, string search)
        {
            return ((value.ToUpper()).Contains(search.ToUpper()));
        }

        private static Dictionary<string, string> DeepCopyJob(Dictionary<string, string> copyMe)
        {
            Dictionary<string, string> copiedJob = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> kvp in copyMe)
            {
                copiedJob.Add(kvp.Key, kvp.Value);
            }
            return copiedJob;
        }

        private static List<Dictionary<string,string>> DeepCopyAllJobs(List<Dictionary<string,string>> copyMe)
        {
            List<Dictionary<string, string>> copiedList = new List<Dictionary<string, string>>();
            foreach (Dictionary<string, string> job in copyMe)
            {
                copiedList.Add(DeepCopyJob(job));
            }
            return copiedList;
        }
    }
}
