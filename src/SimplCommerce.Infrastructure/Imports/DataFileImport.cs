using System.IO;
using System.Threading.Tasks;
using SimplCommerce.Infrastructure.ResultTypes;

namespace SimplCommerce.Infrastructure.Imports
{
    public abstract class DataFileImport<TData, TResult>
    {
        protected abstract Task<MemoryStream> RetrieveAsync();
        protected abstract ActionFeedback<TData> Parse(MemoryStream fileStream);
        protected abstract Task<ActionFeedback<TResult>> ImportAsync(TData data);

        public async Task<ActionFeedback<TResult>> RunImportAsync()
        {
            var fileStream = await RetrieveAsync();
            var parsingFeedback = Parse(fileStream);
            if (parsingFeedback.Success)
            {
                return await ImportAsync(parsingFeedback.Result);
            }
            return ActionFeedback<TResult>.Fail(parsingFeedback.ErrorMessage);
        }
    }
}
