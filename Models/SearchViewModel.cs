// ============================================================
// File: SearchViewModel.cs
// Author: Victor Marrujo
// Course: CST-350
// Description: ViewModel carrying search form input and results
//              between the SearchController and Search views.
// ============================================================

namespace BibleVerseApp.Models
{
    /// <summary>
    /// View model for the Bible verse search feature.
    /// Carries both the search parameters from the user and the
    /// results list returned from the DAL.
    /// </summary>
    public class SearchViewModel
    {
        /// <summary>Gets or sets the keyword or phrase entered by the user.</summary>
        public string SearchTerm { get; set; } = string.Empty;

        /// <summary>Gets or sets whether to include Old Testament verses in results.</summary>
        public bool IncludeOldTestament { get; set; } = true;

        /// <summary>Gets or sets whether to include New Testament verses in results.</summary>
        public bool IncludeNewTestament { get; set; } = true;

        /// <summary>Gets or sets the list of matching verses returned by the search.</summary>
        public List<BibleVerse> Results { get; set; } = new List<BibleVerse>();

        /// <summary>
        /// Returns true if a search has been performed and results are populated.
        /// </summary>
        public bool HasResults => Results != null && Results.Count > 0;
    }
}


