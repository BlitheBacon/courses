using SQLite;

namespace Courses
{
    public interface ISQLiteDb
    {
        SQLiteAsyncConnection GetConnection();
    }
}