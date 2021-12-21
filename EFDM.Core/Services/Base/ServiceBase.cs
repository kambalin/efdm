using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDM.Core.Services.Base {

    public abstract class ServiceBase {

        protected readonly ILogger Logger;

        #region constructors

        public ServiceBase(ILogger logger) {
            this.Logger = logger;
        }

        #endregion constructors

        #region log

        protected void WriteLog(string message) {
            if (Logger == null)
                return;
            Logger.LogInformation(message);
        }

        protected void WriteError(Exception exception, string message = null) {
            if (Logger == null)
                return;
            Logger.LogError(exception, message);
        }

        protected void WriteError(string message) {
            if (Logger == null)
                return;
            Logger.LogError(message);
        }

        #endregion log
    }
}
