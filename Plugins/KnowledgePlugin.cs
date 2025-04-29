using Azure;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using Azure.Search.Documents;
using Microsoft.Extensions.Options;

namespace OnboardyAgent.Plugins;

[Description("This is RAG plugin to retrieve knowledge")]
public class KnowledgePlugin
{
    private readonly AzureAISearchSettings _settings;

    public KnowledgePlugin(IOptions<AzureAISearchSettings> settings)
    {
        _settings = settings.Value;
    }

    [KernelFunction]
    [Description("Reads information and file URL from knwoledge base")]
    public async Task<List<SearchResultWithTitle>> GetData(string message)
    {
        SearchResults<SearchDocument> response;

        var indexClient = new SearchIndexClient(new Uri(_settings.AzureAISearchUrl), new AzureKeyCredential(_settings.AzureAISearchKey));
        var searchClient = indexClient.GetSearchClient(_settings.AzureAISearchIndex);

        var searchOptions = new SearchOptions
        {
            Size = 3,
            Select = { "chunk", "title" },
            IncludeTotalCount = true,
            QueryType = SearchQueryType.Semantic
        };

        //TOO FINISH

        List<SearchResultWithTitle> searchResults = new List<SearchResultWithTitle>();

        try
        {
            response = await searchClient.SearchAsync<SearchDocument>(message, searchOptions);
            await foreach (SearchResult<SearchDocument> r in response.GetResultsAsync())
            {
                var content = r.Document["chunk"];
                var title = r.Document["title"];
                var score = r.Score;


                var documentPath = $"https://onboardystorage.blob.core.windows.net/knowledge/{title}";

                searchResults.Add(new SearchResultWithTitle(documentPath, content.ToString()));
            }
        } 
        catch(Exception e)
        {

        }


        return searchResults;
    }
}
