FROM xxxxxxxxxxxxxxxxx/dotnet/core/sdk:3.0 AS build
EXPOSE 5000
# COPY ./nuget.config /
WORKDIR /app
# copy csproj and restore as distinct layers
COPY ./*.csproj ./
WORKDIR /app/
# --configfile ../nuget.config
RUN dotnet restore 
# copy and publish app and libraries
WORKDIR /app/
COPY ./. ./
WORKDIR /app/
RUN dotnet publish -o out
FROM xxxxxxxxxxxxx/dotnet/core/aspnet:3.0 AS runtime
ENV ORA_SDTZ "America/Sao_Paulo"
ENV TZ ${ORA_SDTZ}
RUN echo ${TZ} > /etc/timezone 
RUN ln -fs "/usr/share/zoneinfo/${TZ}" /etc/localtime
RUN dpkg-reconfigure -f noninteractive tzdata
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "SmallApi.dll"]
