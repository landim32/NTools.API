using System;
using Core.Domain;
using DB.Infra.Context;
using NAuth.Domain.Impl.Core;
using NAuth.Domain.Interfaces.Core;

namespace DB.Infra
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly NAuthContext _ccsContext;
        private readonly ILogCore _log;

        public UnitOfWork(ILogCore log, NAuthContext ccsContext)
        {
            this._ccsContext = ccsContext;
            _log = log;
        }

        public ITransaction BeginTransaction()
        {
            try
            {
                _log.Log("Iniciando bloco de transação.", Levels.Trace);
                return new TransactionDisposable(_log, _ccsContext.Database.BeginTransaction());
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}
