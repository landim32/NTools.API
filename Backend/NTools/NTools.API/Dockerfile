#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ENV ASPNETCORE_ENVIRONMENT=Docker
WORKDIR /app
EXPOSE 443

# Build libgdiplus with pango
RUN apt-get update && apt-get install  -y --allow-unauthenticated \
     libgif-dev autoconf libtool automake build-essential gettext libglib2.0-dev libcairo2-dev libtiff-dev libexif-dev
RUN apt-get update && apt-get install -y libpango1.0-dev 

RUN apt-get install -y --allow-unauthenticated git
RUN git clone https://github.com/mono/libgdiplus.git /libgdiplus
WORKDIR /libgdiplus
RUN ./autogen.sh --with-pango --prefix=/usr
RUN make  
RUN make install

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["NAuth.API/emagine.pfx", "NAuth.API/"]
COPY ["NAuth.API/NAuth.API.csproj", "NAuth.API/"]
COPY ["NAuth.Application/NAuth.Application.csproj", "NAuth.Application/"]
COPY ["NAuth.Domain/NAuth.Domain.csproj", "NAuth.Domain/"]
COPY ["Core.Domain/Core.Domain.csproj", "Core.Domain/"]
COPY ["NAuth.DTO/NAuth.DTO.csproj", "NAuth.DTO/"]
COPY ["DB.Infra/DB.Infra.csproj", "DB.Infra/"]
RUN dotnet restore "NAuth.API/NAuth.API.csproj"
COPY . .
WORKDIR "/src/NAuth.API"
RUN dotnet build "NAuth.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NAuth.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NAuth.API.dll"]