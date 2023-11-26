chcp 65001
@echo off

if "%1"=="" (
for %%f in (protocols\datatable\*.proto) do	call protoc.exe --proto_path=protocols\datatable --csharp_out=..\Assets\GamePlay\Scripts\Protobuf\DataTable %%f) else (
call protoc.exe --proto_path=protocols\datatable --csharp_out=..\Assets\GamePlay\Scripts\Protobuf\DataTable %1
)

if "%1"=="" (
for %%f in (protocols\msg\*.proto) do	call protoc.exe --proto_path=protocols\msg --csharp_out=..\Assets\GamePlay\Scripts\Protobuf\Msg %%f) else (
call protoc.exe --proto_path=protocols\msg --csharp_out=..\Assets\GamePlay\Scripts\Protobuf\Msg %1
)

@echo "finish build......"
pause
