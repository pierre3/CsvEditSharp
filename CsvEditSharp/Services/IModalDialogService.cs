namespace CsvEditSharp.Services
{
    public interface IModalDialogService
    {
        bool? ShowModal();
    }

    public interface IModalDialogService<T> : IModalDialogService
    {
        T Result { get; }
    }
}
