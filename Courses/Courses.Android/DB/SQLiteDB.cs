using SQLite;
using Courses.Droid;
using Xamarin.Forms;
using System.IO;

[assembly: Dependency(typeof(SQLiteDb))]
namespace Courses.Droid
{
    public class SQLiteDb : ISQLiteDb
    {
        public SQLiteAsyncConnection GetConnection()
        {
            var env = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(env, "MySqlLite.db3");
            return new SQLiteAsyncConnection(path);
        }
    }
}