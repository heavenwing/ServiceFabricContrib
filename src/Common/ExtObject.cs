using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ServiceFabricContrib
{
    public static class ExtObject
    {
        public static T DeepCopy<T>(this T objectToCopy)
        {
            var memoryStream = new MemoryStream();
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, objectToCopy);


            memoryStream.Position = 0;
            T returnValue = (T)binaryFormatter.Deserialize(memoryStream);


            memoryStream.Close();
            memoryStream.Dispose();


            return returnValue;
        }
    }
}
