File Downloader

This is a simple C# app that downloads files from a list of URLs and saves them to a directory based on their domain. The app uses async methods to download the files and automatically determines the number of threads to use based on the processor count. It also handles errors gracefully, so it will continue to download other files even if there is a problem with a particular URL.

Requirements
.NET Framework 4.7.2 or later

Usage
Edit the urls.txt file to include the URLs of the files you want to download. Each URL should be on its own line.

Run the FileDownloader.exe file. The files will be downloaded and saved to a directory based on their domain.

Limitations
The app does not check the file type of the URLs, so it will attempt to download any type of file specified in the urls.txt file.
License

This app is licensed under the MIT License. See the LICENSE file for more information.
