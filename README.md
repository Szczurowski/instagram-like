# instagram-like

## installation instructions:

On development\build machine:
1. Install .NET Core 2 (or later) SDK
2. Install NPM version 6 (or later)
3. Have prepared a M$ SQL Server instance with a clean Database (Database) and full access to it
4. Clone repository https://github.com/Szczurowski/instagram-like.git 
5. From the cloned repository location (Location) run the *./sql-scripts/create-tables.sql* script on the Database
6. Point the connection string located in file ./src/Insta.Web/appsettings.json on the Database
6. From within the ./src/Insta.Web execute:
	- dotnet restore
	- npm install
	- dotnet run

## producing production build

1. When ready for production execute:

	- dotnet publish -c Release
