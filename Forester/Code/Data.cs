using Forester.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forester
{
    /// <summary>
    /// Storage of needed data to run program smoothly
    /// </summary>
    public static class Data
    {
        public static Config config;
        public static Account currentAccount;
        public static List<Application> publicAppliactions;
        public static List<Application> privateAllowedApplications;
        public static List<Application> getAllApps
        {
            get
            {
                List<Application> final = new List<Application>();
                final.AddRange(publicAppliactions);
                final.AddRange(privateAllowedApplications);
                return final;
            }
        }
        static Data()
        {
            publicAppliactions = new List<Application>();
            privateAllowedApplications = new List<Application>();
        }

        public static class BasePage
        {
            public static string appName;
            public static string appDescription;
        }
    }
}
