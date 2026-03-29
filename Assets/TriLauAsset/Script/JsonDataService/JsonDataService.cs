using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace MyRule.DataService
{
    public class Vector2Converter : JsonConverter<Vector2>
    {
        public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x"); writer.WriteValue(value.x);
            writer.WritePropertyName("y"); writer.WriteValue(value.y);
            writer.WriteEndObject();
        }
        public override Vector2 ReadJson(JsonReader reader, Type objectType,
            Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            float x = 0, y = 0;
            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                if (reader.TokenType != JsonToken.PropertyName) continue;
                string prop = (string)reader.Value;
                reader.Read();
                if (prop == "x") x = Convert.ToSingle(reader.Value);
                else if (prop == "y") y = Convert.ToSingle(reader.Value);
            }
            return new Vector2(x, y);
        }
    }

    public class Vector2IntConverter : JsonConverter<Vector2Int>
    {
        public override void WriteJson(JsonWriter writer, Vector2Int value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x"); writer.WriteValue(value.x);
            writer.WritePropertyName("y"); writer.WriteValue(value.y);
            writer.WriteEndObject();
        }
        public override Vector2Int ReadJson(JsonReader reader, Type objectType,
            Vector2Int existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            int x = 0, y = 0;
            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                if (reader.TokenType != JsonToken.PropertyName) continue;
                string prop = (string)reader.Value;
                reader.Read();
                if (prop == "x") x = Convert.ToInt32(reader.Value);
                else if (prop == "y") y = Convert.ToInt32(reader.Value);
            }
            return new Vector2Int(x, y);
        }
    }

    public class JsonDataService : IDataService
    {
        private const string KEY = "ggdPhkeOoiv6YMiPWa34kIuOdDUL7NwQFg6l1DVdwN8=";
        private const string IV = "JZuM0HQsWSBVpRHTeRZMYQ==";

        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Converters = { new Vector2Converter(), new Vector2IntConverter() }
        };

        #region Load & Save
        public T LoadData<T>(string RelativePath, bool Encrypted)
        {
            string path = Application.persistentDataPath + RelativePath;

            if (!File.Exists(path))
            {
                Debug.LogWarning($"File not found at {path}. Returning default value.");
                return default;
            }

            try
            {
                if (Encrypted)
                    return ReadEncryptedData<T>(path);

                return JsonConvert.DeserializeObject<T>(File.ReadAllText(path), Settings);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load data due to: {e.Message} {e.StackTrace}");
                return default;
            }
        }

        public bool SaveData<T>(string RelativePath, T Data, bool Encrypted)
        {
            string path = Application.persistentDataPath + RelativePath;

            try
            {
                if (File.Exists(path))
                {
                    Debug.Log("Data exists. Deleting old file and writing a new one!");
                    File.Delete(path);
                }
                else
                {
                    Debug.Log("Writing file for the first time!");
                }

                using FileStream stream = File.Create(path);
                if (Encrypted)
                {
                    WriteEncryptedData(Data, stream);
                }
                else
                {
                    stream.Close();
                    File.WriteAllText(path, JsonConvert.SerializeObject(Data, Formatting.Indented, Settings));
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to save data due to: {e.Message} {e.StackTrace}");
                return false;
            }
        }
        #endregion

        #region Write & Read Encrypted Data
        private void WriteEncryptedData<T>(T Data, FileStream Stream)
        {
            using Aes aesProvider = Aes.Create();
            aesProvider.Key = Convert.FromBase64String(KEY);
            aesProvider.IV = Convert.FromBase64String(IV);
            using ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor();
            using CryptoStream cryptoStream = new CryptoStream(Stream, cryptoTransform, CryptoStreamMode.Write);

            Debug.Log($"Initialization Vector: {Convert.ToBase64String(aesProvider.IV)}");
            Debug.Log($"Key: {Convert.ToBase64String(aesProvider.Key)}");

            string json = JsonConvert.SerializeObject(Data, Formatting.Indented, Settings);
            cryptoStream.Write(Encoding.ASCII.GetBytes(json));
        }

        private T ReadEncryptedData<T>(string Path)
        {
            byte[] fileBytes = File.ReadAllBytes(Path);
            using Aes aesProvider = Aes.Create();
            aesProvider.Key = Convert.FromBase64String(KEY);
            aesProvider.IV = Convert.FromBase64String(IV);

            using ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor(aesProvider.Key, aesProvider.IV);
            using MemoryStream decryptionStream = new MemoryStream(fileBytes);
            using CryptoStream cryptoStream = new CryptoStream(decryptionStream, cryptoTransform, CryptoStreamMode.Read);
            using StreamReader reader = new StreamReader(cryptoStream);

            string result = reader.ReadToEnd();
            Debug.Log($"Decrypted result: {result}");
            return JsonConvert.DeserializeObject<T>(result, Settings);
        }
        #endregion
    }
}