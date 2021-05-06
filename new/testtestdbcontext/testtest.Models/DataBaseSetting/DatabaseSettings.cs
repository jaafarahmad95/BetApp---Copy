using System;
using System.Collections.Generic;
using System.Text;

namespace testtest.Models.DataBaseSetting
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string AppUserCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IDatabaseSettings
    {
        string AppUserCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
