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
            _database.CreateTable<ItemScrapped>();
        }

        public List<Doc> Docs()
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

        public List<ItemScrapped> Items()
        {
            try
            {
                var table = _database.Table<ItemScrapped>();

                return table.ToList();
            }
            catch (Exception ex)
            {
                return new List<ItemScrapped>();
            }
        }

        public int Create<T>(T entity)
        {
            return _database.Insert(entity);
        }

        public int Update<T>(T entity)
        {
            return _database.Update(entity);
        }

        public int Delete(Doc entity)
        {
            return _database.Delete(entity);
        }
    }
}

