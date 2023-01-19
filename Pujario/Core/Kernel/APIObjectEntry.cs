using System;

namespace Pujario.Core.Kernel;

public interface IProvideManagedObjectView<T>
{
    unsafe bool TryGetObject(out T result);
}

/// <summary>
/// Abstract class for API pointer|token holders
/// </summary>
/// <remarks> 
/// Code style propose to name inherrited classes using prefix 'E' (EMyObject)
/// </remarks>
public abstract class APIObjectEntry : IDisposable
{
    /// <summary>
    /// An valid pointer recieved from native API; represents unmanaged object 
    /// </summary>
    public IntPtr P { get; private set; }

    protected APIObjectEntry(IntPtr p) { P = p; }

    /// <summary>
    /// Will be called from Dispose or Destructor if P != NULL
    /// </summary>
    protected virtual void Free() { P = IntPtr.Zero; }

    ~APIObjectEntry() { if (P != IntPtr.Zero) Free(); }

    public void Dispose()
    {
        if (P != IntPtr.Zero) Free();
        GC.SuppressFinalize(this);
    }
}


