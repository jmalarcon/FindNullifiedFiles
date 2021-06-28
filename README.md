# FindNullifiedFiles

![FindNullifiedFiles icon](FindNullifiedFiles_250.png)

This is a quick&dirty small utility I wrote to find all the files having only nulls that are inside an specified folder (or subfolders).

I needed this to find a lot of files a cloud drive I was using broke, filling them with nulls.

It's written with .NET 5 and will work in Windows, Linux and Mac, although I've only compiled it for Windows (just open the project and compile it for your desired operating system).

## Usage

```bash
FindNullifiedFiles PathToFolder [Max Age of files (30)] [Number of butes to inspect (10)]
```

By default, it will search for files with a maximum age of **30 days**, and instead of reading the full file contents, for the sake of speed, it will read by default only **the first 10 bytes** to check them for *nulls*. You can change both parameters from the command line:

```bash
FindNullifiedFiles "C:\My Files" 90 30 > MyNullFiles.txt
```

This will search all the files inside the `C:\My Files` folder and subfolders, with a maximum age of 30 days that have a `null` in the first 30 bytes  and send the results to a .txt file in the current folder. You can then use the text file to process the files with Microsoft Excel or any other system you want.

Please note that the program shows some information in the first ("Beginning...") and last lines (when the search was done and how many nullified files it has found) that you may want to remove.

If there's some kind of error reading a file you'll see a line that starts with `"Error reading file..."`, so you can check it manually.

Hope this helps!
