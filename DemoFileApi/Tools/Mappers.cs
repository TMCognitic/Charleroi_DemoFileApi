using DemoFileApi.Models;
using System.Data;

namespace DemoFileApi.Tools
{
    public static class Mappers
    {
        internal static FileDescription ToFileDescription(this IDataRecord record)
        {
            return new FileDescription()
            {
                Uid = (Guid)record["Uid"],
                FileName = (string)record["FileName"],
                Enable = (bool)record["Enable"]
            };
        }

        internal static FileContent ToFileContent(this IDataRecord record)
        {
            return new FileContent()
            {
                FileName = (string)record["FileName"],
                Content = (byte[])record["FileContent"]
            };
        }
    }
}
