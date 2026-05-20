namespace LibraryInformationSystem
{
    
    public interface ILibraryService
    {
        void InitializeDefaultData();
        void AddDataMenu();
        void ViewDataMenu();
        void SearchMenu();
        void SummaryMenu();
        void SortMenu();
        void DeleteMenu();
    }
}
