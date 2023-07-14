using Forester.Data.Connection;
using Forester.Models.API;
using Forester.Models.Configurations;
using Forester.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Forester.Data
{
    public abstract class Database
    {
        protected abstract string APIprefix { get; }

        RequestManager? requestManager;
        protected RequestManager RequestManager { get => requestManager!; }

        FileTransferManager? fileTransferManager;
        protected FileTransferManager FileTransferManager { get => fileTransferManager!; }

        ErrorMessageService errorMessageService;

        public Database(RequestManager requestManager, ErrorMessageService errorMessageService)
        {
            this.requestManager = requestManager;
            this.errorMessageService = errorMessageService;
        }

        public Database(FileTransferManager fileTransferManager, ErrorMessageService errorMessageService)
        {
            this.fileTransferManager = fileTransferManager;
            this.errorMessageService = errorMessageService;
        }

        public Database(RequestManager requestManager, FileTransferManager fileTransferManager, ErrorMessageService errorMessageService)
        {
            this.requestManager = requestManager;
            this.fileTransferManager = fileTransferManager;
            this.errorMessageService = errorMessageService;
        }

        protected void SendError(string friendlyError, HttpStatusCode statusCode, string technicalError)
        {
            errorMessageService.SendErrorMessage(new ErrorMessage()
            {
                FriendlyMessage = friendlyError,
                StatusCode = statusCode,
                TechnicalMessage = technicalError 
            });
        }

        protected string GenerateURI(string action) => (action.StartsWith('?') ? APIprefix.Remove(APIprefix.Length - 1) : APIprefix) + action;
        protected string GenerateURI() => APIprefix.Remove(APIprefix.Length - 1);
    }
}
