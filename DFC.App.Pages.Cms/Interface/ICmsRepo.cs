namespace DFC.App.Pages.Cms.Interface
{
    public interface ICmsRepo
    {
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns>The response.</returns>
        Task<TResponse> GetData<TResponse>(string query)
            where TResponse : class;

        Task<TResponse> GetSqlData<TResponse>(string queryName);
    }
}
