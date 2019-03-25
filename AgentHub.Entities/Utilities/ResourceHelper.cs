using System.IO;
using System.Reflection;

namespace AgentHub.Entities.Utilities
{
    public static class ResourceHelper
    {
        public static string ReadResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    return string.Empty;

                using (var reader = new StreamReader(stream))
                {
                    var sqlScript = reader.ReadToEnd();
                    
                    return sqlScript;
                }
            }
        }
    }
}
