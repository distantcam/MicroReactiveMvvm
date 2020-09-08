rd docs\api /s /q

dotnet build --configuration Release

InheritDoc -b src -o

xmldocmd src\MicroReactiveMVVM\bin\Release\net472\MicroReactiveMVVM.dll docs/api
