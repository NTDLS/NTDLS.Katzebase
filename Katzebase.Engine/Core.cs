﻿using Katzebase.Engine.Caching;
using Katzebase.Engine.Documents;
using Katzebase.Engine.Health;
using Katzebase.Engine.Indexes;
using Katzebase.Engine.IO;
using Katzebase.Engine.Locking;
using Katzebase.Engine.Logging;
using Katzebase.Engine.Query;
using Katzebase.Engine.Schemas;
using Katzebase.Engine.Sessions;
using Katzebase.Engine.Transactions;
using Katzebase.PrivateLibrary;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Caching;

namespace Katzebase.Engine
{
    public class Core
    {
        internal IOManager IO;
        internal LockManager Locking;
        internal CacheManager Cache;
        internal KatzebaseSettings settings;

        public SchemaManager Schemas;
        public DocumentManager Documents;
        public TransactionManager Transactions;
        public LogManager Log;
        public HealthManager Health;
        public SessionManager Sessions;
        public PersistIndexManager Indexes;
        public QueryManager Query;

        public MemoryCache LookupOptimizationCache { get; set; } = new MemoryCache("ConditionLookupOptimization");

        public Core(KatzebaseSettings settings)
        {
            this.settings = settings;

            Log = new LogManager(this);

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            Log.Write($"{fileVersionInfo.ProductName} v{fileVersionInfo.ProductVersion} PID:{Process.GetCurrentProcess().Id}");

            Log.Write("Initializing cache manager.");
            Cache = new CacheManager(this);

            Log.Write("Initializing IO manager.");
            IO = new IOManager(this);

            Log.Write("Initializing health manager.");
            Health = new HealthManager(this);

            Log.Write("Initializing index manager.");
            Indexes = new PersistIndexManager(this);

            Log.Write("Initializing session manager.");
            Sessions = new SessionManager(this);

            Log.Write("Initializing lock manager.");
            Locking = new LockManager(this);

            Log.Write("Initializing transaction manager.");
            Transactions = new TransactionManager(this);

            Log.Write("Initializing schema manager.");
            Schemas = new SchemaManager(this);

            Log.Write("Initializing document manager.");
            Documents = new DocumentManager(this);

            Log.Write("Initializing query manager.");
            Query = new QueryManager(this);

            Log.Write("Initilization complete.");
        }

        public void Start()
        {
            Log.Write("Starting the server.");

            Transactions.Recover();
        }

        public void Stop()
        {
            Log.Write("Stopping the server.");

            Health.Close();
            Log.Close();
        }
    }
}
