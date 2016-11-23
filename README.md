# ccollab2eeplatform
Data from CodeCollaborator to EagleEye-Platform

# Usage
ccollab2ee [options]  

options:  
	-t, --task-id=VALUE	the task Id.  
	-h, --help		show this message and exit  

e.g.  
ccollab2ee --task-id="57837029c66dc1a4570962b6"  
ccollab2ee -t "57837029c66dc1a4570962b6"  
ccollab2ee -h  
ccollab2ee --help  

# Notes
1. in this version, the tool will wait a keyboard hit before exit.
2. the csv files file name will be displayed in the console window
3. the csv file will be removed just before tool exit