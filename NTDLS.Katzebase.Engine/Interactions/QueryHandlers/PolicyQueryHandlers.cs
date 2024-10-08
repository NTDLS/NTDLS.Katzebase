﻿using NTDLS.Katzebase.Api.Exceptions;
using NTDLS.Katzebase.Api.Payloads.Response;
using NTDLS.Katzebase.Engine.Sessions;
using NTDLS.Katzebase.Parsers.Query.SupportingTypes;
using static NTDLS.Katzebase.Parsers.Constants;

namespace NTDLS.Katzebase.Engine.Interactions.QueryHandlers
{
    /// <summary>
    /// Internal class methods for handling query requests related to policies.
    /// </summary>
    internal class PolicyQueryHandlers
    {
        private readonly EngineCore _core;

        public PolicyQueryHandlers(EngineCore core)
        {
            _core = core;

            try
            {
            }
            catch (Exception ex)
            {
                Management.LogManager.Error($"Failed to instantiate schema query handler.", ex);
                throw;
            }
        }

        internal KbActionResponse ExecuteCreateAccount(SessionState session, PreparedQuery preparedQuery)
        {
            try
            {
                using var transactionReference = _core.Transactions.APIAcquire(session);

                if (preparedQuery.SubQueryType == SubQueryType.Account)
                {
                    var accountName = preparedQuery.GetAttribute<string>(PreparedQuery.Attribute.Username);
                    var passwordHash = preparedQuery.GetAttribute<string>(PreparedQuery.Attribute.PasswordHash);
                    _core.Policies.CreateAccount(transactionReference.Transaction, accountName, passwordHash);
                }
                else
                {
                    throw new KbNotImplementedException();
                }

                return transactionReference.CommitAndApplyMetricsThenReturnResults();
            }
            catch (Exception ex)
            {
                Management.LogManager.Error($"Failed to execute account create for process id {session.ProcessId}.", ex);
                throw;
            }
        }

        internal KbActionResponse ExecuteCreateRole(SessionState session, PreparedQuery preparedQuery)
        {
            try
            {
                using var transactionReference = _core.Transactions.APIAcquire(session);

                if (preparedQuery.SubQueryType == SubQueryType.Role)
                {
                    var roleName = preparedQuery.GetAttribute<string>(PreparedQuery.Attribute.RoleName);
                    var IsAdministrator = preparedQuery.GetAttribute(PreparedQuery.Attribute.IsAdministrator, false);
                    _core.Policies.CreateRole(transactionReference.Transaction, roleName, IsAdministrator);
                }
                else
                {
                    throw new KbNotImplementedException();
                }

                return transactionReference.CommitAndApplyMetricsThenReturnResults();
            }
            catch (Exception ex)
            {
                Management.LogManager.Error($"Failed to execute role create for process id {session.ProcessId}.", ex);
                throw;
            }
        }

        internal KbActionResponse ExecuteAddUserToRole(SessionState session, PreparedQuery preparedQuery)
        {
            try
            {
                using var transactionReference = _core.Transactions.APIAcquire(session);

                if (preparedQuery.SubQueryType == SubQueryType.AddUserToRole)
                {
                    var roleName = preparedQuery.GetAttribute<string>(PreparedQuery.Attribute.RoleName);
                    var username = preparedQuery.GetAttribute<string>(PreparedQuery.Attribute.Username);
                    _core.Policies.AddUserToRole(transactionReference.Transaction, roleName, username);
                }
                else
                {
                    throw new KbNotImplementedException();
                }

                return transactionReference.CommitAndApplyMetricsThenReturnResults();
            }
            catch (Exception ex)
            {
                Management.LogManager.Error($"Failed to execute add user to role for process id {session.ProcessId}.", ex);
                throw;
            }
        }

        internal KbActionResponse ExecuteRemoveUserFromRole(SessionState session, PreparedQuery preparedQuery)
        {
            try
            {
                using var transactionReference = _core.Transactions.APIAcquire(session);

                if (preparedQuery.SubQueryType == SubQueryType.RemoveUserFromRole)
                {
                    var roleName = preparedQuery.GetAttribute<string>(PreparedQuery.Attribute.RoleName);
                    var username = preparedQuery.GetAttribute<string>(PreparedQuery.Attribute.Username);
                    _core.Policies.RemoveUserFromRole(transactionReference.Transaction, roleName, username);
                }
                else
                {
                    throw new KbNotImplementedException();
                }

                return transactionReference.CommitAndApplyMetricsThenReturnResults();
            }
            catch (Exception ex)
            {
                Management.LogManager.Error($"Failed to execute add user to role for process id {session.ProcessId}.", ex);
                throw;
            }
        }
    }
}
