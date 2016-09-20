# Lift

# Purpose
Initially a tool to start executables with different arguments by right-clicking on the task bar icon and selecting it from the Jump List.  
I wanted to have more space in the task bar and not have the same executable with different cli-arguments multiple times.

Now it allows to start or open frequently used tools, folders or files from a Jump List (right click on task bar entry) or from the program window itself.

# If not all items are displayed
Have a look at [this superuser entry](http://superuser.com/questions/1035179/how-do-i-increase-the-number-of-items-on-a-jump-list-in-windows-10) to increase the number of displayed entries (also on Windows 10).  
It worked for me on Windows 10 build 14393.187.

## Copyright and Licensing
### Third party code
I am using the ObservableSortedList code, which is copyrighted by Roman Starkov. The code is [available on BitBucket](https://bitbucket.org/rstarkov/wpfcrutches/src/tip/ObservableSortedList.cs?fileviewer=file-view-default) without licensing information, and located here in _WpfCrutches/ObservableSortedList.cs_.

Inside _Helpers/IconTool.cs_ I am using code based on [a tutorial by Diptimaya Patra](http://www.c-sharpcorner.com/uploadfile/dpatra/get-icon-from-filename-in-wpf/) to retrieve the icon of a filename. And code [written by Bradley Smith](http://www.c-sharpcorner.com/uploadfile/dpatra/get-icon-from-filename-in-wpf/) to extract the icon of an executable assigned to a specific file type.  

### My own code
The rest of the code stands is released to the public domain with the Unlicense, please refer to <http://unlicense.org/>
