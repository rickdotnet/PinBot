FROM mcr.microsoft.com/dotnet/sdk:5.0 as dotnet-build
COPY /src .
RUN dotnet publish PinBot.sln -c Release -r linux-x64 --self-contained -o /app

FROM mcr.microsoft.com/dotnet/aspnet:5.0
COPY --from=dotnet-build /app .

ENTRYPOINT ["dotnet", "PinBot.Bot.dll"]