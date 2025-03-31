# Use the official .NET 8.0 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the project file and restore any dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application code
COPY . ./

# Build the app
RUN dotnet publish -c Release -o out

# Use the official .NET 8.0 runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Set the working directory inside the container
WORKDIR /app

# Expose port 80 (or the port your app uses)
EXPOSE 80

# Copy the built app from the previous stage
COPY --from=build /app/out .

# Set the entry point to run the app
ENTRYPOINT ["dotnet", "MimdinareMain.dll"]  # Replace with the name of your main project DLL
