using Forester.Models.API;
using Forester.Views.App.Developer.ManageApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Forester.ViewModels.App.Developer.ManageApp
{
    internal class Allowance : ViewModelBase
    {
        public static Allowance Current;

        public Allowance()
        {
            System.Diagnostics.Debug.WriteLine("Allowence - Coś jest złego!!!!!!!!!!!!!!!!");

            Current = this;
        }

        public void UpdateDevelopersAllowed(bool isDeveloper = false)
        {
            Allowed allowed = new Allowed()
            {
                name = ManageAppViewModel.Current.currentApp.name,
                allowedDevelopers = Codevelopers.Current.developers.ToArray(),
                allowedUsers = PrivateAccess.Current.allowedUsers.ToArray()
            };

            var response = ServerConnection.Put("/api/Applications/UpdateAllowed", allowed);

            if (response.StatusCode != HttpStatusCode.OK)
                if (isDeveloper)
                    Codevelopers.Current.SetErrorMessage("User not found");
                else
                    PrivateAccess.Current.SetErrorMessage("User not found");
            else
                if (isDeveloper)
                    Codevelopers.Current.SetErrorMessage("Added");
                else
                    PrivateAccess.Current.SetErrorMessage("Added");

            SetAllowedUsers(ManageAppViewModel.Current.currentApp);
        }

        public void SetAllowedUsers(Application app)
        {
            var response = ServerConnection.Get("/api/Applications/GetAllowed?name=" + app.name);

            var bank = JsonConverter.Deserialize<Allowed>(response.Content.ReadAsStringAsync().Result);

            PrivateAccess.Current.SetAllowedUsers(bool.Parse(app.isPrivate), bank.allowedUsers);
            Codevelopers.Current.SetAllowedDevelopers(bank.allowedDevelopers);
        }
    }
}
