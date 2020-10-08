using System;
using System.Collections.Generic;
using System.Text;

namespace BuildChecker.Interfaces
{
    public interface IBuilder
    {
        string GetDeviceAttributes();
        string GetCallerAttributes();
        string GetProducts();
        string BuildFetchUUPRequest(string uupEncryptedData);
        string BuildFileGetRequest(string uuid, string rev, string uupEncryptedData);
    }
}
