using Client.Pages;
using Client.Resources;
using Client.Resources.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Client.Classes.Visual
{
    public class ShowDocuments
    {
        Session session;
        MainWorkPage page;
        public ShowDocuments(Session Session, MainWorkPage MainWorkPage) 
        {
            session = Session;
            page = MainWorkPage;
        }
        public void UpdateDocuments(int FolderCount)
        {
            if (FolderCount == -1) { return; }
            int howManyDocs = session.receivedFolders[FolderCount].Documents.Count;
            page.DocumentsListBox.Items.Clear();
            UpdateDocumentsFile(howManyDocs, FolderCount);
        }

        private void UpdateDocumentsFile(int howManyDocs, int FolderCount) 
        {
            int n = page.InPatternBox.SelectedIndex;
            page.DocumentsListBox.Items.Clear();
            switch (n)
            {
                case 1:
                    FileTimeSort(FolderCount);
                    PrintDocAndPattern(howManyDocs, FolderCount);
                    UpdateDocumentsPattern(FolderCount);
                    break;
                case 2:
                    FileTimeSortReversed(FolderCount);
                    PrintDocAndPattern(howManyDocs, FolderCount);
                    UpdateDocumentsPattern(FolderCount);
                    break;
                case 3:
                    FileNameSort(FolderCount);
                    PrintDocAndPattern(howManyDocs, FolderCount);
                    UpdateDocumentsPattern(FolderCount);
                    break;
                case 4:
                    FileNameSortReversed(FolderCount);
                    PrintDocAndPattern(howManyDocs, FolderCount);
                    UpdateDocumentsPattern(FolderCount);
                    break;
                case 0:
                    UpdateDocumentsPattern(FolderCount);
                    FileNameSortReversed(FolderCount);
                    PrintDocAndPattern(howManyDocs, FolderCount);
                    break;
            }
        }

        private void PrintDocAndPattern(int howManyDocs, int FolderCount)
        {
            for (int i = 0; i < howManyDocs; i++)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Inlines.Add(session.receivedFolders[FolderCount].Documents[i].DocumentName);
                int IDInPattern = Convert.ToInt32(session.receivedFolders[FolderCount].Documents[i].InPatternID);
                --IDInPattern;


                if (IDInPattern >= 0)
                {
                    string NameInPattern = session.receivedRequiredInPatterns[IDInPattern].DocumentName;
                    textBlock.Inlines.Add(new Run("\n" + NameInPattern) { FontWeight = FontWeights.Bold, Tag = NameInPattern });
                }
                page.DocumentsListBox.Items.Add(textBlock);
            }
        }

        private void FileTimeSort(int FolderID)
        {
            var docks = session.receivedFolders[FolderID].Documents.Cast<ServerDocument>().OrderBy(d => d.DocumentID).ToList();
            for (int i = 0; i < session.receivedFolders.Count; i++)
            {
                session.receivedFolders[FolderID].Documents = docks;
            }
        }
        private void FileTimeSortReversed(int FolderID)
        {
            var docks = session.receivedFolders[FolderID].Documents.Cast<ServerDocument>().OrderByDescending(d => d.DocumentID).ToList();
            for (int i = 0; i < session.receivedFolders.Count; i++)
            {
                session.receivedFolders[FolderID].Documents = docks;
            }
        }
        private void FileNameSort(int FolderID)
        {
            var docks = session.receivedFolders[FolderID].Documents.Cast<ServerDocument>().OrderBy(d => d.DocumentName).ToList();
            for (int i = 0; i < session.receivedFolders.Count; i++)
            {
                session.receivedFolders[FolderID].Documents = docks;
            }
        }
        private void FileNameSortReversed(int FolderID)
        {
            var docks = session.receivedFolders[FolderID].Documents.Cast<ServerDocument>().OrderByDescending(d => d.DocumentName).ToList();
            for (int i = 0; i < session.receivedFolders.Count; i++)
            {
                session.receivedFolders[FolderID].Documents = docks;
            }
        }
        private void UpdateDocumentsPattern(int FolderCount)
        {

            int patternID = 0;
            if (session.receivedFolders[FolderCount].PatternID != null)
            {
                patternID = Convert.ToInt32(session.receivedFolders[FolderCount].PatternID);
            }
            else { return; }
            patternID--;

            if (patternID >= 0)
            {
                List<RequiredInPattern> NeedInFolderObj = session.receivedPatterns[patternID].RequiredInPatterns;
                List<string> NeedInFolderStr = new List<string>();
                for (int i = 0; i < NeedInFolderObj.Count; i++)
                {
                    var doc = session.receivedFolders[FolderCount].Documents.FirstOrDefault(d => d.InPatternID == NeedInFolderObj[i].DocumentPatternID);
                    if (doc == null)
                    {
                        NeedInFolderStr.Add(NeedInFolderObj[i].DocumentName);
                    }
                }

                for (int i = 0; i < NeedInFolderStr.Count; i++)
                {
                    TextBlock textBlock = new TextBlock();
                    textBlock.Inlines.Add(new Run("\b" + NeedInFolderStr[i]) { FontWeight = FontWeights.Bold });
                    page.DocumentsListBox.Items.Add(textBlock);
                }
            }
        }

    }
}
