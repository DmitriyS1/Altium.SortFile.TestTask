# How to run
1. Run the FileGenerator project
2. Follow the menu and generate a file
3. Copy the path of the created file from the console
4. Run Sorting service
5. Insert path from the 3rd step

# I couldn't get the same speed as described in the ticket.
For 1 GB file the program uses 671 MB of RAM and it takes from 6 to 10 minutes to sort a file
I'm limited to the Hard Disk Speed.
I tried to vary the size of the sorted files and it was getting worse and worse. I tried to increase the value to use more RAM than HDD. The best way I found is now in the `SortingService.Program.cs` on the 18th line. But all of that depends on the hard disk write speed. You can vary up to 12M (3rd parameter on the 18th line in the Program.cs file)

# I created a parallel version of the algorithm and it works 4 times faster than the one-thread version.
But the parallel version takes all the RAM I have. Around 10GB to sort 1 GB file.
As I understand, the main concern is to reduce RAM consumption to sort 100GB file. In theory, we can use all the RAM we have to solve the task as fast as possible. To run the parallel version you need to uncomment lines 38-42, and 50. And comment lines 32-36, and 49.
