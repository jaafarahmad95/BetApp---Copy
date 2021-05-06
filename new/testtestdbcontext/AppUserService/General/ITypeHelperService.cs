namespace testtest.Service.General
{
    public interface ITypeHelperService
    {
        bool TypeHasProperties<T>(string fields);
    }
}