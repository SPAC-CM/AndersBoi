# Find PDF's from links
Uses a .xlsx dokument with links to download PDF's. the progran will give a Rapport in folder for how many it downloaded, and what file get downloaded by there BR_Nummer
How a console to show there it is in the process
The files will be downloaded into the download folder by default
The program run automaticly. Do not need input or interaction.


# Requrioment
The folder needs a file called "GRI_2017_2020.xlsx", if not there the program will fail.
- Move coluems around from Original "GRI_2017_2020.xlsx" will result in errors.

Close XL dokument then program is running. If not, it can not update them.

# Use
Go into the FileProject_V2 folder and run like a normal .NET project

# Testing enviorment
For tesing, it's set to run the first 20 links and download them. To make it run the holde XL, remove the change to rowCount in line 15 in Program 
