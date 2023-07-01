using ForesterAPI.Data;

namespace ForesterAPI.Services
{
    public class RequirementChecker
    {
        AccountDatabase accountDatabase;
        ApplicationDatabase applicationDatabase;

        public RequirementChecker(AccountDatabase accountDatabase, ApplicationDatabase applicationDatabase)
        {
            this.accountDatabase = accountDatabase;
            this.applicationDatabase = applicationDatabase;
        }

        public bool DoesOwnRequirement(string login, string name)
        {
            if (!accountDatabase.DoesExist(login))
                return false;

            var account = accountDatabase.Get(login);

            var developed = applicationDatabase.GetOwned(account);

            if (!developed.Any(x => x.Name == name))
                return false;

            return true;
        }

        public bool DoesDevelopRequirement(string login, string name)
        {
            if (!accountDatabase.DoesExist(login))
                return false;

            var account = accountDatabase.Get(login);

            var developed = applicationDatabase.GetDeveloped(account);

            if (!developed.Any(x => x.Name == name))
                return false;

            return true;
        }

        public bool IsAllowedRequirement(string login, string name)
        {
            if (!accountDatabase.DoesExist(login))
                return false;

            var account = accountDatabase.Get(login);

            var developed = applicationDatabase.GetAllowed(account);

            if (!developed.Any(x => x.Name == name))
                return false;

            return true;
        }
    }
}
