using ForesterAPI.Data.Connection;

namespace ForesterAPI.Data
{
    public class ApplicationDatabase
    {
        SQLManager sqlManager;

        public ApplicationDatabase(SQLManager SQLManager)
        {
            this.sqlManager = SQLManager;
        }

    }
}
