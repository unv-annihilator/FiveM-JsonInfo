@echo off
pushd InfoJsonClient
dotnet publish -c Release
popd

pushd InfoJsonServer
dotnet publish -c Release
popd

rmdir /s /q dist
mkdir dist

copy /y fxmanifest.lua dist
xcopy /y /e InfoJsonClient\bin\Release\net452\publish dist\Client\bin\Release\net452\publish\
xcopy /y /e InfoJsonServer\bin\Release\netstandard2.0\publish dist\Server\bin\Release\netstandard2.0\publish\