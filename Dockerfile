FROM mcr.microsoft.com/dotnet/sdk:8.0 AS test

WORKDIR /app

# Copy solution and project files
COPY Miniblog.Core.sln ./
COPY src/Miniblog.Core.csproj ./src/
COPY tests/Miniblog.Core.Tests.csproj ./tests/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the code
COPY . .

# Install ReportGenerator tool for test coverage reports
RUN dotnet tool install -g dotnet-reportgenerator-globaltool

# Add dotnet tools to PATH
ENV PATH="${PATH}:/root/.dotnet/tools"

# Run tests with coverage
CMD ["bash", "-c", "echo GLIBC VERSION && ldd --version && echo GLIBC VERSION CHECK && dotnet test --collect:'XPlat Code Coverage' --results-directory ./tests/TestResults && find ./tests/TestResults -name 'coverage.cobertura.xml' -exec cp {} ./tests/TestResults/coverage.cobertura.xml \\;"]
