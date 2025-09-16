using Cysharp.Threading.Tasks;
using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class FileManager : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SaveImageToIndexedDB(string fileName, string base64Data, int dataLength, string callbackObjectName, string callbackMethodName);

    [DllImport("__Internal")]
    private static extern void LoadImageFromIndexedDB(string fileName, string callbackObjectName, string callbackMethodName);
#endif

    public static string ReadFile(string fileName)
    {
        try
        {
            string path = GetFilePath(fileName);
            if (File.Exists(path))
            {
                using StreamReader reader = new StreamReader(path);
                return reader.ReadToEnd();
            }
            Logger.LogWarning($"File not found: {path}", "FileManager");
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to read file {fileName}: {ex.Message}", "FileManager");
            return null;
        }
    }

    public static async UniTask WriteToFile(string fileName, string content)
    {
        try
        {
            string path = GetFilePath(fileName);
            using (var fileStream = new FileStream(path, FileMode.Create))
            using (var writer = new StreamWriter(fileStream))
            {
                await writer.WriteAsync(content);
                Logger.Log($"File written to: {path}", "FileManager");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to write file {fileName}: {ex.Message}", "FileManager");
        }
    }

    public static bool FileExists(string fileName)
    {
        try
        {
            return File.Exists(GetFilePath(fileName));
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error checking file existence {fileName}: {ex.Message}", "FileManager");
            return false;
        }
    }

    public static string GetFilePath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }

    public static async UniTask<string> SaveImage(string data, bool isBase64 = false)
    {
        string fileName = $"catch_{DateTime.Now.Ticks}.jpg";
#if UNITY_WEBGL && !UNITY_EDITOR
        string savePath = fileName;
#else
        string savePath = GetFilePath(fileName);
#endif
        Logger.Log($"Saving image to: {savePath}", "FileManager");

        try
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            string base64 = isBase64 ? data : Convert.ToBase64String(File.ReadAllBytes(data));
            if (string.IsNullOrEmpty(base64))
            {
                Debug.LogError("Base64 data is empty");
                return null;
            }
            Logger.Log($"Saving base64 data (length: {base64.Length}) to IndexedDB", "FileManager");

            var tcs = new UniTaskCompletionSource<string>();
            var callbackObject = new GameObject("ImageSaveCallback");
            var callbackComponent = callbackObject.AddComponent<ImageSaveCallback>();
            callbackComponent.SetCallback((result) =>
            {
                if (result == "success")
                {
                    tcs.TrySetResult(savePath);
                }
                else
                {
                    tcs.TrySetException(new Exception($"Save error: {result}"));
                }
            });

            SaveImageToIndexedDB(fileName, base64, base64.Length, callbackObject.name, nameof(ImageSaveCallback.OnImageSaved));
            return await tcs.Task;
#else
            byte[] imageBytes = isBase64 ? Convert.FromBase64String(data) : File.ReadAllBytes(data);
            Logger.Log($"Writing {imageBytes.Length} bytes to: {savePath}", "FileManager");
            await File.WriteAllBytesAsync(savePath, imageBytes);
            return savePath;
#endif
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to save image {fileName}: {ex.Message}", "FileManager");
            return null;
        }
        finally
        {
            var obj = GameObject.Find("ImageSaveCallback");
            if (obj != null) Destroy(obj);
        }
    }

    public static async UniTask<string> SaveTexture(Texture2D texture, string fileNamePrefix = "qr")
    {
        string fileName = $"{fileNamePrefix}_{DateTime.Now.Second}_{DateTime.Now.Ticks}.png";
#if UNITY_WEBGL && !UNITY_EDITOR
        string savePath = fileName; 
#else
        string savePath = GetFilePath(fileName);
#endif
        Logger.Log($"Saving texture to: {savePath}", "FileManager");

        try
        {
            byte[] textureBytes = texture.EncodeToPNG();
            if (textureBytes == null || textureBytes.Length == 0)
            {
                Logger.LogError("Texture encoding failed: Empty or null bytes", "FileManager");
                return null;
            }

#if UNITY_WEBGL && !UNITY_EDITOR
            string base64 = Convert.ToBase64String(textureBytes);
            if (string.IsNullOrEmpty(base64))
            {
                Logger.LogError("Base64 conversion failed for texture", "FileManager");
                return null;
            }
            Logger.Log($"Saving texture base64 data (length: {base64.Length}) to IndexedDB", "FileManager");

            var tcs = new UniTaskCompletionSource<string>();
            var callbackObject = new GameObject("ImageSaveCallback");
            var callbackComponent = callbackObject.AddComponent<ImageSaveCallback>();
            callbackComponent.SetCallback((result) =>
            {
                if (result == "success")
                {
                    tcs.TrySetResult(savePath);
                }
                else
                {
                    tcs.TrySetException(new Exception($"Save texture error: {result}"));
                }
            });

            SaveImageToIndexedDB(fileName, base64, base64.Length, callbackObject.name, nameof(ImageSaveCallback.OnImageSaved));
            return await tcs.Task;
#else
            await File.WriteAllBytesAsync(savePath, textureBytes);
            Logger.Log($"Texture saved to: {savePath}", "FileManager");
            return savePath;
#endif
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to save texture {fileName}: {ex.Message}", "FileManager");
            return null;
        }
        finally
        {
            var obj = GameObject.Find("ImageSaveCallback");
            if (obj != null) Destroy(obj);
        }
    }

    public static async UniTask<Texture2D> LoadImage(string fileName)
    {
        Logger.Log($"Loading image: {fileName}", "FileManager");

        try
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            string loadFileName = Path.GetFileName(fileName);
            var tcs = new UniTaskCompletionSource<string>();
            var callbackObject = new GameObject("ImageLoadCallback");
            var callbackComponent = callbackObject.AddComponent<ImageLoadCallback>();
            callbackComponent.SetCallback((result) =>
            {
                if (string.IsNullOrEmpty(result) || result.StartsWith("Error:") || result.StartsWith("IndexedDB"))
                {
                    tcs.TrySetException(new Exception($"Load error: {result ?? "Null data"}"));
                }
                else
                {
                    tcs.TrySetResult(result);
                }
            });

            LoadImageFromIndexedDB(loadFileName, callbackObject.name, nameof(ImageLoadCallback.OnImageLoaded));
            string base64Data = await tcs.Task;

            byte[] imageBytes = Convert.FromBase64String(base64Data);
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(imageBytes))
            {
                Logger.Log($"Image loaded successfully: {fileName}", "FileManager");
                return texture;
            }
            else
            {
                Logger.LogError($"Failed to load image data: {fileName}", "FileManager");
                Destroy(texture);
                return null;
            }
#else
            string path = fileName.StartsWith(Application.persistentDataPath) ? fileName : GetFilePath(fileName);
            if (!File.Exists(path))
            {
                Logger.LogError($"File not found: {path}", "FileManager");
                return null;
            }

            byte[] imageBytes = await File.ReadAllBytesAsync(path);
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(imageBytes))
            {
                Logger.Log($"Image loaded successfully: {path}", "FileManager");
                return texture;
            }
            else
            {
                Logger.LogError($"Failed to load image data: {path}", "FileManager");
                Destroy(texture);
                return null;
            }
#endif
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to load image {fileName}: {ex.Message}", "FileManager");
            return null;
        }
        finally
        {
            var obj = GameObject.Find("ImageLoadCallback");
            if (obj != null) Destroy(obj);
        }
    }

    private class ImageLoadCallback : MonoBehaviour
    {
        private Action<string> callback;

        public void SetCallback(Action<string> cb)
        {
            callback = cb;
        }

        public void OnImageLoaded(string base64Data)
        {
            Logger.Log($"Received base64 data from IndexedDB (length: {base64Data?.Length ?? 0}): {(base64Data != null && base64Data.Length > 50 ? base64Data.Substring(0, 50) + "..." : base64Data ?? "null")}", "ImageLoadCallback");
            callback?.Invoke(base64Data);
            Destroy(gameObject);
        }
    }

    private class ImageSaveCallback : MonoBehaviour
    {
        private Action<string> callback;

        public void SetCallback(Action<string> cb)
        {
            callback = cb;
        }

        public void OnImageSaved(string result)
        {
            Logger.Log($"Save result: {result}", "ImageSaveCallback");
            callback?.Invoke(result);
            Destroy(gameObject);
        }
    }
}