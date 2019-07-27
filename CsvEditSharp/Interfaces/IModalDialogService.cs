namespace CsvEditSharp.Interfaces
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
