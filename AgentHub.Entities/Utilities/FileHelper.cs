using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentHub.Entities.Utilities
{
    public static class FileHelper
    {
        public static void DeleteFile(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);
            }
            catch (Exception exception)
            {
                LogHelper.LogException(exception);
            }
        }
    }
}
