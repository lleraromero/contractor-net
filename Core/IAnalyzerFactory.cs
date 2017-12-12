namespace Contractor.Core
{
    public interface IAnalyzerFactory
    {
        IAnalyzer CreateAnalyzer();
        int GeneratedQueriesCount { get; set; }
        int UnprovenQueriesCount { get; set; }
        int DependencyQueriesCount { get; set; }
    }
}