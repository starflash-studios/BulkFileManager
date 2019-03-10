# BulkFileManager
Bulk File Manager (by Starflash Studios)
Usage: BFM.exe [directory] [/r, /l, and /w + pattern] [/c +command]
            
Arguments:
	/r = Recursive Searching Mode (Scans both directory AND subdirectories)
	/l = Local Mode (Replaces ./ with current directory -- Saves time, but can break on some instances
	/w [x] = Wildcard Mode, where [x] is your wildcard (ie *.txt)
	/c [x] = Command Mode, where [x] is your command (Add any of the below replacements if desired)
            
Command Replacements:
	`p = Path (ie c:\test\example.png)
	`f = File (ie example.png)
	`e = Extension (ie .png)
	`d = Directory (ie c:\test)
	`n = Filename (ie example)
