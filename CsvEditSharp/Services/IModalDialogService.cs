namespace CsvEditSharp.Services
{
    public interface IModalDialogService<T>
    {
        T Result { get; }
        bool? ShowModal();
    }
}
