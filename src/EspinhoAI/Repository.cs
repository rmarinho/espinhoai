using System;
using EspinhoAI.Models;
using SQLite;

namespace EspinhoAI
{
	public class Repository
	{
        private readonly SQLiteConnection _database;

        public Repository()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "entities.db");
             _database = new SQLiteConnection(dbPath);
            _database.CreateTable<Doc>();
        }

        public List<Doc> List()
        {
            try
            {
                var table = _database.Table<Doc>();
               
                return table.ToList();
            }
            catch (Exception ex)
            {
                return new List<Doc>();
            }
        }

        public int Create(Doc entity)
        {
            return _database.Insert(entity);
        }

        public int Update(Doc entity)
        {
            return _database.Update(entity);
        }

        public int Delete(Doc entity)
        {
            return _database.Delete(entity);
        }
    }
}

