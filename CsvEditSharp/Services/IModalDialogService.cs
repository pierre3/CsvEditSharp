namespace CsvEditSharp.Services
{
    public interface IModalDialogService
    {
        bool? ShowModal(params object[] parameters);
    }

    public interface IModalDialogService<T> : IModalDialogService
    {
        T Result { get; }
    }
}
