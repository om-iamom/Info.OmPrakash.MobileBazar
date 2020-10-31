using MobileBazar.Catalog.API.Settings.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileBazar.Catalog.API.Settings.Services
{
    public class CatalogDatabaseSettings : ICatalogDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set ; }
        public string CollectionName { get ; set ; }
    }
}
