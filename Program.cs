using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace ConsoleApp1
{
    class Program
    {
        // Path where attachments will be saved
        static string basePath = @"c:\temp\emails\";
        // Integer to store to the total size of all the files - displayed
        // after processing to indicate possible PST file size saving
        static int totalfilesize = 0;

        static void Main(string[] args)
        {
            EnumerateAccounts();
        }

        // Uses recursion to enumerate Outlook subfolders.
        static void EnumerateFolders(Outlook.Folder folder)
        {
            Outlook.Folders childFolders = folder.Folders;
            if (childFolders.Count > 0)
            {
                // loop through each childFolder (aka sub-folder) in current folder
                foreach (Outlook.Folder childFolder in childFolders)
                {
                    // We only want Inbox folders - ignore Contacts and others
                    if (childFolder.FolderPath.Contains("Inbox"))
                    {
                        // Write the folder path.
                        Console.WriteLine(childFolder.FolderPath);
                        // Call EnumerateFolders using childFolder, 
                        // to see if there are any sub-folders within this one
                        EnumerateFolders(childFolder);
                    }
                }
            }
            // pass folder to IterateMessages which processes individual email messages
            Console.WriteLine("Looking for items in " + folder.FolderPath);
            IterateMessages(folder);
        }

        // Loops through each item (aka email) in a folder
        static void IterateMessages(Outlook.Folder folder)
        {
            // attachment extensions to save
            string[] extensionsArray = { ".pdf", ".doc", ".xls",
            ".ppt", ".vsd", ".zip", ".rar", ".txt", ".csv", ".proj" };

            // Iterate through all items ("messages") in a folder
            var fi = folder.Items;
            if (fi != null)
            {
                try
                {
                    foreach (Object item in fi)
                    {
                        Outlook.MailItem mi = (Outlook.MailItem)item;
                        var attachments = mi.Attachments;
                        // Only process item if it has one or more attachments
                        if (attachments.Count != 0)
                        {

                            // Create a directory to store the attachment 
                            if (!Directory.Exists(basePath + folder.FolderPath))
                            {
                                Directory.CreateDirectory(basePath + folder.FolderPath);
                            }

                            // Loop through each attachment
                            for (int i = 1; i <= mi.Attachments.Count; i++)
                            {
                                // Check wither any of the strings in the 
                                // extensionsArray are contained within the filename
                                var fn = mi.Attachments[i].FileName.ToLower();
                                if (extensionsArray.Any(fn.Contains))
                                {

                                    // Create a further sub-folder for the sender
                                    if (!Directory.Exists(basePath + folder.FolderPath +
                                        @"\" + mi.Sender.Address))
                                    {
                                        Directory.CreateDirectory(basePath +
                                            folder.FolderPath + @"\" + mi.Sender.Address);
                                    }
                                    totalfilesize = totalfilesize + mi.Attachments[i].Size;
                                    if (!File.Exists(basePath + folder.FolderPath + @"\" +
                                        mi.Sender.Address + @"\" + mi.Attachments[i].FileName))
                                    {
                                        Console.WriteLine("Saving " + mi.Attachments[i].FileName);
                                        mi.Attachments[i].SaveAsFile(basePath + folder.FolderPath +
                                            @"\" + mi.Sender.Address + @"\" +
                        mi.Attachments[i].FileName);
                                        // Uncomment next line to delete attachment after saving it
                                        // mi.Attachments[i].Delete();
                                    }
                                    else
                                    {
                                        Console.WriteLine("Already saved " + mi.Attachments[i].FileName);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    // Console.WriteLine("An error occurred: '{0}'", e);
                }
            }
        }

        // Retrieves the email address for a given account object
        static string EnumerateAccountEmailAddress(Outlook.Account account)
        {
            try
            {
                if (string.IsNullOrEmpty(account.SmtpAddress) || string.IsNullOrEmpty(account.UserName))
                {
                    Outlook.AddressEntry oAE = account.CurrentUser.AddressEntry as Outlook.AddressEntry;
                    if (oAE.Type == "EX")
                    {
                        Outlook.ExchangeUser oEU = oAE.GetExchangeUser() as Outlook.ExchangeUser;
                        return oEU.PrimarySmtpAddress;
                    }
                    else
                    {
                        return oAE.Address;
                    }
                }
                else
                {
                    return account.SmtpAddress;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }

        // Displays introduction text, lists each Account, and prompts user to select one for processing.
        static void EnumerateAccounts()
        {
            Console.Clear();
            Console.WriteLine("Outlook Attachment Extractor v0.1");
            Console.WriteLine("---------------------------------");
            int id;
            Outlook.Application Application = new Outlook.Application();
            Outlook.Accounts accounts = Application.Session.Accounts;

            string response = "";
            while (true == true)
            {
                id = 1;
                foreach (Outlook.Account account in accounts)
                {
                    Console.WriteLine(id + ":" + EnumerateAccountEmailAddress(account));
                    id++;
                }
                Console.WriteLine("Q: Quit Application");

                response = Console.ReadLine().ToUpper();
                if (response == "Q")
                {
                    Console.WriteLine("Quitting");
                    return;
                }
                if (response != "")
                {
                    if (Int32.Parse(response.Trim()) >= 1 && Int32.Parse(response.Trim()) < id)
                    {
                        Console.WriteLine("Processing: " +
                            accounts[Int32.Parse(response.Trim())].DisplayName);
                        Console.WriteLine("Processing: " +
                            EnumerateAccountEmailAddress(accounts[Int32.Parse(response.Trim())]));

                        Outlook.Folder selectedFolder =
                            Application.Session.DefaultStore.GetRootFolder() as Outlook.Folder;
                        selectedFolder = GetFolder(@"\\" +
                            accounts[Int32.Parse(response.Trim())].DisplayName);
                        EnumerateFolders(selectedFolder);
                        Console.WriteLine("Finished Processing " +
                            accounts[Int32.Parse(response.Trim())].DisplayName);
                        Console.WriteLine("");
                    }
                    else
                    {
                        Console.WriteLine("Invalid Account Selected");
                    }
                }
            }
        }

        // Returns Folder object based on folder path
        static Outlook.Folder GetFolder(string folderPath)
        {
            Console.WriteLine("Looking for: " + folderPath);
            Outlook.Folder folder;
            string backslash = @"\";
            try
            {
                if (folderPath.StartsWith(@"\\"))
                {
                    folderPath = folderPath.Remove(0, 2);
                }
                String[] folders = folderPath.Split(backslash.ToCharArray());
                Outlook.Application Application = new Outlook.Application();
                folder = Application.Session.Folders[folders[0]] as Outlook.Folder;
                if (folder != null)
                {
                    for (int i = 1; i <= folders.GetUpperBound(0); i++)
                    {
                        Outlook.Folders subFolders = folder.Folders;
                        folder = subFolders[folders[i]] as Outlook.Folder;
                        if (folder == null)
                        {
                            return null;
                        }
                    }
                }
                return folder;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
