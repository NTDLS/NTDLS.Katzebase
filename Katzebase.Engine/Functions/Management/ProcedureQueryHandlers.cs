﻿using Katzebase.Engine.Query;
using Katzebase.PublicLibrary.Payloads;

namespace Katzebase.Engine.Functions.Management
{
    /// <summary>
    /// Internal class methods for handling query requests related to procedures.
    /// </summary>
    internal class ProcedureQueryHandlers
    {
        private readonly Core core;

        public ProcedureQueryHandlers(Core core)
        {
            this.core = core;

            try
            {
            }
            catch (Exception ex)
            {
                core.Log.Write($"Failed to instanciate procedures query handler.", ex);
                throw;
            }
        }

        internal KbQueryResultCollection ExecuteExec(ulong processId, PreparedQuery preparedQuery)
        {
            try
            {
                using (var txRef = core.Transactions.Acquire(processId))
                {
                    var result = core.Procedures.ExecuteProcedure(txRef.Transaction, preparedQuery.ProcedureCall);

                    txRef.Commit();
                    result.Metrics = txRef.Transaction.PT?.ToCollection();
                    //The KbQueryResultCollection is a collection of resutls, each will have their own warnings and messages.
                    //result.Messages = txRef.Transaction.Messages;
                    //result.Warnings = txRef.Transaction.Warnings;
                    return result;
                }
            }
            catch (Exception ex)
            {
                core.Log.Write($"Failed to execute document sample for process id {processId}.", ex);
                throw;
            }
        }
    }
}
