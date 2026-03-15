using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.Collections;
using System.Collections.Generic;

public class AddressableLoader : MonoBehaviour
{
    public static AddressableLoader Instance { get; private set; }

    public event Action<float> OnDownloadProgress;
    public event Action<string> OnStatusChanged;
    public event Action OnDownloadComplete;
    public event Action<string> OnDownloadFailed;

    private bool isInitialized;
    private bool isDownloading;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartDownload()
    {
        if (isDownloading) return;
        StartCoroutine(DownloadRoutine());
    }

    private IEnumerator DownloadRoutine()
    {
        isDownloading = true;

        OnStatusChanged?.Invoke("Checking connection...");
        yield return new WaitForSeconds(0.3f);

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            isDownloading = false;
            OnDownloadFailed?.Invoke("No internet connection");
            yield break;
        }

        OnStatusChanged?.Invoke("Initializing...");

        bool initSuccess = false;

        var initOp = Addressables.InitializeAsync();
        initOp.Completed += handle =>
        {
            initSuccess = handle.Status == AsyncOperationStatus.Succeeded;
        };
        yield return initOp;

        if (!initSuccess)
        {
            isDownloading = false;
            OnDownloadFailed?.Invoke("Failed to initialize Addressables");
            yield break;
        }

        isInitialized = true;

        OnStatusChanged?.Invoke("Checking for updates...");

        bool catalogCheckDone = false;
        List<string> catalogsToUpdate = null;

        var catalogOp = Addressables.CheckForCatalogUpdates(false);
        catalogOp.Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null && handle.Result.Count > 0)
                catalogsToUpdate = handle.Result;
            catalogCheckDone = true;
        };
        yield return catalogOp;

        if (catalogsToUpdate != null && catalogsToUpdate.Count > 0)
        {
            OnStatusChanged?.Invoke("Updating catalog...");

            bool catalogUpdateDone = false;
            var updateOp = Addressables.UpdateCatalogs(catalogsToUpdate, false);
            updateOp.Completed += handle =>
            {
                if (handle.Status != AsyncOperationStatus.Succeeded)
                    Debug.LogWarning("AddressableLoader: Catalog update failed, using cached catalog");
                catalogUpdateDone = true;
            };
            yield return updateOp;
        }

        OnStatusChanged?.Invoke("Checking download size...");

        long downloadSize = 0;
        bool sizeCheckDone = false;
        bool sizeCheckFailed = false;

        var sizeOp = Addressables.GetDownloadSizeAsync("music");
        sizeOp.Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
                downloadSize = handle.Result;
            else
                sizeCheckFailed = true;
            sizeCheckDone = true;
        };
        yield return sizeOp;

        if (sizeCheckFailed)
        {
            isDownloading = false;
            OnDownloadFailed?.Invoke("Failed to check download size");
            yield break;
        }

        if (downloadSize > 0)
        {
            OnStatusChanged?.Invoke("Downloading music...");

            bool downloadDone = false;
            bool downloadFailed = false;

            var downloadOp = Addressables.DownloadDependenciesAsync("music", false);
            downloadOp.Completed += handle =>
            {
                downloadFailed = handle.Status != AsyncOperationStatus.Succeeded;
                downloadDone = true;
            };

            while (!downloadDone)
            {
                if (downloadOp.IsValid())
                {
                    float progress = downloadOp.GetDownloadStatus().Percent;
                    OnDownloadProgress?.Invoke(progress);
                }
                yield return null;
            }

            if (downloadFailed)
            {
                isDownloading = false;
                OnDownloadFailed?.Invoke("Download failed");
                yield break;
            }
        }
        else
        {
            OnStatusChanged?.Invoke("Music already cached");
            OnDownloadProgress?.Invoke(1f);
            yield return new WaitForSeconds(0.3f);
        }

        isDownloading = false;
        OnStatusChanged?.Invoke("Ready!");
        OnDownloadComplete?.Invoke();
    }

    public bool IsInitialized()
    {
        return isInitialized;
    }
}
