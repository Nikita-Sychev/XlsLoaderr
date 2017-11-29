using System;
using System.Configuration;
using Configuration;
using Contracts;
using SqlWork;

namespace XlsLoader.Base.Connect
{
    public class BaseService : IDisposable
    {
        public BaseService(IUserIdentity identity, string connName)
        {
            var connStr = CoreConfig.GetConnectionString(connName);
            sqlExecutor = SqlExecutor.CreateOracleExecutor(connStr, identity, false, ConfigurationManager.AppSettings.Get("AllowAnanimus") == "true");
        }
        protected SqlExecutor sqlExecutor { get; set; }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    sqlExecutor.Dispose();
                }
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
