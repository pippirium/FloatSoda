using System.Diagnostics;
using FloatSoda.OVR.Exceptions;

namespace FloatSoda.OVR.Overlay;

/// <summary>Owns the SteamVR identity and native handle of an overlay.</summary>
public interface IOverlayIdentity : IDisposable
{
    /// <summary>Gets the stable key used to identify the overlay in SteamVR.</summary>
    string Key { get; }
    /// <summary>Gets the user-facing overlay name.</summary>
    string Name { get; }
    /// <summary>Gets the native OpenVR overlay handle.</summary>
    ulong Handle { get; }

    /// <summary>
    /// Gets whether <see cref="IDisposable.Dispose"/> keeps the native overlay alive instead of destroying it.
    /// </summary>
    /// <remarks>
    /// <see langword="true"/> once the identity released the handle without destroying it, or when it
    /// never owned one. Releasing is idempotent, so releasing twice is harmless.
    /// </remarks>
    bool Detached { get; }

    /// <summary>
    /// Releases ownership of the native overlay without destroying it.
    /// </summary>
    /// <remarks>
    /// The overlay itself keeps living in SteamVR, and <see cref="IDisposable.Dispose"/> becomes a no-op.
    /// SteamVR still destroys the overlay when the owning process is told to quit or when
    /// SteamVR itself shuts down. Calling this on an already released identity does nothing.
    /// </remarks>
    void Detach();
}

/// <summary>Creates or adopts the identity of a regular OpenVR overlay.</summary>
public class OverlayIdentity : IOverlayIdentity
{
    private ulong _handle;
    /// <inheritdoc />
    public string Key { get; protected init; }
    /// <inheritdoc />
    public string Name { get; protected init; }

    /// <inheritdoc />
    public ulong Handle
    {
        get => _handle;
        private set => _handle = value;
    }

    /// <inheritdoc />
    public bool Detached => _handle == OpenVR.k_ulOverlayHandleInvalid;

    /// <summary>Creates a new overlay identity or adopts an existing handle.</summary>
    /// <param name="key">The stable key used to identify the overlay in SteamVR.</param>
    /// <param name="name">The user-facing overlay name.</param>
    /// <param name="overlayHandle">An existing handle to adopt, or the invalid handle to create a new overlay.</param>
    public OverlayIdentity(string key, string name, ulong overlayHandle = OpenVR.k_ulOverlayHandleInvalid)
    {
        Key = key;
        Name = name;
        _handle = overlayHandle;

        if (_handle == OpenVR.k_ulOverlayHandleInvalid)
        {
            CreateOverlay(Key, Name, ref _handle).ThrowIfError();
        }
    }

    /// <summary>Destroys the owned OpenVR overlay if it has not already been destroyed.</summary>
    public virtual void Dispose()
    {
        if (Handle != OpenVR.k_ulOverlayHandleInvalid)
        {
            OpenVR.Overlay.DestroyOverlay(Handle).ThrowIfError();
            Handle = OpenVR.k_ulOverlayHandleInvalid;
        }
    }

    /// <inheritdoc />
    public void Detach() => Handle = OpenVR.k_ulOverlayHandleInvalid;

    private static EVROverlayError CreateOverlay(string key, string name, ref ulong handle)
    {
        var error = OpenVR.Overlay.CreateOverlay(key, name, ref handle);
        if (error != EVROverlayError.KeyInUse) return error;
        if (!TryDestroyStaleOverlay(key, out var cleanupError)) return cleanupError;

        handle = OpenVR.k_ulOverlayHandleInvalid;
        return OpenVR.Overlay.CreateOverlay(key, name, ref handle);
    }

    internal static EVROverlayError CreateDashboardOverlay(
        string key, string name, ref ulong mainHandle, ref ulong thumbnailHandle)
    {
        var error = OpenVR.Overlay.CreateDashboardOverlay(key, name, ref mainHandle, ref thumbnailHandle);
        if (error != EVROverlayError.KeyInUse) return error;
        if (!TryDestroyStaleOverlay(key, out var cleanupError)) return cleanupError;

        mainHandle = OpenVR.k_ulOverlayHandleInvalid;
        thumbnailHandle = OpenVR.k_ulOverlayHandleInvalid;
        return OpenVR.Overlay.CreateDashboardOverlay(key, name, ref mainHandle, ref thumbnailHandle);
    }

    /// <summary>
    /// 同じキーのオーバーレイが既にある場合、最後に描画したプロセスが終了済みのときだけ回収します。
    /// 所有プロセスを確認できない場合は、実行中のオーバーレイを壊さないよう回収しません。
    /// </summary>
    private static bool TryDestroyStaleOverlay(string key, out EVROverlayError error)
    {
        var handle = OpenVR.k_ulOverlayHandleInvalid;
        error = OpenVR.Overlay.FindOverlay(key, ref handle);
        if (error != EVROverlayError.None || handle == OpenVR.k_ulOverlayHandleInvalid) return false;

        var renderingPid = OpenVR.Overlay.GetOverlayRenderingPid(handle);
        if (IsProcessAliveOrUnknown(renderingPid))
        {
            error = EVROverlayError.KeyInUse;
            return false;
        }

        error = OpenVR.Overlay.DestroyOverlay(handle);
        return error == EVROverlayError.None;
    }

    private static bool IsProcessAliveOrUnknown(uint processId)
    {
        // PID が未設定、または現在のプロセスとして確認できない場合は安全側に倒す。
        if (processId == 0 || processId > int.MaxValue) return true;

        try
        {
            using var process = Process.GetProcessById((int)processId);
            return !process.HasExited;
        }
        catch (ArgumentException)
        {
            return false;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        catch (Exception)
        {
            return true;
        }
    }
}

/// <summary>Owns the main and thumbnail handles of a SteamVR dashboard overlay.</summary>
public class DashboardOverlayIdentity : IOverlayIdentity
{
    /// <inheritdoc />
    public string Key { get; init; }
    /// <inheritdoc />
    public string Name { get; init; }
    /// <inheritdoc />
    public ulong Handle { get; private set; }
    /// <summary>Gets the native handle for the dashboard thumbnail.</summary>
    public ulong ThumbnailHandle { get; private set; }
    /// <inheritdoc />
    public bool Detached => Handle == OpenVR.k_ulOverlayHandleInvalid;

    /// <summary>Creates a dashboard overlay and its thumbnail.</summary>
    /// <param name="key">The stable key used to identify the overlay in SteamVR.</param>
    /// <param name="name">The user-facing dashboard name.</param>
    public DashboardOverlayIdentity(string key, string name)
    {
        Key = key;
        Name = name;

        ulong mainHandle = OpenVR.k_ulOverlayHandleInvalid;
        ulong thumbHandle = OpenVR.k_ulOverlayHandleInvalid;

        OverlayIdentity.CreateDashboardOverlay(Key, Name, ref mainHandle, ref thumbHandle).ThrowIfError();

        Handle = mainHandle;
        ThumbnailHandle = thumbHandle;
    }

    /// <summary>Destroys the dashboard overlay if it has not already been destroyed.</summary>
    public void Dispose()
    {
        if (Handle != OpenVR.k_ulOverlayHandleInvalid)
        {
            OpenVR.Overlay.DestroyOverlay(Handle).ThrowIfError();
            Handle = OpenVR.k_ulOverlayHandleInvalid;
        }
    }

    /// <inheritdoc />
    public void Detach()
    {
        Handle = OpenVR.k_ulOverlayHandleInvalid;
        ThumbnailHandle = OpenVR.k_ulOverlayHandleInvalid;
    }
}
