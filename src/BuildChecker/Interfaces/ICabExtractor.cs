using System;
using System.Collections.Generic;
using System.Text;

namespace BuildChecker.Interfaces
{
    public interface ICabExtractor
    {
        bool Extract(string sourceFile, string destFolder, string filter);
    }
}
